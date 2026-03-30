using Flattiverse.Connector.Events;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Cargo subsystem of a controllable.
/// </summary>
public class CargoSubsystem : Subsystem
{
    private const float ClassicShipMaximumMetal = 20f;
    private const float ClassicShipMaximumCarbon = 20f;
    private const float ClassicShipMaximumHydrogen = 20f;
    private const float ClassicShipMaximumSilicon = 20f;
    private const float ClassicShipMaximumNebula = 24f;

    private readonly float _maximumMetal;
    private readonly float _maximumCarbon;
    private readonly float _maximumHydrogen;
    private readonly float _maximumSilicon;
    private float _maximumNebula;

    private float _currentMetal;
    private float _currentCarbon;
    private float _currentHydrogen;
    private float _currentSilicon;
    private float _currentNebula;
    private float _nebulaHue;

    internal CargoSubsystem(Controllable controllable, bool exists, float maximumMetal, float maximumCarbon, float maximumHydrogen,
        float maximumSilicon, float maximumNebula, SubsystemSlot slot) : base(controllable, "Cargo", exists, slot)
    {
        _maximumMetal = exists ? maximumMetal : 0f;
        _maximumCarbon = exists ? maximumCarbon : 0f;
        _maximumHydrogen = exists ? maximumHydrogen : 0f;
        _maximumSilicon = exists ? maximumSilicon : 0f;
        _maximumNebula = exists ? maximumNebula : 0f;
        _currentMetal = 0f;
        _currentCarbon = 0f;
        _currentHydrogen = 0f;
        _currentSilicon = 0f;
        _currentNebula = 0f;
        _nebulaHue = 0f;
    }

    internal static CargoSubsystem CreateClassicShipCargo(Controllable controllable)
    {
        return new CargoSubsystem(controllable, true, ClassicShipMaximumMetal, ClassicShipMaximumCarbon, ClassicShipMaximumHydrogen,
            ClassicShipMaximumSilicon, ClassicShipMaximumNebula, SubsystemSlot.Cargo);
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
    /// Current stored nebula.
    /// </summary>
    public float CurrentNebula
    {
        get { return _currentNebula; }
    }

    /// <summary>
    /// Average hue of the stored nebula.
    /// </summary>
    public float NebulaHue
    {
        get { return _nebulaHue; }
    }

    internal void SetMaximumNebula(float maximumNebula)
    {
        _maximumNebula = Exists ? maximumNebula : 0f;

        if (_currentNebula > _maximumNebula)
            _currentNebula = _maximumNebula;
    }

    internal void ResetRuntime()
    {
        _currentMetal = 0f;
        _currentCarbon = 0f;
        _currentHydrogen = 0f;
        _currentSilicon = 0f;
        _currentNebula = 0f;
        _nebulaHue = 0f;
        ResetRuntimeStatus();
    }

    internal void UpdateRuntime(float currentMetal, float currentCarbon, float currentHydrogen, float currentSilicon, float currentNebula,
        float nebulaHue, SubsystemStatus status)
    {
        _currentMetal = currentMetal;
        _currentCarbon = currentCarbon;
        _currentHydrogen = currentHydrogen;
        _currentSilicon = currentSilicon;
        _currentNebula = currentNebula;
        _nebulaHue = nebulaHue;
        UpdateRuntimeStatus(status);
    }

    internal FlattiverseEvent? CreateRuntimeEvent()
    {
        if (!Exists || !ShouldEmitRuntimeEvent())
            return null;

        return new CargoSubsystemEvent(Controllable, Slot, Status, _currentMetal, _currentCarbon, _currentHydrogen, _currentSilicon,
            _currentNebula, _nebulaHue);
    }
}
