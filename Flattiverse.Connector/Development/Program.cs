using System.Runtime.CompilerServices;
using Flattiverse.Connector;
using Flattiverse.Connector.Hierarchy;
using Flattiverse.Connector.MissionSelection;

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
        
        Galaxy galaxy = await universe.ManualJoin("ws://127.0.0.1:5000/game/galaxies/0", "7da8b2443edf6477a71d788a3dba46c51fba7f7fe89435f223f972ac5fc80a8e", 0);

        Console.WriteLine($" + Galaxy: {galaxy.Name}");

        foreach (Team team in galaxy.Teams)
            Console.WriteLine($"   + Team: {team.Name}");

        foreach (Cluster cluster in galaxy.Clusters)
            Console.WriteLine($"   + Cluster: {cluster.Name}");

        foreach (Player player in galaxy.Players)
            Console.WriteLine($"   + Player: {player.Name}");

        Controllable ship = await galaxy.RegisterShip("HeartOfGold", galaxy.ShipsDesigns["Cruiser"]);
        
        
        
        await Task.Delay(60000);
    }
}