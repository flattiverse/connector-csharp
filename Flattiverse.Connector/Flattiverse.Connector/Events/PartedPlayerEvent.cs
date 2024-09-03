using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// This event is raised when a player leaves the universe.
/// </summary>
public class PartedPlayerEvent : PlayerEvent
{
    /// <inheritdoc/>
    public override EventKind Kind => EventKind.PlayerParted;
    
    internal PartedPlayerEvent(Player player) : base(player)
    {
    }
    
    /// <inheritdoc/>
    public override string ToString() => $"{Stamp:HH:mm:ss.fff} \"{Player.Name}\" parted the galaxy with team {Player.Team.Name} as {Player.Kind}.";
}