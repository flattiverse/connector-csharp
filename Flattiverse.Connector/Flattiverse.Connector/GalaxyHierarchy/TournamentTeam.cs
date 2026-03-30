using TournamentAccount = Flattiverse.Connector.Account.Account;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Snapshot of one team inside a configured tournament.
/// </summary>
public class TournamentTeam
{
    private readonly Team _team;
    private readonly TournamentAccount[] _participants;
    private readonly int _wins;

    internal TournamentTeam(Team team, TournamentAccount[] participants, int wins)
    {
        _team = team;
        _participants = participants;
        _wins = wins;
    }

    /// <summary>
    /// The normal galaxy team that participates in the tournament.
    /// </summary>
    public Team Team
    {
        get { return _team; }
    }

    /// <summary>
    /// Accounts assigned to this tournament team.
    /// </summary>
    public IReadOnlyList<TournamentAccount> Participants
    {
        get { return _participants; }
    }

    /// <summary>
    /// Number of matches already won by this team in the mirrored match history.
    /// </summary>
    public int Wins
    {
        get { return _wins; }
    }
}
