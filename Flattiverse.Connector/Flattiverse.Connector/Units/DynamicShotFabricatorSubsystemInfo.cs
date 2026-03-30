namespace Flattiverse.Connector.Units;

/// <summary>
/// Visible snapshot of a dynamic shot fabricator subsystem on a scanned player unit.
/// </summary>
public class DynamicShotFabricatorSubsystemInfo
{
    private bool _exists;
    private float _minimumRate;
    private float _maximumRate;
    private bool _active;
    private float _rate;
    private SubsystemStatus _status;
    private float _consumedEnergyThisTick;
    private float _consumedIonsThisTick;
    private float _consumedNeutrinosThisTick;

    internal DynamicShotFabricatorSubsystemInfo()
    {
        _exists = false;
        _minimumRate = 0f;
        _maximumRate = 0f;
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
    /// The minimum configurable shot fabrication rate.
    /// </summary>
    public float MinimumRate
    {
        get { return _minimumRate; }
    }

    /// <summary>
    /// The maximum configurable shot fabrication rate.
    /// </summary>
    public float MaximumRate
    {
        get { return _maximumRate; }
    }

    /// <summary>
    /// Whether the fabricator was active during the reported tick.
    /// This is separate from <see cref="Rate" /> because a non-zero configured rate can still be inactive.
    /// </summary>
    public bool Active
    {
        get { return _active; }
    }

    /// <summary>
    /// Configured shot fabrication rate.
    /// </summary>
    public float Rate
    {
        get { return _rate; }
    }

    /// <summary>
    /// Tick-local runtime status reported for the shot fabricator subsystem.
    /// </summary>
    public SubsystemStatus Status
    {
        get { return _status; }
    }

    /// <summary>
    /// Energy consumed by fabrication during the reported tick.
    /// </summary>
    public float ConsumedEnergyThisTick
    {
        get { return _consumedEnergyThisTick; }
    }

    /// <summary>
    /// Ions consumed by fabrication during the reported tick.
    /// </summary>
    public float ConsumedIonsThisTick
    {
        get { return _consumedIonsThisTick; }
    }

    /// <summary>
    /// Neutrinos consumed by fabrication during the reported tick.
    /// </summary>
    public float ConsumedNeutrinosThisTick
    {
        get { return _consumedNeutrinosThisTick; }
    }

    internal void Update(bool exists, float minimumRate, float maximumRate, bool active, float rate, SubsystemStatus status,
        float consumedEnergyThisTick, float consumedIonsThisTick, float consumedNeutrinosThisTick)
    {
        _exists = exists;
        _minimumRate = exists ? minimumRate : 0f;
        _maximumRate = exists ? maximumRate : 0f;
        _active = exists && active;
        _rate = exists ? rate : 0f;
        _status = exists ? status : SubsystemStatus.Off;
        _consumedEnergyThisTick = exists ? consumedEnergyThisTick : 0f;
        _consumedIonsThisTick = exists ? consumedIonsThisTick : 0f;
        _consumedNeutrinosThisTick = exists ? consumedNeutrinosThisTick : 0f;
    }
}
