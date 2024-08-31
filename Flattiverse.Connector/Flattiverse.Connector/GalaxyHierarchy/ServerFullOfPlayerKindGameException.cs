namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Thrown, if the galaxy is full of the specified PlayerKind.
/// </summary>
public class ServerFullOfPlayerKindGameException : GameException
{
    /// <summary>
    /// The PlayerKind of which the galaxy is full.
    /// </summary>
    public readonly PlayerKind PlayerKind;
    
    internal ServerFullOfPlayerKindGameException(PlayerKind kind) : base(0x08, ErrorMessage(kind))
    {
        PlayerKind = kind;
    }

    private static string ErrorMessage(PlayerKind kind)
    {
        switch (kind)
        {
            case PlayerKind.Admin:
                return "[0x08] Server is full of admins. (Too many admins connected to the galaxy server.)";
            case PlayerKind.Spectator:
                return "[0x08] Server is full of spectators. (Too many spectators connected to the galaxy server.)";
            case PlayerKind.Player:
                return "[0x08] All player slots are taken. Please wait until players leave the galaxy.";
            default:
                return $"[0x08] Server is full of things with code 0x{(int)kind:X02}.";
        }
    }
}