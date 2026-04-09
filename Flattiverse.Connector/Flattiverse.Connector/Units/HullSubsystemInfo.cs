using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// Visible snapshot of a hull subsystem on a scanned player unit.
/// </summary>
public class HullSubsystemInfo
{
    private bool _exists;
    private float _maximum;
    private float _current;
    private SubsystemStatus _status;

    internal HullSubsystemInfo()
    {
        _exists = false;
        _maximum = 0f;
        _current = 0f;
        _status = SubsystemStatus.Off;
    }

    internal HullSubsystemInfo(HullSubsystemInfo other)
    {
        _exists = other._exists;
        _maximum = other._maximum;
        _current = other._current;
        _status = other._status;
    }

    /// <summary>
    /// Indicates whether the scanned unit actually has this hull subsystem installed.
    /// </summary>
    public bool Exists
    {
        get { return _exists; }
    }

    /// <summary>
    /// Maximum hull integrity of the scanned unit.
    /// </summary>
    public float Maximum
    {
        get { return _maximum; }
    }

    /// <summary>
    /// Current hull integrity of the scanned unit.
    /// </summary>
    public float Current
    {
        get { return _current; }
    }

    /// <summary>
    /// Tick-local runtime status reported for the hull subsystem.
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
            _status = SubsystemStatus.Off;
            return true;
        }

        if (!reader.Read(out _maximum) ||
            !reader.Read(out _current) ||
            !reader.Read(out byte status))
            return false;

        _status = (SubsystemStatus)status;
        return true;
    }
}
