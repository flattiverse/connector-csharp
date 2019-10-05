using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// Specifies the gamemode of a universe group.
    /// </summary>
    public enum UniverseMode
    {
        /// <summary>
        /// A mission universe group.
        /// </summary>
        Mission = 0,
        /// <summary>
        /// A shoot the flag universe group.
        /// </summary>
        STF,
        /// <summary>
        /// A domination universe group.
        /// </summary>
        Domination
    }
}
