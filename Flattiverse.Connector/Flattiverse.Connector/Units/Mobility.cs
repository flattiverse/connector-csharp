namespace Flattiverse.Connector.Units;

/// <summary>
/// Specified the mobility of an unit.
/// </summary>
public enum Mobility
{
    /// <summary>
    /// The unit doesn't move at all.
    /// </summary>
    Still = 0x00,
    /// <summary>
    /// The unit has a steady movement.
    /// </summary>
    Steady = 0x01,
    /// <summary>
    /// The unit is mobile.
    /// </summary>
    Mobile = 0x02
}