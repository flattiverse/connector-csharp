namespace Flattiverse.Connector;

/// <summary>
/// Identifies the concrete subsystem slot within a controllable.
/// </summary>
public enum SubsystemSlot : byte
{
    /// <summary>
    /// Primary energy battery slot.
    /// </summary>
    EnergyBattery = 0x00,

    /// <summary>
    /// Primary ion battery slot.
    /// </summary>
    IonBattery = 0x01,

    /// <summary>
    /// Primary neutrino battery slot.
    /// </summary>
    NeutrinoBattery = 0x02,

    /// <summary>
    /// Primary energy-cell slot.
    /// </summary>
    EnergyCell = 0x10,

    /// <summary>
    /// Primary ion-cell slot.
    /// </summary>
    IonCell = 0x11,

    /// <summary>
    /// Primary neutrino-cell slot.
    /// </summary>
    NeutrinoCell = 0x12,

    /// <summary>
    /// Hull integrity slot.
    /// </summary>
    Hull = 0x18,

    /// <summary>
    /// Shield integrity slot.
    /// </summary>
    Shield = 0x19,

    /// <summary>
    /// Armor integrity slot.
    /// </summary>
    Armor = 0x1A,

    /// <summary>
    /// Hull repair slot.
    /// </summary>
    Repair = 0x1B,

    /// <summary>
    /// Cargo slot.
    /// </summary>
    Cargo = 0x50,

    /// <summary>
    /// Resource miner slot.
    /// </summary>
    ResourceMiner = 0x51,

    /// <summary>
    /// Nebula collector slot.
    /// </summary>
    NebulaCollector = 0x52,

    /// <summary>
    /// Primary scanner slot.
    /// </summary>
    PrimaryScanner = 0x20,

    /// <summary>
    /// Secondary scanner slot.
    /// </summary>
    SecondaryScanner = 0x21,

    /// <summary>
    /// Tertiary scanner slot.
    /// </summary>
    TertiaryScanner = 0x22,

    /// <summary>
    /// Primary engine slot.
    /// </summary>
    PrimaryEngine = 0x30,

    /// <summary>
    /// Secondary engine slot.
    /// </summary>
    SecondaryEngine = 0x31,

    /// <summary>
    /// Tertiary engine slot.
    /// </summary>
    TertiaryEngine = 0x32,

    /// <summary>
    /// Jump-drive slot.
    /// </summary>
    JumpDrive = 0x33,

    /// <summary>
    /// Dynamic shot launcher slot.
    /// </summary>
    DynamicShotLauncher = 0x40,

    /// <summary>
    /// Dynamic shot magazine slot.
    /// </summary>
    DynamicShotMagazine = 0x41,

    /// <summary>
    /// Dynamic shot fabricator slot.
    /// </summary>
    DynamicShotFabricator = 0x42,

    /// <summary>
    /// Dynamic interceptor launcher slot.
    /// </summary>
    DynamicInterceptorLauncher = 0x43,

    /// <summary>
    /// Dynamic interceptor magazine slot.
    /// </summary>
    DynamicInterceptorMagazine = 0x44,

    /// <summary>
    /// Dynamic interceptor fabricator slot.
    /// </summary>
    DynamicInterceptorFabricator = 0x45,

    /// <summary>
    /// Railgun slot.
    /// </summary>
    Railgun = 0x46,
}
