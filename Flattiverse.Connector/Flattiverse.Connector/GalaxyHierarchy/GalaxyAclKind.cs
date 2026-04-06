namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Access-control role inside one galaxy ACL.
/// </summary>
public enum GalaxyAclKind
{
    /// <summary>
    /// Controls normal player logins that use the player API key.
    /// </summary>
    Player = 0x01,

    /// <summary>
    /// Controls admin logins that use the admin API key.
    /// </summary>
    Admin = 0x04
}
