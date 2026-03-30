namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Direction used for railgun firing.
/// </summary>
public enum RailgunDirection : byte
{
    /// <summary>
    /// No direction was processed in the current tick.
    /// </summary>
    None = 0x00,

    /// <summary>
    /// Fire along the current ship angle.
    /// </summary>
    Front = 0x01,

    /// <summary>
    /// Fire opposite to the current ship angle.
    /// </summary>
    Back = 0x02,
}
