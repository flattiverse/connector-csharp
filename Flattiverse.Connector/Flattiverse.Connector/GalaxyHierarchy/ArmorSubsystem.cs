using Flattiverse.Connector.Network;

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

    internal ArmorSubsystem(Controllable controllable, string name, PacketReader reader, SubsystemSlot slot) :
        base(controllable, name, false, slot)
    {
        _reduction = 0f;
        _blockedDirectDamageThisTick = 0f;
        _blockedRadiationDamageThisTick = 0f;

        if (!reader.Read(out byte exists))
            throw new InvalidDataException("Couldn't read controllable armor state.");

        SetExists(exists != 0);

        if (!Exists)
            return;

        if (!reader.Read(out byte tier) ||
            !reader.Read(out float reduction) ||
            !reader.Read(out byte status) ||
            !reader.Read(out float blockedDirectDamageThisTick) ||
            !reader.Read(out float blockedRadiationDamageThisTick))
            throw new InvalidDataException("Couldn't read controllable armor state.");

        SetReduction(reduction);
        UpdateRuntime(blockedDirectDamageThisTick, blockedRadiationDamageThisTick, (SubsystemStatus)status);
        SetReportedTier(tier);
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
        RefreshTier();
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

    internal bool Update(PacketReader reader)
    {
        if (!Exists)
            return true;

        if (!reader.Read(out float blockedDirectDamageThisTick) ||
            !reader.Read(out float blockedRadiationDamageThisTick) ||
            !reader.Read(out byte status))
            return false;

        UpdateRuntime(blockedDirectDamageThisTick, blockedRadiationDamageThisTick, (SubsystemStatus)status);
        return true;
    }

    internal FlattiverseEvent? CreateRuntimeEvent()
    {
        if (!Exists || !ShouldEmitRuntimeEvent())
            return null;

        return new ArmorSubsystemEvent(Controllable, Slot, Status, _reduction, _blockedDirectDamageThisTick, _blockedRadiationDamageThisTick);
    }

    protected override void RefreshTier()
    {
        if (!Exists)
        {
            SetTier(0);
            return;
        }

        for (byte tier = 1; tier <= ShipUpgradeBalancing.GetMaximumTier(Slot); tier++)
        {
            ShipBalancing.GetArmor(tier, out float reduction, out float load);

            if (Matches(_reduction, reduction))
            {
                SetTier(tier);
                return;
            }
        }

        SetTier(0);
    }
}
