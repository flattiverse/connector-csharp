using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Raised when a player's connection has dropped but the player snapshot is still present for cleanup.
/// </summary>
public class DisconnectedPlayerEvent : PlayerEvent
{
    /// <inheritdoc />
    public override EventKind Kind => EventKind.PlayerDisconnected;
    
    internal DisconnectedPlayerEvent(Player player) : base(player)
    {
    }
    
    /// <summary>
    /// Returns a compact diagnostic representation of the event.
    /// </summary>
    public override string ToString() => $"{Stamp:HH:mm:ss.fff} \"{Player.Name}\" disconnected from the galaxy while cleanup is still pending.";
}
