using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Runtime update of a modern-ship engine subsystem on your own controllable.
/// </summary>
public class ModernShipEngineSubsystemEvent : ControllableSubsystemEvent
{
    public readonly float CurrentThrust;
    public readonly float TargetThrust;
    public readonly float ConsumedEnergyThisTick;
    public readonly float ConsumedIonsThisTick;
    public readonly float ConsumedNeutrinosThisTick;

    internal ModernShipEngineSubsystemEvent(Controllable controllable, SubsystemSlot slot, SubsystemStatus status, float currentThrust,
        float targetThrust, float consumedEnergyThisTick, float consumedIonsThisTick, float consumedNeutrinosThisTick) :
        base(controllable, slot, status)
    {
        CurrentThrust = currentThrust;
        TargetThrust = targetThrust;
        ConsumedEnergyThisTick = consumedEnergyThisTick;
        ConsumedIonsThisTick = consumedIonsThisTick;
        ConsumedNeutrinosThisTick = consumedNeutrinosThisTick;
    }

    public override EventKind Kind => EventKind.ModernShipEngineSubsystem;

    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} Modern ship engine subsystem event: Controllable=\"{Controllable.Name}\", Slot={Slot}, Status={Status}, CurrentThrust={CurrentThrust:0.###}, TargetThrust={TargetThrust:0.###}, Consumed=({ConsumedEnergyThisTick:0.###},{ConsumedIonsThisTick:0.###},{ConsumedNeutrinosThisTick:0.###}).";
    }
}
