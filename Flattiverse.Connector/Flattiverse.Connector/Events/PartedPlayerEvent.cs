using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Raised when a player snapshot is removed from the local galaxy mirror.
/// </summary>
public class PartedPlayerEvent : PlayerEvent
{
    /// <inheritdoc />
    public override EventKind Kind => EventKind.PlayerParted;
    
    internal PartedPlayerEvent(Player player) : base(player)
    {
    }
    
    /// <summary>
    /// Returns a compact diagnostic representation of the event.
    /// </summary>
    public override string ToString() => $"{Stamp:HH:mm:ss.fff} \"{Player.Name}\" parted the galaxy with team {Player.Team.Name} as {Player.Kind}.";
}
