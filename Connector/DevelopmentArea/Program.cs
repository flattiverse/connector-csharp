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
            Console.ForegroundColor = ConsoleColor.Gray;

            using (Server server = new Server())
            {
                await server.Login("Player0", "Password");

                Universe universe = server.Universes["Beginners Course"];

                await universe.Join();

                Controllable controllable = await universe.NewShip("Bounty");

                Console.WriteLine($" * Registered: {controllable.Name} at position: {controllable.Position} with a hull of {controllable.Hull}.");

                await controllable.Continue();

                Console.WriteLine($" * Registered: {controllable.Name} at position: {controllable.Position} with a hull of {controllable.Hull}.");

                FlattiverseEvent @event;

                while (true)
                {
                    Queue<FlattiverseEvent> events = await server.GatherEvents();

                    while (events.TryDequeue(out @event))
                    {
                        Console.WriteLine(@event.ToString());
                    }
                }
            }

//            using (Server server = new Server())
//            {
//#if DEBUG
//                await server.Login("Player1", "Password");
//#else
//                await server.Login("Player0", "Password");
//#endif

//                Console.WriteLine($" * Name: {server.Player.Name}.");

//                foreach (Universe universe in server.Universes)
//                {
//                    Console.WriteLine($" * {universe.Name} @{universe.ID} Type={universe.Mode} Status={universe.Status} BasePriv={universe.DefaultPrivileges}");

//                    foreach (Team team in universe.Teams)
//                    {
//                        Console.Write("   * (Team) [");

//                        Console.ForegroundColor = team.ConsoleColor;

//                        Console.Write("█████");

//                        Console.ForegroundColor = ConsoleColor.Gray;

//                        Console.WriteLine($"] {team.Name}");
//                    }

//                    foreach (Galaxy galaxy in universe.Galaxies)
//                        Console.WriteLine($"   * (Galaxy) {galaxy.Name}");

//                    foreach (UniverseSystem system in universe.Systems)
//                        Console.WriteLine($"   * (Component) {system}");

//                    foreach (KeyValuePair<Account, Privileges> privs in universe.QueryPrivileges())
//                        Console.WriteLine($"   * (Privilege) {privs.Key.Name}: {privs.Value.ToString()}");
//                }

//                Console.WriteLine("\nQuery of 3 Players:");

//                Console.WriteLine($" * " + (await server.QueryAccount("Player1").ConfigureAwait(false)).Name);
//                Console.WriteLine($" * " + (await server.QueryAccount("Player2").ConfigureAwait(false)).Name);

//                try
//                {
//                    Account acc = await server.QueryAccount("asdf");
//                }
//                catch (Exception exception)
//                {
//                    Console.WriteLine($" * Query \"asdf\": {exception.Message}");
//                }

//                Console.WriteLine("\nQuery of all Players:");

//                foreach (Account acc in server.QueryAccounts(null, false))
//                    Console.WriteLine($" * {acc.Name} [{acc.Status}]");

//                Console.Write("\nChanging Privileges on \"Beginners Course\": ");

//                try
//                {
//                    await server.Universes["Beginners Course"].AlterPrivileges(await server.QueryAccount("GhostTyper").ConfigureAwait(false), Privileges.Join | Privileges.ManageUniverse).ConfigureAwait(false);

//                    Console.WriteLine("SUCCESS.");
//                }
//                catch (Exception exception)
//                {
//                    Console.WriteLine(exception.Message);
//                }

//                Console.Write("\nSetting Privileges on \"Development\": ");

//                try
//                {
//                    await server.Universes["Development"].AlterPrivileges(await server.QueryAccount("GhostTyper").ConfigureAwait(false), Privileges.Join | Privileges.ManageUniverse).ConfigureAwait(false);

//                    Console.WriteLine("SUCCESS.");
//                }
//                catch (Exception exception)
//                {
//                    Console.WriteLine(exception.Message);
//                }

//                Console.Write("\nChanging Privileges on \"Development\": ");

//                try
//                {
//                    await server.Universes["Development"].AlterPrivileges(await server.QueryAccount("GhostTyper").ConfigureAwait(false), Privileges.Join | Privileges.ManageUniverse | Privileges.ManageRegions).ConfigureAwait(false);

//                    Console.WriteLine("SUCCESS.");
//                }
//                catch (Exception exception)
//                {
//                    Console.WriteLine(exception.Message);
//                }

//                Console.Write("\nRemoving Privileges on \"Development\": ");

//                try
//                {
//                    await server.Universes["Development"].AlterPrivileges(await server.QueryAccount("GhostTyper").ConfigureAwait(false), server.Universes["Development"].DefaultPrivileges).ConfigureAwait(false);

//                    Console.WriteLine("SUCCESS.");
//                }
//                catch (Exception exception)
//                {
//                    Console.WriteLine(exception.Message);
//                }

//                Console.ForegroundColor = ConsoleColor.White;

//                Console.Write("\nChanging own Password to Password: ");

//                try
//                {
//                    await (await server.QueryAccount(server.Player.Name).ConfigureAwait(false)).ChangePassword("Password").ConfigureAwait(false);

//                    Console.ForegroundColor = ConsoleColor.Green;

//                    Console.WriteLine("SUCCESS.");
//                }
//                catch (Exception exception)
//                {
//                    Console.WriteLine(exception.Message);
//                }

//                Console.ForegroundColor = ConsoleColor.Gray;

//                Console.WriteLine("\nChanging Password of GhostTyper to Password: ");

//                try
//                {
//                    await (await server.QueryAccount("GhostTyper").ConfigureAwait(false)).ChangePassword("Password").ConfigureAwait(false);

//                    Console.WriteLine("SUCCESS.");
//                }
//                catch (Exception exception)
//                {
//                    Console.WriteLine(exception.Message);
//                }

//                // MAP EDITS

//                Console.Write("\nQuerying \"Zirp\" from \"Development\": ");

//                try
//                {
//                    Console.WriteLine(await server.Universes["Development"].Galaxies["Dev #0"].QueryUnitXml("Zirp"));
//                }
//                catch (Exception exception)
//                {
//                    Console.WriteLine(exception.Message);
//                }

//                Console.Write("\nQuerying \"UnknownUnit\" from \"Development\": ");

//                try
//                {
//                    Console.WriteLine(await server.Universes["Development"].Galaxies["Dev #0"].QueryUnitXml("UnknownUnit"));
//                }
//                catch (Exception exception)
//                {
//                    Console.WriteLine(exception.Message);
//                }

//                Console.Write("\nCreating \"UnknownUnit\" in \"Development\": ");

//                try
//                {
//                    await server.Universes["Development"].Galaxies["Dev #0"].UpdateUnitXml("<Sun Name=\"UnknownUnit\" Radius=\"300\" PositionX=\"0\" PositionY=\"0\" Gravity=\"0.7\" Radiation=\"2\" PowerOutput=\"150\" />");

//                    Console.WriteLine("SUCCESS.");
//                }
//                catch (Exception exception)
//                {
//                    Console.WriteLine(exception.Message);
//                }

//                Thread.Sleep(1000);

//                Console.Write("\nUpdating \"UnknownUnit\" in \"Development\": ");

//                try
//                {
//                    await server.Universes["Development"].Galaxies["Dev #0"].UpdateUnitXml("<Sun Name=\"UnknownUnit\" Radius=\"400\" PositionX=\"0\" PositionY=\"0\" Gravity=\"0.7\" Radiation=\"2\" PowerOutput=\"150\" />");

//                    Console.WriteLine("SUCCESS.");
//                }
//                catch (Exception exception)
//                {
//                    Console.WriteLine(exception.Message);
//                }

//                Console.Write("\nQuerying \"UnknownUnit\" from \"Development\": ");

//                try
//                {
//                    Console.WriteLine(await server.Universes["Development"].Galaxies["Dev #0"].QueryUnitXml("UnknownUnit"));
//                }
//                catch (Exception exception)
//                {
//                    Console.WriteLine(exception.Message);
//                }

//                Thread.Sleep(1000);

//                Console.Write("\nDeleting \"UnknownUnit\" in \"Development\": ");

//                try
//                {
//                    await server.Universes["Development"].Galaxies["Dev #0"].DeleteUnit("UnknownUnit");

//                    Console.WriteLine("SUCCESS.");
//                }
//                catch (Exception exception)
//                {
//                    Console.WriteLine(exception.Message);
//                }

//                Console.WriteLine("\nChecking Unit XML <Sun Name=\"UnknownUnit\" Radius=\"300\" PositionX=\"0\" PositionY=\"0\" Gravity=\"0.7\" Radiation=\"2\" PowerOutput=\"150\" />:");

//                try
//                {
//                    Console.WriteLine(await server.CheckUnitXml("<Sun Name=\"UnknownUnit\" Radius=\"300\" PositionX=\"0\" PositionY=\"0\" Gravity=\"0.7\" Radiation=\"2\" PowerOutput=\"150\" />"));
//                }
//                catch (Exception exception)
//                {
//                    Console.WriteLine(exception.Message);
//                }

//                Console.WriteLine("\nChecking Unit XML <Sun Name=\"#UnknownUnit\" Radius=\"300\" PositionX=\"0\" PositionY=\"0\" Gravity=\"0.7\" Radiation=\"2\" PowerOutput=\"150\" />:");

//                try
//                {
//                    Console.WriteLine(await server.CheckUnitXml("<Sun Name=\"#UnknownUnit\" Radius=\"300\" PositionX=\"0\" PositionY=\"0\" Gravity=\"0.7\" Radiation=\"2\" PowerOutput=\"150\" />"));
//                }
//                catch (Exception exception)
//                {
//                    Console.WriteLine(exception.Message);
//                }

//                Console.ForegroundColor = ConsoleColor.Gray;

//                // END OF MAP EDITS NOW: REGIONS

//                Console.WriteLine("\nCreating three Regions in Development\\Dev #0...");

//                await server.Universes["Development"].Galaxies["Dev #0"].UpdateRegion(new Region(0xFD) { Name = null }).ConfigureAwait(false);
//                await server.Universes["Development"].Galaxies["Dev #0"].UpdateRegion(new Region(0xFE) { Name = "Hallo1" }).ConfigureAwait(false);
//                await server.Universes["Development"].Galaxies["Dev #0"].UpdateRegion(new Region(0xFF) { Name = "Hallo2" }).ConfigureAwait(false);

//                Console.WriteLine("Updating the Region in Development\\Dev #0...");

//                await server.Universes["Development"].Galaxies["Dev #0"].UpdateRegion(new Region(0xFF) { Name = "Hallo" }).ConfigureAwait(false);

//                Console.WriteLine("Querying Regions in Development\\Dev #0:");

//                foreach (Region region in await server.Universes["Development"].Galaxies["Dev #0"].QueryRegions().ConfigureAwait(false))
//                    Console.WriteLine($" * {region.Name ?? "<null>"}");

//                Console.WriteLine("Deleting three Regions in Development\\Dev #0...");

//                await server.Universes["Development"].Galaxies["Dev #0"].DeleteRegion(new Region(0xFD)).ConfigureAwait(false);
//                await server.Universes["Development"].Galaxies["Dev #0"].DeleteRegion(new Region(0xFE)).ConfigureAwait(false);
//                await server.Universes["Development"].Galaxies["Dev #0"].DeleteRegion(new Region(0xFF)).ConfigureAwait(false);

//                Console.WriteLine("Done Regions.");

//                // END OF REGIONS

//                Console.WriteLine();

//                Console.ForegroundColor = ConsoleColor.Yellow;
//                Console.WriteLine("  The following sequence is a demonstration of how to live view the\n  universe and how to edit it.\n");
//                Console.WriteLine("  Here you can see, how and of which delay you will get unit updates\n  if you edit some of the units.");
//                Console.ForegroundColor = ConsoleColor.Gray;

//                Console.WriteLine("\nPlayers online:");

//                foreach (Player player in server.Players)
//                    Console.WriteLine($" * {player.Name} with a ping of {player.Ping}.");

//                server.MetaEvent += metaEvent;
//                server.ScanEvent += scanEvent;

//                await server.Universes["Development"].Join(server.Universes["Development"].Teams["Dark Blue"]);

//                Thread.Sleep(1000);

//                Console.WriteLine("Starting View...");

//                await server.Universes["Development"].Galaxies["Dev #0"].StartView();

//                Console.WriteLine("View started.");

//                Thread.Sleep(1000);

//                Console.WriteLine("\nCreating some \"UnknownUnit\"s in \"Development\"...\n");

//                try
//                {
//                    await server.Universes["Development"].Galaxies["Dev #0"].UpdateUnitXml("<Sun Name=\"UnknownUnit\" Radius=\"300\" PositionX=\"0\" PositionY=\"0\" Gravity=\"0.7\" Radiation=\"2\" PowerOutput=\"150\"><Plasma Level=\"3\" Amount=\"2\" Radius=\"700\" /></Sun>");
//                }
//                catch
//                { }

//                Thread.Sleep(250);

//                try
//                {
//                    await server.Universes["Development"].Galaxies["Dev #0"].UpdateUnitXml("<Planet Name=\"UnknownUnit\" Radius=\"300\" PositionX=\"0\" PositionY=\"0\" Gravity=\"0.7\" Radiation=\"2\" />");
//                }
//                catch
//                { }

//                Thread.Sleep(250);

//                try
//                {
//                    await server.Universes["Development"].Galaxies["Dev #0"].UpdateUnitXml("<Moon Name=\"UnknownUnit\" Radius=\"300\" PositionX=\"0\" PositionY=\"0\" Gravity=\"0.7\" Radiation=\"2\" />");
//                }
//                catch
//                { }

//                Thread.Sleep(250);

//                try
//                {
//                    await server.Universes["Development"].Galaxies["Dev #0"].UpdateUnitXml("<Meteoroid Name=\"UnknownUnit\" Radius=\"300\" PositionX=\"0\" PositionY=\"0\" Gravity=\"0.7\" Radiation=\"2\" />");
//                }
//                catch
//                { }

//                Thread.Sleep(250);

//                try
//                {
//                    await server.Universes["Development"].Galaxies["Dev #0"].UpdateUnitXml("<Buoy Name=\"UnknownUnit\" Radius=\"300\" PositionX=\"0\" PositionY=\"0\" Gravity=\"0.7\" Radiation=\"2\">Meine kleine Message...</Buoy>");
//                }
//                catch
//                { }

//                Thread.Sleep(250);

//                try
//                {
//                    await server.Universes["Development"].Galaxies["Dev #0"].UpdateUnitXml("<Target Name=\"UnknownUnit\" Radius=\"300\" PositionX=\"0\" PositionY=\"0\" Sequence=\"2\" />");
//                }
//                catch
//                { }

//                Thread.Sleep(1000);

//                Console.WriteLine("\nDeleting \"UnknownUnit\" in \"Development\"...\n");

//                try
//                {
//                    await server.Universes["Development"].Galaxies["Dev #0"].DeleteUnit("UnknownUnit");
//                }
//                catch
//                { }

//                Thread.Sleep(1000);

//                Console.WriteLine("\nStopping View...\n");

//                await server.Universes["Development"].Galaxies["Dev #0"].StopView();

//                Thread.Sleep(1000);

//                Console.WriteLine("\nPlayers online:");

//                foreach (Player player in server.Players)
//                    Console.WriteLine($" * {player.Name} with a ping of {player.Ping}.");

//                await server.Universes["Development"].Part();

//                Console.WriteLine();

//                Thread.Sleep(1000);

//                Console.WriteLine();

//                Console.WriteLine(" * SILENCE?!");

//                Thread.Sleep(1000);

//                Console.WriteLine(" * YEP!");

//                Thread.Sleep(1000);

//                Console.WriteLine("\nDone.");
//            }
        }

        private static void metaEvent(FlattiverseEvent @event)
        {
            Console.WriteLine($" * [META-EVENT] {@event}");
        }

        private static void scanEvent(FlattiverseEvent @event)
        {
            //Console.WriteLine($" * [SCAN-EVENT] {@event}");

            if (@event is NewUnitEvent)
            {
                switch (((NewUnitEvent)@event).Unit)
                {
                    case Target target:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($" * [TARGET] {target.Name} Sequence: {target.Sequence}.");
                        break;
                    case Sun sun:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($" * [SUN] {sun.Name} Corona: {(sun.Corona == null ? "NO" : $"l={sun.Corona.Plasma} r={sun.Corona.Radius} a={sun.Corona.Amount}")}.");
                        break;
                    case Planet planet:
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($" * [PLANET] {planet.Name}.");
                        break;
                    case Moon moon:
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($" * [MOON] {moon.Name}.");
                        break;
                    case Meteoroid meteoroid:
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine($" * [METEOROID] {meteoroid.Name}.");
                        break;
                    case Buoy buoy:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine($" * [BUOY] {buoy.Name} message={buoy.Message}.");
                        break;
                }

                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }
    }
}