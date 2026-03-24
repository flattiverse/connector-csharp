using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Galaxy-wide system chat announcing that someone hit the own flag.
/// </summary>
public class OwnFlagHitChatEvent : SystemChatEvent
{
    /// <summary>
    /// Player who triggered the own goal.
    /// </summary>
    public readonly Player Player;

    /// <summary>
    /// Controllable that triggered the own goal.
    /// </summary>
    public readonly ControllableInfo ControllableInfo;

    /// <summary>
    /// Team configured on the flag.
    /// </summary>
    public readonly Team FlagTeam;

    /// <summary>
    /// Name of the flag.
    /// </summary>
    public readonly string FlagName;

    internal OwnFlagHitChatEvent(Player player, ControllableInfo controllableInfo, Team flagTeam, string flagName)
        : base($"{player.Name} / {controllableInfo.Name} hit the own flag \"{flagName}\" of team {flagTeam.Name}. The other teams gladly take the free point.")
    {
        Player = player;
        ControllableInfo = controllableInfo;
        FlagTeam = flagTeam;
        FlagName = flagName;
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.OwnFlagHitChat;

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} [SYSTEM] [{Player.Team.Name}] {Player.Name} / {ControllableInfo.Name} hit the own flag \"{FlagName}\" of team {FlagTeam.Name}. The other teams gladly take the free point.";
    }
}
