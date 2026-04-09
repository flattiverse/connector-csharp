using Flattiverse.Connector.Network;

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

    internal ArmorSubsystemInfo(ArmorSubsystemInfo other)
    {
        _exists = other._exists;
        _reduction = other._reduction;
        _status = other._status;
        _blockedDirectDamageThisTick = other._blockedDirectDamageThisTick;
        _blockedRadiationDamageThisTick = other._blockedRadiationDamageThisTick;
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

    internal bool Update(PacketReader reader)
    {
        if (!reader.Read(out byte exists))
            return false;

        _exists = exists != 0;

        if (!_exists)
        {
            _reduction = 0f;
            _status = SubsystemStatus.Off;
            _blockedDirectDamageThisTick = 0f;
            _blockedRadiationDamageThisTick = 0f;
            return true;
        }

        if (!reader.Read(out _reduction) ||
            !reader.Read(out byte status) ||
            !reader.Read(out _blockedDirectDamageThisTick) ||
            !reader.Read(out _blockedRadiationDamageThisTick))
            return false;

        _status = (SubsystemStatus)status;
        return true;
    }
}
