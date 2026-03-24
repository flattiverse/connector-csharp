using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Runtime update of a shield subsystem on your own controllable.
/// </summary>
public class ShieldSubsystemEvent : ControllableSubsystemEvent
{
    /// <summary>
    /// The current shield integrity.
    /// </summary>
    public readonly float Current;

    /// <summary>
    /// Whether shield loading was active for the tick.
    /// </summary>
    public readonly bool Active;

    /// <summary>
    /// The configured shield load rate.
    /// </summary>
    public readonly float Rate;

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

    internal ShieldSubsystemEvent(Controllable controllable, SubsystemSlot slot, SubsystemStatus status, float current, bool active, float rate,
        float consumedEnergyThisTick, float consumedIonsThisTick, float consumedNeutrinosThisTick) :
        base(controllable, slot, status)
    {
        Current = current;
        Active = active;
        Rate = rate;
        ConsumedEnergyThisTick = consumedEnergyThisTick;
        ConsumedIonsThisTick = consumedIonsThisTick;
        ConsumedNeutrinosThisTick = consumedNeutrinosThisTick;
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.ShieldSubsystem;

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} Shield subsystem event: Controllable=\"{Controllable.Name}\", Slot={Slot}, Status={Status}, Current={Current:0.###}, Active={Active}, Rate={Rate:0.###}, Consumed=({ConsumedEnergyThisTick:0.###},{ConsumedIonsThisTick:0.###},{ConsumedNeutrinosThisTick:0.###}).";
    }
}
