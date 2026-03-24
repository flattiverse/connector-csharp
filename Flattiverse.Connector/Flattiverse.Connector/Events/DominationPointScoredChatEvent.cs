using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Galaxy-wide system chat announcing that a domination point has scored.
/// </summary>
public class DominationPointScoredChatEvent : SystemChatEvent
{
    /// <summary>
    /// Team that scored the domination point.
    /// </summary>
    public readonly Team Team;

    /// <summary>
    /// Name of the domination point.
    /// </summary>
    public readonly string DominationPointName;

    internal DominationPointScoredChatEvent(Team team, string dominationPointName)
        : base($"Team {team.Name} scored domination point \"{dominationPointName}\".")
    {
        Team = team;
        DominationPointName = dominationPointName;
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.DominationPointScoredChat;

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} [SYSTEM] Team {Team.Name} scored domination point \"{DominationPointName}\".";
    }
}
