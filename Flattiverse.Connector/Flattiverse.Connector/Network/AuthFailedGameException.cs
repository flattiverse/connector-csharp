namespace Flattiverse.Connector.Network;

/// <summary>
/// Thrown upon connect if you presented a wrong API key.
/// </summary>
public class AuthFailedGameException : GameException
{
    internal AuthFailedGameException() : base(0x03, "[0x03] Authentication failed: Missing, wrong or unused API key.")
    {
    }
}