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
            //for (int p = 0; p < 4; p++)
            //{
            //    int sp = p;

            //    ThreadPool.QueueUserWorkItem(async delegate
            //    {
            //        while (true)
            //            try
            //            {
            //                using (Server server = new Server())
            //                {
            //                    await server.Login("Player" + sp.ToString(), "Password");

            //                    for (int i = 0; i < 8; i++)
            //                    {
            //                        Console.Write(sp.ToString());

            //                        await server.Universes["Haraldmania"].Join(server.Universes["Haraldmania"].Teams["Dark Blue"]);
            //                        await server.Universes["Haraldmania"].Part();
            //                    }

            //                    Console.Write("L");

            //                    await server.Universes["Haraldmania"].Join(server.Universes["Haraldmania"].Teams["Dark Blue"]);
            //                }
            //            }
            //            catch
            //            {
            //                Console.Write("D");
            //            }
            //});
            //}

            //Console.ReadKey(false);

            //return;

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
                    Console.WriteLine($" * {universe.Name} @{universe.ID} Type={universe.Mode} Status={universe.Status}");

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

                    foreach (UniverseSystem system in universe.Systems)
                        Console.WriteLine($"   * (Component) {system}");
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