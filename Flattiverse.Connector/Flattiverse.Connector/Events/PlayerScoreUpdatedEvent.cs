using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Is raised when a player score has been updated.
/// </summary>
public class PlayerScoreUpdatedEvent : PlayerEvent
{
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

    internal PlayerScoreUpdatedEvent(Player player, uint oldKills, uint oldDeaths, uint oldMission, uint newKills, uint newDeaths, uint newMission) : base(player)
    {
        OldKills = oldKills;
        OldDeaths = oldDeaths;
        OldMission = oldMission;
        NewKills = newKills;
        NewDeaths = newDeaths;
        NewMission = newMission;
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.PlayerScoreUpdated;

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} Player score updated: Id={Player.Id}, Name=\"{Player.Name}\", Kills={OldKills}->{NewKills}, Deaths={OldDeaths}->{NewDeaths}, Mission={OldMission}->{NewMission}.";
    }
}
