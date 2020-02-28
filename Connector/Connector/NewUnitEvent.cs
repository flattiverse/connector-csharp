using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// An event indicating that a new unit enetered your scan horizon.
    /// </summary>
    public class NewUnitEvent : UnitEvent
    {
        internal NewUnitEvent(Unit unit) : base(unit)
        {
        }

        /// <summary>
        /// The string representation of this object.
        /// </summary>
        /// <returns>The String.</returns>
        public override string ToString()
        {
            return $"New Unit: {Unit}";
        }
    }
}
