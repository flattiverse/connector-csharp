using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// A resource which can be found on planets, moons or meteoroids.
    /// </summary>
    public enum FlattiverseResourceKind
    {
        /// <summary>
        /// Plasma Level 1 which can be gathered at a sun.
        /// </summary>
        PlasmaRed = 0x00,
        /// <summary>
        /// Plasma Level 2 which can be gathered at a sun.
        /// </summary>
        PlasmaOrange,
        /// <summary>
        /// Plasma Level 3 which can be gathered at a sun.
        /// </summary>
        PlasmaYellow,
        /// <summary>
        /// Plasma Level 4 which can be gathered at a sun.
        /// </summary>
        PlasmaGreen,
        /// <summary>
        /// Plasma Level 5 which can be gathered at a sun.
        /// </summary>
        PlasmaCyan,
        /// <summary>
        /// Plasma Level 6 which can be gathered at a sun.
        /// </summary>
        PlasmaBlue,
        /// <summary>
        /// Plasma Level 7 which can be gathered at a sun.
        /// </summary>
        PlasmaMagenta,
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
        Silicon,
        /// <summary>
        /// A very seldom resource only found at black holes.
        /// </summary>
        SpaceCrystal,
        /// <summary>
        /// Ammunition Level 1.
        /// </summary>
        AmmunitionRed,
        /// <summary>
        /// Ammunition Level 2.
        /// </summary>
        AmmunitionOrange,
        /// <summary>
        /// Ammunition Level 3.
        /// </summary>
        AmmunitionYellow,
        /// <summary>
        /// Ammunition Level 4.
        /// </summary>
        AmmunitionGreen,
        /// <summary>
        /// Ammunition Level 5.
        /// </summary>
        AmmunitionCyan,
        /// <summary>
        /// Ammunition Level 6.
        /// </summary>
        AmmunitionBlue,
        /// <summary>
        /// Ammunition Level 7.
        /// </summary>
        AmmunitionMagenta,
        /// <summary>
        /// No resource.
        /// </summary>
        None
    }
}
