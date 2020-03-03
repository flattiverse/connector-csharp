using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse.Events
{
    /// <summary>
    /// An event indicating that a unit in your scan horizon has been updated.
    /// </summary>
    public class UpdatedUnitEvent : UnitEvent
    {
        internal UpdatedUnitEvent(Units.Unit unit) : base(unit)
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

        public override FlattiverseEventKind Kind => FlattiverseEventKind.UpdatedUnit;
    }
}
