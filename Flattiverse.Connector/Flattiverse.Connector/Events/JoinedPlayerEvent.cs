using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Raised when a player snapshot becomes known to the connector.
/// </summary>
public class JoinedPlayerEvent : PlayerEvent
{
    /// <inheritdoc />
    public override EventKind Kind => EventKind.PlayerJoined;
    
    internal JoinedPlayerEvent(Player player) : base(player)
    {
    }
    
    /// <summary>
    /// Returns a compact diagnostic representation of the event.
    /// </summary>
    public override string ToString() => $"{Stamp:HH:mm:ss.fff} \"{Player.Name}\" joined the galaxy with team {Player.Team.Name} as {Player.Kind}.";
}
