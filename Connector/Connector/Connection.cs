using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Flattiverse
{
    public class Connection : IDisposable
    {
        private ClientWebSocket client;

        private Uri uri;

        private Thread socketWorker;

        public readonly Units UnitManager;

        internal readonly BlockManager blockManager;

        internal bool connected;

        private static readonly JsonDocumentOptions options = new JsonDocumentOptions() { AllowTrailingCommas = false, CommentHandling = JsonCommentHandling.Disallow, MaxDepth = 4 };

        public Connection(string host, string apiKey) 
        {
            client = new ClientWebSocket();
            uri = new Uri($"ws://{host}?auth={apiKey}");
            socketWorker = new Thread(socketWork);
            UnitManager = new Units(this);
            blockManager  = new BlockManager(this);
        }

        public async Task ConnectAsync()
        {
            await client.ConnectAsync(uri, CancellationToken.None);
            connected = true;
            socketWorker.Start();
        }

        internal async Task SendCommand(Packet packet)
        {
            await client.SendAsync(packet.Compile(), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        internal async void socketWork()
        {
            byte[] recv = new byte[16384];
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
                    if (result.Count == 16384)
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
                    document = JsonDocument.Parse(recvMemory.Slice(0, result.Count), options);
                }
                catch (Exception exception)
                {
                    throw new Exception($"Ambiguous JSON content: {exception}.");
                }

                if (document.RootElement.ValueKind != JsonValueKind.Object)
                {
                    throw new Exception($"Root element needs to be a JSON object. Received instead: {document.RootElement.ValueKind}.");
                }

                //if (!document.RootElement.TryGetProperty("command", out element))
                //{
                //    throw new Exception("command property is missing.");
                //}

                //if (element.ValueKind != JsonValueKind.String)
                //{
                //    throw new Exception($"command property must be a string. You sent {element.ValueKind}.");
                //}

                //command = element.GetString();

                //if (string.IsNullOrWhiteSpace(command))
                //{
                //    throw new Exception($"command can't be null or empty.");
                //}

                if (!document.RootElement.TryGetProperty("id", out element))
                {
                    throw new Exception($"id property is missing.");
                }

                if (element.ValueKind != JsonValueKind.String)
                {
                    throw new Exception($"id property must be a string. You sent {element.ValueKind}.");
                }

                id = element.GetString();

                if (string.IsNullOrWhiteSpace(id))
                {
                    throw new Exception($"id can't be null or empty.");
                }

                blockManager.Answer(new Packet(id, document));
            }
        }


        public async void Dispose()
        {
            if(client.State == WebSocketState.Open)
                await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        }
    }
}
