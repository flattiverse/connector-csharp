namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Protocol error <c>0x3B</c>: thrown when an admin login is denied by the galaxy admin ACL.
/// </summary>
public class AdminAccessRestrictedGameException : GameException
{
    internal AdminAccessRestrictedGameException() : base(0x3B, "[0x3B] Admin access to this galaxy is restricted by ACL.")
    {
    }
}
