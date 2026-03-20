namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// The account already has an active session in a galaxy.
/// </summary>
public class AccountAlreadyLoggedInGameException : GameException
{
    internal AccountAlreadyLoggedInGameException() : base(0x09, "[0x09] Account already has an active galaxy session.")
    {
    }
}
