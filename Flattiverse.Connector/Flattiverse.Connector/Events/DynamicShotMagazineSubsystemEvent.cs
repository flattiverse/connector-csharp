using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Runtime update of a dynamic shot magazine subsystem on your own controllable.
/// </summary>
public class DynamicShotMagazineSubsystemEvent : ControllableSubsystemEvent
{
    /// <summary>
    /// The currently stored shots.
    /// </summary>
    public readonly float CurrentShots;

    internal DynamicShotMagazineSubsystemEvent(Controllable controllable, SubsystemSlot slot, SubsystemStatus status, float currentShots) :
        base(controllable, slot, status)
    {
        CurrentShots = currentShots;
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.DynamicShotMagazineSubsystem;

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} Dynamic shot magazine subsystem event: Controllable=\"{Controllable.Name}\", Slot={Slot}, Status={Status}, CurrentShots={CurrentShots:0.###}.";
    }
}
