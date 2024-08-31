using Flattiverse.Connector;
using Flattiverse.Connector.Events;
using Flattiverse.Connector.GalaxyHierarchy;

namespace Development;

class Program
{
    static async Task Main(string[] args)
    {
        // Galaxy galaxy = await Galaxy.Connect("ws://127.0.0.1:5000", "7666FC8BDADC000ACDE68691EBE7D30F6D0F6AC001431A18886EE2D9F176AB9E", "Test").ConfigureAwait(false);
        Galaxy galaxy1 = await Galaxy.Connect("Player", "ws://127.0.0.1:5000", "28A00943F2C0181A0C5DB3F4DE3E23E987A4C060CC39F45DCB6ED2A86F00EAC5", "Test").ConfigureAwait(false);
        Galaxy galaxy2 = await Galaxy.Connect("Spectator", "ws://127.0.0.1:5000").ConfigureAwait(false);

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
            Console.WriteLine($" * {player.Name} with ping: {player.Ping} ms.");

        await Task.Delay(10000000).ConfigureAwait(false);
    }
}