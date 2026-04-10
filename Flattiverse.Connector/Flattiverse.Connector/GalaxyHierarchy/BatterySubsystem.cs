using Flattiverse.Connector.Network;

using Flattiverse.Connector.Events;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Passive battery subsystem of a controllable.
/// </summary>
public class BatterySubsystem : Subsystem
{
    private float _maximum;
    private float _current;
    private float _consumedThisTick;

    internal BatterySubsystem(Controllable controllable, string name, bool exists, float maximum, SubsystemSlot slot) :
        base(controllable, name, exists, slot)
    {
        _maximum = 0f;
        _current = 0f;
        _consumedThisTick = 0f;

        SetMaximum(maximum);
    }

    internal BatterySubsystem(Controllable controllable, string name, PacketReader reader, SubsystemSlot slot) :
        base(controllable, name, false, slot)
    {
        _maximum = 0f;
        _current = 0f;
        _consumedThisTick = 0f;

        if (!reader.Read(out byte exists))
            throw new InvalidDataException("Couldn't read controllable battery state.");

        SetExists(exists != 0);

        if (!Exists)
            return;

        if (!reader.Read(out byte tier) ||
            !reader.Read(out float maximum) ||
            !reader.Read(out float current) ||
            !reader.Read(out float consumedThisTick) ||
            !reader.Read(out byte status))
            throw new InvalidDataException("Couldn't read controllable battery state.");

        SetMaximum(maximum);
        UpdateRuntime(current, consumedThisTick, (SubsystemStatus)status);
        SetReportedTier(tier);
    }

    internal static BatterySubsystem CreateClassicShipEnergyBattery(Controllable controllable)
    {
        return new BatterySubsystem(controllable, "EnergyBattery", true, 20000f, SubsystemSlot.EnergyBattery);
    }

    internal static BatterySubsystem CreateMissingBattery(Controllable controllable, string name, SubsystemSlot slot)
    {
        return new BatterySubsystem(controllable, name, false, 0f, slot);
    }

    /// <summary>
    /// The maximum storable amount for this battery.
    /// </summary>
    public float Maximum
    {
        get { return _maximum; }
    }

    /// <summary>
    /// The current stored amount.
    /// </summary>
    public float Current
    {
        get { return _current; }
    }

    /// <summary>
    /// The currently free battery capacity.
    /// </summary>
    public float Free
    {
        get { return _maximum - _current; }
    }

    /// <summary>
    /// The amount consumed during the current server tick.
    /// </summary>
    public float ConsumedThisTick
    {
        get { return _consumedThisTick; }
    }

    internal void SetMaximum(float maximum)
    {
        _maximum = Exists ? maximum : 0f;
        RefreshTier();

        if (_current > _maximum)
            _current = _maximum;
    }

    internal void CopyFrom(BatterySubsystem other)
    {
        CopyBaseFrom(other);
        _maximum = other._maximum;
        _current = other._current;
        _consumedThisTick = other._consumedThisTick;
    }

    internal void ResetRuntime()
    {
        _current = 0f;
        _consumedThisTick = 0f;
        ResetRuntimeStatus();
    }

    internal void UpdateRuntime(float current, float consumedThisTick, SubsystemStatus status)
    {
        _current = current;
        _consumedThisTick = consumedThisTick;
        UpdateRuntimeStatus(status);
    }

    internal bool Update(PacketReader reader)
    {
        if (!Exists)
            return true;

        if (!reader.Read(out float current) ||
            !reader.Read(out float consumedThisTick) ||
            !reader.Read(out byte status))
            return false;

        UpdateRuntime(current, consumedThisTick, (SubsystemStatus)status);
        return true;
    }

    internal FlattiverseEvent? CreateRuntimeEvent()
    {
        if (!Exists || !ShouldEmitRuntimeEvent())
            return null;

        return new BatterySubsystemEvent(Controllable, Slot, Status, _current, _consumedThisTick);
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
            float maximum;
            float load;

            switch (Slot)
            {
                case SubsystemSlot.EnergyBattery:
                    ShipBalancing.GetBattery(tier, out maximum, out load);
                    break;
                case SubsystemSlot.IonBattery:
                    ShipBalancing.GetIonBattery(tier, out maximum, out load);
                    break;
                case SubsystemSlot.NeutrinoBattery:
                    ShipBalancing.GetNeutrinoBattery(tier, out maximum, out load);
                    break;
                default:
                    maximum = 0f;
                    load = 0f;
                    break;
            }

            if (Matches(_maximum, maximum))
            {
                SetTier(tier);
                return;
            }
        }

        SetTier(0);
    }
}
