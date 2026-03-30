using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Runtime update of a dynamic interceptor launcher subsystem on your own controllable.
/// </summary>
public class DynamicInterceptorLauncherSubsystemEvent : DynamicShotLauncherSubsystemEvent
{
    internal DynamicInterceptorLauncherSubsystemEvent(Controllable controllable, SubsystemSlot slot, SubsystemStatus status,
        Vector relativeMovement, ushort ticks, float load, float damage, float consumedEnergyThisTick, float consumedIonsThisTick,
        float consumedNeutrinosThisTick) : base(controllable, slot, status, relativeMovement, ticks, load, damage, consumedEnergyThisTick,
        consumedIonsThisTick, consumedNeutrinosThisTick)
    {
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.DynamicInterceptorLauncherSubsystem;

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} Dynamic interceptor launcher subsystem event: Controllable=\"{Controllable.Name}\", Slot={Slot}, Status={Status}, RelativeMovement={RelativeMovement}, Ticks={Ticks}, Load={Load:0.###}, Damage={Damage:0.###}, Consumed=({ConsumedEnergyThisTick:0.###},{ConsumedIonsThisTick:0.###},{ConsumedNeutrinosThisTick:0.###}).";
    }
}
