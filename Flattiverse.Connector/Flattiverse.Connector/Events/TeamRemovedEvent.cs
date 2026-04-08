namespace Flattiverse.Connector.Events;

/// <summary>
/// Is raised when a team has been removed.
/// </summary>
public class TeamRemovedEvent : FlattiverseEvent
{
    /// <summary>
    /// Snapshot of the removed team.
    /// </summary>
    public readonly TeamSnapshot Team;

    internal TeamRemovedEvent(TeamSnapshot team)
    {
        Team = team;
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.TeamRemoved;

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} Team removed: Id={Team.Id}, Name=\"{Team.Name}\", Red={Team.Red}, Green={Team.Green}, Blue={Team.Blue}, Playable={Team.Playable}.";
    }
}
