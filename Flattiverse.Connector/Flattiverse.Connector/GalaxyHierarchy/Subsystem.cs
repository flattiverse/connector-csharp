namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Base type for persistent controllable subsystems.
/// </summary>
public abstract class Subsystem
{
    private readonly Controllable _controllable;
    private readonly string _name;
    private readonly bool _exists;
    private readonly SubsystemSlot _slot;
    private SubsystemStatus _status;
    private bool _hasLastEmittedStatus;
    private SubsystemStatus _lastEmittedStatus;

    internal Subsystem(Controllable controllable, string name, bool exists, SubsystemSlot slot)
    {
        _controllable = controllable;
        _name = name;
        _exists = exists;
        _slot = slot;
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

    /// <summary>
    /// A human readable subsystem name.
    /// </summary>
    public string Name
    {
        get { return _name; }
    }

    /// <summary>
    /// true if this controllable actually provides the subsystem.
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
    /// The latest runtime status reported by the server.
    /// </summary>
    public SubsystemStatus Status
    {
        get { return _status; }
    }

    internal void ResetRuntimeStatus()
    {
        _status = SubsystemStatus.Off;
        _hasLastEmittedStatus = false;
        _lastEmittedStatus = SubsystemStatus.Off;
    }

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
