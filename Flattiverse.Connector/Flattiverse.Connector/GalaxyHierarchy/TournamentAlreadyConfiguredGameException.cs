namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Protocol error <c>0x32</c>: thrown when an admin tries to configure a tournament although one already exists.
/// </summary>
public class TournamentAlreadyConfiguredGameException : GameException
{
    internal TournamentAlreadyConfiguredGameException() : base(0x32, "[0x32] A tournament is already configured.")
    {
    }
}
