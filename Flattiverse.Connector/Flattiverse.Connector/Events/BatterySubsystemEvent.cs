using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Runtime update of a battery subsystem on your own controllable.
/// </summary>
public class BatterySubsystemEvent : ControllableSubsystemEvent
{
    /// <summary>
    /// The current stored amount.
    /// </summary>
    public readonly float Current;

    /// <summary>
    /// The amount consumed during the current server tick.
    /// </summary>
    public readonly float ConsumedThisTick;

    internal BatterySubsystemEvent(Controllable controllable, SubsystemSlot slot, SubsystemStatus status, float current, float consumedThisTick) :
        base(controllable, slot, status)
    {
        Current = current;
        ConsumedThisTick = consumedThisTick;
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.BatterySubsystem;

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} Battery subsystem event: Controllable=\"{Controllable.Name}\", Slot={Slot}, Status={Status}, Current={Current:0.###}, Consumed={ConsumedThisTick:0.###}.";
    }
}
