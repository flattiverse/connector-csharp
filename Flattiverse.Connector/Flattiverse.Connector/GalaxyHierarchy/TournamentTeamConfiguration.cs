namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Connector-side configuration entry for one tournament team.
/// </summary>
public class TournamentTeamConfiguration
{
    private readonly Team _team;
    private readonly int[] _accountIds;

    /// <summary>
    /// Creates one tournament-team configuration entry.
    /// </summary>
    /// <param name="team">Galaxy team that should participate in the tournament.</param>
    /// <param name="accountIds">Persistent account ids assigned to that team.</param>
    public TournamentTeamConfiguration(Team team, int[] accountIds)
    {
        _team = team;
        _accountIds = accountIds;
    }

    /// <summary>
    /// Galaxy team that should be used as tournament side.
    /// </summary>
    public Team Team
    {
        get { return _team; }
    }

    /// <summary>
    /// Persistent account ids assigned to <see cref="Team" />.
    /// </summary>
    public IReadOnlyList<int> AccountIds
    {
        get { return _accountIds; }
    }
}
