namespace Flattiverse.Connector.Units;

/// <summary>
/// Visible snapshot of a shield subsystem on a scanned player unit.
/// </summary>
public class ShieldSubsystemInfo
{
    private bool _exists;
    private float _maximum;
    private float _current;
    private bool _active;
    private float _rate;
    private SubsystemStatus _status;
    private float _consumedEnergyThisTick;
    private float _consumedIonsThisTick;
    private float _consumedNeutrinosThisTick;

    internal ShieldSubsystemInfo()
    {
        _exists = false;
        _maximum = 0f;
        _current = 0f;
        _active = false;
        _rate = 0f;
        _status = SubsystemStatus.Off;
        _consumedEnergyThisTick = 0f;
        _consumedIonsThisTick = 0f;
        _consumedNeutrinosThisTick = 0f;
    }

    /// <summary>
    /// Indicates whether the subsystem exists on the scanned unit.
    /// </summary>
    public bool Exists
    {
        get { return _exists; }
    }

    /// <summary>
    /// The maximum shield integrity.
    /// </summary>
    public float Maximum
    {
        get { return _maximum; }
    }

    /// <summary>
    /// The current shield integrity.
    /// </summary>
    public float Current
    {
        get { return _current; }
    }

    /// <summary>
    /// Whether shield loading was active for the tick.
    /// </summary>
    public bool Active
    {
        get { return _active; }
    }

    /// <summary>
    /// The configured shield load rate.
    /// </summary>
    public float Rate
    {
        get { return _rate; }
    }

    /// <summary>
    /// Status of the reported shield subsystem.
    /// </summary>
    public SubsystemStatus Status
    {
        get { return _status; }
    }

    /// <summary>
    /// Energy consumed by the shield during the tick.
    /// </summary>
    public float ConsumedEnergyThisTick
    {
        get { return _consumedEnergyThisTick; }
    }

    /// <summary>
    /// Ions consumed by the shield during the tick.
    /// </summary>
    public float ConsumedIonsThisTick
    {
        get { return _consumedIonsThisTick; }
    }

    /// <summary>
    /// Neutrinos consumed by the shield during the tick.
    /// </summary>
    public float ConsumedNeutrinosThisTick
    {
        get { return _consumedNeutrinosThisTick; }
    }

    internal void Update(bool exists, float maximum, float current, bool active, float rate, SubsystemStatus status, float consumedEnergyThisTick,
        float consumedIonsThisTick, float consumedNeutrinosThisTick)
    {
        _exists = exists;
        _maximum = exists ? maximum : 0f;
        _current = exists ? current : 0f;
        _active = exists && active;
        _rate = exists ? rate : 0f;
        _status = exists ? status : SubsystemStatus.Off;
        _consumedEnergyThisTick = exists ? consumedEnergyThisTick : 0f;
        _consumedIonsThisTick = exists ? consumedIonsThisTick : 0f;
        _consumedNeutrinosThisTick = exists ? consumedNeutrinosThisTick : 0f;
    }
}
