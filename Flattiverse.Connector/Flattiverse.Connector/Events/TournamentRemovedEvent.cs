using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Raised when the currently mirrored tournament is removed from the galaxy.
/// </summary>
public class TournamentRemovedEvent : FlattiverseEvent
{
    private readonly Tournament _tournament;

    internal TournamentRemovedEvent(Tournament tournament)
    {
        _tournament = tournament;
    }

    /// <summary>
    /// Last tournament snapshot before removal.
    /// </summary>
    public Tournament Tournament
    {
        get { return _tournament; }
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.TournamentRemoved;

    /// <inheritdoc/>
    public override string ToString() => $"Tournament removed from stage {_tournament.Stage}.";
}
