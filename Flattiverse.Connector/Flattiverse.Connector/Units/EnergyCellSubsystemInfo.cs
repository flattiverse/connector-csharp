namespace Flattiverse.Connector.Units;

/// <summary>
/// Visible snapshot of an energy-cell subsystem on a scanned player unit.
/// </summary>
public class EnergyCellSubsystemInfo
{
    private bool _exists;
    private float _efficiency;
    private float _collectedThisTick;
    private SubsystemStatus _status;

    internal EnergyCellSubsystemInfo()
    {
        _exists = false;
        _efficiency = 0f;
        _collectedThisTick = 0f;
        _status = SubsystemStatus.Off;
    }

    /// <summary>
    /// true if the subsystem exists.
    /// </summary>
    public bool Exists
    {
        get { return _exists; }
    }

    /// <summary>
    /// The loading efficiency.
    /// </summary>
    public float Efficiency
    {
        get { return _efficiency; }
    }

    /// <summary>
    /// The amount collected during the current server tick.
    /// </summary>
    public float CollectedThisTick
    {
        get { return _collectedThisTick; }
    }

    /// <summary>
    /// The runtime status for the current server tick.
    /// </summary>
    public SubsystemStatus Status
    {
        get { return _status; }
    }

    internal void Update(bool exists, float efficiency, float collectedThisTick, SubsystemStatus status)
    {
        _exists = exists;
        _efficiency = exists ? efficiency : 0f;
        _collectedThisTick = exists ? collectedThisTick : 0f;
        _status = exists ? status : SubsystemStatus.Off;
    }
}
