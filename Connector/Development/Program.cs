using Flattiverse;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

class Program
{
    private static async Task Main(string[] args)
    {        
        using (Connection connection = new Connection("127.0.0.1" , "TestUser"))
        {
            await connection.ConnectAsync();

            //await connection.UnitManager.Create(0, 0, "test", UnitKind.Sun, 20, 70, 120, 10, 60);

            //await connection.UnitManager.Update(0, 0, "test", UnitKind.Sun, 20, 70, 200, 10, 60);

            //await connection.UnitManager.Delete();




        }








    }

    public async void test()
    {
        var client = new ClientWebSocket();

        await client.ConnectAsync(new Uri("ws://127.0.0.1"), CancellationToken.None);

        ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[4096]);

        while (client.State == WebSocketState.Open)
        {
            Console.Write("Request: ");

            byte[] sendBuffer;

            using (MemoryStream stream = new MemoryStream())
            using (Utf8JsonWriter writer = new Utf8JsonWriter(stream))
            {
                writer.WriteStartObject();
                writer.WriteString("command", "createunit");
                writer.WriteString("id", "abc");

                writer.WriteStartObject("data");
                writer.WriteNumber("universe", 0);

                writer.WriteStartObject("unit");
                writer.WriteString("name", "test");
                writer.WriteString("kind", "Sun");

                writer.WriteStartObject("position");
                writer.WriteNumber("x", 12);
                writer.WriteNumber("y", 89);
                writer.WriteEndObject();

                writer.WriteNumber("radius", 99);
                writer.WriteNumber("corona", 23);
                writer.WriteNumber("gravity", 329);

                writer.WriteEndObject();

                writer.WriteEndObject();
                writer.WriteEndObject();
                writer.Flush();

                sendBuffer = stream.ToArray();
            }

            await client.SendAsync(sendBuffer, WebSocketMessageType.Text, true, CancellationToken.None);

            Console.ForegroundColor = ConsoleColor.White;

            var result = await client.ReceiveAsync(buffer, CancellationToken.None);
            var message = Encoding.UTF8.GetString(buffer.Array!, 0, result.Count);
            Console.WriteLine(message);

            Console.ForegroundColor = ConsoleColor.Gray;

            break;
        }

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        return;
    }
}