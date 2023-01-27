﻿using Flattiverse.Events;
using System.Buffers;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Reflection;
using System.Text.Json;

namespace Flattiverse
{
    internal class Connection : IDisposable
    {
        private ClientWebSocket client;

        private Uri uri;

        public readonly UniverseGroup UniverseGroup;

        internal readonly BlockManager blockManager;

        internal bool connected;

        internal static readonly JsonWriterOptions jsonOptions = new JsonWriterOptions() { Indented = false };

        internal static readonly JsonDocumentOptions jsonDocOptions = new JsonDocumentOptions() { AllowTrailingCommas = false, CommentHandling = JsonCommentHandling.Disallow, MaxDepth = 8 };

        private static int recvBufferLimit = 262144;

        private static readonly Dictionary<string, MethodInfo> commands = new Dictionary<string, MethodInfo>();

        private static bool initialized = false;

        internal Connection(Uri host, UniverseGroup universeGroup)
        {
            client = new ClientWebSocket();
            uri = host;
            blockManager = new BlockManager(this);

            UniverseGroup = universeGroup;
        }

        internal Connection(string host, string apiKey, bool https, UniverseGroup universeGroup) 
        {
            client = new ClientWebSocket();

            if (https)
                uri = new Uri($"wss://{host}?auth={apiKey}");
            else
                uri = new Uri($"ws://{host}?auth={apiKey}");

            blockManager = new BlockManager(this);
            UniverseGroup = universeGroup;
        }

        internal async Task ConnectAsync()
        {
            await client.ConnectAsync(uri, CancellationToken.None);
            connected = true;

            ThreadPool.QueueUserWorkItem(delegate { socketWork(); });

            await UniverseGroup.initialTcs.Task;
        }

        internal async Task SendCommand(string command, string blockId, List<ClientCommandParameter> parameters)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (Utf8JsonWriter writer = new Utf8JsonWriter(ms, jsonOptions))
                {
                    writer.WriteStartObject();

                    writer.WriteString("command", command);
                    writer.WriteString("id", blockId);

                    foreach (ClientCommandParameter cp in parameters)
                        cp.WriteJson(writer);

                    writer.WriteEndObject();
                }

                //ms.Position = 0;
                //string debugText = new StreamReader(ms).ReadToEnd();

                await client.SendAsync(ms.ToArray(), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        internal async Task SendPong()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (Utf8JsonWriter writer = new Utf8JsonWriter(ms))
                {
                    writer.WriteStartObject();

                    writer.WriteString("command", "pong");
                    writer.WriteString("id", "0");
                    writer.WriteString("ticksAsString", DateTime.UtcNow.Ticks.ToString());

                    writer.WriteEndObject();
                }

                await client.SendAsync(ms.ToArray(), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        internal async void socketWork()
        {
            //byte[] recv = new byte[recvBufferLimit];
            //Memory<byte> recvMemory = new Memory<byte>(recv);

            byte[] recv = null;
            Memory<byte> recvMemory = default;

            ValueWebSocketReceiveResult result;
            JsonDocument document;
            JsonElement element;

            bool rentNew = true;


            while (true)
            {
                try
                {
                    if (rentNew)
                    {
                        recv = ArrayPool<byte>.Shared.Rent(recvBufferLimit);
                        recvMemory = new Memory<byte>(recv);
                        rentNew = false;
                    }

                    result = await client.ReceiveAsync(recvMemory, CancellationToken.None);
                }
                catch (Exception ex)
                {
                    connected = false;
                    blockManager.ConnectionClosed(new Exception($"Exception in ReceiveAsync: {ex.Message}"));
                    return;
                }

                if (client.State == WebSocketState.CloseReceived)
                {
                    connected = false;

                    try
                    {
                        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Normal closure", CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        blockManager.ConnectionClosed(new Exception($"Fatal exception in CloseAsync: {ex.Message}"));
                        return;
                    }

                    return;
                }
                else if (client.State != WebSocketState.Open)
                {
                    connected = false;
                    blockManager.ConnectionClosed(new Exception($"Connection closed due to {client.State}."));
                    return;
                }

                if (!result.EndOfMessage)
                {

                    connected = false;

                    Exception e;

                    if (result.Count == recvBufferLimit)
                        e = new Exception($"Connection closed due to Messages can't exceed 256.");
                    else
                        e = new Exception($"Connection closed due to split Message.");

                    await payloadExceptionSocketClose(e);

                    return;
                }

                if (result.MessageType != WebSocketMessageType.Text)
                {
                    connected = false;
                    await payloadExceptionSocketClose(new Exception($"Connection closed due to Messages not being of type text."));
                    return;
                }

                if (result.Count == 0)
                {
                    connected = false;
                    await payloadExceptionSocketClose(new Exception($"Connection closed due to Messages being empty."));
                    return;
                }

                try
                {
                    document = JsonDocument.Parse(recvMemory.Slice(0, result.Count), jsonDocOptions);
                }
                catch (Exception exception)
                {
                    connected = false;
                    await payloadExceptionSocketClose(new Exception($"Connection closed due to Ambiguous JSON content: {exception}."));
                    return;
                }

                if (document.RootElement.ValueKind != JsonValueKind.Object)
                {
                    connected = false;
                    await payloadExceptionSocketClose(new Exception($"Connection closed due Root element needs to be a JSON object. Received instead: {document.RootElement.ValueKind}."));
                    return;
                }

                if (document.RootElement.TryGetProperty("kind", out element))
                {

                    if (element.ValueKind != JsonValueKind.String)
                    {
                        await payloadExceptionSocketClose(new Exception($"kind property must be a number. Received {element.ValueKind}."));
                        return;
                    }

                    string kind = element.GetString()!;

                    if (string.IsNullOrWhiteSpace(kind))
                    {
                        await payloadExceptionSocketClose(new Exception($"kind property can't be null or empty."));
                        return;
                    }

                    switch (kind)
                    {
                        case "success":
                        case "error":
                            if(!tryGetId(document, out string? blockId, out Exception? ex))
                            {
                                await payloadExceptionSocketClose(ex);
                                return;
                            }

                            rentNew = true;
                            blockManager.Answer(blockId!, recv!, document);

                            continue;
                        case "events":
                            if (document.RootElement.TryGetProperty("payload", out element))
                            {
                                if (element.ValueKind != JsonValueKind.Array)
                                {
                                    await payloadExceptionSocketClose(new Exception($"Payload must be a Array. You sent {element.ValueKind}."));
                                    return;
                                }

                                try
                                {
                                    if (!Utils.Traverse(document.RootElement, out int tick, "tick"))
                                        throw new Exception("Event does not contain a valid tick property.");

                                    foreach (JsonElement fvEvent in element.EnumerateArray())
                                        UniverseGroup.pushEvent(FlattiverseEvent.parse(this, fvEvent, tick));

                                    if (!initialized)
                                    {
                                        ThreadPool.QueueUserWorkItem(delegate { UniverseGroup.initialTcs.SetResult(); });
                                        initialized= true;
                                    }

                                    continue;
                                }
                                catch (Exception parseEx)
                                {
                                    await payloadExceptionSocketClose(parseEx);
                                    return;
                                }
                            }
                            else
                            {
                                await payloadExceptionSocketClose(new Exception("Events do not contain a payload."));
                                return;
                            }
                        //case "ping":
                        //    try
                        //    {
                        //        await SendPong();
                        //    }
                        //    catch (Exception pongEx)
                        //    {
                        //        await payloadExceptionSocketClose(pongEx);
                        //        return;
                        //    }
                        //    break;
                        default:
                            await payloadExceptionSocketClose(new Exception($"Unkown message kind: {kind}."));
                            return;
                    }
                }

                if (document.RootElement.TryGetProperty("fatal", out element))
                {

                    if (element.ValueKind != JsonValueKind.String)
                    {
                        await payloadExceptionSocketClose(new Exception($"Fatal must be a string. You sent {element.ValueKind}."));
                        return;
                    }

                    string? fatal = element.GetString();

                    if (string.IsNullOrWhiteSpace(fatal))
                    {
                        await payloadExceptionSocketClose(new Exception($"Fatal can't be null or empty."));
                        return;
                    }

                    await fatalExceptionSocketClose(new Exception($"Fatal excetion: {fatal}"));
                    return;
                }
            }
        }

        private bool tryGetId(JsonDocument document, out string? id, out Exception? ex)
        {
            JsonElement element;
            id = null;
            ex = null;

            if (document.RootElement.TryGetProperty("id", out element))
            {
                if (element.ValueKind != JsonValueKind.String)
                {
                    ex = new Exception($"Id property must be a string. Received {element.ValueKind}.");
                    return false;
                }

                id = element.GetString();

                if (string.IsNullOrWhiteSpace(id))
                {
                    ex = new Exception($"Id can't be null or empty.");
                    return false;
                }
            }

            return true;
        }

        internal async Task payloadExceptionSocketClose(Exception? ex)
        {
            blockManager.ConnectionClosed(ex);

            try
            {
                await client.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Closed because of an exception", CancellationToken.None);
            }
            catch { }
        }

        private async Task fatalExceptionSocketClose(Exception ex)
        {
            blockManager.ConnectionClosed(ex);

            try
            {
                await client.CloseAsync(WebSocketCloseStatus.InternalServerError, "Closed because of an exception", CancellationToken.None);
            }
            catch { }
        }

        internal static bool responseSuccess(JsonDocument response, out string? error)
        {
            if (response.RootElement.ValueKind != JsonValueKind.Object)
                throw new Exception($"Response error: Root element needs to be a JSON object. Received instead: {response.RootElement.ValueKind}.");

            JsonElement element;
            if (!response.RootElement.TryGetProperty("kind", out element))
                throw new Exception("Response error: kind property is missing.");

            if (element.ValueKind != JsonValueKind.String)
                throw new Exception($"Response error: kind property must be a string. You sent {element.ValueKind}.");

            string kind = element.GetString()!;

            if(string.IsNullOrEmpty(kind))
                throw new Exception($"Response error: kind property cant be empty.");

            if (kind == "success")
            {
                error = null;
                return true;
            }

            if (!response.RootElement.TryGetProperty("result", out element))
                throw new Exception("Response error: result property is missing.");

            if (element.ValueKind != JsonValueKind.String)
                throw new Exception($"Response error: result property must be a string. You sent {element.ValueKind}.");

            string result = element.GetString()!;

            if (string.IsNullOrEmpty(kind))
                throw new Exception($"Response error: result property cant be empty.");

            error = result;
            return false;
        }

        public void Dispose()
        {
            try
            {
                if (client.State == WebSocketState.Open)
                    client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None).Wait();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error while disposing connection: {e.Message}");
            }
        }
    }
}