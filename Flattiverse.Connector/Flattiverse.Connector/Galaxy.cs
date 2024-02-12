using Flattiverse.Connector.Network;

namespace Flattiverse.Connector;

public class Galaxy
{
    private Connection connection;

    internal Galaxy(Universe universe)
    {
        connection = new Connection(universe);
    }

    internal async Task Connect(string uri, string auth, byte team)
    {
        await connection.Connect(uri, auth, team);
    }
}