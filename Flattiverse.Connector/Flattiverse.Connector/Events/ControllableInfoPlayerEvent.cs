using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// A parent event managing controllable info events.
/// </summary>
public class ControllableInfoPlayerEvent : PlayerEvent
{
    /// <summary>
    /// The corresponding PlayerUnit the ControllableInfo informs about. 
    /// </summary>
    public readonly ControllableInfo ControllableInfo;

    internal ControllableInfoPlayerEvent(Player player, ControllableInfo controllableInfo) : base(player)
    {
        ControllableInfo = controllableInfo;
    }
}