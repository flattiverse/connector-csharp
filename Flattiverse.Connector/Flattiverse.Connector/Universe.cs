using Flattiverse.Connector.Hierarchy;

namespace Flattiverse.Connector;

public class Universe
{
    public async Task<Galaxy> Join(string uri, string auth, byte teamId)
    {
        Galaxy galaxy = new Galaxy(this);
        
        await galaxy.Connect(uri, auth, teamId);
        
        return galaxy;
    }
}