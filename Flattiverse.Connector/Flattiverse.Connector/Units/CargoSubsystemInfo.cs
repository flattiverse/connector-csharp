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

    internal void Update(bool exists, float maximumMetal, float maximumCarbon, float maximumHydrogen, float maximumSilicon,
        float maximumNebula, float currentMetal, float currentCarbon, float currentHydrogen, float currentSilicon, float currentNebula,
        float nebulaHue, SubsystemStatus status)
    {
        _exists = exists;
        _maximumMetal = exists ? maximumMetal : 0f;
        _maximumCarbon = exists ? maximumCarbon : 0f;
        _maximumHydrogen = exists ? maximumHydrogen : 0f;
        _maximumSilicon = exists ? maximumSilicon : 0f;
        _maximumNebula = exists ? maximumNebula : 0f;
        _currentMetal = exists ? currentMetal : 0f;
        _currentCarbon = exists ? currentCarbon : 0f;
        _currentHydrogen = exists ? currentHydrogen : 0f;
        _currentSilicon = exists ? currentSilicon : 0f;
        _currentNebula = exists ? currentNebula : 0f;
        _nebulaHue = exists ? nebulaHue : 0f;
        _status = exists ? status : SubsystemStatus.Off;
    }
}
