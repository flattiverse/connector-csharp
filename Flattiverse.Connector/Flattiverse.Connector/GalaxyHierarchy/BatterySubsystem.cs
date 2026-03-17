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

        if (_current > _maximum)
            _current = _maximum;
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

    internal FlattiverseEvent? CreateRuntimeEvent()
    {
        if (!Exists || !ShouldEmitRuntimeEvent())
            return null;

        return new BatterySubsystemEvent(Controllable, Slot, Status, _current, _consumedThisTick);
    }
}
