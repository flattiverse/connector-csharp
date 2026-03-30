namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Protocol error <c>0x39</c>: thrown when tournament configuration is attempted in a galaxy game mode that does not
/// allow tournaments.
/// </summary>
public class TournamentModeNotAllowedGameException : GameException
{
    internal TournamentModeNotAllowedGameException() : base(0x39, "[0x39] Tournaments are not allowed for the current galaxy game mode.")
    {
    }
}
