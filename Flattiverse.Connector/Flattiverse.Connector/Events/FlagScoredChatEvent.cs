using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Galaxy-wide system chat announcing that a flag has been scored.
/// </summary>
public class FlagScoredChatEvent : SystemChatEvent
{
    /// <summary>
    /// Player who triggered the score.
    /// </summary>
    public readonly Player Player;

    /// <summary>
    /// Controllable that triggered the score.
    /// </summary>
    public readonly ControllableInfo ControllableInfo;

    /// <summary>
    /// Team configured on the flag.
    /// </summary>
    public readonly Team FlagTeam;

    /// <summary>
    /// Name of the scored flag.
    /// </summary>
    public readonly string FlagName;

    internal FlagScoredChatEvent(Player player, ControllableInfo controllableInfo, Team flagTeam, string flagName)
        : base($"{player.Name} / {controllableInfo.Name} scored flag \"{flagName}\" of team {flagTeam.Name}.")
    {
        Player = player;
        ControllableInfo = controllableInfo;
        FlagTeam = flagTeam;
        FlagName = flagName;
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.FlagScoredChat;

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} [SYSTEM] [{Player.Team.Name}] {Player.Name} / {ControllableInfo.Name} scored flag \"{FlagName}\" of team {FlagTeam.Name}.";
    }
}
