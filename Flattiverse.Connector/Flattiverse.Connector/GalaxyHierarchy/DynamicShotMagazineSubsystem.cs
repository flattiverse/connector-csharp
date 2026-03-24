using Flattiverse.Connector.Events;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Dynamic shot magazine subsystem of a controllable.
/// </summary>
public class DynamicShotMagazineSubsystem : Subsystem
{
    private const float MaximumShotsValue = 5f;

    private float _currentShots;

    internal DynamicShotMagazineSubsystem(Controllable controllable, string name, bool exists, SubsystemSlot slot) :
        base(controllable, name, exists, slot)
    {
        _currentShots = 0f;
    }

    /// <summary>
    /// The magazine capacity in shots.
    /// </summary>
    public float MaximumShots
    {
        get { return MaximumShotsValue; }
    }

    /// <summary>
    /// The currently stored shots.
    /// </summary>
    public float CurrentShots
    {
        get { return _currentShots; }
    }

    internal void ResetRuntime()
    {
        _currentShots = 0f;
        ResetRuntimeStatus();
    }

    internal void UpdateRuntime(float currentShots, SubsystemStatus status)
    {
        _currentShots = currentShots;
        UpdateRuntimeStatus(status);
    }

    internal FlattiverseEvent? CreateRuntimeEvent()
    {
        if (!Exists || !ShouldEmitRuntimeEvent())
            return null;

        return new DynamicShotMagazineSubsystemEvent(Controllable, Slot, Status, _currentShots);
    }
}
