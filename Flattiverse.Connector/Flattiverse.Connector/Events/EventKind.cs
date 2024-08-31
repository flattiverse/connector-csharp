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
    /// The connection has been terminated.
    /// </summary>
    ConnectionTerminated = 0xF0,
    
    /// <summary>
    /// A tick happened.
    /// </summary>
    GalaxyTick = 0xFF
}