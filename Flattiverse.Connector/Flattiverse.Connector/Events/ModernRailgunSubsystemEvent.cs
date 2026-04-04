using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Runtime update of a modern railgun subsystem on your own controllable.
/// </summary>
public class ModernRailgunSubsystemEvent : ClassicRailgunSubsystemEvent
{
    internal ModernRailgunSubsystemEvent(Controllable controllable, SubsystemSlot slot, SubsystemStatus status, RailgunDirection direction,
        float consumedEnergyThisTick, float consumedIonsThisTick, float consumedNeutrinosThisTick) :
        base(controllable, slot, status, direction, consumedEnergyThisTick, consumedIonsThisTick, consumedNeutrinosThisTick)
    {
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.ModernRailgunSubsystem;

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} Modern railgun subsystem event: Controllable=\"{Controllable.Name}\", Slot={Slot}, Status={Status}, Direction={Direction}, Consumed=({ConsumedEnergyThisTick:0.###},{ConsumedIonsThisTick:0.###},{ConsumedNeutrinosThisTick:0.###}).";
    }
}
