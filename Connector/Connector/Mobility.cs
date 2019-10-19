using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// Specifies the mobility of an unit.
    /// </summary>
    public enum Mobility
    {
        /// <summary>
        /// The unit is still and will reside forever on it's location.
        /// </summary>
        Still,
        /// <summary>
        /// The unit obeys a steady course, not affected by gravity.
        /// </summary>
        Steady,
        /// <summary>
        /// The unit is mobile and therefore affected by gravity, etc.
        /// </summary>
        Mobile
    }
}
