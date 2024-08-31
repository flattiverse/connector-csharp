using Flattiverse.Connector;
using Flattiverse.Connector.Events;
using Flattiverse.Connector.GalaxyHierarchy;

namespace Development;

class Program
{
    static async Task Main(string[] args)
    {
        Galaxy galaxy1 = await Galaxy.Connect("Player", "ws://www.flattiverse.com/somewhere", "SOME_AUTH_KEY", "Test").ConfigureAwait(false);
        Galaxy galaxy2 = await Galaxy.Connect("Spectator", "ws://www.flattiverse.com/somewhere").ConfigureAwait(false);

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
            Console.WriteLine($" * {player.Name}.");

        await Task.Delay(10000000).ConfigureAwait(false);
    }
}