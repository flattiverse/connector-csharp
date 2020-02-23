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
                    Console.WriteLine($" * {universe.Name} @{universe.ID} Type={universe.Mode} Status={universe.Status} BasePriv={universe.DefaultPrivileges}");

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

                    await foreach (KeyValuePair<Account?, Privileges> privs in universe.QueryPrivileges())
                        Console.WriteLine($"   * (Privilege) {privs.Key.Name}: {privs.Value.ToString()}");
                }

                Console.WriteLine("\nQuery of 3 Players:");

                Console.WriteLine($" * " + (await server.QueryAccount("Player1").ConfigureAwait(false)).Name);
                Console.WriteLine($" * " + (await server.QueryAccount("Player2").ConfigureAwait(false)).Name);

                try
                {
                    Account acc = await server.QueryAccount("asdf");

                    Thread.Sleep(1);
                }
                catch (Exception exception)
                {
                    Console.WriteLine($" * Query \"asdf\": {exception.Message}");
                }

                Console.WriteLine("\nQuery of all Players:");

                await foreach (Account acc in server.QueryAccounts(null, false))
                    Console.WriteLine($" * {acc.Name}");

                Console.WriteLine("\nPlayers online:");

                foreach (Player player in server.Players)
                    Console.WriteLine($" * {player.Name} with a ping of {player.Ping}.");

                server.MetaEvent += metaEvent;

                await server.Universes["Development"].Join(server.Universes["Development"].Teams["Dark Blue"]);

                Console.WriteLine("\nKey?");

                Console.ReadKey();

                Console.WriteLine("\nPlayers online:");

                foreach (Player player in server.Players)
                    Console.WriteLine($" * {player.Name} with a ping of {player.Ping}.");

                await server.Universes["Development"].Part();

                Console.WriteLine("Done.");
            }
        }

        private static void metaEvent(FlattiverseEvent @event)
        {
            Console.WriteLine($" * [META-EVENT] {@event}");
        }
    }
}