using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Raised when the galaxy starts mirroring a newly configured tournament.
/// </summary>
public class CreatedTournamentEvent : FlattiverseEvent
{
    private readonly Tournament _tournament;

    internal CreatedTournamentEvent(Tournament tournament)
    {
        _tournament = tournament;
    }

    /// <summary>
    /// Newly mirrored tournament snapshot.
    /// </summary>
    public Tournament Tournament
    {
        get { return _tournament; }
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.TournamentCreated;

    /// <inheritdoc/>
    public override string ToString() => $"Tournament created: {_tournament.Mode} in stage {_tournament.Stage}.";
}
