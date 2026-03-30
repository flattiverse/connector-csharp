namespace Flattiverse.Connector.Units;

/// <summary>
/// Visible snapshot of an armor subsystem on a scanned player unit.
/// </summary>
public class ArmorSubsystemInfo
{
    private bool _exists;
    private float _reduction;
    private SubsystemStatus _status;
    private float _blockedDirectDamageThisTick;
    private float _blockedRadiationDamageThisTick;

    internal ArmorSubsystemInfo()
    {
        _exists = false;
        _reduction = 0f;
        _status = SubsystemStatus.Off;
        _blockedDirectDamageThisTick = 0f;
        _blockedRadiationDamageThisTick = 0f;
    }

    /// <summary>
    /// Indicates whether the subsystem exists on the scanned unit.
    /// </summary>
    public bool Exists
    {
        get { return _exists; }
    }

    /// <summary>
    /// Flat damage reduction applied before hull damage is computed.
    /// Armor has no own hit points; it only reduces incoming direct and radiation damage.
    /// </summary>
    public float Reduction
    {
        get { return _reduction; }
    }

    /// <summary>
    /// Tick-local runtime status reported for the armor subsystem.
    /// </summary>
    public SubsystemStatus Status
    {
        get { return _status; }
    }

    /// <summary>
    /// Direct collision or weapon damage blocked during the current server tick.
    /// </summary>
    public float BlockedDirectDamageThisTick
    {
        get { return _blockedDirectDamageThisTick; }
    }

    /// <summary>
    /// Radiation damage blocked during the current server tick.
    /// </summary>
    public float BlockedRadiationDamageThisTick
    {
        get { return _blockedRadiationDamageThisTick; }
    }

    internal void Update(bool exists, float reduction, SubsystemStatus status, float blockedDirectDamageThisTick,
        float blockedRadiationDamageThisTick)
    {
        _exists = exists;
        _reduction = exists ? reduction : 0f;
        _status = exists ? status : SubsystemStatus.Off;
        _blockedDirectDamageThisTick = exists ? blockedDirectDamageThisTick : 0f;
        _blockedRadiationDamageThisTick = exists ? blockedRadiationDamageThisTick : 0f;
    }
}
