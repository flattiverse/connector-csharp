namespace Flattiverse.Connector.Units;

/// <summary>
/// Specified the mobility of a unit.
/// </summary>
public enum Mobility
{
    /// <summary>
    /// The unit doesn't move at all.
    /// </summary>
    Still = 0x01,
    /// <summary>
    /// The unit has a steady movement.
    /// </summary>
    Steady = 0x02,
    /// <summary>
    /// The unit is mobile.
    /// </summary>
    Mobile = 0x04
}