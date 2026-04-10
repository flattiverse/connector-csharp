using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Base type for persistent controllable subsystems.
/// </summary>
public abstract class Subsystem
{
    private readonly Controllable _controllable;
    private readonly string _name;
    private bool _exists;
    private readonly SubsystemSlot _slot;
    private byte _tier;
    private SubsystemStatus _status;
    private bool _hasLastEmittedStatus;
    private SubsystemStatus _lastEmittedStatus;

    internal Subsystem(Controllable controllable, string name, bool exists, SubsystemSlot slot)
    {
        _controllable = controllable;
        _name = name;
        _exists = exists;
        _slot = slot;
        _tier = exists ? (byte)1 : (byte)0;
        _status = SubsystemStatus.Off;
        _hasLastEmittedStatus = false;
        _lastEmittedStatus = SubsystemStatus.Off;
    }

    /// <summary>
    /// The controllable this subsystem belongs to.
    /// </summary>
    protected Controllable Controllable
    {
        get { return _controllable; }
    }

    protected bool ModernShip
    {
        get { return _controllable is ModernShipControllable; }
    }

    /// <summary>
    /// A human readable subsystem name.
    /// </summary>
    public string Name
    {
        get { return _name; }
    }

    /// <summary>
    /// True if this controllable actually has the subsystem installed in this slot.
    /// Missing subsystems keep reporting default values and cannot be commanded.
    /// </summary>
    public bool Exists
    {
        get { return _exists; }
    }

    /// <summary>
    /// The concrete slot this subsystem occupies.
    /// </summary>
    public SubsystemSlot Slot
    {
        get { return _slot; }
    }

    /// <summary>
    /// Logical subsystem family independent of the concrete slot.
    /// </summary>
    public SubsystemKind Kind
    {
        get { return SubsystemTierCatalogs.GetKind(_slot); }
    }

    /// <summary>
    /// Current installed tier reported by the server.
    /// Tier 0 means that this slot is currently not installed.
    /// </summary>
    public byte Tier
    {
        get { return _tier; }
    }

    /// <summary>
    /// Current target tier while a tier change is in progress.
    /// Equals <see cref="Tier"/> when no tier change is pending for this slot.
    /// </summary>
    public byte TargetTier
    {
        get { return _controllable.GetTierChangeTargetTier(_slot, _tier); }
    }

    /// <summary>
    /// Remaining ticks of the currently running upgrade or downgrade affecting this slot.
    /// Returns 0 when no tier change is pending.
    /// </summary>
    public ushort RemainingTierChangeTicks
    {
        get { return _controllable.GetRemainingTierChangeTicks(_slot); }
    }

    /// <summary>
    /// Full static tier catalog for this subsystem family on the current ship type.
    /// </summary>
    public virtual IReadOnlyList<SubsystemTierInfo> TierInfos
    {
        get { return StaticTierInfos; }
    }

    /// <summary>
    /// Metadata of the currently installed tier.
    /// </summary>
    public SubsystemTierInfo TierInfo
    {
        get { return TierInfos[_tier]; }
    }

    /// <summary>
    /// Metadata of the currently targeted tier during a running tier change.
    /// </summary>
    public SubsystemTierInfo TargetTierInfo
    {
        get { return TierInfos[TargetTier]; }
    }

    private protected IReadOnlyList<SubsystemTierInfo> StaticTierInfos
    {
        get { return SubsystemTierCatalogs.GetTierInfos(_slot, ModernShip); }
    }

    internal float CurrentStructuralLoad
    {
        get { return StaticTierInfos[_tier].StructuralLoad; }
    }

    /// <summary>
    /// The latest runtime status reported by the server.
    /// </summary>
    public SubsystemStatus Status
    {
        get { return _status; }
    }

    /// <summary>
    /// Starts one upgrade step for this subsystem slot.
    /// This also works for currently missing subsystems at tier 0.
    /// </summary>
    public async Task Upgrade()
    {
        if (!Controllable.Active)
            throw new SpecifiedElementNotFoundGameException();

        await Controllable.Cluster.Galaxy.Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0xAE;
            writer.Write(Controllable.Id);
            writer.Write((byte)_slot);
        });
    }

    /// <summary>
    /// Starts one downgrade step for this subsystem slot.
    /// </summary>
    public async Task Downgrade()
    {
        if (!Controllable.Active)
            throw new SpecifiedElementNotFoundGameException();

        await Controllable.Cluster.Galaxy.Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0xAF;
            writer.Write(Controllable.Id);
            writer.Write((byte)_slot);
        });
    }

    internal void ResetRuntimeStatus()
    {
        _status = SubsystemStatus.Off;
        _hasLastEmittedStatus = false;
        _lastEmittedStatus = SubsystemStatus.Off;
    }

    internal void SetExists(bool exists)
    {
        _exists = exists;

        RefreshTier();

        if (!_exists)
            ResetRuntimeStatus();
    }

    protected void SetTier(byte tier)
    {
        _tier = tier;
    }

    internal void SetReportedTier(byte tier)
    {
        SetTier(tier);
    }

    private protected void CopyBaseFrom(Subsystem other)
    {
        _exists = other._exists;
        _tier = other._tier;
        _status = other._status;
        _hasLastEmittedStatus = other._hasLastEmittedStatus;
        _lastEmittedStatus = other._lastEmittedStatus;
    }

    protected static bool Matches(float left, float right)
    {
        return MathF.Abs(left - right) <= 0.0001f;
    }

    protected abstract void RefreshTier();

    internal void UpdateRuntimeStatus(SubsystemStatus status)
    {
        _status = _exists ? status : SubsystemStatus.Off;
    }

    /// <summary>
    /// Returns true when a runtime event should be emitted for status transitions or active ticks.
    /// </summary>
    protected bool ShouldEmitRuntimeEvent()
    {
        if (_status == SubsystemStatus.Worked || _status == SubsystemStatus.Failed)
        {
            _lastEmittedStatus = _status;
            _hasLastEmittedStatus = true;
            return true;
        }

        if (!_hasLastEmittedStatus || _lastEmittedStatus != _status)
        {
            _lastEmittedStatus = _status;
            _hasLastEmittedStatus = true;
            return true;
        }

        return false;
    }
}

