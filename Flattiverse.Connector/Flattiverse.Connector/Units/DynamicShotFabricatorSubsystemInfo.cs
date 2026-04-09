using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// Visible snapshot of a dynamic shot fabricator subsystem on a scanned player unit.
/// </summary>
public class DynamicShotFabricatorSubsystemInfo
{
    private const float MinimumRateValue = 0f;

    private bool _exists;
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
        _maximumRate = 0f;
        _active = false;
        _rate = 0f;
        _status = SubsystemStatus.Off;
        _consumedEnergyThisTick = 0f;
        _consumedIonsThisTick = 0f;
        _consumedNeutrinosThisTick = 0f;
    }

    internal DynamicShotFabricatorSubsystemInfo(DynamicShotFabricatorSubsystemInfo other)
    {
        _exists = other._exists;
        _maximumRate = other._maximumRate;
        _active = other._active;
        _rate = other._rate;
        _status = other._status;
        _consumedEnergyThisTick = other._consumedEnergyThisTick;
        _consumedIonsThisTick = other._consumedIonsThisTick;
        _consumedNeutrinosThisTick = other._consumedNeutrinosThisTick;
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
        get { return MinimumRateValue; }
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

    internal bool Update(PacketReader reader)
    {
        if (!reader.Read(out byte exists))
            return false;

        _exists = exists != 0;

        if (!_exists)
        {
            _maximumRate = 0f;
            _active = false;
            _rate = 0f;
            _status = SubsystemStatus.Off;
            _consumedEnergyThisTick = 0f;
            _consumedIonsThisTick = 0f;
            _consumedNeutrinosThisTick = 0f;
            return true;
        }

        if (!reader.Read(out _maximumRate) ||
            !reader.Read(out byte active) ||
            !reader.Read(out _rate) ||
            !reader.Read(out byte status) ||
            !reader.Read(out _consumedEnergyThisTick) ||
            !reader.Read(out _consumedIonsThisTick) ||
            !reader.Read(out _consumedNeutrinosThisTick))
            return false;

        _active = active != 0;
        _status = (SubsystemStatus)status;
        return true;
    }
}
