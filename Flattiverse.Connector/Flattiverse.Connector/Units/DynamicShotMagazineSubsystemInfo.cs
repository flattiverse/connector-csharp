namespace Flattiverse.Connector.Units;

/// <summary>
/// Visible snapshot of a dynamic shot magazine subsystem on a scanned player unit.
/// </summary>
public class DynamicShotMagazineSubsystemInfo
{
    private bool _exists;
    private float _maximumShots;
    private float _currentShots;
    private SubsystemStatus _status;

    internal DynamicShotMagazineSubsystemInfo()
    {
        _exists = false;
        _maximumShots = 0f;
        _currentShots = 0f;
        _status = SubsystemStatus.Off;
    }

    /// <summary>
    /// Indicates whether the subsystem exists on the scanned unit.
    /// </summary>
    public bool Exists
    {
        get { return _exists; }
    }

    /// <summary>
    /// The magazine capacity in shots.
    /// </summary>
    public float MaximumShots
    {
        get { return _maximumShots; }
    }

    /// <summary>
    /// Currently available ammunition measured in shots.
    /// </summary>
    public float CurrentShots
    {
        get { return _currentShots; }
    }

    /// <summary>
    /// Tick-local runtime status reported for the shot magazine subsystem.
    /// </summary>
    public SubsystemStatus Status
    {
        get { return _status; }
    }

    internal void Update(bool exists, float maximumShots, float currentShots, SubsystemStatus status)
    {
        _exists = exists;
        _maximumShots = exists ? maximumShots : 0f;
        _currentShots = exists ? currentShots : 0f;
        _status = exists ? status : SubsystemStatus.Off;
    }
}
