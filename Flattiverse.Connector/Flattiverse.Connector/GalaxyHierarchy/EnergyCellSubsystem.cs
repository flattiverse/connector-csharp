using Flattiverse.Connector.Events;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Passive energy cell subsystem of a controllable.
/// </summary>
public class EnergyCellSubsystem : Subsystem
{
    private float _efficiency;
    private float _collectedThisTick;

    internal EnergyCellSubsystem(Controllable controllable, string name, bool exists, float efficiency, SubsystemSlot slot) :
        base(controllable, name, exists, slot)
    {
        _efficiency = 0f;
        _collectedThisTick = 0f;

        SetEfficiency(efficiency);
    }

    internal static EnergyCellSubsystem CreateClassicShipEnergyCell(Controllable controllable)
    {
        return new EnergyCellSubsystem(controllable, "EnergyCell", true, 0.4f, SubsystemSlot.EnergyCell);
    }

    internal static EnergyCellSubsystem CreateMissingCell(Controllable controllable, string name, SubsystemSlot slot)
    {
        return new EnergyCellSubsystem(controllable, name, false, 0f, slot);
    }

    /// <summary>
    /// The loading efficiency of this cell.
    /// </summary>
    public float Efficiency
    {
        get { return _efficiency; }
    }

    /// <summary>
    /// The amount collected through this cell during the current server tick.
    /// </summary>
    public float CollectedThisTick
    {
        get { return _collectedThisTick; }
    }

    internal void SetEfficiency(float efficiency)
    {
        _efficiency = Exists ? efficiency : 0f;
    }

    internal void ResetRuntime()
    {
        _collectedThisTick = 0f;
        ResetRuntimeStatus();
    }

    internal void UpdateRuntime(float collectedThisTick, SubsystemStatus status)
    {
        _collectedThisTick = collectedThisTick;
        UpdateRuntimeStatus(status);
    }

    internal FlattiverseEvent? CreateRuntimeEvent()
    {
        if (!Exists || !ShouldEmitRuntimeEvent())
            return null;

        return new EnergyCellSubsystemEvent(Controllable, Slot, Status, _collectedThisTick);
    }
}
