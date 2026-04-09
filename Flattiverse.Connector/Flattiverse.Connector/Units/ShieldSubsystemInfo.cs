using Flattiverse.Connector.Network;

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

    internal ShieldSubsystemInfo(ShieldSubsystemInfo other)
    {
        _exists = other._exists;
        _maximum = other._maximum;
        _current = other._current;
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
    /// Whether shield loading was active for the reported tick.
    /// A shield can exist while being inactive, for example when its configured rate is zero.
    /// </summary>
    public bool Active
    {
        get { return _active; }
    }

    /// <summary>
    /// Configured shield loading rate.
    /// Higher rates charge faster but also increase the quadratic tick cost.
    /// </summary>
    public float Rate
    {
        get { return _rate; }
    }

    /// <summary>
    /// Tick-local runtime status reported for the shield subsystem.
    /// </summary>
    public SubsystemStatus Status
    {
        get { return _status; }
    }

    /// <summary>
    /// Energy consumed by shield loading during the reported tick.
    /// This is usually zero if the shield was inactive or already full.
    /// </summary>
    public float ConsumedEnergyThisTick
    {
        get { return _consumedEnergyThisTick; }
    }

    /// <summary>
    /// Ions consumed by shield loading during the reported tick.
    /// </summary>
    public float ConsumedIonsThisTick
    {
        get { return _consumedIonsThisTick; }
    }

    /// <summary>
    /// Neutrinos consumed by shield loading during the reported tick.
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
            _maximum = 0f;
            _current = 0f;
            _active = false;
            _rate = 0f;
            _status = SubsystemStatus.Off;
            _consumedEnergyThisTick = 0f;
            _consumedIonsThisTick = 0f;
            _consumedNeutrinosThisTick = 0f;
            return true;
        }

        if (!reader.Read(out _maximum) ||
            !reader.Read(out _current) ||
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
