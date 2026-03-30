namespace Flattiverse.Connector.Units;

/// <summary>
/// Describes how a unit moves on the map.
/// </summary>
public enum Mobility
{
    /// <summary>
    /// The unit does not move.
    /// </summary>
    Still = 0x01,

    /// <summary>
    /// The unit moves with a predefined steady movement.
    /// </summary>
    Steady = 0x02,

    /// <summary>
    /// The unit can actively change its movement at runtime.
    /// </summary>
    Mobile = 0x04
}
