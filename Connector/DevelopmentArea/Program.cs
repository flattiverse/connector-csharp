using Flattiverse;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopmentArea
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using (Server server = new Server())
            {
                byte[] hash = Crypto.HashPassword("Anonymous", "Password");

                Stopwatch sw = Stopwatch.StartNew();

                await server.Login("Anonymous", hash);

                Console.WriteLine($" * {sw.Elapsed}.");

                foreach (Universe universe in server.Universes)
                {
                    Console.WriteLine($" * {universe.Name} @{universe.ID} Type={universe.Mode}");

                    foreach (Team team in universe.Teams)
                    {
                        Console.Write("   * [");

                        Console.ForegroundColor = team.ConsoleColor;

                        Console.Write("█████");

                        Console.ForegroundColor = ConsoleColor.Gray;

                        Console.WriteLine($"] {team.Name}");
                    }
                }
            }
        }
    }
}
