using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Base type for events about a player's public <see cref="GalaxyHierarchy.ControllableInfo" /> entries.
/// </summary>
public class ControllableInfoPlayerEvent : PlayerEvent
{
    /// <summary>
    /// Public controllable-registration snapshot the event refers to.
    /// This is not the owner-side <see cref="GalaxyHierarchy.Controllable" /> runtime object.
    /// </summary>
    public readonly ControllableInfo ControllableInfo;

    internal ControllableInfoPlayerEvent(Player player, ControllableInfo controllableInfo) : base(player)
    {
        ControllableInfo = controllableInfo;
    }
}
