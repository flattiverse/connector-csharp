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
                await server.Login("Player1", "Password");
#else
                await server.Login("Player0", "Password");
#endif

                Console.WriteLine($" * Name: {server.Player.Name}.");

                foreach (Universe universe in server.Universes)
                {
                    Console.WriteLine($" * {universe.Name} @{universe.ID} Type={universe.Mode}");

                    foreach (Team team in universe.Teams)
                    {
                        Console.Write("   * (Team) [");

                        Console.ForegroundColor = team.ConsoleColor;

                        Console.Write("█████");

                        Console.ForegroundColor = ConsoleColor.Gray;

                        Console.WriteLine($"] {team.Name}");
                    }

                    foreach (Galaxy galaxy in universe.Galaxies)
                        Console.WriteLine($"   * (Galaxy) {galaxy.Name}");
                }

                Console.WriteLine("\nPlayers online:");

                foreach (Player player in server.Players)
                    Console.WriteLine($" * {player.Name} with a ping of {player.Ping}.");

                server.MetaEvent += metaEvent;

                await server.Universes["Haraldmania"].Join(server.Universes["Haraldmania"].Teams["Dark Blue"]);

                Console.WriteLine("\nKey?");

                Console.ReadKey();

                Console.WriteLine("\nPlayers online:");

                foreach (Player player in server.Players)
                    Console.WriteLine($" * {player.Name} with a ping of {player.Ping}.");

                await server.Universes["Haraldmania"].Part();

                Console.WriteLine("Done.");
            }
        }

        private static void metaEvent(FlattiverseEvent @event)
        {
            Console.WriteLine($" * {@event}");
        }
    }
}