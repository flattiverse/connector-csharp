namespace Flattiverse.Connector.Events;

/// <summary>
/// Specifies the various event kinds for a better switch() experience.
/// </summary>
public enum EventKind : byte
{
    /// <summary>
    /// The galaxy settings have been updated.
    /// </summary>
    GalaxySettingsUpdated = 0x01,

    /// <summary>
    /// A team has been created.
    /// </summary>
    TeamCreated = 0x04,

    /// <summary>
    /// A team has been updated.
    /// </summary>
    TeamUpdated = 0x05,

    /// <summary>
    /// A team has been removed.
    /// </summary>
    TeamRemoved = 0x06,

    /// <summary>
    /// A team score has been updated.
    /// </summary>
    TeamScoreUpdated = 0x07,

    /// <summary>
    /// A cluster has been created.
    /// </summary>
    ClusterCreated = 0x08,

    /// <summary>
    /// A cluster has been updated.
    /// </summary>
    ClusterUpdated = 0x09,

    /// <summary>
    /// A cluster has been removed.
    /// </summary>
    ClusterRemoved = 0x0A,

    /// <summary>
    /// The server announced the compile profile it was built with.
    /// </summary>
    CompiledWithMessage = 0x0B,
    
    /// <summary>
    /// A player has joined the galaxy.
    /// </summary>
    PlayerJoined = 0x10,
    
    /// <summary>
    /// A player has parted the galaxy.
    /// </summary>
    PlayerParted = 0x11,

    /// <summary>
    /// A player score has been updated.
    /// </summary>
    PlayerScoreUpdated = 0x12,
    
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
    /// A PlayerUnit score has been updated.
    /// </summary>
    ControllableInfoScoreUpdated = 0x25,

    /// <summary>
    /// A PlayerUnit was unregistered.
    /// </summary>
    ControllableInfoClosed = 0x2F,

    /// <summary>
    /// Runtime update of a battery subsystem on your own controllable.
    /// </summary>
    BatterySubsystem = 0x80,

    /// <summary>
    /// Runtime update of an energy-cell subsystem on your own controllable.
    /// </summary>
    EnergyCellSubsystem = 0x81,

    /// <summary>
    /// Runtime update of a scanner subsystem on your own controllable.
    /// </summary>
    ScannerSubsystem = 0x82,

    /// <summary>
    /// Runtime update of an engine subsystem on your own controllable.
    /// </summary>
    ClassicShipEngineSubsystem = 0x83,

    /// <summary>
    /// Runtime update of a shot launcher subsystem on your own controllable.
    /// </summary>
    ShotWeaponSubsystem = 0x84,

    /// <summary>
    /// Runtime update of a hull subsystem on your own controllable.
    /// </summary>
    HullSubsystem = 0x85,
    
    /// <summary>
    /// You see a new unit.
    /// </summary>
    NewUnit = 0x30,
    
    /// <summary>
    /// An existing unit has been updated.
    /// </summary>
    UpdatedUnit = 0x31,

    /// <summary>
    /// A previously known unit has been altered by an admin through map editing.
    /// </summary>
    UnitAlteredByAdmin = 0x3E,
    
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
