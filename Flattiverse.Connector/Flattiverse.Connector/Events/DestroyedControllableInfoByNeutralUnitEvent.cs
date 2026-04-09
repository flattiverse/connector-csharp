using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Raised when one controllable runtime is destroyed by a non-player world unit.
/// </summary>
public class DestroyedControllableInfoByNeutralUnitEvent : DestroyedControllableInfoEvent
{
    /// <summary>
    /// Unit kind of the non-player destroyer.
    /// </summary>
    public readonly UnitKind CollidersKind;
    
    /// <summary>
    /// Name of the non-player destroyer.
    /// </summary>
    public readonly string CollidersName;
    
    internal DestroyedControllableInfoByNeutralUnitEvent(Player player, ControllableInfo controllableInfo,
        UnitKind collidersKind, string collidersName) :
        base(player, controllableInfo, PlayerUnitDestroyedReason.CollidedWithNeutralUnit)
    {
        CollidersKind = collidersKind;
        CollidersName = collidersName;
    }

    /// <inheritdoc/>
    public override string ToString() => $"{Stamp:HH:mm:ss.fff} Player {Player.Name} of Team {Player.Team.Name} controllable {ControllableInfo.Name} of type {ControllableInfo.Kind} was destroyed by {CollidersKind} named {CollidersName}.";
}
