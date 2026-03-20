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
    /// Enemy-player kill count before the update.
    /// </summary>
    public readonly uint OldPlayerKills;

    /// <summary>
    /// Death count before the update.
    /// </summary>
    public readonly uint OldDeaths;

    /// <summary>
    /// Enemy-player death count before the update.
    /// </summary>
    public readonly uint OldPlayerDeaths;

    /// <summary>
    /// Friendly kill count before the update.
    /// </summary>
    public readonly uint OldFriendlyKills;

    /// <summary>
    /// Friendly death count before the update.
    /// </summary>
    public readonly uint OldFriendlyDeaths;

    /// <summary>
    /// NPC kill count before the update.
    /// </summary>
    public readonly uint OldNpcKills;

    /// <summary>
    /// NPC death count before the update.
    /// </summary>
    public readonly uint OldNpcDeaths;

    /// <summary>
    /// Neutral death count before the update.
    /// </summary>
    public readonly uint OldNeutralDeaths;

    /// <summary>
    /// Mission score before the update.
    /// </summary>
    public readonly uint OldMission;

    /// <summary>
    /// Kill count after the update.
    /// </summary>
    public readonly uint NewKills;

    /// <summary>
    /// Enemy-player kill count after the update.
    /// </summary>
    public readonly uint NewPlayerKills;

    /// <summary>
    /// Death count after the update.
    /// </summary>
    public readonly uint NewDeaths;

    /// <summary>
    /// Enemy-player death count after the update.
    /// </summary>
    public readonly uint NewPlayerDeaths;

    /// <summary>
    /// Friendly kill count after the update.
    /// </summary>
    public readonly uint NewFriendlyKills;

    /// <summary>
    /// Friendly death count after the update.
    /// </summary>
    public readonly uint NewFriendlyDeaths;

    /// <summary>
    /// NPC kill count after the update.
    /// </summary>
    public readonly uint NewNpcKills;

    /// <summary>
    /// NPC death count after the update.
    /// </summary>
    public readonly uint NewNpcDeaths;

    /// <summary>
    /// Neutral death count after the update.
    /// </summary>
    public readonly uint NewNeutralDeaths;

    /// <summary>
    /// Mission score after the update.
    /// </summary>
    public readonly uint NewMission;

    internal TeamScoreUpdatedEvent(Team team, uint oldPlayerKills, uint oldPlayerDeaths, uint oldFriendlyKills, uint oldFriendlyDeaths,
        uint oldNpcKills, uint oldNpcDeaths, uint oldNeutralDeaths, uint oldMission, uint newPlayerKills, uint newPlayerDeaths,
        uint newFriendlyKills, uint newFriendlyDeaths, uint newNpcKills, uint newNpcDeaths, uint newNeutralDeaths, uint newMission)
    {
        Team = team;
        OldKills = oldPlayerKills;
        OldPlayerKills = oldPlayerKills;
        OldDeaths = oldPlayerDeaths;
        OldPlayerDeaths = oldPlayerDeaths;
        OldFriendlyKills = oldFriendlyKills;
        OldFriendlyDeaths = oldFriendlyDeaths;
        OldNpcKills = oldNpcKills;
        OldNpcDeaths = oldNpcDeaths;
        OldNeutralDeaths = oldNeutralDeaths;
        OldMission = oldMission;
        NewKills = newPlayerKills;
        NewPlayerKills = newPlayerKills;
        NewDeaths = newPlayerDeaths;
        NewPlayerDeaths = newPlayerDeaths;
        NewFriendlyKills = newFriendlyKills;
        NewFriendlyDeaths = newFriendlyDeaths;
        NewNpcKills = newNpcKills;
        NewNpcDeaths = newNpcDeaths;
        NewNeutralDeaths = newNeutralDeaths;
        NewMission = newMission;
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.TeamScoreUpdated;

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} Team score updated: Id={Team.Id}, PlayerKills={OldPlayerKills}->{NewPlayerKills}, PlayerDeaths={OldPlayerDeaths}->{NewPlayerDeaths}, FriendlyKills={OldFriendlyKills}->{NewFriendlyKills}, FriendlyDeaths={OldFriendlyDeaths}->{NewFriendlyDeaths}, NpcKills={OldNpcKills}->{NewNpcKills}, NpcDeaths={OldNpcDeaths}->{NewNpcDeaths}, NeutralDeaths={OldNeutralDeaths}->{NewNeutralDeaths}, Mission={OldMission}->{NewMission}.";
    }
}
