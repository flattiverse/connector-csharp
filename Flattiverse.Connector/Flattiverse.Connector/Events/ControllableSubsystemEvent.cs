using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Base type for owner-only subsystem runtime events of your own controllables.
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
    /// Runtime status reported for the current server tick.
    /// This status is independent from configuration flags such as <c>Active</c> on specific subsystem types.
    /// </summary>
    public readonly SubsystemStatus Status;

    internal ControllableSubsystemEvent(Controllable controllable, SubsystemSlot slot, SubsystemStatus status) : base()
    {
        Controllable = controllable;
        Slot = slot;
        Status = status;
    }
}
