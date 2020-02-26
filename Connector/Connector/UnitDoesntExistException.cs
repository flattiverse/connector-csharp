using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// Thrown, when you request an action on a unit, which just doesn't exist (or is not available) in this context.
    /// </summary>
    public class UnitDoesntExistException : ArgumentException
    {
        internal UnitDoesntExistException() : base ("The specified unit doesn't exist or is not available in this context.")
        {
        }
    }
}
