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
        /// The shield, which is driven by plasma.
        /// </summary>
        Shield,
        /// <summary>
        /// The scanner infrastructure.
        /// </summary>
        Scanner,
        /// <summary>
        /// The energy systems: battery and solar cell.
        /// </summary>
        Energy,
        /// <summary>
        /// The cargo systems: storage of materials.
        /// </summary>
        Cargo
    }
}