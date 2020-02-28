using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// The 6 stages of plasma refinement. Cast to an int to get the level.
    /// </summary>
    public enum Plasma
    {
        /// <summary>
        /// Plasma level 1 - the "normal" plasma.
        /// </summary>
        Red = 1,
        /// <summary>
        /// Plasma level 2 - Refinement step 1.
        /// </summary>
        Orange,
        /// <summary>
        /// Plasma level 3 - Refinement step 2.
        /// </summary>
        Yellow,
        /// <summary>
        /// Plasma level 4 - Refinement step 3.
        /// </summary>
        Green,
        /// <summary>
        /// Plasma level 5 - Refinement step 4.
        /// </summary>
        Blue,
        /// <summary>
        /// Plasma level 6 - Refinement step 5.
        /// </summary>
        Magenta
    }
}
