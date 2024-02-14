using System.Runtime.CompilerServices;
using Flattiverse.Connector;

internal class Program
{
    private static async Task Main(string[] args)
    {
        // A new beginning and a test - again.

        string apiKey = "auth";
        Universe universe = new Universe();

<<<<<<< Updated upstream
        Galaxy galaxy = await universe.Join("ws://127.0.0.1:5000", apiKey, 0x00);

        await galaxy.SendMessage(1, "This is a message.");
=======
        Galaxy galaxy = await universe.Join("ws://127.0.0.1:5000", "0000000000000000000000000000000000000000000000000000000000000000", 0x00);
>>>>>>> Stashed changes

        await Task.Delay(60000);
    }
}