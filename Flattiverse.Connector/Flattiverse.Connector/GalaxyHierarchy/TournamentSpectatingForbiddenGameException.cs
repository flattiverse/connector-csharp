namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Protocol error <c>0x37</c>: thrown when a spectator login is attempted in a tournament stage that forbids
/// spectating.
/// </summary>
public class TournamentSpectatingForbiddenGameException : GameException
{
    internal TournamentSpectatingForbiddenGameException() : base(0x37, "[0x37] Spectating is forbidden in the current tournament stage.")
    {
    }
}
