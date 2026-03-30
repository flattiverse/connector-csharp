namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Result entry for one already finished tournament match.
/// </summary>
public class TournamentMatchResult
{
    private readonly int _matchNumber;
    private readonly Team _winningTeam;

    internal TournamentMatchResult(int matchNumber, Team winningTeam)
    {
        _matchNumber = matchNumber;
        _winningTeam = winningTeam;
    }

    /// <summary>
    /// One-based match number within the tournament series.
    /// </summary>
    public int MatchNumber
    {
        get { return _matchNumber; }
    }

    /// <summary>
    /// Team that won the referenced match.
    /// </summary>
    public Team WinningTeam
    {
        get { return _winningTeam; }
    }
}
