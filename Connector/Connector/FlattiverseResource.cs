using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// A resource which can be found on planets, moons or meteoroids.
    /// </summary>
    public enum FlattiverseResource
    {
        /// <summary>
        /// No resource.
        /// </summary>
        None = 0x00,
        /// <summary>
        /// Iron. A base resource needed almost for everything.
        /// </summary>
        Iron,
        /// <summary>
        /// Copper.
        /// </summary>
        Copper,
        /// <summary>
        /// Gold. This yellow shiny thing.
        /// </summary>
        Gold,
        /// <summary>
        /// Platinum.
        /// </summary>
        Platinum,
        /// <summary>
        /// Lithium.
        /// </summary>
        Lithium,
        /// <summary>
        /// Silicon.
        /// </summary>
        Silicon
    }
}
