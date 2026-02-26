namespace Flattiverse.Connector.Events;

/// <summary>
/// Is raised when a team has been created.
/// </summary>
public class TeamCreatedEvent : FlattiverseEvent
{
    /// <summary>
    /// Snapshot of the created team.
    /// </summary>
    public readonly TeamSnapshot Team;

    internal TeamCreatedEvent(TeamSnapshot team)
    {
        Team = team;
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.TeamCreated;

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} Team created: Id={Team.Id}, Name=\"{Team.Name}\", Red={Team.Red}, Green={Team.Green}, Blue={Team.Blue}.";
    }
}
