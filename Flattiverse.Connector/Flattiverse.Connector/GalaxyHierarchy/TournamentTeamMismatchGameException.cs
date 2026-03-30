namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Protocol error <c>0x38</c>: thrown when the chosen login or controllable team conflicts with the account's
/// configured tournament team.
/// </summary>
public class TournamentTeamMismatchGameException : GameException
{
    internal TournamentTeamMismatchGameException() : base(0x38, "[0x38] This account is assigned to a different tournament team.")
    {
    }
}
