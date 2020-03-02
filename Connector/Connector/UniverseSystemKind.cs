using System;

namespace Flattiverse
{
    /// <summary>
    /// The kind of a universe system
    /// </summary>
    public enum UniverseSystemKind
    {
        /// <summary>
        /// The engine of the ship. They can accelerate a ship only in the forward direction.
        /// </summary>
        Engine = 0,
        /// <summary>
        /// Thrusters. They can turn the ship around (in both directions).
        /// </summary>
        Thruster,
        /// <summary>
        /// The chassis of the ship. It generally specifies the size of the ship. (And therefore how many components, with which level can be put into the ship.)
        /// </summary>
        Chassis,
        /// <summary>
        /// The armor. Generally increasing your hit-points.
        /// </summary>
        Armor,
        /// <summary>
        /// The shield, which is driven by plasma.
        /// </summary>
        Shield,
        /// <summary>
        /// The broad (but not so far) scanner.
        /// </summary>
        ScannerBroad,
        /// <summary>
        /// The long range scanner. (Which doesn't span such a wide area.)
        /// </summary>
        ScannerLong,
        /// <summary>
        /// The energy systems: battery and solar cell.
        /// </summary>
        Energy,
        /// <summary>
        /// The plasma systems: storage, collector, refiner.
        /// </summary>
        Plasma,
        /// <summary>
        /// A torpedo launcher directed forward.
        /// </summary>
        LauncherForward,
        /// <summary>
        /// A torpedo launcher directed backwards.
        /// </summary>
        LauncherBackward,
        /// <summary>
        /// A set of reactors which will automatically produce energy, plasma and materials.
        /// </summary>
        Reactor,
        /// <summary>
        /// The materials system: storage and collector.
        /// </summary>
        Materials
    }
}