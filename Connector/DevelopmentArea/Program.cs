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

                    foreach (KeyValuePair<Account?, Privileges> privs in universe.QueryPrivileges())
                        Console.WriteLine($"   * (Privilege) {privs.Key.Name}: {privs.Value.ToString()}");
                }

                Console.WriteLine("\nQuery of 3 Players:");

                Console.WriteLine($" * " + (await server.QueryAccount("Player1").ConfigureAwait(false)).Name);
                Console.WriteLine($" * " + (await server.QueryAccount("Player2").ConfigureAwait(false)).Name);

                try
                {
                    Account acc = await server.QueryAccount("asdf");
                }
                catch (Exception exception)
                {
                    Console.WriteLine($" * Query \"asdf\": {exception.Message}");
                }

                Console.WriteLine("\nQuery of all Players:");

                foreach (Account acc in server.QueryAccounts(null, false))
                    Console.WriteLine($" * {acc.Name}");

                Console.Write("\nChanging Privileges on \"Beginners Course\": ");

                try
                {
                    await server.Universes["Beginners Course"].AlterPrivileges(await server.QueryAccount("GhostTyper").ConfigureAwait(false), Privileges.Join | Privileges.ManageUniverse).ConfigureAwait(false);

                    Console.WriteLine("SUCCESS.");
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }

                Console.Write("\nSetting Privileges on \"Development\": ");

                try
                {
                    await server.Universes["Development"].AlterPrivileges(await server.QueryAccount("GhostTyper").ConfigureAwait(false), Privileges.Join | Privileges.ManageUniverse).ConfigureAwait(false);

                    Console.WriteLine("SUCCESS.");
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }

                Console.Write("\nChanging Privileges on \"Development\": ");

                try
                {
                    await server.Universes["Development"].AlterPrivileges(await server.QueryAccount("GhostTyper").ConfigureAwait(false), Privileges.Join | Privileges.ManageUniverse | Privileges.ManageRegions).ConfigureAwait(false);

                    Console.WriteLine("SUCCESS.");
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }

                Console.Write("\nRemoving Privileges on \"Development\": ");

                try
                {
                    await server.Universes["Development"].AlterPrivileges(await server.QueryAccount("GhostTyper").ConfigureAwait(false), server.Universes["Development"].DefaultPrivileges).ConfigureAwait(false);

                    Console.WriteLine("SUCCESS.");
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }

                // MAP EDITS

                ConsoleColor color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Map Edit Section. Press Key, whenever the code doesn't automatically continue. :)\n");
                Console.ForegroundColor = color;

                Console.Write("\nQuerying \"Zirp\" from \"Development\": ");

                try
                {
                    Console.WriteLine(await server.Universes["Development"].Galaxies["Dev #0"].QueryUnitXml("Zirp"));
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }

                Console.Write("\nQuerying \"UnknownUnit\" from \"Development\": ");

                try
                {
                    Console.WriteLine(await server.Universes["Development"].Galaxies["Dev #0"].QueryUnitXml("UnknownUnit"));
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }

                Console.Write("\nCreating \"UnknownUnit\" in \"Development\": ");

                try
                {
                    await server.Universes["Development"].Galaxies["Dev #0"].UpdateUnitXml("<Sun Name=\"UnknownUnit\" Radius=\"300\" PositionX=\"0\" PositionY=\"0\" Gravity=\"0.7\" Radiation=\"2\" PowerOutput=\"150\" />");

                    Console.WriteLine("SUCCESS.");
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }

                Console.ReadKey(false);

                Console.Write("\nUpdating \"UnknownUnit\" in \"Development\": ");

                try
                {
                    await server.Universes["Development"].Galaxies["Dev #0"].UpdateUnitXml("<Sun Name=\"UnknownUnit\" Radius=\"400\" PositionX=\"0\" PositionY=\"0\" Gravity=\"0.7\" Radiation=\"2\" PowerOutput=\"150\" />");

                    Console.WriteLine("SUCCESS.");
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }

                Console.Write("\nQuerying \"UnknownUnit\" from \"Development\": ");

                try
                {
                    Console.WriteLine(await server.Universes["Development"].Galaxies["Dev #0"].QueryUnitXml("UnknownUnit"));
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }

                Console.ReadKey(false);

                Console.Write("\nDeleting \"UnknownUnit\" in \"Development\": ");

                try
                {
                    await server.Universes["Development"].Galaxies["Dev #0"].DeleteUnit("UnknownUnit");

                    Console.WriteLine("SUCCESS.");
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }

                // EOF MAP EDITS

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