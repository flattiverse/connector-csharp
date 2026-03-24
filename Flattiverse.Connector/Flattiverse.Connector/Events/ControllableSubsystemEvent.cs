using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Base class for subsystem runtime events of your own controllables.
/// </summary>
public abstract class ControllableSubsystemEvent : FlattiverseEvent
{
    /// <summary>
    /// The controllable whose subsystem emitted this runtime update.
    /// </summary>
    public readonly Controllable Controllable;

    /// <summary>
    /// The concrete subsystem slot on the controllable.
    /// </summary>
    public readonly SubsystemSlot Slot;

    /// <summary>
    /// The status reported for the current server tick.
    /// </summary>
    public readonly SubsystemStatus Status;

    internal ControllableSubsystemEvent(Controllable controllable, SubsystemSlot slot, SubsystemStatus status) : base()
    {
        Controllable = controllable;
        Slot = slot;
        Status = status;
    }
}
