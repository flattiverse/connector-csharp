using Flattiverse;
using Flattiverse.Events;
using Flattiverse.Units;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;

class Program
{
    private static async Task Main(string[] args)
    {
        // This is the real server. You can change it back to 127.0.0.1.
        using (UniverseGroup universeGroup = new UniverseGroup("127.0.0.1", "AdminUser", false))
        //using (UniverseGroup universeGroup = new UniverseGroup("www.flattiverse.com/api/universes/beginnersGround.ws", "AdminUser", true))
        {
            await universeGroup.ConnectAsync();

            universeGroup.ConnectionClosed += Connection_ConnectionClosed;

            Universe universe = universeGroup.EnumerateUniverses().First();

            //await universe.Set(@"{""name"":""SomeUnit"",""kind"":""Sun"",""position"":{""x"":20,""y"":70},""radius"":120,""gravity"":10,""corona"":60}");

            //await universe.Set(@"{""name"":""SomeUnit"",""kind"":""Sun"",""position"":{""x"":60,""y"":70},""radius"":120,""gravity"":10,""corona"":60}");

            //await universe.Delete("SomeUnit");

            //await connection.UniverseGroup.SendBroadCastMessage("Hello there!");




            ThreadPool.QueueUserWorkItem(async delegate
            {
                while (true)
                {
                    FlattiverseEvent ev = await universeGroup.NextEvent();
                    if(ev is not TickCompleteEvent)
                        Console.WriteLine($"{ev.GetType().Name}");
                }
                
            });

            while (true)
            {
                Thread.Sleep(2000);
                await universe.Set(@"{""name"":""SomeUnit"",""kind"":""sun"",""position"":{""x"":20,""y"":70},""radius"":120,""gravity"":10,""corona"":60}");
                Thread.Sleep(2000);
                await universe.Set(@"{""name"":""SomeUnit"",""kind"":""sun"",""position"":{""x"":50,""y"":70},""radius"":120,""gravity"":10,""corona"":60}");
                Thread.Sleep(2000);
                await universe.Delete("SomeUnit");
                Thread.Sleep(2000);
                await universeGroup.SendBroadCastMessage("Some broadcast message");
                Thread.Sleep(2000);
                await universeGroup.SendUniMessage("Some broadcast message", universeGroup.EnumerateUsers().First());

            }


            Thread.Sleep(2000000);


            ;
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