namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Session-level runtime automation aspects.
/// </summary>
public enum RuntimeDisclosureAspect : byte
{
    /// <summary>
    /// Thrust or engine-control decisions.
    /// </summary>
    EngineControl = 0,

    /// <summary>
    /// Pathing and movement-target selection.
    /// </summary>
    Navigation = 1,

    /// <summary>
    /// Scanner activation and scan-shape handling.
    /// </summary>
    ScannerControl = 2,

    /// <summary>
    /// Ballistic setup and aiming.
    /// </summary>
    WeaponAiming = 3,

    /// <summary>
    /// Weapon target selection.
    /// </summary>
    WeaponTargetSelection = 4,

    /// <summary>
    /// Runtime resource management such as energy allocation.
    /// </summary>
    ResourceControl = 5,

    /// <summary>
    /// Multi-ship fleet coordination.
    /// </summary>
    FleetControl = 6,

    /// <summary>
    /// Mission selection and assignment.
    /// </summary>
    MissionControl = 7,

    /// <summary>
    /// Loadout and subsystem setup choices.
    /// </summary>
    LoadoutControl = 8,

    /// <summary>
    /// Chat behavior.
    /// </summary>
    Chat = 9,
}
