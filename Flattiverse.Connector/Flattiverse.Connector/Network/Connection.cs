using System.Net.WebSockets;

namespace Flattiverse.Connector.Network
{
    class Connection
    {
        private readonly ClientWebSocket socket;

        private const string version = "0";

        private bool connected = true;

        public readonly Universe Universe;

        public Connection(Universe universe, string uri, string auth, string team)
        {
            Universe = universe;

            Uri parsedUri = new Uri($"{uri}?auth={auth}&version={version}&team={team}");

            socket = new ClientWebSocket();

            try
            {
                socket.ConnectAsync(parsedUri, CancellationToken.None).GetAwaiter().GetResult();
            }
            catch (Exception exception)
            {
                // Okay, so this is the purest bullshit of all time. We _have_ to parse the text of the exception to get any
                // idea of what went wrong. Because the WebSocket won't export http status code for us nor the content body
                // of the HTTP response.
                //
                // .NET 7 would support this, but we want to stay LTS for user libraries. :)
                // Also it's quite hard to manually upgrade a HttpClient because the response doesn't have Content.ReadAsWebSocketAsync. XD
                //
                // I guess I'm too old for this.
                //
                // HOWEVER, we try to give the best information we can to the user. :)

                switch (exception.Message.ToLower())
                {
                    case "unable to connect to the remote server":
                        throw new GameException(0xC0);
                    case "the server returned status code '401' when status code '101' was expected.":
                        throw new GameException(0xC1);
                    case "the server returned status code '412' when status code '101' was expected.":
                        throw new GameException(0xC2);
                    case "the server returned status code '417' when status code '101' was expected.":
                        throw new GameException(0xC3);
                    case "the server returned status code '415' when status code '101' was expected.":
                        throw new GameException(0xC4);
                    case "the server returned status code '409' when status code '101' was expected.":
                        throw new GameException(0xC5);
                    case "the server returned status code '502' when status code '101' was expected.":
                        throw new GameException(0xC6);
                    default:
                        throw new GameException(0xCF, exception);
                }
            }

            ThreadPool.QueueUserWorkItem(async delegate { await Recv(); });
        }
        
        private async Task Recv()
        {
            await Task.CompletedTask;
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