namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Protocol error <c>0x31</c>: thrown when a tournament-specific action is requested although no tournament is
/// currently configured.
/// </summary>
public class TournamentNotConfiguredGameException : GameException
{
    internal TournamentNotConfiguredGameException() : base(0x31, "[0x31] No tournament is configured.")
    {
    }
}
