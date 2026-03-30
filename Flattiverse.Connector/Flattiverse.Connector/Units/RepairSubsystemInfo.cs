namespace Flattiverse.Connector.Units;

/// <summary>
/// Visible snapshot of a repair subsystem on a scanned player unit.
/// </summary>
public class RepairSubsystemInfo
{
    private bool _exists;
    private float _minimumRate;
    private float _maximumRate;
    private float _rate;
    private SubsystemStatus _status;
    private float _consumedEnergyThisTick;
    private float _consumedIonsThisTick;
    private float _consumedNeutrinosThisTick;
    private float _repairedHullThisTick;

    internal RepairSubsystemInfo()
    {
        _exists = false;
        _minimumRate = 0f;
        _maximumRate = 0f;
        _rate = 0f;
        _status = SubsystemStatus.Off;
        _consumedEnergyThisTick = 0f;
        _consumedIonsThisTick = 0f;
        _consumedNeutrinosThisTick = 0f;
        _repairedHullThisTick = 0f;
    }

    /// <summary>
    /// Indicates whether the subsystem exists on the scanned unit.
    /// </summary>
    public bool Exists
    {
        get { return _exists; }
    }

    /// <summary>
    /// Minimum configurable repair rate.
    /// </summary>
    public float MinimumRate
    {
        get { return _minimumRate; }
    }

    /// <summary>
    /// Maximum configurable repair rate.
    /// </summary>
    public float MaximumRate
    {
        get { return _maximumRate; }
    }

    /// <summary>
    /// Configured hull-repair rate for the reported tick.
    /// A rate of <c>0</c> means the repair subsystem is effectively off.
    /// </summary>
    public float Rate
    {
        get { return _rate; }
    }

    /// <summary>
    /// Tick-local runtime status reported for the repair subsystem.
    /// The repair subsystem only restores hull and can fail while the unit is moving too fast.
    /// </summary>
    public SubsystemStatus Status
    {
        get { return _status; }
    }

    /// <summary>
    /// Energy consumed by the repair subsystem during the reported tick.
    /// The current server model uses a quadratic cost curve based on the configured rate.
    /// </summary>
    public float ConsumedEnergyThisTick
    {
        get { return _consumedEnergyThisTick; }
    }

    /// <summary>
    /// Ions consumed by the repair subsystem during the reported tick.
    /// </summary>
    public float ConsumedIonsThisTick
    {
        get { return _consumedIonsThisTick; }
    }

    /// <summary>
    /// Neutrinos consumed by the repair subsystem during the reported tick.
    /// </summary>
    public float ConsumedNeutrinosThisTick
    {
        get { return _consumedNeutrinosThisTick; }
    }

    /// <summary>
    /// Hull integrity restored during the reported tick.
    /// </summary>
    public float RepairedHullThisTick
    {
        get { return _repairedHullThisTick; }
    }

    internal void Update(bool exists, float minimumRate, float maximumRate, float rate, SubsystemStatus status, float consumedEnergyThisTick,
        float consumedIonsThisTick, float consumedNeutrinosThisTick, float repairedHullThisTick)
    {
        _exists = exists;
        _minimumRate = exists ? minimumRate : 0f;
        _maximumRate = exists ? maximumRate : 0f;
        _rate = exists ? rate : 0f;
        _status = exists ? status : SubsystemStatus.Off;
        _consumedEnergyThisTick = exists ? consumedEnergyThisTick : 0f;
        _consumedIonsThisTick = exists ? consumedIonsThisTick : 0f;
        _consumedNeutrinosThisTick = exists ? consumedNeutrinosThisTick : 0f;
        _repairedHullThisTick = exists ? repairedHullThisTick : 0f;
    }
}
