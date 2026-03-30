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

    internal void Update(bool exists, float maximum, float current, SubsystemStatus status)
    {
        _exists = exists;
        _maximum = exists ? maximum : 0f;
        _current = exists ? current : 0f;
        _status = exists ? status : SubsystemStatus.Off;
    }
}
