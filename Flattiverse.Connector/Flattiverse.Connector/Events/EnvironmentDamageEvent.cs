using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Owner-only runtime update about passive heat and radiation in the current tick.
/// </summary>
public class EnvironmentDamageEvent : FlattiverseEvent
{
    /// <summary>
    /// The affected controllable.
    /// </summary>
    public readonly Controllable Controllable;

    /// <summary>
    /// Aggregated incoming heat of the tick.
    /// </summary>
    public readonly float Heat;

    /// <summary>
    /// Energy drained by heat in the tick.
    /// </summary>
    public readonly float HeatEnergyCost;

    /// <summary>
    /// Heat that could not be paid and therefore overflowed into radiation.
    /// </summary>
    public readonly float HeatEnergyOverflow;

    /// <summary>
    /// Aggregated incoming radiation of the tick before heat overflow is added.
    /// </summary>
    public readonly float Radiation;

    /// <summary>
    /// Radiation damage before armor reduction.
    /// </summary>
    public readonly float RadiationDamageBeforeArmor;

    /// <summary>
    /// Radiation damage blocked by armor.
    /// </summary>
    public readonly float ArmorBlockedDamage;

    /// <summary>
    /// Hull damage caused by the passive environment in the tick.
    /// </summary>
    public readonly float HullDamage;

    internal EnvironmentDamageEvent(Controllable controllable, float heat, float heatEnergyCost, float heatEnergyOverflow, float radiation,
        float radiationDamageBeforeArmor, float armorBlockedDamage, float hullDamage) : base()
    {
        Controllable = controllable;
        Heat = heat;
        HeatEnergyCost = heatEnergyCost;
        HeatEnergyOverflow = heatEnergyOverflow;
        Radiation = radiation;
        RadiationDamageBeforeArmor = radiationDamageBeforeArmor;
        ArmorBlockedDamage = armorBlockedDamage;
        HullDamage = hullDamage;
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.EnvironmentDamage;

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} Environment damage event: Controllable=\"{Controllable.Name}\", Heat={Heat:0.###}, HeatEnergyCost={HeatEnergyCost:0.###}, HeatOverflow={HeatEnergyOverflow:0.###}, Radiation={Radiation:0.###}, RadiationDamageBeforeArmor={RadiationDamageBeforeArmor:0.###}, ArmorBlockedDamage={ArmorBlockedDamage:0.###}, HullDamage={HullDamage:0.###}.";
    }
}
