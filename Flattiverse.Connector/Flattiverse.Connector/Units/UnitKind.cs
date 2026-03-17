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
    /// Represents a buoy.
    /// </summary>
    Buoy = 0x10,
    /// <summary>
    /// Represents a mission target with configurable waypoint vectors.
    /// </summary>
    MissionTarget = 0x14,
    /// <summary>
    /// Represents a flag target.
    /// </summary>
    Flag = 0x15,
    /// <summary>
    /// Represents a domination point target.
    /// </summary>
    DominationPoint = 0x16,
    /// <summary>
    /// Represents a shot.
    /// </summary>
    Shot = 0xE0,
    /// <summary>
    /// Represents a classical player ship.
    /// </summary>
    ClassicShipPlayerUnit = 0xF0,
    /// <summary>
    /// Represents a new style player ship.
    /// </summary>
    NewShipPlayerUnit = 0xF1,
    /// <summary>
    /// Represents an explosion.
    /// </summary>
    Explosion = 0xFF,
}
