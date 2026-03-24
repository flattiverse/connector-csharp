using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// This event is raised when a player's connection has disconnected but the player is still present for cleanup.
/// </summary>
public class DisconnectedPlayerEvent : PlayerEvent
{
    /// <inheritdoc/>
    public override EventKind Kind => EventKind.PlayerDisconnected;
    
    internal DisconnectedPlayerEvent(Player player) : base(player)
    {
    }
    
    /// <inheritdoc/>
    public override string ToString() => $"{Stamp:HH:mm:ss.fff} \"{Player.Name}\" disconnected from the galaxy while cleanup is still pending.";
}
