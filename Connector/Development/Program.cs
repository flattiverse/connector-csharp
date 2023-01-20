using Flattiverse;
using System.Net.WebSockets;
using System.Text;

class Program
{
    private static async Task Main(string[] args)
    {
        // This is the real server. You can change it back to 127.0.0.1.
        using (Connection connection = new Connection("127.0.0.1", "TestUser", false))
        //using (Connection connection = new Connection("80.255.8.76/api/universes/beginnersGround.ws", "TestUser", false))
        {
            await connection.ConnectAsync();

            connection.ConnectionClosed += Connection_ConnectionClosed;


            Universe universe;

            if (!connection.UniverseGroup.TryGet(0, out universe))
                throw new Exception("Default Universe not found.");

            //await universe.Set(@"{""name"":""SomeUnit"",""kind"":""Sun"",""position"":{""x"":20,""y"":70},""radius"":120,""gravity"":10,""corona"":60}");

            //await universe.Set(@"{""name"":""SomeUnit"",""kind"":""Sun"",""position"":{""x"":60,""y"":70},""radius"":120,""gravity"":10,""corona"":60}");

            //await universe.Delete("SomeUnit");

            Thread.Sleep(20000);



        }

    }

    private static void Connection_ConnectionClosed(Exception? ex)
    {
        ;
    }

    //public async void test()
    //{
    //    var client = new ClientWebSocket();

    //    await client.ConnectAsync(new Uri("ws://127.0.0.1"), CancellationToken.None);

    //    ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[4096]);

    //    while (client.State == WebSocketState.Open)
    //    {
    //        Console.Write("Request: ");

    //        var sendBuffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes("{\"command\":\"OhWhat\",\"id\":\"asd\",\"str\":\"string\"}"));
    //        await client.SendAsync(sendBuffer, WebSocketMessageType.Text, true, CancellationToken.None);

    //        Console.ForegroundColor = ConsoleColor.White;

    //        var result = await client.ReceiveAsync(buffer, CancellationToken.None);
    //        var message = Encoding.UTF8.GetString(buffer.Array!, 0, result.Count);
    //        Console.WriteLine(message);

    //        Console.ForegroundColor = ConsoleColor.Gray;

    //        break;
    //    }

    //    await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
    //}
}