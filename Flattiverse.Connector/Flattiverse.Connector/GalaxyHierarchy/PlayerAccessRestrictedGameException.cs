namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Protocol error <c>0x3A</c>: thrown when a normal player login is denied by the galaxy player ACL.
/// </summary>
public class PlayerAccessRestrictedGameException : GameException
{
    internal PlayerAccessRestrictedGameException() : base(0x3A, "[0x3A] Player access to this galaxy is restricted by ACL.")
    {
    }
}
