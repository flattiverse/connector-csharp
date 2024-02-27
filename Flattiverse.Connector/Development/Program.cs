using System.Runtime.CompilerServices;
using Flattiverse.Connector;
using Flattiverse.Connector.Events;
using Flattiverse.Connector.Hierarchy;
using Flattiverse.Connector.MissionSelection;
using Flattiverse.Connector.Units;

internal class Program
{
    private static async Task Main(string[] args)
    {
        // A new beginning and a test - again.

        Console.WriteLine("Starting join request");

        // Admin key / local host
        //Galaxy galaxy = await universe.Join("ws://127.0.0.1:5000", "68B59217071450554F85DB121F89EC31C54E427D04A8EE16AC72C52AED806631", 0x00);

        // Player key / online
        //Galaxy galaxy = await universe.Join("ws://www.flattiverse.com/game/galaxies/0", "CE43AE41B96111DB66D75AB943A3042755B98F10E6A09AF0D4190B0FFEC13EE8", 0x00);

        Universe universe = new Universe();
        
        foreach (KeyValuePair<string, GalaxyInfo> gInfo in universe.Galaxies)
        {
            Console.WriteLine($" -> {gInfo.Key} {gInfo.Value.GameMode}");

            foreach (KeyValuePair<string, TeamInfo> tInfo in gInfo.Value.Teams)
                Console.WriteLine($"   -> {tInfo.Key} {tInfo.Value.Id}");

            foreach (KeyValuePair<string, PlayerInfo> pInfo in gInfo.Value.Players)
                Console.WriteLine($"   => {pInfo.Key} {pInfo.Value.Id} {pInfo.Value.Team.Name}");
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("And now livedata:");
        Console.ForegroundColor = ConsoleColor.Gray;
        
        //Galaxy galaxy = await universe.Galaxies["Beginners Course"].Join("7da8b2443edf6477a71d788a3dba46c51fba7f7fe89435f223f972ac5fc80a8e", universe.Galaxies["Beginners Course"].Teams["Plebs"]);

        Galaxy galaxy = await universe.ManualJoin("ws://127.0.0.1:5000/game/galaxies/0", "28a00943f2c0181a0c5db3f4de3e23e987a4c060cc39f45dcb6ed2a86f00eac5", 0);
        
        ThreadPool.QueueUserWorkItem(async delegate
        {
            Galaxy galaxy2 = await universe.ManualJoin("ws://127.0.0.1:5000/game/galaxies/0", "6df3b734005dcd57efef3deaf87d4675de608afc0555c2e1ed65aba1e04c6600", 0);

            Controllable ship2 = await galaxy2.RegisterShip("Origin", galaxy2.ShipsDesigns["Cruiser"]);
            
            await ship2.Continue();
            
            // await Task.Delay(5000);

            // await ship2.SetThrusterNozzle(0, ship2.NozzleMax);
            
            await Task.Delay(5000);

            Player player = galaxy2.Players["GhostTyper"];

            await galaxy.Chat("Hallo Du Schnucki.");
            await galaxy.Chat("Hallo Du Schnucki.");
            await galaxy.Chat("Hallo Du Schnucki.");
            
            await ship2.SetThrusterNozzle(0, 0);
            
            while (true)
            {
                await galaxy2.NextEvent();
            }
        });

        await Task.Delay(3000);

        Console.WriteLine($" + Galaxy: {galaxy.Name}");

        foreach (Team team in galaxy.Teams)
            Console.WriteLine($"   + Team: {team.Name}");

        foreach (Cluster cluster in galaxy.Clusters)
            Console.WriteLine($"   + Cluster: {cluster.Name}");

        foreach (Player player in galaxy.Players)
            Console.WriteLine($"   + Player: {player.Name}");

        Controllable ship = await galaxy.RegisterShip("HeartOfGold", galaxy.ShipsDesigns["Cruiser"]);

        Console.WriteLine($"Ship: {ship.Name}, maxEnergy={ship.Energy}/{ship.EnergyMax}");

        await ship.Continue();

        Console.WriteLine($"ShipInfo: {galaxy.Players["GhostTyper"].ControllableInfos["HeartOfGold"].Energy}");
        
        Console.WriteLine($"Ship: {ship.Name}, maxEnergy={ship.Energy}/{ship.EnergyMax}");

        PlayerUnit? track = null;
        
        while (true)
        {
            FlattiverseEvent @event = await galaxy.NextEvent();

            switch (@event)
            {
                case AddedUnitEvent newUnitEvent:
                    if (newUnitEvent.Unit.Name == "Origin")
                        track = (PlayerUnit)newUnitEvent.Unit;
                    break;
            }
            
            if (@event.Kind == EventKind.GalaxyTick)
            {
                // Console.WriteLine($"  OWN POSITION={ship.Position}, DIRECTION={ship.Direction:F}°, THRUSTER={ship.Thruster:0.0000}, NOZZLE={ship.Nozzle:F}°, TURNRATE={ship.Turnrate:F}°, SPEED={ship.Movement.Length}");
                //
                // if (track is not null)
                //     Console.WriteLine($"OTHER POSITION={track.Position}, DIRECTION={track.Direction:F}°, THRUSTER={track.Thruster:0.0000}, NOZZLE={track.Nozzle:F}°, TURNRATE={track.Turnrate:F}°, SPEED={track.Movement.Length}");
            }
            else
                Console.WriteLine(@event);
        }
        
        await Task.Delay(60000);
    }
}