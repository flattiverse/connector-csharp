using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Reflection.Metadata;
using System.Runtime.Serialization;
using System.Text.Json;

namespace Flattiverse
{
    public class Connection : IDisposable
    {
        private ClientWebSocket client;

        private Uri uri;

        public readonly UniverseGroup UniverseGroup;

        internal readonly BlockManager blockManager;

        internal bool connected;

        internal static readonly JsonWriterOptions jsonOptions = new JsonWriterOptions() { Indented = false };

        internal static readonly JsonDocumentOptions jsonDocOptions = new JsonDocumentOptions() { AllowTrailingCommas = false, CommentHandling = JsonCommentHandling.Disallow, MaxDepth = 4 };

        private static int recvBufferLimit = 262144;

        public delegate void ConnectionHandler(Exception? ex);

        public event ConnectionHandler? ConnectionClosed;
       

        public Connection(string host, string apiKey) 
        {
            client = new ClientWebSocket();
            uri = new Uri($"ws://{host}?auth={apiKey}");
            blockManager = new BlockManager(this);

            UniverseGroup = new UniverseGroup(this);
            
        }

        public async Task ConnectAsync()
        {
            await client.ConnectAsync(uri, CancellationToken.None);
            connected = true;

            UniverseGroup.addUniverse(0);

            ThreadPool.QueueUserWorkItem(delegate { socketWork(); });
        }

        internal async Task SendCommand(string command, string blockId, List<CommandParameter> parameters)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (Utf8JsonWriter writer = new Utf8JsonWriter(ms, jsonOptions))
                {
                    writer.WriteStartObject();

                    writer.WriteString("command", command);
                    writer.WriteString("id", blockId);

                    foreach (CommandParameter cp in parameters)
                        cp.WriteJson(writer);

                    writer.WriteEndObject();
                }

                await client.SendAsync(ms.ToArray(), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        internal async void socketWork()
        {
            byte[] recv = new byte[recvBufferLimit];
            ValueWebSocketReceiveResult result;
            Memory<byte> recvMemory = new Memory<byte>(recv);
            JsonDocument document;
            JsonElement element;
            string? id;

            while (true)
            {
                try
                {
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

                if (document.RootElement.TryGetProperty("id", out element))
                {

                    if (element.ValueKind != JsonValueKind.String)
                    {
                        await payloadExceptionSocketClose(new Exception($"Id property must be a string. Received {element.ValueKind}."));
                        return;
                    }

                    id = element.GetString();

                    if (string.IsNullOrWhiteSpace(id))
                    {
                        await payloadExceptionSocketClose(new Exception($"Id can't be null or empty."));
                        return;
                    }

                    blockManager.Answer(id, document);

                    continue;
                }

                if (document.RootElement.TryGetProperty("message", out element))
                {

                    if (element.ValueKind != JsonValueKind.String)
                    {
                        await payloadExceptionSocketClose(new Exception($"Message must be a string. You sent {element.ValueKind}."));
                        return;
                    }

                    string? message = element.GetString();

                    if (string.IsNullOrWhiteSpace(message))
                    {
                        await payloadExceptionSocketClose(new Exception($"Message can't be null or empty."));
                        return;
                    }

                    //do broadcasting messages here
                    continue;
                }

                //something unexpected happened
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

                await fatalExceptionSocketClose(new Exception($"Unkown json format received."));
                return;
            }
        }

        internal void ConnectionClose(Exception? exception)
        {
            if (ConnectionClosed != null)
                ConnectionClosed(exception);
        }

        private async Task payloadExceptionSocketClose(Exception ex)
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
