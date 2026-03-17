using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Is raised when a team score has been updated.
/// </summary>
public class TeamScoreUpdatedEvent : FlattiverseEvent
{
    /// <summary>
    /// The team whose score changed.
    /// </summary>
    public readonly Team Team;

    /// <summary>
    /// Kill count before the update.
    /// </summary>
    public readonly uint OldKills;

    /// <summary>
    /// Death count before the update.
    /// </summary>
    public readonly uint OldDeaths;

    /// <summary>
    /// Mission score before the update.
    /// </summary>
    public readonly uint OldMission;

    /// <summary>
    /// Kill count after the update.
    /// </summary>
    public readonly uint NewKills;

    /// <summary>
    /// Death count after the update.
    /// </summary>
    public readonly uint NewDeaths;

    /// <summary>
    /// Mission score after the update.
    /// </summary>
    public readonly uint NewMission;

    internal TeamScoreUpdatedEvent(Team team, uint oldKills, uint oldDeaths, uint oldMission, uint newKills, uint newDeaths, uint newMission)
    {
        Team = team;
        OldKills = oldKills;
        OldDeaths = oldDeaths;
        OldMission = oldMission;
        NewKills = newKills;
        NewDeaths = newDeaths;
        NewMission = newMission;
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.TeamScoreUpdated;

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} Team score updated: Id={Team.Id}, Kills={OldKills}->{NewKills}, Deaths={OldDeaths}->{NewDeaths}, Mission={OldMission}->{NewMission}.";
    }
}
