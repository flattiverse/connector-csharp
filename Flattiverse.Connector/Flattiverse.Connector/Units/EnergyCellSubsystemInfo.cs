using Flattiverse.Connector.Network;

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

    internal EnergyCellSubsystemInfo(EnergyCellSubsystemInfo other)
    {
        _exists = other._exists;
        _efficiency = other._efficiency;
        _collectedThisTick = other._collectedThisTick;
        _status = other._status;
    }

    /// <summary>
    /// Indicates whether the scanned unit actually has this energy-cell subsystem installed.
    /// </summary>
    public bool Exists
    {
        get { return _exists; }
    }

    /// <summary>
    /// Conversion efficiency of the cell for its matching environmental intake source.
    /// </summary>
    public float Efficiency
    {
        get { return _efficiency; }
    }

    /// <summary>
    /// Amount collected during the current server tick.
    /// </summary>
    public float CollectedThisTick
    {
        get { return _collectedThisTick; }
    }

    /// <summary>
    /// Tick-local runtime status reported for this energy cell.
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
            _efficiency = 0f;
            _collectedThisTick = 0f;
            _status = SubsystemStatus.Off;
            return true;
        }

        if (!reader.Read(out _efficiency) ||
            !reader.Read(out _collectedThisTick) ||
            !reader.Read(out byte status))
            return false;

        _status = (SubsystemStatus)status;
        return true;
    }
}
