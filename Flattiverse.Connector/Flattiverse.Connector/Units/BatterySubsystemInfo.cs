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
    /// true if the subsystem exists.
    /// </summary>
    public bool Exists
    {
        get { return _exists; }
    }

    /// <summary>
    /// The maximum storable amount.
    /// </summary>
    public float Maximum
    {
        get { return _maximum; }
    }

    /// <summary>
    /// The currently stored amount.
    /// </summary>
    public float Current
    {
        get { return _current; }
    }

    /// <summary>
    /// The amount consumed during the current server tick.
    /// </summary>
    public float ConsumedThisTick
    {
        get { return _consumedThisTick; }
    }

    /// <summary>
    /// The runtime status for the current server tick.
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
