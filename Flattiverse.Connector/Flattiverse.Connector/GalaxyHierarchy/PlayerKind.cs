namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Specifies the kind of the client connected to the server.
/// </summary>
public enum PlayerKind
{
    /// <summary>
    /// It is a regular player which can register ships, etc.
    /// </summary>
    Player = 0x01,
    /// <summary>
    /// It's a spectator.
    /// </summary>
    Spectator = 0x02,
    /// <summary>
    /// It's an admin.
    /// </summary>
    Admin = 0x04
}