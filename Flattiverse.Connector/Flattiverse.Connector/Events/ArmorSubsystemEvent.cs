using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Runtime update of an armor subsystem on your own controllable.
/// </summary>
public class ArmorSubsystemEvent : ControllableSubsystemEvent
{
    /// <summary>
    /// Flat damage reduction applied before the hull.
    /// </summary>
    public readonly float Reduction;

    /// <summary>
    /// Direct damage blocked during the current tick.
    /// </summary>
    public readonly float BlockedDirectDamageThisTick;

    /// <summary>
    /// Radiation damage blocked during the current tick.
    /// </summary>
    public readonly float BlockedRadiationDamageThisTick;

    internal ArmorSubsystemEvent(Controllable controllable, SubsystemSlot slot, SubsystemStatus status, float reduction,
        float blockedDirectDamageThisTick, float blockedRadiationDamageThisTick) : base(controllable, slot, status)
    {
        Reduction = reduction;
        BlockedDirectDamageThisTick = blockedDirectDamageThisTick;
        BlockedRadiationDamageThisTick = blockedRadiationDamageThisTick;
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.ArmorSubsystem;

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} Armor subsystem event: Controllable=\"{Controllable.Name}\", Slot={Slot}, Status={Status}, Reduction={Reduction:0.###}, BlockedDirect={BlockedDirectDamageThisTick:0.###}, BlockedRadiation={BlockedRadiationDamageThisTick:0.###}.";
    }
}
