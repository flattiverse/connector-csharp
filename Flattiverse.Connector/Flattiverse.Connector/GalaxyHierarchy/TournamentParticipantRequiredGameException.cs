namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Protocol error <c>0x36</c>: thrown when a login, registration, or respawn requires tournament participation but the
/// current account is not part of the configured tournament lists.
/// </summary>
public class TournamentParticipantRequiredGameException : GameException
{
    internal TournamentParticipantRequiredGameException() : base(0x36, "[0x36] This account does not participate in the configured tournament.")
    {
    }
}
