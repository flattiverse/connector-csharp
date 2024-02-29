using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events;

/// <summary>
/// This event is fired when a controllable is destroyed.
/// </summary>
public abstract class ControllableDestroyedEvent : FlattiverseEvent
{
    /// <summary>
    /// Your affected Controllable.
    /// </summary>
    public readonly Controllable Controllable;

    internal ControllableDestroyedEvent(Controllable controllable)
    {
        Controllable = controllable;
    }
    
    /// <summary>
    /// The reason for the death of your Controllable.
    /// </summary>
    public abstract DestructionReason Reason { get; }
}