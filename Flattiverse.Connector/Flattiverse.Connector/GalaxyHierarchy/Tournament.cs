namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Immutable snapshot of one configured tournament as mirrored by the connector.
/// </summary>
public class Tournament
{
    private readonly TournamentStage _stage;
    private readonly TournamentMode _mode;
    private readonly uint _durationTicks;
    private readonly TournamentTeam[] _teams;
    private readonly TournamentMatchResult[] _matchHistory;

    internal Tournament(TournamentStage stage, TournamentMode mode, uint durationTicks, TournamentTeam[] teams,
        TournamentMatchResult[] matchHistory)
    {
        _stage = stage;
        _mode = mode;
        _durationTicks = durationTicks;
        _teams = teams;
        _matchHistory = matchHistory;
    }

    /// <summary>
    /// Current lifecycle stage of the tournament.
    /// </summary>
    public TournamentStage Stage
    {
        get { return _stage; }
    }

    /// <summary>
    /// Match format used by the tournament, for example <see cref="TournamentMode.BestOf5" />.
    /// </summary>
    public TournamentMode Mode
    {
        get { return _mode; }
    }

    /// <summary>
    /// Planned match duration in server ticks for each game of the series.
    /// </summary>
    public uint DurationTicks
    {
        get { return _durationTicks; }
    }

    /// <summary>
    /// Participating teams together with their configured account list and currently known win count.
    /// </summary>
    public IReadOnlyList<TournamentTeam> Teams
    {
        get { return _teams; }
    }

    /// <summary>
    /// Ordered history of already finished matches.
    /// </summary>
    public IReadOnlyList<TournamentMatchResult> MatchHistory
    {
        get { return _matchHistory; }
    }

    /// <summary>
    /// Number of the currently live match while the tournament is commencing or running.
    /// During preparation this already points at the next match to start, derived as <c>MatchHistory.Count + 1</c>.
    /// </summary>
    public int CurrentMatchNumber
    {
        get { return _matchHistory.Length + 1; }
    }
}
