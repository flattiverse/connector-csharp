using Flattiverse.Connector.Hierarchy;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events;

/// <summary>
/// This event is fired when a controllable is destroyed.
/// </summary>
public abstract class ControllableDestroyedEvent : FlattiverseEvent
{
    /// <summary>
    /// The player that lost the Controllable.
    /// </summary>
    public readonly Player Player;
    
    /// <summary>
    /// The affected ControllableInfo.
    /// </summary>
    public readonly ControllableInfo ControllableInfo;

    internal ControllableDestroyedEvent(Player player, ControllableInfo controllableInfo)
    {
        Player = player;
        ControllableInfo = controllableInfo;
    }
    
    /// <summary>
    /// The reason for the death of your Controllable.
    /// </summary>
    public abstract DestructionReason Reason { get; }
}