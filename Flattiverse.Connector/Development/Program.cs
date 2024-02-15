using System.Runtime.CompilerServices;
using Flattiverse.Connector;

internal class Program
{
    private static async Task Main(string[] args)
    {
        // A new beginning and a test - again.

        Universe universe = new Universe();

        Galaxy galaxy = await universe.Join("ws://127.0.0.1:5000", "F7160CB2E108EF022D317AECFF3DF958EB2C4437D5A24DA7E18EC727236A1AE5", 0x00);

        //await galaxy.SendMessage(1, "This is a message.");

        Random rng = new Random();

        for (int i = 0; i < 10; i++)
        {
            int number = rng.Next();

            if (await galaxy.IsEven(number))
                Console.WriteLine($"Number {number,11} is even");
            else
                Console.WriteLine($"Number {number,11} is not even");
        }

        await Task.Delay(60000);
    }
}