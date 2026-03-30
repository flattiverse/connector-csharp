using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Runtime update of a railgun subsystem on your own controllable.
/// </summary>
public class RailgunSubsystemEvent : ControllableSubsystemEvent
{
    /// <summary>
    /// The direction processed in the current tick.
    /// </summary>
    public readonly RailgunDirection Direction;

    /// <summary>
    /// The energy consumed during the current server tick.
    /// </summary>
    public readonly float ConsumedEnergyThisTick;

    /// <summary>
    /// The ions consumed during the current server tick.
    /// </summary>
    public readonly float ConsumedIonsThisTick;

    /// <summary>
    /// The neutrinos consumed during the current server tick.
    /// </summary>
    public readonly float ConsumedNeutrinosThisTick;

    internal RailgunSubsystemEvent(Controllable controllable, SubsystemSlot slot, SubsystemStatus status, RailgunDirection direction,
        float consumedEnergyThisTick, float consumedIonsThisTick, float consumedNeutrinosThisTick) : base(controllable, slot, status)
    {
        Direction = direction;
        ConsumedEnergyThisTick = consumedEnergyThisTick;
        ConsumedIonsThisTick = consumedIonsThisTick;
        ConsumedNeutrinosThisTick = consumedNeutrinosThisTick;
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.RailgunSubsystem;

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} Railgun subsystem event: Controllable=\"{Controllable.Name}\", Slot={Slot}, Status={Status}, Direction={Direction}, Consumed=({ConsumedEnergyThisTick:0.###},{ConsumedIonsThisTick:0.###},{ConsumedNeutrinosThisTick:0.###}).";
    }
}
