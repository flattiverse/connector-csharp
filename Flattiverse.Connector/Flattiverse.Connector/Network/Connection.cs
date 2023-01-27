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
        private static readonly JsonDocumentOptions options = new JsonDocumentOptions() { AllowTrailingCommas = false, CommentHandling = JsonCommentHandling.Disallow, MaxDepth = 4 };
        private readonly ClientWebSocket socket;

        private readonly object querySync = new object();
        private readonly Dictionary<string, Query> queries = new Dictionary<string, Query>();

        private readonly Random rng = new Random();
        private static readonly char[] tokens = Utils.GenerateAllAllowedOneByteUtf8CharsWithoutSpace();

        private readonly Queue<FlattiverseEvent> pendingEvents = new Queue<FlattiverseEvent>();
        private readonly Queue<TaskCompletionSource<FlattiverseEvent>> pendingEventWaiters = new Queue<TaskCompletionSource<FlattiverseEvent>>();
        private readonly object syncEvents = new object();

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

            while (true)
            {
                recv = ArrayPool<byte>.Shared.Rent(262144);
                recvMemory = new Memory<byte>(recv);

                try
                {
                    result = await socket.ReceiveAsync(recvMemory, CancellationToken.None);
                }
                catch
                {
                    socket.Dispose();
                    // Hie rirgendwie den Spieler informieren, dass jetzt alles Scheiße ist.
                    return;
                }

                switch (socket.State)
                {
                    case WebSocketState.CloseReceived:
                        if (await close(WebSocketCloseStatus.NormalClosure, "Server requested to close the connection."))
                            return;
                        else
                            continue;
                    case WebSocketState.Open:
                        break;
                    default:
                        if (await close(WebSocketCloseStatus.NormalClosure, $"Invalid WebSocket state: {socket.State}."))
                            return;
                        else
                            continue;
                }

                if (!result.EndOfMessage)
                    if (await close(WebSocketCloseStatus.MessageTooBig, "Sent messages can't exceed 256 KiB and can't be split up."))
                        return;
                    else
                        continue;

                if (result.MessageType != WebSocketMessageType.Text)
                    if (await close(WebSocketCloseStatus.InvalidMessageType, "Require JSON/Text frame."))
                        return;
                    else
                        continue;

                try
                {
                    document = JsonDocument.Parse(recvMemory.Slice(0, result.Count), options);
                }
                catch (Exception exception)
                {
                    if (await close(WebSocketCloseStatus.InvalidPayloadData, $"Messages must consist of valid JSON data containing a valid command: {exception.Message}"))
                        return;
                    else
                        continue;
                }

                if (!Utils.Traverse(document.RootElement, out kind, "kind"))
                    if (await close(WebSocketCloseStatus.InvalidPayloadData, $"Ni kind received."))
                        return;
                    else
                        continue;

                switch (kind)
                {
                    case "success":
                        if (!Utils.Traverse(document.RootElement, out id, "id") || !queries.TryGetValue(id, out query))
                        {
                            PushFailureEvent("Property \"id\" missing on messages from kind \"success\".");
                            break;
                        }

                        query.Answer(recv, document);
                        break;
                    case "failure":
                        if (!Utils.Traverse(document.RootElement, out code, "code"))
                        {
                            PushFailureEvent("Missing property \"code\" (number) on message from kind \"failure\".");
                            break;
                        }

                        if (!Utils.Traverse(document.RootElement, out id, "id") || !queries.TryGetValue(id, out query))
                        {
                            PushFailureEvent("Property \"id\" missing on messages from kind \"failure\".");
                            break;
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
                                PushEvent(EventRouter.CreateFromJson(subElement));
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
                        await socket.CloseOutputAsync(status, message, CancellationToken.None);

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

            lock (querySync)
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
            await socket.SendAsync(new Memory<byte>(query, 0, length), WebSocketMessageType.Text, true, CancellationToken.None);
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
        }
    }
}
