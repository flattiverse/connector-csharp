using System.Runtime.CompilerServices;
using Flattiverse.Connector;

internal class Program
{
    private static async Task Main(string[] args)
    {
        // A new beginning and a test - again.

        string apiKey = "auth";
        Universe universe = new Universe();

        Galaxy galaxy = await universe.Join("ws://127.0.0.1:5000", "F7160CB2E108EF022D317AECFF3DF958EB2C4437D5A24DA7E18EC727236A1AE5", 0x00);

        await galaxy.SendMessage(1, "This is a message.");

        await Task.Delay(60000);
    }
}