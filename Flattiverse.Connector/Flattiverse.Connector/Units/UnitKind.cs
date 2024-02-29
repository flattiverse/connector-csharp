namespace Flattiverse.Connector.Units;

/// <summary>
/// The different kinds of celestial bodies.
/// </summary>
public enum UnitKind
{
    /// <summary>
    /// A sun. Suns have sections which are central to the sun's position.
    /// These sections give energy, ions or both to players.
    /// </summary>
    Sun = 0x00,

    /// <summary>
    /// Black holes have a strong gravitational pull.
    /// </summary>
    /// <remarks>
    /// They can be used to change the direction of your units without using energy.
    /// </remarks>
    BlackHole = 0x01,

    /// <summary>
    /// A planet. A harvestable unit that can be mined for resources.
    /// The biggest harvestable unit.
    /// </summary>
    Planet = 0x04,

    /// <summary>
    /// A moon. A harvestable unit that can be mined for resources.
    /// Smaller than planets but bigger than meteoroids.
    /// </summary>
    Moon = 0x05,

     /// <summary>
    /// A meteoroid. A harvestable unit that can be mined for resources.
    /// Smaller than planets and moons.
    /// </summary>
    Meteoroid = 0x06,

    /// <summary>
    /// Cosmetic unit that can be used to mark locations in the universe.
    /// </summary>
    Buoy = 0x10,

    /// <summary>
    /// Player units are controlled by other players.
    /// Can be either firend or enemy based on team affilitaion.
    /// </summary>
    PlayerUnit = 0xF0
}