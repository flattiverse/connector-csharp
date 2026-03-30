using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Galaxy-wide system chat announcing that a ship hit the next mission target in sequence.
/// </summary>
public class MissionTargetHitChatEvent : SystemChatEvent
{
    /// <summary>
    /// Player who hit the mission target.
    /// </summary>
    public readonly Player Player;

    /// <summary>
    /// Controllable that hit the mission target.
    /// </summary>
    public readonly ControllableInfo ControllableInfo;

    /// <summary>
    /// Sequence number of the mission target that was hit.
    /// </summary>
    public readonly ushort MissionTargetSequence;

    internal MissionTargetHitChatEvent(Player player, ControllableInfo controllableInfo, ushort missionTargetSequence)
        : base($"Ship {controllableInfo.Name} of player {player.Name} hit mission target of sequence #{missionTargetSequence}.")
    {
        Player = player;
        ControllableInfo = controllableInfo;
        MissionTargetSequence = missionTargetSequence;
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.MissionTargetHitChat;

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} [SYSTEM] [{Player.Team.Name}] Ship {ControllableInfo.Name} of player {Player.Name} hit mission target of sequence #{MissionTargetSequence}.";
    }
}
