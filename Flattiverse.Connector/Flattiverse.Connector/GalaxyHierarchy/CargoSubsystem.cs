using Flattiverse.Connector.Network;

using Flattiverse.Connector.Events;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Cargo subsystem of a controllable.
/// </summary>
public class CargoSubsystem : Subsystem
{
    private const float ClassicShipMaximumMetal = 250f;
    private const float ClassicShipMaximumCarbon = 12f;
    private const float ClassicShipMaximumHydrogen = 12f;
    private const float ClassicShipMaximumSilicon = 12f;
    private const float ClassicShipMaximumNebula = 16f;

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

    internal CargoSubsystem(Controllable controllable, PacketReader reader, SubsystemSlot slot) :
        base(controllable, "Cargo", false, slot)
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

        if (!reader.Read(out byte exists))
            throw new InvalidDataException("Couldn't read controllable cargo state.");

        SetExists(exists != 0);

        if (!Exists)
            return;

        if (!reader.Read(out byte tier) ||
            !reader.Read(out float maximumMetal) ||
            !reader.Read(out float maximumCarbon) ||
            !reader.Read(out float maximumHydrogen) ||
            !reader.Read(out float maximumSilicon) ||
            !reader.Read(out float maximumNebula) ||
            !reader.Read(out float currentMetal) ||
            !reader.Read(out float currentCarbon) ||
            !reader.Read(out float currentHydrogen) ||
            !reader.Read(out float currentSilicon) ||
            !reader.Read(out float currentNebula) ||
            !reader.Read(out float nebulaHue) ||
            !reader.Read(out byte status))
            throw new InvalidDataException("Couldn't read controllable cargo state.");

        SetMaximums(maximumMetal, maximumCarbon, maximumHydrogen, maximumSilicon, maximumNebula);
        UpdateRuntime(currentMetal, currentCarbon, currentHydrogen, currentSilicon, currentNebula, nebulaHue, (SubsystemStatus)status);
        SetReportedTier(tier);
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

    internal void SetMaximums(float maximumMetal, float maximumCarbon, float maximumHydrogen, float maximumSilicon, float maximumNebula)
    {
        _maximumMetal = Exists ? maximumMetal : 0f;
        _maximumCarbon = Exists ? maximumCarbon : 0f;
        _maximumHydrogen = Exists ? maximumHydrogen : 0f;
        _maximumSilicon = Exists ? maximumSilicon : 0f;
        _maximumNebula = Exists ? maximumNebula : 0f;
        RefreshTier();

        if (_currentMetal > _maximumMetal)
            _currentMetal = _maximumMetal;

        if (_currentCarbon > _maximumCarbon)
            _currentCarbon = _maximumCarbon;

        if (_currentHydrogen > _maximumHydrogen)
            _currentHydrogen = _maximumHydrogen;

        if (_currentSilicon > _maximumSilicon)
            _currentSilicon = _maximumSilicon;

        if (_currentNebula > _maximumNebula)
            _currentNebula = _maximumNebula;
    }

    internal void CopyFrom(CargoSubsystem other)
    {
        CopyBaseFrom(other);
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

    internal bool Update(PacketReader reader)
    {
        if (!Exists)
            return true;

        if (!reader.Read(out float currentMetal) ||
            !reader.Read(out float currentCarbon) ||
            !reader.Read(out float currentHydrogen) ||
            !reader.Read(out float currentSilicon) ||
            !reader.Read(out float currentNebula) ||
            !reader.Read(out float nebulaHue) ||
            !reader.Read(out byte status))
            return false;

        UpdateRuntime(currentMetal, currentCarbon, currentHydrogen, currentSilicon, currentNebula, nebulaHue, (SubsystemStatus)status);
        return true;
    }

    internal FlattiverseEvent? CreateRuntimeEvent()
    {
        if (!Exists || !ShouldEmitRuntimeEvent())
            return null;

        return new CargoSubsystemEvent(Controllable, Slot, Status, _currentMetal, _currentCarbon, _currentHydrogen, _currentSilicon,
            _currentNebula, _nebulaHue);
    }

    protected override void RefreshTier()
    {
        if (!Exists)
        {
            SetTier(0);
            return;
        }

        for (byte tier = 1; tier <= ShipUpgradeBalancing.GetMaximumTier(Slot); tier++)
        {
            ShipBalancing.GetCargo(tier, out float maximumMetal, out float maximumCarbon, out float maximumHydrogen, out float maximumSilicon,
                out float maximumNebula, out float load);

            if (Matches(_maximumMetal, maximumMetal) && Matches(_maximumCarbon, maximumCarbon) &&
                Matches(_maximumHydrogen, maximumHydrogen) && Matches(_maximumSilicon, maximumSilicon) &&
                Matches(_maximumNebula, maximumNebula))
            {
                SetTier(tier);
                return;
            }
        }

        SetTier(0);
    }
}
