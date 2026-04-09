using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// Visible snapshot of a cargo subsystem on a scanned player unit.
/// </summary>
public class CargoSubsystemInfo
{
    private bool _exists;
    private float _maximumMetal;
    private float _maximumCarbon;
    private float _maximumHydrogen;
    private float _maximumSilicon;
    private float _maximumNebula;
    private float _currentMetal;
    private float _currentCarbon;
    private float _currentHydrogen;
    private float _currentSilicon;
    private float _currentNebula;
    private float _nebulaHue;
    private SubsystemStatus _status;

    internal CargoSubsystemInfo()
    {
        _exists = false;
        _maximumMetal = 0f;
        _maximumCarbon = 0f;
        _maximumHydrogen = 0f;
        _maximumSilicon = 0f;
        _maximumNebula = 0f;
        _currentMetal = 0f;
        _currentCarbon = 0f;
        _currentHydrogen = 0f;
        _currentSilicon = 0f;
        _currentNebula = 0f;
        _nebulaHue = 0f;
        _status = SubsystemStatus.Off;
    }

    internal CargoSubsystemInfo(CargoSubsystemInfo other)
    {
        _exists = other._exists;
        _maximumMetal = other._maximumMetal;
        _maximumCarbon = other._maximumCarbon;
        _maximumHydrogen = other._maximumHydrogen;
        _maximumSilicon = other._maximumSilicon;
        _maximumNebula = other._maximumNebula;
        _currentMetal = other._currentMetal;
        _currentCarbon = other._currentCarbon;
        _currentHydrogen = other._currentHydrogen;
        _currentSilicon = other._currentSilicon;
        _currentNebula = other._currentNebula;
        _nebulaHue = other._nebulaHue;
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
    /// Maximum metal capacity.
    /// </summary>
    public float MaximumMetal
    {
        get { return _maximumMetal; }
    }

    /// <summary>
    /// Maximum carbon capacity.
    /// </summary>
    public float MaximumCarbon
    {
        get { return _maximumCarbon; }
    }

    /// <summary>
    /// Maximum hydrogen capacity.
    /// </summary>
    public float MaximumHydrogen
    {
        get { return _maximumHydrogen; }
    }

    /// <summary>
    /// Maximum silicon capacity.
    /// </summary>
    public float MaximumSilicon
    {
        get { return _maximumSilicon; }
    }

    /// <summary>
    /// Maximum nebula capacity.
    /// </summary>
    public float MaximumNebula
    {
        get { return _maximumNebula; }
    }

    /// <summary>
    /// Current stored metal.
    /// </summary>
    public float CurrentMetal
    {
        get { return _currentMetal; }
    }

    /// <summary>
    /// Current stored carbon.
    /// </summary>
    public float CurrentCarbon
    {
        get { return _currentCarbon; }
    }

    /// <summary>
    /// Current stored hydrogen.
    /// </summary>
    public float CurrentHydrogen
    {
        get { return _currentHydrogen; }
    }

    /// <summary>
    /// Current stored silicon.
    /// </summary>
    public float CurrentSilicon
    {
        get { return _currentSilicon; }
    }

    /// <summary>
    /// Current stored nebula material.
    /// Nebula cargo is tracked separately from the normal metal, carbon, hydrogen, and silicon stores.
    /// </summary>
    public float CurrentNebula
    {
        get { return _currentNebula; }
    }

    /// <summary>
    /// Average hue of the nebula material currently stored in cargo.
    /// </summary>
    public float NebulaHue
    {
        get { return _nebulaHue; }
    }

    /// <summary>
    /// Tick-local runtime status reported for the cargo subsystem.
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
            _maximumMetal = 0f;
            _maximumCarbon = 0f;
            _maximumHydrogen = 0f;
            _maximumSilicon = 0f;
            _maximumNebula = 0f;
            _currentMetal = 0f;
            _currentCarbon = 0f;
            _currentHydrogen = 0f;
            _currentSilicon = 0f;
            _currentNebula = 0f;
            _nebulaHue = 0f;
            _status = SubsystemStatus.Off;
            return true;
        }

        if (!reader.Read(out _maximumMetal) ||
            !reader.Read(out _maximumCarbon) ||
            !reader.Read(out _maximumHydrogen) ||
            !reader.Read(out _maximumSilicon) ||
            !reader.Read(out _maximumNebula) ||
            !reader.Read(out _currentMetal) ||
            !reader.Read(out _currentCarbon) ||
            !reader.Read(out _currentHydrogen) ||
            !reader.Read(out _currentSilicon) ||
            !reader.Read(out _currentNebula) ||
            !reader.Read(out _nebulaHue) ||
            !reader.Read(out byte status))
            return false;

        _status = (SubsystemStatus)status;
        return true;
    }
}
