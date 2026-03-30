using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Raised when the mirrored tournament snapshot changes.
/// </summary>
public class TournamentUpdatedEvent : FlattiverseEvent
{
    private readonly Tournament _oldTournament;
    private readonly Tournament _newTournament;

    internal TournamentUpdatedEvent(Tournament oldTournament, Tournament newTournament)
    {
        _oldTournament = oldTournament;
        _newTournament = newTournament;
    }

    /// <summary>
    /// Tournament snapshot before the update.
    /// </summary>
    public Tournament OldTournament
    {
        get { return _oldTournament; }
    }

    /// <summary>
    /// Tournament snapshot after the update.
    /// </summary>
    public Tournament NewTournament
    {
        get { return _newTournament; }
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.TournamentUpdated;

    /// <inheritdoc/>
    public override string ToString() => $"Tournament updated: {_oldTournament.Stage} -> {_newTournament.Stage}.";
}
