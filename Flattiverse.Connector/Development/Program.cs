using System.Runtime.CompilerServices;
using Flattiverse.Connector;

internal class Program
{
    private static async Task Main(string[] args)
    {        
        // A new beginning and a test - again.

        Universe universe = new Universe();

        Galaxy galaxy = await universe.Join("ws://127.0.0.1:5000", "auth", 0x00);

        await Task.Delay(60000);
    }
}