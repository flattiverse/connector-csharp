using System.Diagnostics;
using System.Net.Sockets;
using System.Net.WebSockets;
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
            string? command;
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
                    throw new Exception($"Exception in ReceiveAsync: {ex}");
                }

                if (client.State == WebSocketState.CloseReceived)
                {
                    connected = false;
                    Console.WriteLine($"Connection closed due to {client.State}.");

                    try
                    {
                        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Thank for choosing deutsche bahn", CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"fatal failed in CloseAsync: {ex.Message}");
                    }

                    return;
                }
                else if (client.State != WebSocketState.Open)
                {
                    connected = false;
                    throw new Exception($"Connection closed due to {client.State}.");
                }

                if (!result.EndOfMessage)
                {
                    if (result.Count == recvBufferLimit)
                        throw new Exception("Messages can't exceed 16KB.");
                    else
                        throw new Exception("You can't split up messages manually.");
                }

                if (result.MessageType != WebSocketMessageType.Text)
                {
                    throw new Exception("Only messages of type text are allowed.");
                }

                if (result.Count == 0)
                {
                    throw new Exception("Empty messages aren't allowed.");
                }

                try
                {
                    document = JsonDocument.Parse(recvMemory.Slice(0, result.Count), jsonDocOptions);
                }
                catch (Exception exception)
                {
                    throw new Exception($"Ambiguous JSON content: {exception}.");
                }

                if (document.RootElement.ValueKind != JsonValueKind.Object)
                {
                    throw new Exception($"Root element needs to be a JSON object. Received instead: {document.RootElement.ValueKind}.");
                }

                if (document.RootElement.TryGetProperty("id", out element))
                {

                    if (element.ValueKind != JsonValueKind.String)
                    {
                        throw new Exception($"id property must be a string. You sent {element.ValueKind}.");
                    }

                    id = element.GetString();

                    if (string.IsNullOrWhiteSpace(id))
                    {
                        throw new Exception($"id can't be null or empty.");
                    }

                    blockManager.Answer(id, document);

                    continue;
                }

                if (document.RootElement.TryGetProperty("message", out element))
                {

                    if (element.ValueKind != JsonValueKind.String)
                    {
                        throw new Exception($"id message must be a string. You sent {element.ValueKind}.");
                    }

                    string message = element.GetString();

                    if (string.IsNullOrWhiteSpace(message))
                    {
                        throw new Exception($"message can't be null or empty.");
                    }

                    //do broadcasting messages here
                    continue;
                }

                //something unexpected happened
                if (document.RootElement.TryGetProperty("fatal", out element))
                {

                    if (element.ValueKind != JsonValueKind.String)
                    {
                        throw new Exception($"id message must be a string. You sent {element.ValueKind}.");
                    }

                    string message = element.GetString();

                    throw new Exception($"Fatal excetion: {message}");
                }

                throw new Exception($"Something unexpected happened");

            }
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
