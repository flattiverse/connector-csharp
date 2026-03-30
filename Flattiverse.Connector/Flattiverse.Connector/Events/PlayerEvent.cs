using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Base type for events that are primarily about one player snapshot.
/// </summary>
public class PlayerEvent : FlattiverseEvent
{
    /// <summary>
    /// Player snapshot this event refers to.
    /// </summary>
    public readonly Player Player;

    internal PlayerEvent(Player player) : base()
    {
        Player = player;
    }
}
