namespace Flattiverse.Connector.Units;

/// <summary>
/// Specifies of which kind a unit is. 
/// </summary>
public enum UnitKind : byte
{
    /// <summary>
    /// Represents a sun.
    /// </summary>
    Sun = 0x00,
    /// <summary>
    /// Represents a black hole.
    /// </summary>
    BlackHole = 0x01,
    /// <summary>
    /// Represents a planet.
    /// </summary>
    Planet = 0x08,
    /// <summary>
    /// Represents a moon.
    /// </summary>
    Moon = 0x09,
    /// <summary>
    /// Represents a meteoroid.
    /// </summary>
    Meteoroid = 0x0A,
    /// <summary>
    /// Represents a classical player ship.
    /// </summary>
    ClassicalShipPlayerUnit = 0xF0,
    /// <summary>
    /// Represents a new style player ship.
    /// </summary>
    NewShipPlayerUnit = 0xF1
}