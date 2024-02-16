using System.Runtime.CompilerServices;
using Flattiverse.Connector;
using Flattiverse.Connector.Hierarchy;

internal class Program
{
    private static async Task Main(string[] args)
    {
        // A new beginning and a test - again.

        Universe universe = new Universe();

        //Admin key
        //Galaxy galaxy = await universe.Join("ws://127.0.0.1:5000", "5AE4A6FD01FFCB104F594D7510160766FF9BE6731058D9469CB404C999CC7BF0", 0x00);

        //Player key
        Galaxy galaxy = await universe.Join("ws://127.0.0.1:5000", "7CA8F14EE55FCD522FB8FB4B4E09BEB7D5892D8341A0740FCC596D1CEC1D9D13", 0x00);

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