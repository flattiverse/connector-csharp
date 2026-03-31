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
    /// Modern scanner slot at the ship nose.
    /// </summary>
    ModernScannerN = 0x23,

    /// <summary>
    /// Modern scanner slot at the ship north-east mount.
    /// </summary>
    ModernScannerNE = 0x24,

    /// <summary>
    /// Modern scanner slot at the ship east mount.
    /// </summary>
    ModernScannerE = 0x25,

    /// <summary>
    /// Modern scanner slot at the ship south-east mount.
    /// </summary>
    ModernScannerSE = 0x26,

    /// <summary>
    /// Modern scanner slot at the ship stern.
    /// </summary>
    ModernScannerS = 0x27,

    /// <summary>
    /// Modern scanner slot at the ship south-west mount.
    /// </summary>
    ModernScannerSW = 0x28,

    /// <summary>
    /// Modern scanner slot at the ship west mount.
    /// </summary>
    ModernScannerW = 0x29,

    /// <summary>
    /// Modern scanner slot at the ship north-west mount.
    /// </summary>
    ModernScannerNW = 0x2A,

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
    /// Modern engine slot at the ship nose.
    /// </summary>
    ModernEngineN = 0x34,

    /// <summary>
    /// Modern engine slot at the ship north-east mount.
    /// </summary>
    ModernEngineNE = 0x35,

    /// <summary>
    /// Modern engine slot at the ship east mount.
    /// </summary>
    ModernEngineE = 0x36,

    /// <summary>
    /// Modern engine slot at the ship south-east mount.
    /// </summary>
    ModernEngineSE = 0x37,

    /// <summary>
    /// Modern engine slot at the ship stern.
    /// </summary>
    ModernEngineS = 0x38,

    /// <summary>
    /// Modern engine slot at the ship south-west mount.
    /// </summary>
    ModernEngineSW = 0x39,

    /// <summary>
    /// Modern engine slot at the ship west mount.
    /// </summary>
    ModernEngineW = 0x3A,

    /// <summary>
    /// Modern engine slot at the ship north-west mount.
    /// </summary>
    ModernEngineNW = 0x3B,

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

    /// <summary>
    /// Static shot launcher slot at the ship nose.
    /// </summary>
    StaticShotLauncherN = 0x60,

    /// <summary>
    /// Static shot launcher slot at the ship north-east mount.
    /// </summary>
    StaticShotLauncherNE = 0x61,

    /// <summary>
    /// Static shot launcher slot at the ship east mount.
    /// </summary>
    StaticShotLauncherE = 0x62,

    /// <summary>
    /// Static shot launcher slot at the ship south-east mount.
    /// </summary>
    StaticShotLauncherSE = 0x63,

    /// <summary>
    /// Static shot launcher slot at the ship stern.
    /// </summary>
    StaticShotLauncherS = 0x64,

    /// <summary>
    /// Static shot launcher slot at the ship south-west mount.
    /// </summary>
    StaticShotLauncherSW = 0x65,

    /// <summary>
    /// Static shot launcher slot at the ship west mount.
    /// </summary>
    StaticShotLauncherW = 0x66,

    /// <summary>
    /// Static shot launcher slot at the ship north-west mount.
    /// </summary>
    StaticShotLauncherNW = 0x67,

    /// <summary>
    /// Static shot magazine slot at the ship nose.
    /// </summary>
    StaticShotMagazineN = 0x68,

    /// <summary>
    /// Static shot magazine slot at the ship north-east mount.
    /// </summary>
    StaticShotMagazineNE = 0x69,

    /// <summary>
    /// Static shot magazine slot at the ship east mount.
    /// </summary>
    StaticShotMagazineE = 0x6A,

    /// <summary>
    /// Static shot magazine slot at the ship south-east mount.
    /// </summary>
    StaticShotMagazineSE = 0x6B,

    /// <summary>
    /// Static shot magazine slot at the ship stern.
    /// </summary>
    StaticShotMagazineS = 0x6C,

    /// <summary>
    /// Static shot magazine slot at the ship south-west mount.
    /// </summary>
    StaticShotMagazineSW = 0x6D,

    /// <summary>
    /// Static shot magazine slot at the ship west mount.
    /// </summary>
    StaticShotMagazineW = 0x6E,

    /// <summary>
    /// Static shot magazine slot at the ship north-west mount.
    /// </summary>
    StaticShotMagazineNW = 0x6F,

    /// <summary>
    /// Static shot fabricator slot at the ship nose.
    /// </summary>
    StaticShotFabricatorN = 0x70,

    /// <summary>
    /// Static shot fabricator slot at the ship north-east mount.
    /// </summary>
    StaticShotFabricatorNE = 0x71,

    /// <summary>
    /// Static shot fabricator slot at the ship east mount.
    /// </summary>
    StaticShotFabricatorE = 0x72,

    /// <summary>
    /// Static shot fabricator slot at the ship south-east mount.
    /// </summary>
    StaticShotFabricatorSE = 0x73,

    /// <summary>
    /// Static shot fabricator slot at the ship stern.
    /// </summary>
    StaticShotFabricatorS = 0x74,

    /// <summary>
    /// Static shot fabricator slot at the ship south-west mount.
    /// </summary>
    StaticShotFabricatorSW = 0x75,

    /// <summary>
    /// Static shot fabricator slot at the ship west mount.
    /// </summary>
    StaticShotFabricatorW = 0x76,

    /// <summary>
    /// Static shot fabricator slot at the ship north-west mount.
    /// </summary>
    StaticShotFabricatorNW = 0x77,

    /// <summary>
    /// Static interceptor launcher slot at the ship east mount.
    /// </summary>
    StaticInterceptorLauncherE = 0x78,

    /// <summary>
    /// Static interceptor launcher slot at the ship west mount.
    /// </summary>
    StaticInterceptorLauncherW = 0x79,

    /// <summary>
    /// Static interceptor magazine slot at the ship east mount.
    /// </summary>
    StaticInterceptorMagazineE = 0x7A,

    /// <summary>
    /// Static interceptor magazine slot at the ship west mount.
    /// </summary>
    StaticInterceptorMagazineW = 0x7B,

    /// <summary>
    /// Static interceptor fabricator slot at the ship east mount.
    /// </summary>
    StaticInterceptorFabricatorE = 0x7C,

    /// <summary>
    /// Static interceptor fabricator slot at the ship west mount.
    /// </summary>
    StaticInterceptorFabricatorW = 0x7D,

    /// <summary>
    /// Modern railgun slot at the ship nose.
    /// </summary>
    ModernRailgunN = 0x80,

    /// <summary>
    /// Modern railgun slot at the ship north-east mount.
    /// </summary>
    ModernRailgunNE = 0x81,

    /// <summary>
    /// Modern railgun slot at the ship east mount.
    /// </summary>
    ModernRailgunE = 0x82,

    /// <summary>
    /// Modern railgun slot at the ship south-east mount.
    /// </summary>
    ModernRailgunSE = 0x83,

    /// <summary>
    /// Modern railgun slot at the ship stern.
    /// </summary>
    ModernRailgunS = 0x84,

    /// <summary>
    /// Modern railgun slot at the ship south-west mount.
    /// </summary>
    ModernRailgunSW = 0x85,

    /// <summary>
    /// Modern railgun slot at the ship west mount.
    /// </summary>
    ModernRailgunW = 0x86,

    /// <summary>
    /// Modern railgun slot at the ship north-west mount.
    /// </summary>
    ModernRailgunNW = 0x87,
}
