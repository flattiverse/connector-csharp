namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// The account already has an active regular player session in a galaxy.
/// </summary>
public class AccountAlreadyLoggedInGameException : GameException
{
    internal AccountAlreadyLoggedInGameException() : base(0x09, "[0x09] Account already has an active player galaxy session.")
    {
    }
}
