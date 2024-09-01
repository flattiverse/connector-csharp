namespace Flattiverse.Connector.Events;

/// <summary>
/// Specifies the various event kinds for a better switch() experience.
/// </summary>
public enum EventKind : byte
{
    /// <summary>
    /// A player has joined the galaxy.
    /// </summary>
    PlayerJoined = 0x10,
    
    /// <summary>
    /// A player has parted the galaxy.
    /// </summary>
    PlayerParted = 0x11,
    
    /// <summary>
    /// You received a galaxy chat message.
    /// </summary>
    ChatGalaxy = 0xC4,
    
    /// <summary>
    /// You received a team chat message.
    /// </summary>
    ChatTeam = 0xC5,
    
    /// <summary>
    /// You received a private message of a team member.
    /// </summary>
    ChatPlayer = 0xC6,
    
    /// <summary>
    /// The connection has been terminated.
    /// </summary>
    ConnectionTerminated = 0xF0,
    
    /// <summary>
    /// A tick happened.
    /// </summary>
    GalaxyTick = 0xFF
}