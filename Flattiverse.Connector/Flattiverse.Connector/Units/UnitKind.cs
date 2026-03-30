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
    /// Represents a current field that induces movement on mobile units.
    /// </summary>
    CurrentField = 0x02,
    /// <summary>
    /// Represents a nebula.
    /// </summary>
    Nebula = 0x03,
    /// <summary>
    /// Represents a storm source that periodically spawns whirls.
    /// </summary>
    Storm = 0x20,
    /// <summary>
    /// Represents a storm whirl that is still announcing itself.
    /// </summary>
    StormCommencingWhirl = 0x21,
    /// <summary>
    /// Represents an active storm whirl.
    /// </summary>
    StormActiveWhirl = 0x22,
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
    /// Represents a worm hole.
    /// </summary>
    WormHole = 0x11,
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
    /// Represents an energy charge power-up.
    /// </summary>
    EnergyChargePowerUp = 0x70,
    /// <summary>
    /// Represents an ion charge power-up.
    /// </summary>
    IonChargePowerUp = 0x71,
    /// <summary>
    /// Represents a neutrino charge power-up.
    /// </summary>
    NeutrinoChargePowerUp = 0x72,
    /// <summary>
    /// Represents a metal cargo power-up.
    /// </summary>
    MetalCargoPowerUp = 0x73,
    /// <summary>
    /// Represents a carbon cargo power-up.
    /// </summary>
    CarbonCargoPowerUp = 0x74,
    /// <summary>
    /// Represents a hydrogen cargo power-up.
    /// </summary>
    HydrogenCargoPowerUp = 0x75,
    /// <summary>
    /// Represents a silicon cargo power-up.
    /// </summary>
    SiliconCargoPowerUp = 0x76,
    /// <summary>
    /// Represents a shield charge power-up.
    /// </summary>
    ShieldChargePowerUp = 0x77,
    /// <summary>
    /// Represents a hull repair power-up.
    /// </summary>
    HullRepairPowerUp = 0x78,
    /// <summary>
    /// Represents a shot charge power-up.
    /// </summary>
    ShotChargePowerUp = 0x79,
    /// <summary>
    /// Represents a switch that can affect gates.
    /// </summary>
    Switch = 0x60,
    /// <summary>
    /// Represents a gate that can open and close.
    /// </summary>
    Gate = 0x61,
    /// <summary>
    /// Represents a shot.
    /// </summary>
    Shot = 0xE0,
    /// <summary>
    /// Represents an interceptor projectile.
    /// </summary>
    Interceptor = 0xE1,
    /// <summary>
    /// Represents a rail projectile.
    /// </summary>
    Rail = 0xE2,
    /// <summary>
    /// Represents a classical player ship.
    /// </summary>
    ClassicShipPlayerUnit = 0xF0,
    /// <summary>
    /// Represents a new style player ship.
    /// </summary>
    NewShipPlayerUnit = 0xF1,
    /// <summary>
    /// Represents an interceptor explosion.
    /// </summary>
    InterceptorExplosion = 0xFE,
    /// <summary>
    /// Explosion unit.
    /// </summary>
    Explosion = 0xFF,
}
