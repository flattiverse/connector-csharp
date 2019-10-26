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
#if DEBUG
                byte[] hash = Crypto.HashPassword("Player1", "Password");
#else
                byte[] hash = Crypto.HashPassword("Player0", "Password");
#endif

                Stopwatch sw = Stopwatch.StartNew();

#if DEBUG
                await server.Login("Player1", hash);
#else
                await server.Login("Player0", hash);
#endif

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

                Console.WriteLine("\nPlayers online:");

                foreach (Player player in server.Players)
                    Console.WriteLine($" * {player.Name} with a ping of {player.Ping}.");

                Console.WriteLine("\nKey?");

                Console.ReadKey();

                Console.WriteLine("\nPlayers online:");

                foreach (Player player in server.Players)
                    Console.WriteLine($" * {player.Name} with a ping of {player.Ping}.");
            }
        }
    }
}