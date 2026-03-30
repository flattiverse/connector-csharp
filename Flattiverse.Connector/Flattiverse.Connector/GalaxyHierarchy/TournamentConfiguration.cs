namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Client-side tournament configuration that can be uploaded with <see cref="Galaxy.ConfigureTournament" />.
/// </summary>
public class TournamentConfiguration
{
    private readonly TournamentMode _mode;
    private readonly uint _durationTicks;
    private readonly TournamentTeamConfiguration[] _teams;
    private readonly byte[] _winningTeamIds;

    /// <summary>
    /// Creates a new connector-side tournament configuration.
    /// </summary>
    /// <param name="mode">Series format of the tournament.</param>
    /// <param name="durationTicks">Planned duration of one match in server ticks.</param>
    /// <param name="teams">Configured tournament teams with their participant account ids.</param>
    /// <param name="winningTeamIds">
    /// Ordered winner history of already finished matches. This lets an admin continue a partially played series.
    /// </param>
    public TournamentConfiguration(TournamentMode mode, uint durationTicks, TournamentTeamConfiguration[] teams, byte[] winningTeamIds)
    {
        _mode = mode;
        _durationTicks = durationTicks;
        _teams = teams;
        _winningTeamIds = winningTeamIds;
    }

    /// <summary>
    /// Series format that the server should use for the tournament.
    /// </summary>
    public TournamentMode Mode
    {
        get { return _mode; }
    }

    /// <summary>
    /// Planned duration of each tournament match in server ticks.
    /// </summary>
    public uint DurationTicks
    {
        get { return _durationTicks; }
    }

    /// <summary>
    /// Tournament teams and their participant account ids in connector-side form.
    /// </summary>
    public IReadOnlyList<TournamentTeamConfiguration> Teams
    {
        get { return _teams; }
    }

    /// <summary>
    /// Ordered winner-team ids for already completed matches.
    /// </summary>
    public IReadOnlyList<byte> WinningTeamIds
    {
        get { return _winningTeamIds; }
    }
}
