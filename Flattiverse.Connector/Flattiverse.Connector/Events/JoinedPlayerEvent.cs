using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// This event is raised when a player joins.
/// </summary>
public class JoinedPlayerEvent : PlayerEvent
{
    /// <inheritdoc/>
    public override EventKind Kind => EventKind.PlayerJoined;
    
    internal JoinedPlayerEvent(Player player) : base(player)
    {
    }
    
    /// <inheritdoc/>
    public override string ToString() => $"{Stamp:HH:mm:ss.fff} \"{Player.Name}\" joined the galaxy with team {Player.Team.Name} as {Player.Kind}.";
}