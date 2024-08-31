using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// A parent event managing player events.
/// </summary>
public class PlayerEvent : FlattiverseEvent
{
    /// <summary>
    /// The player this event handles.
    /// </summary>
    public readonly Player Player;

    internal PlayerEvent(Player player) : base()
    {
        Player = player;
    }
}