using Flattiverse.Connector.Events;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Hull integrity subsystem of a controllable.
/// </summary>
public class HullSubsystem : Subsystem
{
    private float _maximum;
    private float _current;

    internal HullSubsystem(Controllable controllable, string name, bool exists, float maximum, SubsystemSlot slot) :
        base(controllable, name, exists, slot)
    {
        _maximum = 0f;
        _current = 0f;

        SetMaximum(maximum);
    }

    internal static HullSubsystem CreateClassicShipHull(Controllable controllable)
    {
        return new HullSubsystem(controllable, "Hull", true, 50f, SubsystemSlot.Hull);
    }

    /// <summary>
    /// The maximum hull integrity.
    /// </summary>
    public float Maximum
    {
        get { return _maximum; }
    }

    /// <summary>
    /// The current hull integrity.
    /// </summary>
    public float Current
    {
        get { return _current; }
    }

    internal void SetMaximum(float maximum)
    {
        _maximum = Exists ? maximum : 0f;

        if (_current > _maximum)
            _current = _maximum;
    }

    internal void ResetRuntime()
    {
        _current = 0f;
        ResetRuntimeStatus();
    }

    internal void UpdateRuntime(float current, SubsystemStatus status)
    {
        _current = current;
        UpdateRuntimeStatus(status);
    }

    internal FlattiverseEvent? CreateRuntimeEvent()
    {
        if (!Exists || !ShouldEmitRuntimeEvent())
            return null;

        return new HullSubsystemEvent(Controllable, Slot, Status, _current);
    }
}
