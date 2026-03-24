using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Runtime update of a dynamic shot fabricator subsystem on your own controllable.
/// </summary>
public class DynamicShotFabricatorSubsystemEvent : ControllableSubsystemEvent
{
    /// <summary>
    /// Whether the fabricator was active for the tick.
    /// </summary>
    public readonly bool Active;

    /// <summary>
    /// The configured fabrication rate.
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

    internal DynamicShotFabricatorSubsystemEvent(Controllable controllable, SubsystemSlot slot, SubsystemStatus status, bool active, float rate,
        float consumedEnergyThisTick, float consumedIonsThisTick, float consumedNeutrinosThisTick) :
        base(controllable, slot, status)
    {
        Active = active;
        Rate = rate;
        ConsumedEnergyThisTick = consumedEnergyThisTick;
        ConsumedIonsThisTick = consumedIonsThisTick;
        ConsumedNeutrinosThisTick = consumedNeutrinosThisTick;
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.DynamicShotFabricatorSubsystem;

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} Dynamic shot fabricator subsystem event: Controllable=\"{Controllable.Name}\", Slot={Slot}, Status={Status}, Active={Active}, Rate={Rate:0.###}, Consumed=({ConsumedEnergyThisTick:0.###},{ConsumedIonsThisTick:0.###},{ConsumedNeutrinosThisTick:0.###}).";
    }
}
