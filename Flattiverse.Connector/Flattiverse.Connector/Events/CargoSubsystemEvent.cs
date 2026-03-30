using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Runtime update of a cargo subsystem on your own controllable.
/// </summary>
public class CargoSubsystemEvent : ControllableSubsystemEvent
{
    /// <summary>
    /// Metal currently stored in cargo.
    /// </summary>
    public readonly float CurrentMetal;

    /// <summary>
    /// Carbon currently stored in cargo.
    /// </summary>
    public readonly float CurrentCarbon;

    /// <summary>
    /// Hydrogen currently stored in cargo.
    /// </summary>
    public readonly float CurrentHydrogen;

    /// <summary>
    /// Silicon currently stored in cargo.
    /// </summary>
    public readonly float CurrentSilicon;

    /// <summary>
    /// Nebula currently stored in cargo.
    /// </summary>
    public readonly float CurrentNebula;

    /// <summary>
    /// Average hue of the stored nebula.
    /// </summary>
    public readonly float NebulaHue;

    internal CargoSubsystemEvent(Controllable controllable, SubsystemSlot slot, SubsystemStatus status, float currentMetal,
        float currentCarbon, float currentHydrogen, float currentSilicon, float currentNebula, float nebulaHue) : base(controllable, slot, status)
    {
        CurrentMetal = currentMetal;
        CurrentCarbon = currentCarbon;
        CurrentHydrogen = currentHydrogen;
        CurrentSilicon = currentSilicon;
        CurrentNebula = currentNebula;
        NebulaHue = nebulaHue;
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.CargoSubsystem;

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} Cargo subsystem event: Controllable=\"{Controllable.Name}\", Slot={Slot}, Status={Status}, Cargo=({CurrentMetal:0.###},{CurrentCarbon:0.###},{CurrentHydrogen:0.###},{CurrentSilicon:0.###}), Nebula={CurrentNebula:0.###}, Hue={NebulaHue:0.###}.";
    }
}
