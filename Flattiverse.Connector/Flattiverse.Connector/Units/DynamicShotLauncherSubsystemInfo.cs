using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// Visible snapshot of a configurable shot launcher on a scanned player unit.
/// The launcher stores the currently configured projectile profile that would be used for the next shot.
/// </summary>
public class DynamicShotLauncherSubsystemInfo
{
    private bool _exists;
    private float _minimumRelativeMovement;
    private float _maximumRelativeMovement;
    private ushort _minimumTicks;
    private ushort _maximumTicks;
    private float _minimumLoad;
    private float _maximumLoad;
    private float _minimumDamage;
    private float _maximumDamage;
    private Vector _relativeMovement;
    private ushort _ticks;
    private float _load;
    private float _damage;
    private SubsystemStatus _status;
    private float _consumedEnergyThisTick;
    private float _consumedIonsThisTick;
    private float _consumedNeutrinosThisTick;

    internal DynamicShotLauncherSubsystemInfo()
    {
        _exists = false;
        _minimumRelativeMovement = 0f;
        _maximumRelativeMovement = 0f;
        _minimumTicks = 0;
        _maximumTicks = 0;
        _minimumLoad = 0f;
        _maximumLoad = 0f;
        _minimumDamage = 0f;
        _maximumDamage = 0f;
        _relativeMovement = new Vector();
        _ticks = 0;
        _load = 0f;
        _damage = 0f;
        _status = SubsystemStatus.Off;
        _consumedEnergyThisTick = 0f;
        _consumedIonsThisTick = 0f;
        _consumedNeutrinosThisTick = 0f;
    }

    internal DynamicShotLauncherSubsystemInfo(DynamicShotLauncherSubsystemInfo other)
    {
        _exists = other._exists;
        _minimumRelativeMovement = other._minimumRelativeMovement;
        _maximumRelativeMovement = other._maximumRelativeMovement;
        _minimumTicks = other._minimumTicks;
        _maximumTicks = other._maximumTicks;
        _minimumLoad = other._minimumLoad;
        _maximumLoad = other._maximumLoad;
        _minimumDamage = other._minimumDamage;
        _maximumDamage = other._maximumDamage;
        _relativeMovement = new Vector(other._relativeMovement);
        _ticks = other._ticks;
        _load = other._load;
        _damage = other._damage;
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
    /// Minimum allowed relative movement for the shot.
    /// </summary>
    public float MinimumRelativeMovement
    {
        get { return _minimumRelativeMovement; }
    }

    /// <summary>
    /// Maximum allowed relative movement for the shot.
    /// </summary>
    public float MaximumRelativeMovement
    {
        get { return _maximumRelativeMovement; }
    }

    /// <summary>
    /// Minimum lifetime in ticks.
    /// </summary>
    public ushort MinimumTicks
    {
        get { return _minimumTicks; }
    }

    /// <summary>
    /// Maximum lifetime in ticks.
    /// </summary>
    public ushort MaximumTicks
    {
        get { return _maximumTicks; }
    }

    /// <summary>
    /// Minimum explosion load.
    /// </summary>
    public float MinimumLoad
    {
        get { return _minimumLoad; }
    }

    /// <summary>
    /// Maximum explosion load.
    /// </summary>
    public float MaximumLoad
    {
        get { return _maximumLoad; }
    }

    /// <summary>
    /// Minimum damage.
    /// </summary>
    public float MinimumDamage
    {
        get { return _minimumDamage; }
    }

    /// <summary>
    /// Maximum damage.
    /// </summary>
    public float MaximumDamage
    {
        get { return _maximumDamage; }
    }

    /// <summary>
    /// Projectile movement relative to the launching unit that is currently configured on the server.
    /// </summary>
    public Vector RelativeMovement
    {
        get { return new Vector(_relativeMovement); }
    }

    /// <summary>
    /// Configured projectile lifetime in ticks.
    /// </summary>
    public ushort Ticks
    {
        get { return _ticks; }
    }

    /// <summary>
    /// Configured explosion load applied when the projectile expires.
    /// </summary>
    public float Load
    {
        get { return _load; }
    }

    /// <summary>
    /// Configured direct damage of the projectile.
    /// </summary>
    public float Damage
    {
        get { return _damage; }
    }

    /// <summary>
    /// Tick-local runtime status reported for the launcher subsystem.
    /// </summary>
    public SubsystemStatus Status
    {
        get { return _status; }
    }

    /// <summary>
    /// Energy consumed by the launcher during the reported tick.
    /// </summary>
    public float ConsumedEnergyThisTick
    {
        get { return _consumedEnergyThisTick; }
    }

    /// <summary>
    /// Ions consumed by the launcher during the reported tick.
    /// </summary>
    public float ConsumedIonsThisTick
    {
        get { return _consumedIonsThisTick; }
    }

    /// <summary>
    /// Neutrinos consumed by the launcher during the reported tick.
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
            _minimumRelativeMovement = 0f;
            _maximumRelativeMovement = 0f;
            _minimumTicks = 0;
            _maximumTicks = 0;
            _minimumLoad = 0f;
            _maximumLoad = 0f;
            _minimumDamage = 0f;
            _maximumDamage = 0f;
            _relativeMovement = new Vector();
            _ticks = 0;
            _load = 0f;
            _damage = 0f;
            _status = SubsystemStatus.Off;
            _consumedEnergyThisTick = 0f;
            _consumedIonsThisTick = 0f;
            _consumedNeutrinosThisTick = 0f;
            return true;
        }

        if (!reader.Read(out _minimumRelativeMovement) ||
            !reader.Read(out _maximumRelativeMovement) ||
            !reader.Read(out _minimumTicks) ||
            !reader.Read(out _maximumTicks) ||
            !reader.Read(out _minimumLoad) ||
            !reader.Read(out _maximumLoad) ||
            !reader.Read(out _minimumDamage) ||
            !reader.Read(out _maximumDamage) ||
            !Vector.FromReader(reader, out _relativeMovement) ||
            !reader.Read(out _ticks) ||
            !reader.Read(out _load) ||
            !reader.Read(out _damage) ||
            !reader.Read(out byte status) ||
            !reader.Read(out _consumedEnergyThisTick) ||
            !reader.Read(out _consumedIonsThisTick) ||
            !reader.Read(out _consumedNeutrinosThisTick))
            return false;

        _status = (SubsystemStatus)status;
        return true;
    }
}
