using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// An event indicating that a unit in your scan horizon has been updated.
    /// </summary>
    public class UpdatedUnitEvent : UnitEvent
    {
        internal UpdatedUnitEvent(Unit unit) : base(unit)
        {
        }

        /// <summary>
        /// The string representation of this object.
        /// </summary>
        /// <returns>The String.</returns>
        public override string ToString()
        {
            return $"Updated Unit: {Unit}";
        }
    }
}
