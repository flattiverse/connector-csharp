using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Raised when one controllable runtime is destroyed by colliding with a neutral world unit.
/// </summary>
public class NeutralDestroyedControllableInfoPlayerEvent : DestroyedControllableInfoPlayerEvent
{
    /// <summary>
    /// Unit kind of the neutral collider.
    /// </summary>
    public readonly UnitKind CollidersKind;
    
    /// <summary>
    /// Name of the neutral collider.
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
