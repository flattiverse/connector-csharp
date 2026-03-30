using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Runtime update of a repair subsystem on your own controllable.
/// </summary>
public class RepairSubsystemEvent : ControllableSubsystemEvent
{
    /// <summary>
    /// Configured hull repair rate for the tick.
    /// </summary>
    public readonly float Rate;

    /// <summary>
    /// Energy consumed during the current server tick.
    /// </summary>
    public readonly float ConsumedEnergyThisTick;

    /// <summary>
    /// Ions consumed during the current server tick.
    /// </summary>
    public readonly float ConsumedIonsThisTick;

    /// <summary>
    /// Neutrinos consumed during the current server tick.
    /// </summary>
    public readonly float ConsumedNeutrinosThisTick;

    /// <summary>
    /// Hull repaired during the current tick.
    /// </summary>
    public readonly float RepairedHullThisTick;

    internal RepairSubsystemEvent(Controllable controllable, SubsystemSlot slot, SubsystemStatus status, float rate,
        float consumedEnergyThisTick, float consumedIonsThisTick, float consumedNeutrinosThisTick, float repairedHullThisTick) :
        base(controllable, slot, status)
    {
        Rate = rate;
        ConsumedEnergyThisTick = consumedEnergyThisTick;
        ConsumedIonsThisTick = consumedIonsThisTick;
        ConsumedNeutrinosThisTick = consumedNeutrinosThisTick;
        RepairedHullThisTick = repairedHullThisTick;
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.RepairSubsystem;

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} Repair subsystem event: Controllable=\"{Controllable.Name}\", Slot={Slot}, Status={Status}, Rate={Rate:0.###}, RepairedHull={RepairedHullThisTick:0.###}, Consumed=({ConsumedEnergyThisTick:0.###},{ConsumedIonsThisTick:0.###},{ConsumedNeutrinosThisTick:0.###}).";
    }
}
