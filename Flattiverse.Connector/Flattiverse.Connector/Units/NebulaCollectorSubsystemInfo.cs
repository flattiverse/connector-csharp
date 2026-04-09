using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// Visible snapshot of a nebula collector subsystem on a scanned player unit.
/// </summary>
public class NebulaCollectorSubsystemInfo
{
    private bool _exists;
    private float _minimumRate;
    private float _maximumRate;
    private float _rate;
    private SubsystemStatus _status;
    private float _consumedEnergyThisTick;
    private float _consumedIonsThisTick;
    private float _consumedNeutrinosThisTick;
    private float _collectedThisTick;
    private float _collectedHueThisTick;

    internal NebulaCollectorSubsystemInfo()
    {
        _exists = false;
        _minimumRate = 0f;
        _maximumRate = 0f;
        _rate = 0f;
        _status = SubsystemStatus.Off;
        _consumedEnergyThisTick = 0f;
        _consumedIonsThisTick = 0f;
        _consumedNeutrinosThisTick = 0f;
        _collectedThisTick = 0f;
        _collectedHueThisTick = 0f;
    }

    internal NebulaCollectorSubsystemInfo(NebulaCollectorSubsystemInfo other)
    {
        _exists = other._exists;
        _minimumRate = other._minimumRate;
        _maximumRate = other._maximumRate;
        _rate = other._rate;
        _status = other._status;
        _consumedEnergyThisTick = other._consumedEnergyThisTick;
        _consumedIonsThisTick = other._consumedIonsThisTick;
        _consumedNeutrinosThisTick = other._consumedNeutrinosThisTick;
        _collectedThisTick = other._collectedThisTick;
        _collectedHueThisTick = other._collectedHueThisTick;
    }

    /// <summary>
    /// Indicates whether the scanned unit actually has a nebula collector installed.
    /// </summary>
    public bool Exists
    {
        get { return _exists; }
    }

    /// <summary>
    /// Minimum configurable collection rate for the scanned unit.
    /// </summary>
    public float MinimumRate
    {
        get { return _minimumRate; }
    }

    /// <summary>
    /// Maximum configurable collection rate for the scanned unit.
    /// </summary>
    public float MaximumRate
    {
        get { return _maximumRate; }
    }

    /// <summary>
    /// Collector rate mirrored for the reported tick.
    /// </summary>
    public float Rate
    {
        get { return _rate; }
    }

    /// <summary>
    /// Tick-local runtime status reported for the collector.
    /// The collector can fail or switch off when movement, environment, or resource conditions do not allow
    /// collection.
    /// </summary>
    public SubsystemStatus Status
    {
        get { return _status; }
    }

    /// <summary>
    /// Energy consumed by the collector during the reported tick.
    /// </summary>
    public float ConsumedEnergyThisTick
    {
        get { return _consumedEnergyThisTick; }
    }

    /// <summary>
    /// Ions consumed by the collector during the reported tick.
    /// </summary>
    public float ConsumedIonsThisTick
    {
        get { return _consumedIonsThisTick; }
    }

    /// <summary>
    /// Neutrinos consumed by the collector during the reported tick.
    /// </summary>
    public float ConsumedNeutrinosThisTick
    {
        get { return _consumedNeutrinosThisTick; }
    }

    /// <summary>
    /// Nebula amount collected during the reported tick.
    /// </summary>
    public float CollectedThisTick
    {
        get { return _collectedThisTick; }
    }

    /// <summary>
    /// Hue of the nebula sample collected during the reported tick.
    /// This describes the fresh intake and can differ from the averaged cargo hue stored in
    /// <see cref="CargoSubsystemInfo.NebulaHue" />.
    /// </summary>
    public float CollectedHueThisTick
    {
        get { return _collectedHueThisTick; }
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
            _collectedThisTick = 0f;
            _collectedHueThisTick = 0f;
            return true;
        }

        if (!reader.Read(out _minimumRate) ||
            !reader.Read(out _maximumRate) ||
            !reader.Read(out _rate) ||
            !reader.Read(out byte status) ||
            !reader.Read(out _consumedEnergyThisTick) ||
            !reader.Read(out _consumedIonsThisTick) ||
            !reader.Read(out _consumedNeutrinosThisTick) ||
            !reader.Read(out _collectedThisTick) ||
            !reader.Read(out _collectedHueThisTick))
            return false;

        _status = (SubsystemStatus)status;
        return true;
    }
}
