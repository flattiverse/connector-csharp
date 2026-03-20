namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Session-level build/disclosure aspects.
/// </summary>
public enum BuildDisclosureAspect : byte
{
    /// <summary>
    /// Software architecture and design work.
    /// </summary>
    SoftwareDesign = 0,

    /// <summary>
    /// User interface work.
    /// </summary>
    UI = 1,

    /// <summary>
    /// Universe and scene rendering.
    /// </summary>
    UniverseRendering = 2,

    /// <summary>
    /// Input handling.
    /// </summary>
    Input = 3,

    /// <summary>
    /// Engine-control implementation work.
    /// </summary>
    EngineControl = 4,

    /// <summary>
    /// Navigation implementation work.
    /// </summary>
    Navigation = 5,

    /// <summary>
    /// Scanner-control implementation work.
    /// </summary>
    ScannerControl = 6,

    /// <summary>
    /// Weapon-system implementation work.
    /// </summary>
    WeaponSystems = 7,

    /// <summary>
    /// Resource-control implementation work.
    /// </summary>
    ResourceControl = 8,

    /// <summary>
    /// Fleet-control implementation work.
    /// </summary>
    FleetControl = 9,

    /// <summary>
    /// Mission-control implementation work.
    /// </summary>
    MissionControl = 10,

    /// <summary>
    /// Chat implementation work.
    /// </summary>
    Chat = 11,
}
