using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// Privileges an account may have by default or may have assigned in special in an universe.
    /// </summary>
    public enum Privileges
    {
        /// <summary>
        /// No privileges (=banned from universe).
        /// </summary>
        Nothing = 0,
        /// <summary>
        /// Can join and use the universe as a player. If you want to create a pure administrator don't select this but other roles.
        /// </summary>
        Join = 1,
        /// <summary>
        /// Can manage units (create, alter and delete).
        /// </summary>
        ManageUnits = 2,
        /// <summary>
        /// Can manage regions (start regions, spawn regions, etc.)
        /// </summary>
        ManageRegions = 4,
        /// <summary>
        /// Can manage systems. You also need this privilege to make inter system wormholes when editing units.
        /// </summary>
        ManageSystems = 8,
        /// <summary>
        /// Can manage the universe itself. e.g. change name, decription, game type, etc.
        /// </summary>
        ManageUniverse = 16
    }
}
