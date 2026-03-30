using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Runtime update of a dynamic interceptor fabricator subsystem on your own controllable.
/// </summary>
public class DynamicInterceptorFabricatorSubsystemEvent : DynamicShotFabricatorSubsystemEvent
{
    internal DynamicInterceptorFabricatorSubsystemEvent(Controllable controllable, SubsystemSlot slot, SubsystemStatus status, bool active,
        float rate, float consumedEnergyThisTick, float consumedIonsThisTick, float consumedNeutrinosThisTick) :
        base(controllable, slot, status, active, rate, consumedEnergyThisTick, consumedIonsThisTick, consumedNeutrinosThisTick)
    {
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.DynamicInterceptorFabricatorSubsystem;

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} Dynamic interceptor fabricator subsystem event: Controllable=\"{Controllable.Name}\", Slot={Slot}, Status={Status}, Active={Active}, Rate={Rate:0.###}, Consumed=({ConsumedEnergyThisTick:0.###},{ConsumedIonsThisTick:0.###},{ConsumedNeutrinosThisTick:0.###}).";
    }
}
