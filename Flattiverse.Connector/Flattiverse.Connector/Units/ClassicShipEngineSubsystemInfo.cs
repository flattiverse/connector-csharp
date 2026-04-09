using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// Visible snapshot of a classic-ship engine subsystem on a scanned player unit.
/// </summary>
public class ClassicShipEngineSubsystemInfo
{
    private bool _exists;
    private float _maximum;
    private Vector _current;
    private Vector _target;
    private SubsystemStatus _status;
    private float _consumedEnergyThisTick;
    private float _consumedIonsThisTick;
    private float _consumedNeutrinosThisTick;

    internal ClassicShipEngineSubsystemInfo()
    {
        _exists = false;
        _maximum = 0f;
        _current = new Vector();
        _target = new Vector();
        _status = SubsystemStatus.Off;
        _consumedEnergyThisTick = 0f;
        _consumedIonsThisTick = 0f;
        _consumedNeutrinosThisTick = 0f;
    }

    internal ClassicShipEngineSubsystemInfo(ClassicShipEngineSubsystemInfo other)
    {
        _exists = other._exists;
        _maximum = other._maximum;
        _current = new Vector(other._current);
        _target = new Vector(other._target);
        _status = other._status;
        _consumedEnergyThisTick = other._consumedEnergyThisTick;
        _consumedIonsThisTick = other._consumedIonsThisTick;
        _consumedNeutrinosThisTick = other._consumedNeutrinosThisTick;
    }

    /// <summary>
    /// Indicates whether the scanned unit actually has this engine subsystem installed.
    /// </summary>
    public bool Exists
    {
        get { return _exists; }
    }

    /// <summary>
    /// Maximum configurable impulse length of the engine command.
    /// </summary>
    public float Maximum
    {
        get { return _maximum; }
    }

    /// <summary>
    /// Current engine impulse applied by the server.
    /// This is the thrust vector, not the ship's world-space movement vector.
    /// </summary>
    public Vector Current
    {
        get { return new Vector(_current); }
    }

    /// <summary>
    /// Target engine impulse currently configured on the server.
    /// </summary>
    public Vector Target
    {
        get { return new Vector(_target); }
    }

    /// <summary>
    /// Tick-local runtime status reported for the engine subsystem.
    /// </summary>
    public SubsystemStatus Status
    {
        get { return _status; }
    }

    /// <summary>
    /// The energy consumed during the current server tick.
    /// </summary>
    public float ConsumedEnergyThisTick
    {
        get { return _consumedEnergyThisTick; }
    }

    /// <summary>
    /// The ions consumed during the current server tick.
    /// </summary>
    public float ConsumedIonsThisTick
    {
        get { return _consumedIonsThisTick; }
    }

    /// <summary>
    /// The neutrinos consumed during the current server tick.
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
            _current = new Vector();
            _target = new Vector();
            _status = SubsystemStatus.Off;
            _consumedEnergyThisTick = 0f;
            _consumedIonsThisTick = 0f;
            _consumedNeutrinosThisTick = 0f;
            return true;
        }

        if (!reader.Read(out _maximum) ||
            !Vector.FromReader(reader, out _current) ||
            !Vector.FromReader(reader, out _target) ||
            !reader.Read(out byte status) ||
            !reader.Read(out _consumedEnergyThisTick) ||
            !reader.Read(out _consumedIonsThisTick) ||
            !reader.Read(out _consumedNeutrinosThisTick))
            return false;

        _status = (SubsystemStatus)status;
        return true;
    }
}
