using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse.Events
{
    /// <summary>
    /// An event indicating that a new unit entered your scan horizon.
    /// </summary>
    public class NewUnitEvent : UnitEvent
    {
        internal NewUnitEvent(Units.Unit unit) : base(unit)
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

        /// <summary>
        /// The kind of the event.
        /// </summary>
        public override FlattiverseEventKind Kind => FlattiverseEventKind.NewUnit;
    }
}
