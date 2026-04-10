using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Raised when a public controllable entry dies.
/// </summary>
public class DestroyedControllableInfoEvent : ControllableInfoEvent
{
    /// <summary>
    /// Reason the referenced controllable was destroyed.
    /// </summary>
    public readonly PlayerUnitDestroyedReason Reason;
    
    internal DestroyedControllableInfoEvent(Player player, ControllableInfo controllableInfo, PlayerUnitDestroyedReason reason) :
        base(player, controllableInfo)
    {
        Reason = reason;
    }

    /// <inheritdoc />
    public override EventKind Kind => EventKind.ControllableInfoDestroyed;

    /// <summary>
    /// Returns a compact diagnostic representation of the event.
    /// </summary>
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
            case PlayerUnitDestroyedReason.ByClusterRemoval:
                return
                    $"{Stamp:HH:mm:ss.fff} Player {Player.Name} of Team {Player.Team.Name} controllable {ControllableInfo.Name} of type {ControllableInfo.Kind} got destroyed because its cluster was removed.";
            case PlayerUnitDestroyedReason.LostInDeepSpace:
                return
                    $"{Stamp:HH:mm:ss.fff} Player {Player.Name} of Team {Player.Team.Name} controllable {ControllableInfo.Name} of type {ControllableInfo.Kind} was lost in deep space.";
            default:
                return
                    $"{Stamp:HH:mm:ss.fff} Player {Player.Name} of Team {Player.Team.Name} controllable {ControllableInfo.Name} of type {ControllableInfo.Kind} got destroyed.";
        }
    }
}
