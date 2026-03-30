using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Runtime update of a dynamic interceptor magazine subsystem on your own controllable.
/// </summary>
public class DynamicInterceptorMagazineSubsystemEvent : DynamicShotMagazineSubsystemEvent
{
    internal DynamicInterceptorMagazineSubsystemEvent(Controllable controllable, SubsystemSlot slot, SubsystemStatus status,
        float currentShots) : base(controllable, slot, status, currentShots)
    {
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.DynamicInterceptorMagazineSubsystem;

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} Dynamic interceptor magazine subsystem event: Controllable=\"{Controllable.Name}\", Slot={Slot}, Status={Status}, CurrentShots={CurrentShots:0.###}.";
    }
}
