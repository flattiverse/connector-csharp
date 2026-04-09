using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// Visible snapshot of a resource miner subsystem on a scanned player unit.
/// </summary>
public class ResourceMinerSubsystemInfo
{
    private bool _exists;
    private float _minimumRate;
    private float _maximumRate;
    private float _rate;
    private SubsystemStatus _status;
    private float _consumedEnergyThisTick;
    private float _consumedIonsThisTick;
    private float _consumedNeutrinosThisTick;
    private float _minedMetalThisTick;
    private float _minedCarbonThisTick;
    private float _minedHydrogenThisTick;
    private float _minedSiliconThisTick;

    internal ResourceMinerSubsystemInfo()
    {
        _exists = false;
        _minimumRate = 0f;
        _maximumRate = 0f;
        _rate = 0f;
        _status = SubsystemStatus.Off;
        _consumedEnergyThisTick = 0f;
        _consumedIonsThisTick = 0f;
        _consumedNeutrinosThisTick = 0f;
        _minedMetalThisTick = 0f;
        _minedCarbonThisTick = 0f;
        _minedHydrogenThisTick = 0f;
        _minedSiliconThisTick = 0f;
    }

    internal ResourceMinerSubsystemInfo(ResourceMinerSubsystemInfo other)
    {
        _exists = other._exists;
        _minimumRate = other._minimumRate;
        _maximumRate = other._maximumRate;
        _rate = other._rate;
        _status = other._status;
        _consumedEnergyThisTick = other._consumedEnergyThisTick;
        _consumedIonsThisTick = other._consumedIonsThisTick;
        _consumedNeutrinosThisTick = other._consumedNeutrinosThisTick;
        _minedMetalThisTick = other._minedMetalThisTick;
        _minedCarbonThisTick = other._minedCarbonThisTick;
        _minedHydrogenThisTick = other._minedHydrogenThisTick;
        _minedSiliconThisTick = other._minedSiliconThisTick;
    }

    /// <summary>
    /// Indicates whether the subsystem exists on the scanned unit.
    /// </summary>
    public bool Exists
    {
        get { return _exists; }
    }

    /// <summary>
    /// Minimum configurable mining rate.
    /// </summary>
    public float MinimumRate
    {
        get { return _minimumRate; }
    }

    /// <summary>
    /// Maximum configurable mining rate.
    /// </summary>
    public float MaximumRate
    {
        get { return _maximumRate; }
    }

    /// <summary>
    /// Configured mining rate for the reported tick.
    /// </summary>
    public float Rate
    {
        get { return _rate; }
    }

    /// <summary>
    /// Tick-local runtime status reported for the resource miner subsystem.
    /// The miner can fail, for example when the ship moves too fast or when no valid body is in range.
    /// </summary>
    public SubsystemStatus Status
    {
        get { return _status; }
    }

    /// <summary>
    /// Energy consumed by mining during the reported tick.
    /// </summary>
    public float ConsumedEnergyThisTick
    {
        get { return _consumedEnergyThisTick; }
    }

    /// <summary>
    /// Ions consumed by mining during the reported tick.
    /// </summary>
    public float ConsumedIonsThisTick
    {
        get { return _consumedIonsThisTick; }
    }

    /// <summary>
    /// Neutrinos consumed by mining during the reported tick.
    /// </summary>
    public float ConsumedNeutrinosThisTick
    {
        get { return _consumedNeutrinosThisTick; }
    }

    /// <summary>
    /// Metal mined during the reported tick.
    /// </summary>
    public float MinedMetalThisTick
    {
        get { return _minedMetalThisTick; }
    }

    /// <summary>
    /// Carbon mined during the reported tick.
    /// </summary>
    public float MinedCarbonThisTick
    {
        get { return _minedCarbonThisTick; }
    }

    /// <summary>
    /// Hydrogen mined during the reported tick.
    /// </summary>
    public float MinedHydrogenThisTick
    {
        get { return _minedHydrogenThisTick; }
    }

    /// <summary>
    /// Silicon mined during the reported tick.
    /// </summary>
    public float MinedSiliconThisTick
    {
        get { return _minedSiliconThisTick; }
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
            _minedMetalThisTick = 0f;
            _minedCarbonThisTick = 0f;
            _minedHydrogenThisTick = 0f;
            _minedSiliconThisTick = 0f;
            return true;
        }

        if (!reader.Read(out _minimumRate) ||
            !reader.Read(out _maximumRate) ||
            !reader.Read(out _rate) ||
            !reader.Read(out byte status) ||
            !reader.Read(out _consumedEnergyThisTick) ||
            !reader.Read(out _consumedIonsThisTick) ||
            !reader.Read(out _consumedNeutrinosThisTick) ||
            !reader.Read(out _minedMetalThisTick) ||
            !reader.Read(out _minedCarbonThisTick) ||
            !reader.Read(out _minedHydrogenThisTick) ||
            !reader.Read(out _minedSiliconThisTick))
            return false;

        _status = (SubsystemStatus)status;
        return true;
    }
}
