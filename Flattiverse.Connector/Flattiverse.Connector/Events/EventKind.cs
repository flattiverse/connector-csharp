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
    /// A PlayerUnit has been registered.
    /// </summary>
    ControllableInfoRegistered = 0x20,
    
    /// <summary>
    /// A PlayerUnit did continue the game.
    /// </summary>
    ControllableInfoContinued = 0x21,
    
    /// <summary>
    /// A PlayerUnit was destroyed.
    /// </summary>
    ControllableInfoDestroyed = 0x22,
    
    /// <summary>
    /// A PlayerUnit was unregistered.
    /// </summary>
    ControllableInfoClosed = 0x2F,
    
    /// <summary>
    /// You see a new unit.
    /// </summary>
    NewUnit = 0x30,
    
    /// <summary>
    /// An existing unit has been updated.
    /// </summary>
    UpdatedUnit = 0x31,
    
    /// <summary>
    /// You don't see the unit anymore.
    /// </summary>
    RemovedUnit = 0x3F,
    
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