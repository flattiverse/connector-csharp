using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Runtime update of a classic-ship engine subsystem on your own controllable.
/// </summary>
public class ClassicShipEngineSubsystemEvent : ControllableSubsystemEvent
{
    /// <summary>
    /// The current applied engine vector.
    /// </summary>
    public readonly Vector Current;

    /// <summary>
    /// The configured target engine vector.
    /// </summary>
    public readonly Vector Target;

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

    internal ClassicShipEngineSubsystemEvent(Controllable controllable, SubsystemSlot slot, SubsystemStatus status, Vector current,
        Vector target, float consumedEnergyThisTick, float consumedIonsThisTick, float consumedNeutrinosThisTick) :
        base(controllable, slot, status)
    {
        Current = new Vector(current);
        Target = new Vector(target);
        ConsumedEnergyThisTick = consumedEnergyThisTick;
        ConsumedIonsThisTick = consumedIonsThisTick;
        ConsumedNeutrinosThisTick = consumedNeutrinosThisTick;
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.ClassicShipEngineSubsystem;

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} Engine subsystem event: Controllable=\"{Controllable.Name}\", Slot={Slot}, Status={Status}, Current={Current}, Target={Target}, Consumed=({ConsumedEnergyThisTick:0.###},{ConsumedIonsThisTick:0.###},{ConsumedNeutrinosThisTick:0.###}).";
    }
}
