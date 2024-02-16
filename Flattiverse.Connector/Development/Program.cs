using System.Runtime.CompilerServices;
using Flattiverse.Connector;
using Flattiverse.Connector.Hierarchy;

internal class Program
{
    private static async Task Main(string[] args)
    {
        // A new beginning and a test - again.

        Universe universe = new Universe();

        Console.WriteLine("Starting join request");

        // Admin key / local host
        //Galaxy galaxy = await universe.Join("ws://127.0.0.1:5000", "68B59217071450554F85DB121F89EC31C54E427D04A8EE16AC72C52AED806631", 0x00);

        // Player key / online
        //Galaxy galaxy = await universe.Join("ws://www.flattiverse.com/game/galaxies/0", "CE43AE41B96111DB66D75AB943A3042755B98F10E6A09AF0D4190B0FFEC13EE8", 0x00);
        Galaxy galaxy = await universe.Join("ws://127.0.0.1:5000", "CE43AE41B96111DB66D75AB943A3042755B98F10E6A09AF0D4190B0FFEC13EE8", 0x00);

        Console.WriteLine("Finished join request");

        //await galaxy.SendMessage(1, "This is a message.");

        Random rng = new Random();

        for (int i = 0; i < 10; i++)
        {
            int number = rng.Next();

            Console.WriteLine("Starting IsEven request");

            if (await galaxy.IsEven(number))
                Console.WriteLine($"Number {number,11} is even");
            else
                Console.WriteLine($"Number {number,11} is not even");
        }

        await Task.Delay(60000);
    }
}