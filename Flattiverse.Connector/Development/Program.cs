using Flattiverse.Connector;
using Flattiverse.Connector.Events;
using Flattiverse.Connector.GalaxyHierarchy;

namespace Development;

class Program
{
    static async Task Main(string[] args)
    {
        Galaxy galaxy1 = await Galaxy.Connect("ws://www.flattiverse.com/game/galaxies/0", "SOME_AUTH_KEY", "Test").ConfigureAwait(false);
        Galaxy galaxy2 = await Galaxy.Connect("ws://www.flattiverse.com/game/galaxies/0").ConfigureAwait(false);

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

        await galaxy1.Chat("Halli hallo Universum.");
        await galaxy1.Player.Chat("Halli hallo Player.");
        await galaxy1.Player.Team.Chat("Halli hallo Team.");
        
        galaxy1.Dispose();

        Console.WriteLine("\nEvents von galaxy1:");

        FlattiverseEvent? @event;
        
        try
        {
            while (true)
                Console.WriteLine(await galaxy1.NextEvent().ConfigureAwait(false));
        }
        catch (Exception exception)
        {
            Console.WriteLine($"!!! Exception: {exception.Message}");
        }
        
        await Task.Delay(1000);
        
        Console.WriteLine("\nPlayers, after Account disconnect:");

        foreach (Player player in galaxy2.Players)
            Console.WriteLine($" * {player.Name} with ping: {player.Ping} ms.");

        await Task.Delay(10000000).ConfigureAwait(false);
    }
}