namespace Flattiverse.Connector.Units;

/// <summary>
/// The mobility types of a unit.
/// </summary>
public enum Mobility
{
    /// <summary>
    /// The unit is still and fixed in the world coordinate system.
    /// </summary>
    Still = 0,

    /// <summary>
    /// The unit is steady and has a fixed trajectory relative to the world coordinate system.
    /// </summary>
    Steady,

    /// <summary>
    /// The unit is moving and can move freely in the world coordinate system.
    /// </summary>
    Mobile
}