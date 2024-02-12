using System.Globalization;
using System.Net.WebSockets;

namespace Flattiverse.Connector.Network
{
    class Connection
    {
        private readonly ClientWebSocket socket;

        private readonly CancellationTokenSource cancellationSource;
        private CancellationToken cancellationToken;

        private const string version = "0";

        private object sync;
        
        private bool connected = true;
        private string? disconnectReason; // Is used to report the reason why we did lose the connection.

        public readonly Universe Universe;

        public Connection(Universe universe)
        {
            Universe = universe;
            
            socket = new ClientWebSocket();
            
            cancellationSource = new CancellationTokenSource();
            cancellationToken = cancellationSource.Token;

            sync = new object();
        }
        
        public async Task Connect(string uri, string auth, byte team)
        {
            Uri parsedUri = new Uri($"{uri}?auth={auth}&version={version}&team={team}");

            CultureInfo originalCulture = Thread.CurrentThread.CurrentCulture;
            
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            try
            {
                await socket.ConnectAsync(parsedUri, cancellationToken).ConfigureAwait(false);
            }
            catch (WebSocketException webSocketException)
            {
                // The .NET framework, or specifically the ClientWebSocket class, is very disappointing at this point:
                // It is not possible to request the HTTP body upon a rejection of the connection upgrade, nor to easily
                // and securely query the HTTP error code.

                switch (webSocketException.Message.ToLower())
                {
                    case "unable to connect to the remote server":
                        throw new GameException(0xF1);
                    case "the server returned status code '502' when status code '101' was expected.":
                    case "the server returned status code '504' when status code '101' was expected.":
                        throw new GameException(0xF2);
                    default:
                        throw new GameException(0xF0, webSocketException.Message, webSocketException);
                }
            }
            catch (Exception exception)
            {
                throw new GameException(0xF0, exception.Message, exception);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = originalCulture;
            }

            ThreadPool.QueueUserWorkItem(async delegate { await Recv(); });
        }

        /// <summary>
        /// Sets all variables and cleanupps everything accordingly if this connection ends. The closure of the
        /// web socket is not part of this method.
        /// </summary>
        /// <param name="reason">The reason for the closure.</param>
        private void Terminate(string? reason)
        {
            lock (sync)
                if (connected)
                    connected = false;
                else
                    return;

            disconnectReason ??= reason;
            
            cancellationSource.Dispose();
            socket.Dispose();
        }
        
        private async Task Recv()
        {
            Packet packet;
            byte[] data = GC.AllocateUninitializedArray<byte>(262144, true);
            int position = 0;
            
            WebSocketReceiveResult result;

            while (true)
            {
                try
                {
                    result = await socket.ReceiveAsync(new ArraySegment<byte>(data), cancellationToken).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Terminate($"The connection got closed unexpectedly: {e.Message}");
                    
                    return;
                }

                //Console.WriteLine($"{DateTime.UtcNow:yyyy.MM.dd HH:mm:ss.fff} recv: count={result.Count}, messageType={result.MessageType}.");

                if (result.MessageType != WebSocketMessageType.Binary)
                {
                    switch (result.MessageType)
                    {
                        case WebSocketMessageType.Close:

                            if (result.CloseStatus.HasValue)
                                switch (result.CloseStatus.Value)
                                {
                                    default:
                                        if (result.CloseStatusDescription is null)
                                            disconnectReason ??=
                                                $"Flattiverse server disconnected with reason: {result.CloseStatus.Value}.";
                                        else
                                            disconnectReason ??=
                                                $"Flattiverse server disconnected with reason: [{result.CloseStatus.Value}]: {result.CloseStatusDescription}";
                                        break;
                                }

                            try
                            {
                                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Goodbye.",
                                    cancellationToken);
                            }
                            catch
                            {
                            }

                            Terminate(null);

                            return;
                        case WebSocketMessageType.Text:
                            try
                            {
                                await socket.CloseAsync(WebSocketCloseStatus.InvalidMessageType,
                                    "Can't understand text messages.", cancellationToken);
                            }
                            catch
                            {
                            }

                            Terminate("Protocol error, the server did send a text message which is not supported. Do you have the latest connector version?");

                            return;
                    }

                    Terminate("Unknown error, the server did send a unknown message datagram which is not supported. Do you have the latest connector version?");
                    
                    return;
                }

                if (!result.EndOfMessage)
                {
                    try
                    {
                        await socket.CloseAsync(WebSocketCloseStatus.MessageTooBig,
                            "The message you did send exceeded the limit of 262144 bytes.", cancellationToken);
                    }
                    catch { }

                    Terminate("Protocol error, der server did send a packet which is too big.");
                
                    return;
                }

                position = 0;

                while (position < result.Count)
                {
                    packet = new Packet(data, ref position);
                    
                    Console.Write(packet);

                    PacketReader reader = packet.Read();

                    Console.WriteLine($": {reader.ReadInt32()}, {reader.ReadInt32()}.");
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
                    catch { }
                    return true;
                case WebSocketState.CloseReceived:
                    try
                    {
                        await socket.CloseAsync(status, message, CancellationToken.None);
                    }
                    catch { }

                    try
                    {
                        socket.Dispose();
                    }
                    catch { }
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
                        catch { }
                    }
                    return true;
            }
        }
    }
}