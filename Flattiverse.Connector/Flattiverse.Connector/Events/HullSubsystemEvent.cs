using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Runtime update of a hull subsystem on your own controllable.
/// </summary>
public class HullSubsystemEvent : ControllableSubsystemEvent
{
    /// <summary>
    /// The current hull integrity.
    /// </summary>
    public readonly float Current;

    internal HullSubsystemEvent(Controllable controllable, SubsystemSlot slot, SubsystemStatus status, float current) :
        base(controllable, slot, status)
    {
        Current = current;
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.HullSubsystem;

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} Hull subsystem event: Controllable=\"{Controllable.Name}\", Slot={Slot}, Status={Status}, Current={Current:0.###}.";
    }
}
