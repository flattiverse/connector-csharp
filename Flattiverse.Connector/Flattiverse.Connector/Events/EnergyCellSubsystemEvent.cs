using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Runtime update of an energy-cell subsystem on your own controllable.
/// </summary>
public class EnergyCellSubsystemEvent : ControllableSubsystemEvent
{
    /// <summary>
    /// The amount collected during the current server tick.
    /// </summary>
    public readonly float CollectedThisTick;

    internal EnergyCellSubsystemEvent(Controllable controllable, SubsystemSlot slot, SubsystemStatus status, float collectedThisTick) :
        base(controllable, slot, status)
    {
        CollectedThisTick = collectedThisTick;
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.EnergyCellSubsystem;

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} Energy-cell subsystem event: Controllable=\"{Controllable.Name}\", Slot={Slot}, Status={Status}, Collected={CollectedThisTick:0.###}.";
    }
}
