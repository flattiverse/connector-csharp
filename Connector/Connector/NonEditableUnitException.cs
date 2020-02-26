using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// Thrown, if you try to alter an unit which can't be altered.
    /// </summary>
    public class NonEditableUnitException : ArgumentException
    {
        internal NonEditableUnitException() : base ("The unit you specified to update can't be altered: Maybe it's a player unit, shot or some other active and dynamically generated unit.")
        {
        }
    }
}
