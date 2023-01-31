using Flattiverse.Connector.Events;
using System;
using System.Buffers;
using System.Net.WebSockets;
using System.Security.AccessControl;
using System.Text;
using System.Text.Json;

namespace Flattiverse.Connector.Network
{
    class Connection : IDisposable
    {
        private static readonly JsonDocumentOptions options = new JsonDocumentOptions() { AllowTrailingCommas = false, CommentHandling = JsonCommentHandling.Disallow, MaxDepth = 6 };
        private readonly ClientWebSocket socket;

        private readonly object syncQuery = new object();
        private readonly Dictionary<string, Query> queries = new Dictionary<string, Query>();

        private readonly Random rng = new Random();
        private static readonly char[] tokens = Utils.GenerateAllAllowedOneByteUtf8CharsWithoutSpace();

        private readonly Queue<FlattiverseEvent> pendingEvents = new Queue<FlattiverseEvent>();
        private readonly Queue<TaskCompletionSource<FlattiverseEvent>> pendingEventWaiters = new Queue<TaskCompletionSource<FlattiverseEvent>>();
        private readonly object syncEvents = new object();

        private readonly SemaphoreSlim syncSend = new SemaphoreSlim(1);

        private bool connected = true;

        public readonly UniverseGroup Group;

        public Connection(UniverseGroup group, string uri, string auth)
        {
            Group = group;

            Uri parsedUri = new Uri($"{uri}?auth={auth}");

            socket = new ClientWebSocket();

            socket.ConnectAsync(parsedUri, CancellationToken.None).GetAwaiter().GetResult();

            ThreadPool.QueueUserWorkItem(async delegate { await recv(); });
        }

        private Connection(UniverseGroup group, ClientWebSocket socket)
        {
            Group = group;

            this.socket = socket;

            ThreadPool.QueueUserWorkItem(async delegate { await recv(); });
        }

        public static async Task<Connection> NewAsyncConnection(UniverseGroup group, string uri, string auth)
        {
            Uri parsedUri = new Uri($"{uri}?auth={auth}");

            ClientWebSocket socket = new ClientWebSocket();

            await socket.ConnectAsync(parsedUri, CancellationToken.None).ConfigureAwait(false);

            return new Connection(group, socket);
        }

        internal void shutdown(string? message)
        {
            connected = false;
            socket.Dispose();

            if (message != null)
                PushFailureEvent(message);

            lock (syncQuery)
            {
                foreach (KeyValuePair<string, Query> query in queries)
                    query.Value.Answer(0xF0);

                queries.Clear();
            }
        }

        private async Task recv()
        {
            byte[] recv;
            Memory<byte> recvMemory;

            JsonDocument document;
            JsonElement element;
            string kind;
            string id;
            int code;
            Query? query;

            ValueWebSocketReceiveResult result;
            FlattiverseEvent @event;

            while (true)
            {
                recv = ArrayPool<byte>.Shared.Rent(262144);
                recvMemory = new Memory<byte>(recv);

                try
                {
                    result = await socket.ReceiveAsync(recvMemory, CancellationToken.None);
                }
                catch (Exception exception)
                {
                    shutdown($"WebSocket got disconnected: {exception.Message}");
                    return;
                }

                switch (socket.State)
                {
                    case WebSocketState.CloseReceived:
                        PushFailureEvent($"WebSocket got disconnected with reason \"{socket.CloseStatus}\" and message: {socket.CloseStatusDescription ?? "<none>."} Connection terminated.");

                        connected = false;

                        try
                        {
                            await syncSend.WaitAsync();
                            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Good bye.", CancellationToken.None);
                        }
                        catch { }
                        finally
                        {
                            try
                            {
                                syncSend.Release();
                            }
                            catch { }
                        }
                        shutdown(null);
                        return;
                    case WebSocketState.Open:
                        break;
                    default:
                        shutdown($"WebSocket had invalid state after receive: \"{socket.State}\". Connection terminated.");
                        return;
                }

                if (!result.EndOfMessage)
                {
                    shutdown("Received message has exceedet 256 KiB.");
                    return;
                }

                if (result.MessageType != WebSocketMessageType.Text)
                {
                    shutdown($"Received message has type {result.MessageType}. But we require Text with JSON. Connection terminated.");
                    return;
                }

                try
                {
                    document = JsonDocument.Parse(recvMemory.Slice(0, result.Count), options);
                }
                catch
                {
                    shutdown($"Received message didn't contain valid JSON.");
                    return;
                }

                if (!Utils.Traverse(document.RootElement, out kind, "kind"))
                {
                    shutdown($"Received message didn't contain \"kind\". Connection terminated.");
                    return;
                }

                switch (kind)
                {
                    case "success":
                        lock (syncQuery)
                        {
                            if (!Utils.Traverse(document.RootElement, out id, "id") || !queries.TryGetValue(id, out query))
                            {
                                PushFailureEvent("Property \"id\" missing on messages from kind \"success\".");
                                break;
                            }

                            queries.Remove(id);
                        }

                        query.Answer(recv, document);
                        break;
                    case "failure":
                        if (!Utils.Traverse(document.RootElement, out code, "code"))
                        {
                            PushFailureEvent("Missing property \"code\" (number) on message from kind \"failure\".");
                            break;
                        }

                        lock (syncQuery)
                        {
                            if (!Utils.Traverse(document.RootElement, out id, "id") || !queries.TryGetValue(id, out query))
                            {
                                PushFailureEvent("Property \"id\" missing on messages from kind \"failure\".");
                                break;
                            }

                            queries.Remove(id);
                        }
                        
                        query.Answer(recv, code);
                        break;
                    case "events":
                        if (!Utils.Traverse(document.RootElement, out element, "events") || element.ValueKind != JsonValueKind.Array)
                        {
                            PushFailureEvent("\"events\" Array didn't exist in events message.");
                            break;
                        }

                        foreach (JsonElement subElement in element.EnumerateArray())
                        {
                            if (subElement.ValueKind == JsonValueKind.Object)
                            {
                                @event = EventRouter.CreateFromJson(subElement);
                                @event.Process(Group);
                                PushEvent(@event);
                            }
                            else
                                PushEvent(new RawEvent(subElement));
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Does whatever required to close the connection as properly as possible.
        /// </summary>
        /// <param name="status">The status with what we close the connection.</param>
        /// <param name="message">The message to send to the endpoint.</param>
        /// <returns>true, if the close has been executed "until the end", false if we wait for confirmation.</returns>
        internal async Task<bool> close(WebSocketCloseStatus status, string message)
        {
            switch (socket.State)
            {
                // case WebSocketState.Closed:
                // case WebSocketState.Aborted:
                // case WebSocketState.None:
                // case WebSocketState.Connecting:
                // case WebSocketState.CloseSent:
                default:
                    try
                    {
                        socket.Dispose();
                    }
                    catch {  }
                    return true;
                case WebSocketState.CloseReceived:
                    try
                    {
                        await socket.CloseAsync(status, message, CancellationToken.None);
                    }
                    catch {  }

                    try
                    {
                        socket.Dispose();
                    }
                    catch {  }
                    return true;
                case WebSocketState.Open:
                    try
                    {
                        await socket.CloseAsync(status, message, CancellationToken.None);

                        return false;
                    }
                    catch
                    {
                        try
                        {
                            socket.Dispose();
                        }
                        catch {  }
                    }
                    return true;
            }
        }

        public Query Query(string command)
        {
            Query query;
            string id = $"{tokens[rng.Next(tokens.Length)]}{tokens[rng.Next(tokens.Length)]}";

            if (queries.Count > 1000)
                throw new InvalidOperationException("It looks like you have way too many pending commands.");

            lock (syncQuery)
            {
                while (queries.ContainsKey(id))
                    id = $"{tokens[rng.Next(tokens.Length)]}{tokens[rng.Next(tokens.Length)]}";

                query = new Query(this, command, id);

                queries.Add(id, query);
            }

            return query;
        }

        public async Task Send(byte[] query, int length)
        {
            try
            {
                await syncSend.WaitAsync();
                await socket.SendAsync(new Memory<byte>(query, 0, length), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch { }
            finally
            {
                try
                {
                    syncSend.Release();
                }
                catch { }
            }
        }

        public void PushEvent(FlattiverseEvent @event)
        {
            TaskCompletionSource<FlattiverseEvent>? tcs;

            lock (syncEvents)
            {
                if (pendingEventWaiters.TryDequeue(out tcs))
                {
                    ThreadPool.QueueUserWorkItem(delegate { tcs.SetResult(@event); });
                    return;
                }

                pendingEvents.Enqueue(@event);
            }
        }

        public void PushFailureEvent(string message)
        {
            PushEvent(new FailureEvent(message));
        }

        public async Task<FlattiverseEvent> NextEvent()
        {
            FlattiverseEvent? @event;
            TaskCompletionSource<FlattiverseEvent> tcs;

            lock (syncEvents)
            {
                if (pendingEvents.TryDequeue(out @event))
                    return @event;

                tcs = new TaskCompletionSource<FlattiverseEvent>();
                pendingEventWaiters.Enqueue(tcs);
            }

            return await tcs.Task;
        }

        public void Dispose()
        {
            socket.Dispose();
            syncSend.Dispose();
        }
    }
}
