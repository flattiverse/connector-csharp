using Flattiverse.Connector.Network;

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

    internal RepairSubsystemInfo(RepairSubsystemInfo other)
    {
        _exists = other._exists;
        _minimumRate = other._minimumRate;
        _maximumRate = other._maximumRate;
        _rate = other._rate;
        _status = other._status;
        _consumedEnergyThisTick = other._consumedEnergyThisTick;
        _consumedIonsThisTick = other._consumedIonsThisTick;
        _consumedNeutrinosThisTick = other._consumedNeutrinosThisTick;
        _repairedHullThisTick = other._repairedHullThisTick;
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

    internal bool Update(PacketReader reader)
    {
        if (!reader.Read(out byte exists))
            return false;

        _exists = exists != 0;

        if (!_exists)
        {
            _minimumRate = 0f;
            _maximumRate = 0f;
            _rate = 0f;
            _status = SubsystemStatus.Off;
            _consumedEnergyThisTick = 0f;
            _consumedIonsThisTick = 0f;
            _consumedNeutrinosThisTick = 0f;
            _repairedHullThisTick = 0f;
            return true;
        }

        if (!reader.Read(out _minimumRate) ||
            !reader.Read(out _maximumRate) ||
            !reader.Read(out _rate) ||
            !reader.Read(out byte status) ||
            !reader.Read(out _consumedEnergyThisTick) ||
            !reader.Read(out _consumedIonsThisTick) ||
            !reader.Read(out _consumedNeutrinosThisTick) ||
            !reader.Read(out _repairedHullThisTick))
            return false;

        _status = (SubsystemStatus)status;
        return true;
    }
}
