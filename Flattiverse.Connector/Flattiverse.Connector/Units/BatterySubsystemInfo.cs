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

    internal void Update(bool exists, float maximum, float current, float consumedThisTick, SubsystemStatus status)
    {
        _exists = exists;
        _maximum = exists ? maximum : 0f;
        _current = exists ? current : 0f;
        _consumedThisTick = exists ? consumedThisTick : 0f;
        _status = exists ? status : SubsystemStatus.Off;
    }
}
