using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Raised when one controllable runtime is destroyed by another player-owned controllable runtime.
/// </summary>
public class DestroyedControllableInfoByPlayerUnitEvent : DestroyedControllableInfoEvent
{
    /// <summary>
    /// Controllable entry of the destroyer.
    /// </summary>
    public readonly ControllableInfo DestroyerUnit;
    
    /// <summary>
    /// Owner of the destroyer controllable.
    /// </summary>
    public readonly Player DestroyerPlayer;
    
    internal DestroyedControllableInfoByPlayerUnitEvent(Player player, ControllableInfo controllableInfo,
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
