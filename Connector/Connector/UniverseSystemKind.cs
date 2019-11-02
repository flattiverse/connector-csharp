using System;

namespace Flattiverse
{
    /// <summary>
    /// The kind of a universe system
    /// </summary>
    public enum UniverseSystemKind
    {
        /// <summary>
        /// The hull. This system is your ships hull. A higher level indicates more hull points.
        /// </summary>
        Hull = 0x00,
        /// <summary>
        /// The armor of your ship. A higher level indicates more effective hitpoints for your ship.
        /// </summary>
        Armor,
        // Shield,
        // RadiationShield,
        /// <summary>
        /// Your primary scanner. (Short range scanner.)
        /// </summary>
        Scanner0 = 0x08,
        /// <summary>
        /// Your secondary scanner. (Long range scanner.)
        /// </summary>
        Scanner1,
        /// <summary>
        /// Your ships engine. Higher level equals more acceleration.
        /// </summary>
        Engine = 0x10,
        /// <summary>
        /// Your ships thruster jets. Higher level equals more turn-rate acceleration.
        /// </summary>
        Thruster,
        /// <summary>
        /// You ships solar cells. A higher level means you can harvest pro energy from sunrays.
        /// </summary>
        Cell = 0x18,

        // Here resources shit is starting.

        /// <summary>
        /// The battery of your ship which indeicates the storage capacity of energy of your ship.
        /// </summary>
        Battery = 0xC0
    }
}