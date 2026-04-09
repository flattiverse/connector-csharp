using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// Visible snapshot of a railgun subsystem on a scanned player unit.
/// </summary>
public class ClassicRailgunSubsystemInfo
{
    private bool _exists;
    private float _energyCost;
    private float _metalCost;
    private Flattiverse.Connector.GalaxyHierarchy.RailgunDirection _direction;
    private SubsystemStatus _status;
    private float _consumedEnergyThisTick;
    private float _consumedIonsThisTick;
    private float _consumedNeutrinosThisTick;

    internal ClassicRailgunSubsystemInfo()
    {
        _exists = false;
        _energyCost = 0f;
        _metalCost = 0f;
        _direction = Flattiverse.Connector.GalaxyHierarchy.RailgunDirection.None;
        _status = SubsystemStatus.Off;
        _consumedEnergyThisTick = 0f;
        _consumedIonsThisTick = 0f;
        _consumedNeutrinosThisTick = 0f;
    }

    internal ClassicRailgunSubsystemInfo(ClassicRailgunSubsystemInfo other)
    {
        _exists = other._exists;
        _energyCost = other._energyCost;
        _metalCost = other._metalCost;
        _direction = other._direction;
        _status = other._status;
        _consumedEnergyThisTick = other._consumedEnergyThisTick;
        _consumedIonsThisTick = other._consumedIonsThisTick;
        _consumedNeutrinosThisTick = other._consumedNeutrinosThisTick;
    }

    /// <summary>
    /// Indicates whether the subsystem exists on the scanned unit.
    /// </summary>
    public bool Exists
    {
        get { return _exists; }
    }

    /// <summary>
    /// The energy cost per rail shot.
    /// </summary>
    public float EnergyCost
    {
        get { return _energyCost; }
    }

    /// <summary>
    /// The metal cost per rail shot.
    /// </summary>
    public float MetalCost
    {
        get { return _metalCost; }
    }

    /// <summary>
    /// Direction fired or processed during the reported tick.
    /// The current railgun model uses a fixed front/back direction choice instead of a freely aimed vector.
    /// </summary>
    public Flattiverse.Connector.GalaxyHierarchy.RailgunDirection Direction
    {
        get { return _direction; }
    }

    /// <summary>
    /// Tick-local runtime status reported for the railgun subsystem.
    /// </summary>
    public SubsystemStatus Status
    {
        get { return _status; }
    }

    /// <summary>
    /// Energy consumed by the railgun during the reported tick.
    /// </summary>
    public float ConsumedEnergyThisTick
    {
        get { return _consumedEnergyThisTick; }
    }

    /// <summary>
    /// Ions consumed by the railgun during the reported tick.
    /// </summary>
    public float ConsumedIonsThisTick
    {
        get { return _consumedIonsThisTick; }
    }

    /// <summary>
    /// Neutrinos consumed by the railgun during the reported tick.
    /// </summary>
    public float ConsumedNeutrinosThisTick
    {
        get { return _consumedNeutrinosThisTick; }
    }

    internal bool Update(PacketReader reader)
    {
        if (!reader.Read(out byte exists))
            return false;

        _exists = exists != 0;

        if (!_exists)
        {
            _energyCost = 0f;
            _metalCost = 0f;
            _direction = Flattiverse.Connector.GalaxyHierarchy.RailgunDirection.None;
            _status = SubsystemStatus.Off;
            _consumedEnergyThisTick = 0f;
            _consumedIonsThisTick = 0f;
            _consumedNeutrinosThisTick = 0f;
            return true;
        }

        if (!reader.Read(out _energyCost) ||
            !reader.Read(out _metalCost) ||
            !reader.Read(out byte direction) ||
            !reader.Read(out byte status) ||
            !reader.Read(out _consumedEnergyThisTick) ||
            !reader.Read(out _consumedIonsThisTick) ||
            !reader.Read(out _consumedNeutrinosThisTick))
            return false;

        _direction = (Flattiverse.Connector.GalaxyHierarchy.RailgunDirection)direction;
        _status = (SubsystemStatus)status;
        return true;
    }
}
