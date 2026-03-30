namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Protocol error <c>0x35</c>: thrown when ship registration or respawn is attempted while the current tournament
/// stage has closed registration.
/// </summary>
public class TournamentRegistrationClosedGameException : GameException
{
    internal TournamentRegistrationClosedGameException() : base(0x35, "[0x35] Ship registration is closed in the current tournament stage.")
    {
    }
}
