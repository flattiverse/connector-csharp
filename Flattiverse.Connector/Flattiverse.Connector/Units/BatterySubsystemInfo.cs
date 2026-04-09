using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// Visible snapshot of a battery subsystem on a scanned player unit.
/// </summary>
public class BatterySubsystemInfo
{
    private bool _exists;
    private float _maximum;
    private float _current;
    private float _consumedThisTick;
    private SubsystemStatus _status;

    internal BatterySubsystemInfo()
    {
        _exists = false;
        _maximum = 0f;
        _current = 0f;
        _consumedThisTick = 0f;
        _status = SubsystemStatus.Off;
    }

    internal BatterySubsystemInfo(BatterySubsystemInfo other)
    {
        _exists = other._exists;
        _maximum = other._maximum;
        _current = other._current;
        _consumedThisTick = other._consumedThisTick;
        _status = other._status;
    }

    /// <summary>
    /// Indicates whether the scanned unit actually has this battery subsystem installed.
    /// </summary>
    public bool Exists
    {
        get { return _exists; }
    }

    /// <summary>
    /// Maximum storable amount in this battery.
    /// </summary>
    public float Maximum
    {
        get { return _maximum; }
    }

    /// <summary>
    /// Currently stored amount in this battery.
    /// </summary>
    public float Current
    {
        get { return _current; }
    }

    /// <summary>
    /// Amount consumed during the current server tick.
    /// </summary>
    public float ConsumedThisTick
    {
        get { return _consumedThisTick; }
    }

    /// <summary>
    /// Tick-local runtime status reported for this battery.
    /// </summary>
    public SubsystemStatus Status
    {
        get { return _status; }
    }

    internal bool Update(PacketReader reader)
    {
        if (!reader.Read(out byte exists))
            return false;

        _exists = exists != 0;

        if (!_exists)
        {
            _maximum = 0f;
            _current = 0f;
            _consumedThisTick = 0f;
            _status = SubsystemStatus.Off;
            return true;
        }

        if (!reader.Read(out _maximum) ||
            !reader.Read(out _current) ||
            !reader.Read(out _consumedThisTick) ||
            !reader.Read(out byte status))
            return false;

        _status = (SubsystemStatus)status;
        return true;
    }
}
