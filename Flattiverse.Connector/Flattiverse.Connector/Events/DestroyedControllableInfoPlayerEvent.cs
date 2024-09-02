using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// A PlayerUnit got destroyed.
/// </summary>
public class DestroyedControllableInfoPlayerEvent : ControllableInfoPlayerEvent
{
    public readonly PlayerUnitDestroyedReason Reason;
    
    internal DestroyedControllableInfoPlayerEvent(Player player, ControllableInfo controllableInfo, PlayerUnitDestroyedReason reason) :
        base(player, controllableInfo)
    {
        Reason = reason;
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.ControllableInfoDestroyed;

    /// <inheritdoc/>
    public override string ToString()
    {
        switch (Reason)
        {
            case PlayerUnitDestroyedReason.ByRules:
                return
                    $"{Stamp:HH:mm:ss.fff} Player {Player.Name} of Team {Player.Team.Name} controllable {ControllableInfo.Name} of type {ControllableInfo.Kind} got destroyed due to applied rules.";
            case PlayerUnitDestroyedReason.Suicided:
                return
                    $"{Stamp:HH:mm:ss.fff} Player {Player.Name} of Team {Player.Team.Name} controllable {ControllableInfo.Name} of type {ControllableInfo.Kind} suicided.";
            default:
                return
                    $"{Stamp:HH:mm:ss.fff} Player {Player.Name} of Team {Player.Team.Name} controllable {ControllableInfo.Name} of type {ControllableInfo.Kind} got destroyed.";
        }
    }
}