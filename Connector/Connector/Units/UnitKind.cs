using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse.Units
{
    /// <summary>
    /// Unit kinds.
    /// </summary>
    public enum UnitKind
    {
        /// <summary>
        /// A PlayerUnit.
        /// </summary>
        PlayerUnit = 0x01,
        /// <summary>
        /// A shot.
        /// </summary>
        Shot = 0x02,
        /// <summary>
        /// A target.
        /// </summary>
        Target = 0x04,
        /// <summary>
        /// A sun.
        /// </summary>
        Sun = 0x08,
        /// <summary>
        /// A planet.
        /// </summary>
        Planet = 0x10,
        /// <summary>
        /// A moon.
        /// </summary>
        Moon = 0x11,
        /// <summary>
        /// A meteoroid.
        /// </summary>
        Meteoroid = 0x12,
        /// <summary>
        /// A buoy.
        /// </summary>
        Buoy = 0x20
    }
}
