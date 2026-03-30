using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Runtime update of a dynamic scanner subsystem on your own controllable.
/// </summary>
public class DynamicScannerSubsystemEvent : ControllableSubsystemEvent
{
    /// <summary>
    /// Whether the scanner was active during this server tick.
    /// </summary>
    public readonly bool Active;

    /// <summary>
    /// The current scanner width.
    /// </summary>
    public readonly float CurrentWidth;

    /// <summary>
    /// The current scanner length.
    /// </summary>
    public readonly float CurrentLength;

    /// <summary>
    /// The current scanner angle.
    /// </summary>
    public readonly float CurrentAngle;

    /// <summary>
    /// The target scanner width.
    /// </summary>
    public readonly float TargetWidth;

    /// <summary>
    /// The target scanner length.
    /// </summary>
    public readonly float TargetLength;

    /// <summary>
    /// The target scanner angle.
    /// </summary>
    public readonly float TargetAngle;

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

    internal DynamicScannerSubsystemEvent(Controllable controllable, SubsystemSlot slot, SubsystemStatus status, bool active, float currentWidth,
        float currentLength, float currentAngle, float targetWidth, float targetLength, float targetAngle,
        float consumedEnergyThisTick, float consumedIonsThisTick, float consumedNeutrinosThisTick) : base(controllable, slot, status)
    {
        Active = active;
        CurrentWidth = currentWidth;
        CurrentLength = currentLength;
        CurrentAngle = currentAngle;
        TargetWidth = targetWidth;
        TargetLength = targetLength;
        TargetAngle = targetAngle;
        ConsumedEnergyThisTick = consumedEnergyThisTick;
        ConsumedIonsThisTick = consumedIonsThisTick;
        ConsumedNeutrinosThisTick = consumedNeutrinosThisTick;
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.DynamicScannerSubsystem;

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} Dynamic scanner subsystem event: Controllable=\"{Controllable.Name}\", Slot={Slot}, Status={Status}, Active={Active}, Current=({CurrentWidth:0.###},{CurrentLength:0.###},{CurrentAngle:0.###}), Target=({TargetWidth:0.###},{TargetLength:0.###},{TargetAngle:0.###}), Consumed=({ConsumedEnergyThisTick:0.###},{ConsumedIonsThisTick:0.###},{ConsumedNeutrinosThisTick:0.###}).";
    }
}
