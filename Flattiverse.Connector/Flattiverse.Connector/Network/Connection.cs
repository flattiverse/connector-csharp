using Flattiverse.Connector.Events;
using System.Globalization;
using System.Net.WebSockets;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;
using Flattiverse.Connector.MissionSelection;

namespace Flattiverse.Connector.Network
{
    delegate void ConnectionClosed();
    delegate void PacketReceived(Packet packet);

    internal class Connection
    {
        private readonly ClientWebSocket socket;

        private readonly CancellationTokenSource cancellationSource;
        private CancellationToken cancellationToken;

        private const string version = "4";

        private object sync;

        private readonly SemaphoreSlim syncSend = new SemaphoreSlim(1, 1);
        
        private bool connected = true;
        private string? disconnectReason; // Is used to report the reason why we did lose the connection.

        public readonly Universe Universe;
        
        private ConnectionClosed closedHandler;
        private PacketReceived packetHandler;

        public Connection(Universe universe, ConnectionClosed closedHandler, PacketReceived packetHandler)
        {
            Universe = universe;
            
            socket = new ClientWebSocket();
            
            cancellationSource = new CancellationTokenSource();
            cancellationToken = cancellationSource.Token;

            this.closedHandler = closedHandler;
            this.packetHandler = packetHandler;
            
            sync = new object();
        }
        
        public async Task Connect(string uri, string auth, byte team)
        {
            Uri parsedUri;
            
            if (team > 31)
                parsedUri = new Uri($"{uri}?auth={auth}&version={version}");
            else
                parsedUri = new Uri($"{uri}?auth={auth}&version={version}&team={team}");
            
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

                if (webSocketException.Message.Length < 37)
                    throw new GameException(0xF1, webSocketException.Message, webSocketException);
                else
                    throw GameException.ParseHttpCode(webSocketException.Message.Substring(33, 3));

            }
            catch (Exception exception)
            {
                throw new GameException(0xF1, exception.Message, exception);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = originalCulture;
            }

            ThreadPool.QueueUserWorkItem(async delegate { await Recv(); });
        }

        /// <summary>
        /// Sets all variables and cleanups everything accordingly if this connection ends. The closure of the
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
            syncSend.Dispose();

            try
            {
                closedHandler();
            }
            catch { }
        }
        
        private async Task Recv()
        {
            byte[] data = GC.AllocateUninitializedArray<byte>(4194304, true);
            int position = 0;
            
            WebSocketReceiveResult result;

            while (true)
            {
                try
                {
                    result = await socket.ReceiveAsync(new ArraySegment<byte>(data), cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Terminate($"The connection got closed unexpectedly: {e.Message}");

                    return;
                }

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

                            Terminate(
                                "Protocol error, the server did send a text message which is not supported. Do you have the latest connector version?");

                            return;
                    }

                    Terminate(
                        "Unknown error, the server did send a unknown message datagram which is not supported. Do you have the latest connector version?");

                    return;
                }

                if (!result.EndOfMessage)
                {
                    try
                    {
                        await socket.CloseAsync(WebSocketCloseStatus.MessageTooBig,
                            "The message you did send exceeded the limit of 262144 bytes.", cancellationToken);
                    }
                    catch
                    {
                    }

                    Terminate("Protocol error, the server did send a packet which is too big.");

                    return;
                }

                position = 0;

                while (position < result.Count)
                    try
                    {
                        packetHandler(new Packet(data, ref position));
                    }
                    catch (Exception exception)
                    { }
            }
        }

        /// <summary>
        /// Sends a packet.
        /// </summary>
        /// <param name="packet">The packet to send.</param>
        public async Task Send(Packet packet)
        {
            if (!connected)
                return;
            
            packet.CopyHeader();

            await syncSend.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                await socket.SendAsync(new ArraySegment<byte>(packet.Payload, 0, packet.Offset + packet.Header.Size),
                    WebSocketMessageType.Binary, true, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Terminate($"The connection got closed unexpectedly: {e.Message}");
            }
            finally
            {
                syncSend.Release();
            }
        }
        
        /// <summary>
        /// true, if the connection is still established.
        /// </summary>
        public bool Connected => connected;

        /// <summary>
        /// The reason why we did disconnect from the server.
        /// </summary>
        public string? DisconnectReason => disconnectReason;
    }
}