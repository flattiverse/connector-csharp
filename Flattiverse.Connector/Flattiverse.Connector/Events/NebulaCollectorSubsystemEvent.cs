using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Runtime update of a nebula collector subsystem on your own controllable.
/// </summary>
public class NebulaCollectorSubsystemEvent : ControllableSubsystemEvent
{
    /// <summary>
    /// Collector rate mirrored for the current server tick.
    /// </summary>
    public readonly float Rate;

    /// <summary>
    /// Energy consumed by the collector during the current server tick.
    /// </summary>
    public readonly float ConsumedEnergyThisTick;

    /// <summary>
    /// Ions consumed by the collector during the current server tick.
    /// </summary>
    public readonly float ConsumedIonsThisTick;

    /// <summary>
    /// Neutrinos consumed by the collector during the current server tick.
    /// </summary>
    public readonly float ConsumedNeutrinosThisTick;

    /// <summary>
    /// Nebula amount collected during the current server tick.
    /// </summary>
    public readonly float CollectedThisTick;

    /// <summary>
    /// Hue of the nebula material collected during the current server tick.
    /// </summary>
    public readonly float CollectedHueThisTick;

    internal NebulaCollectorSubsystemEvent(Controllable controllable, SubsystemSlot slot, SubsystemStatus status, float rate,
        float consumedEnergyThisTick, float consumedIonsThisTick, float consumedNeutrinosThisTick, float collectedThisTick,
        float collectedHueThisTick) : base(controllable, slot, status)
    {
        Rate = rate;
        ConsumedEnergyThisTick = consumedEnergyThisTick;
        ConsumedIonsThisTick = consumedIonsThisTick;
        ConsumedNeutrinosThisTick = consumedNeutrinosThisTick;
        CollectedThisTick = collectedThisTick;
        CollectedHueThisTick = collectedHueThisTick;
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.NebulaCollectorSubsystem;

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} Nebula collector subsystem event: Controllable=\"{Controllable.Name}\", Slot={Slot}, Status={Status}, Rate={Rate:0.###}, Consumed=({ConsumedEnergyThisTick:0.###},{ConsumedIonsThisTick:0.###},{ConsumedNeutrinosThisTick:0.###}), Collected={CollectedThisTick:0.###}, Hue={CollectedHueThisTick:0.###}.";
    }
}
