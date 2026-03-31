namespace Flattiverse.Connector.Events;

/// <summary>
/// Connector-side classification of <see cref="FlattiverseEvent" /> types.
/// These values are meant for application-side dispatch and do not directly mirror wire-protocol packet opcodes.
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
    /// A player has been removed from the local galaxy mirror.
    /// </summary>
    PlayerParted = 0x11,

    /// <summary>
    /// A player score has been updated.
    /// </summary>
    PlayerScoreUpdated = 0x12,

    /// <summary>
    /// A player connection has disconnected while cleanup is still pending.
    /// </summary>
    PlayerDisconnected = 0x13,
    
    /// <summary>
    /// A public controllable-registration entry became known.
    /// </summary>
    ControllableInfoRegistered = 0x20,
    
    /// <summary>
    /// A public controllable-registration entry became alive in the world.
    /// </summary>
    ControllableInfoContinued = 0x21,
    
    /// <summary>
    /// A public controllable-registration entry died.
    /// </summary>
    ControllableInfoDestroyed = 0x22,
    
    /// <summary>
    /// The score of a public controllable-registration entry has changed.
    /// </summary>
    ControllableInfoScoreUpdated = 0x25,

    /// <summary>
    /// A public controllable-registration entry has been finally closed and removed.
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
    /// Runtime update of a dynamic scanner subsystem on your own controllable.
    /// </summary>
    DynamicScannerSubsystem = 0x82,

    /// <summary>
    /// Runtime update of an engine subsystem on your own controllable.
    /// </summary>
    ClassicShipEngineSubsystem = 0x83,

    /// <summary>
    /// Runtime update of a dynamic shot launcher subsystem on your own controllable.
    /// </summary>
    DynamicShotLauncherSubsystem = 0x84,

    /// <summary>
    /// Runtime update of a hull subsystem on your own controllable.
    /// </summary>
    HullSubsystem = 0x85,

    /// <summary>
    /// Runtime update of a dynamic shot magazine subsystem on your own controllable.
    /// </summary>
    DynamicShotMagazineSubsystem = 0x86,

    /// <summary>
    /// Runtime update of a dynamic shot fabricator subsystem on your own controllable.
    /// </summary>
    DynamicShotFabricatorSubsystem = 0x87,

    /// <summary>
    /// Runtime update of a shield subsystem on your own controllable.
    /// </summary>
    ShieldSubsystem = 0x88,

    /// <summary>
    /// Runtime update of an armor subsystem on your own controllable.
    /// </summary>
    ArmorSubsystem = 0x89,

    /// <summary>
    /// Runtime update of a repair subsystem on your own controllable.
    /// </summary>
    RepairSubsystem = 0x8A,

    /// <summary>
    /// Owner-only passive heat and radiation update for the current tick.
    /// </summary>
    EnvironmentDamage = 0x8B,

    /// <summary>
    /// Runtime update of a cargo subsystem on your own controllable.
    /// </summary>
    CargoSubsystem = 0x8C,

    /// <summary>
    /// Runtime update of a resource miner subsystem on your own controllable.
    /// </summary>
    ResourceMinerSubsystem = 0x8D,

    /// <summary>
    /// Your controllable has collected a power-up.
    /// </summary>
    PowerUpCollected = 0x8E,

    /// <summary>
    /// Runtime update of a dynamic interceptor launcher subsystem on your own controllable.
    /// </summary>
    DynamicInterceptorLauncherSubsystem = 0x8F,

    /// <summary>
    /// Runtime update of a dynamic interceptor magazine subsystem on your own controllable.
    /// </summary>
    DynamicInterceptorMagazineSubsystem = 0x90,

    /// <summary>
    /// Runtime update of a dynamic interceptor fabricator subsystem on your own controllable.
    /// </summary>
    DynamicInterceptorFabricatorSubsystem = 0x91,

    /// <summary>
    /// Runtime update of a railgun subsystem on your own controllable.
    /// </summary>
    RailgunSubsystem = 0x92,

    /// <summary>
    /// Runtime update of a nebula collector subsystem on your own controllable.
    /// </summary>
    NebulaCollectorSubsystem = 0x93,

    /// <summary>
    /// Runtime update of a modern-ship engine subsystem on your own controllable.
    /// </summary>
    ModernShipEngineSubsystem = 0x94,
    
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
    /// A previously known visible unit has left the local visibility mirror.
    /// </summary>
    RemovedUnit = 0x3F,
    
    /// <summary>
    /// A galaxy-wide system chat announced that a flag has been scored.
    /// </summary>
    FlagScoredChat = 0xC1,

    /// <summary>
    /// A galaxy-wide system chat announced that a domination point has been scored.
    /// </summary>
    DominationPointScoredChat = 0xC2,

    /// <summary>
    /// A galaxy-wide system chat announced that someone hit the own flag.
    /// </summary>
    OwnFlagHitChat = 0xC3,

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
    /// A galaxy-wide system chat announced that a mission target was hit in sequence.
    /// </summary>
    MissionTargetHitChat = 0xC7,

    /// <summary>
    /// A galaxy-wide system chat announced that a flag became active again.
    /// </summary>
    FlagReactivatedChat = 0xC9,

    /// <summary>
    /// A switch changed the state of linked gates.
    /// </summary>
    GateSwitched = 0xCA,

    /// <summary>
    /// A gate restored itself to its configured default state.
    /// </summary>
    GateRestored = 0xCB,

    /// <summary>
    /// A tournament was created.
    /// </summary>
    TournamentCreated = 0xD0,

    /// <summary>
    /// A tournament was updated.
    /// </summary>
    TournamentUpdated = 0xD1,

    /// <summary>
    /// A tournament was removed.
    /// </summary>
    TournamentRemoved = 0xD2,

    /// <summary>
    /// A tournament system message was received.
    /// </summary>
    TournamentMessage = 0xD3,
    
    /// <summary>
    /// The connection has been terminated.
    /// </summary>
    ConnectionTerminated = 0xF0,
    
    /// <summary>
    /// Connector event announcing the next authoritative galaxy tick number.
    /// </summary>
    GalaxyTick = 0xFF
}
