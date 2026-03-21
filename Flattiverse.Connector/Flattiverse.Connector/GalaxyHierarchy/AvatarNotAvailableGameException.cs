namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// The requested player currently has no avatar available.
/// </summary>
public class AvatarNotAvailableGameException : GameException
{
    internal AvatarNotAvailableGameException() : base(0x18, "[0x18] This player has no avatar.")
    {
    }
}
