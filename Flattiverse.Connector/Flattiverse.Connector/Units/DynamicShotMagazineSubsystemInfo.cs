using Flattiverse.Connector.Network;

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

    internal DynamicShotMagazineSubsystemInfo(DynamicShotMagazineSubsystemInfo other)
    {
        _exists = other._exists;
        _maximumShots = other._maximumShots;
        _currentShots = other._currentShots;
        _status = other._status;
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

    internal bool Update(PacketReader reader)
    {
        if (!reader.Read(out byte exists))
            return false;

        _exists = exists != 0;

        if (!_exists)
        {
            _maximumShots = 0f;
            _currentShots = 0f;
            _status = SubsystemStatus.Off;
            return true;
        }

        if (!reader.Read(out _maximumShots) ||
            !reader.Read(out _currentShots) ||
            !reader.Read(out byte status))
            return false;

        _status = (SubsystemStatus)status;
        return true;
    }
}
