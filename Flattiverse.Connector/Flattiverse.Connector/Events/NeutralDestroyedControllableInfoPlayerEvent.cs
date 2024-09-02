using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events;

/// <summary>
/// A PlayerUnit got destroyed by colission with a neutral unit.
/// </summary>
public class NeutralDestroyedControllableInfoPlayerEvent : DestroyedControllableInfoPlayerEvent
{
    /// <summary>
    /// The UnitKind of the unit the PlayerUnit collided with.
    /// </summary>
    public readonly UnitKind CollidersKind;
    
    /// <summary>
    /// The name of the unit the PlayerUnit collided with.
    /// </summary>
    public readonly string CollidersName;
    
    internal NeutralDestroyedControllableInfoPlayerEvent(Player player, ControllableInfo controllableInfo,
        UnitKind collidersKind, string collidersName) :
        base(player, controllableInfo, PlayerUnitDestroyedReason.CollidedWithNeutralUnit)
    {
        CollidersKind = collidersKind;
        CollidersName = collidersName;
    }

    /// <inheritdoc/>
    public override string ToString() => $"{Stamp:HH:mm:ss.fff} Player {Player.Name} of Team {Player.Team.Name} controllable {ControllableInfo.Name} of type {ControllableInfo.Kind} collided with a {CollidersKind} named {CollidersName}.";
}