using Flattiverse.Connector.Events;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Passive armor subsystem of a controllable.
/// </summary>
public class ArmorSubsystem : Subsystem
{
    private float _reduction;
    private float _blockedDirectDamageThisTick;
    private float _blockedRadiationDamageThisTick;

    internal ArmorSubsystem(Controllable controllable, string name, bool exists, SubsystemSlot slot) :
        base(controllable, name, exists, slot)
    {
        _reduction = exists ? 0.5f : 0f;
        _blockedDirectDamageThisTick = 0f;
        _blockedRadiationDamageThisTick = 0f;
    }

    internal static ArmorSubsystem CreateClassicShipArmor(Controllable controllable)
    {
        return new ArmorSubsystem(controllable, "Armor", true, SubsystemSlot.Armor);
    }

    /// <summary>
    /// Flat damage reduction applied before the hull.
    /// </summary>
    public float Reduction
    {
        get { return _reduction; }
    }

    /// <summary>
    /// Direct damage blocked during the current tick.
    /// </summary>
    public float BlockedDirectDamageThisTick
    {
        get { return _blockedDirectDamageThisTick; }
    }

    /// <summary>
    /// Radiation damage blocked during the current tick.
    /// </summary>
    public float BlockedRadiationDamageThisTick
    {
        get { return _blockedRadiationDamageThisTick; }
    }

    /// <summary>
    /// Total damage blocked during the current tick.
    /// </summary>
    public float BlockedTotalThisTick
    {
        get { return _blockedDirectDamageThisTick + _blockedRadiationDamageThisTick; }
    }

    internal void SetReduction(float reduction)
    {
        _reduction = Exists ? reduction : 0f;
    }

    internal void ResetRuntime()
    {
        _blockedDirectDamageThisTick = 0f;
        _blockedRadiationDamageThisTick = 0f;
        ResetRuntimeStatus();
    }

    internal void UpdateRuntime(float blockedDirectDamageThisTick, float blockedRadiationDamageThisTick, SubsystemStatus status)
    {
        _blockedDirectDamageThisTick = blockedDirectDamageThisTick;
        _blockedRadiationDamageThisTick = blockedRadiationDamageThisTick;
        UpdateRuntimeStatus(status);
    }

    internal FlattiverseEvent? CreateRuntimeEvent()
    {
        if (!Exists || !ShouldEmitRuntimeEvent())
            return null;

        return new ArmorSubsystemEvent(Controllable, Slot, Status, _reduction, _blockedDirectDamageThisTick, _blockedRadiationDamageThisTick);
    }
}
