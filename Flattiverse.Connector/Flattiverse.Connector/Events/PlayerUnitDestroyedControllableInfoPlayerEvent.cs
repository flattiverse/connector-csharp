using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events;

/// <summary>
/// A PlayerUnit got destroyed by another PlayerUnit.
/// </summary>
public class PlayerUnitDestroyedControllableInfoPlayerEvent : DestroyedControllableInfoPlayerEvent
{
    /// <summary>
    /// The PlayerUnit which destroyed the PlayerUnit in question.
    /// </summary>
    public readonly ControllableInfo DestroyerUnit;
    
    /// <summary>
    /// The Player of the unit which destroyed the PlayerUnit.
    /// </summary>
    public readonly Player DestroyerPlayer;
    
    internal PlayerUnitDestroyedControllableInfoPlayerEvent(Player player, ControllableInfo controllableInfo,
        PlayerUnitDestroyedReason reason, ControllableInfo destroyerUnit) :
        base(player, controllableInfo, reason)
    {
        DestroyerUnit = destroyerUnit;
        DestroyerPlayer = destroyerUnit.Player;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        switch (Reason)
        {
            case PlayerUnitDestroyedReason.CollidedWithEnemyPlayerUnit:
                return
                    $"{Stamp:HH:mm:ss.fff} Player {Player.Name} of Team {Player.Team.Name}, controllable {ControllableInfo.Name} of type {ControllableInfo.Kind}, got destroyed by colliding with enemy player {DestroyerPlayer.Name} of Team {DestroyerPlayer.Team.Name}, unit {DestroyerUnit.Name} of type {DestroyerUnit.Kind}.";
            case PlayerUnitDestroyedReason.CollidedWithFriendlyPlayerUnit:
                return
                    $"{Stamp:HH:mm:ss.fff} Player {Player.Name} of Team {Player.Team.Name}, controllable {ControllableInfo.Name} of type {ControllableInfo.Kind}, got destroyed by colliding with friendly player {DestroyerPlayer.Name}, unit {DestroyerUnit.Name} of type {DestroyerUnit.Kind}.";
            case PlayerUnitDestroyedReason.ShotByEnemyPlayerUnit:
                return
                    $"{Stamp:HH:mm:ss.fff} Player {Player.Name} of Team {Player.Team.Name}, controllable {ControllableInfo.Name} of type {ControllableInfo.Kind}, was shot by enemy player {DestroyerPlayer.Name} of Team {DestroyerPlayer.Team.Name}, unit {DestroyerUnit.Name} of type {DestroyerUnit.Kind}.";
            case PlayerUnitDestroyedReason.ShotByFriendlyPlayerUnit:
                return
                    $"{Stamp:HH:mm:ss.fff} Player {Player.Name} of Team {Player.Team.Name}, controllable {ControllableInfo.Name} of type {ControllableInfo.Kind}, was shot by friendly player {DestroyerPlayer.Name}, unit {DestroyerUnit.Name} of type {DestroyerUnit.Kind}.";
            default:
                return
                    $"{Stamp:HH:mm:ss.fff} Player {Player.Name} of Team {Player.Team.Name} controllable {ControllableInfo.Name} of type {ControllableInfo.Kind} got destroyed.";
        }
    }
}