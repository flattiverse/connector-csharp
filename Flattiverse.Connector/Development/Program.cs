using Flattiverse.Connector;
using Flattiverse.Connector.Events;
using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Units;

namespace Development;

class Program
{
    static async Task Main(string[] args)
    {
        Galaxy galaxy1 = await Galaxy.Connect("ws://www.flattiverse.com/game/galaxies/0", "SOME_AUTH_KEY", "Pink").ConfigureAwait(false);
        //Galaxy galaxy2 = await Galaxy.Connect("ws://www.flattiverse.com/game/galaxies/0").ConfigureAwait(false);

        // await galaxy0.Chat("Halli, Hallo. :D");
        
        ClassicShipControllable ship = await galaxy1.CreateClassicShip("Hust2");

        await ship.Continue();

        await ship.Move(new Vector(2, 0));
        
        for (int i = 0; i < 25; i++)
        {
            FlattiverseEvent e = await galaxy1.NextEvent();

            Console.WriteLine(e);
        }
        
        /*Controllable c = await galaxy1.CreateClassicShip("blah");

        await Task.Delay(100);

        Console.WriteLine($" * ALIVE? {c.Alive} {galaxy1.Player.ControllableInfos["blah"].Alive}");
        
        await c.Continue();
        
        Console.WriteLine($" * ALIVE? {c.Alive} {galaxy1.Player.ControllableInfos["blah"].Alive}");
        
        while (true)
        {
            FlattiverseEvent e = await galaxy1.NextEvent();

            Console.WriteLine(e);
        }*/

        /*Galaxy galaxy2 = await Galaxy.Connect("ws://127.0.0.1:5000").ConfigureAwait(false);

        Console.WriteLine($" * {galaxy1.Name} / [{galaxy1.Player.Id}] {galaxy1.Player.Name} / {galaxy1.Player.Team.Name} / {galaxy1.Player.Kind}");

        await Task.Delay(1000);

        Console.WriteLine("\nPlayers:");

        foreach (Player player in galaxy2.Players)
            Console.WriteLine($" * {player.Name}.");

        Console.WriteLine("\nClusters:");

        foreach (Cluster cluster in galaxy2.Clusters)
            Console.WriteLine($" * {cluster.Name}.");

        Console.WriteLine("\nTeams:");

        foreach (Team team in galaxy2.Teams)
            Console.WriteLine($" * {team.Name}.");

        ClassicShipControllable ship = await galaxy1.CreateClassicShip("Suuupi");

        Console.WriteLine($" * NEW CONTROLLABLE: {ship.Name}, id={ship.Id}");

        await ship.Continue();

        await Task.Delay(1000);

        await ship.Suicide();

        // ship.Dispose();

        await galaxy1.Chat("Halli hallo Universum.");
        await galaxy1.Player.Chat("Halli hallo Player.");
        await galaxy1.Player.Team.Chat("Halli hallo Team.");

        galaxy1.Dispose();

        Console.WriteLine($" => Hip now inactive? {!ship.Active}");

        Console.WriteLine("\nEvents von galaxy1:");

        FlattiverseEvent? @event;

        galaxy2.Dispose();

        try
        {
            while (true)
                Console.WriteLine(await galaxy2.NextEvent().ConfigureAwait(false));
        }
        catch (Exception exception)
        {
            Console.WriteLine($"!!! Exception: {exception.Message}");
        }

        await Task.Delay(1000);

        Console.WriteLine("\nPlayers, after Account disconnect:");

        foreach (Player player in galaxy2.Players)
            Console.WriteLine($" * {player.Name} with ping: {player.Ping} ms.");*/

        await Task.Delay(10000000).ConfigureAwait(false);
    }
}