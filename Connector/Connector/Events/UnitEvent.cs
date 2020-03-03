using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse.Events
{
    /// <summary>
    /// An event indicating that something happend to a unit.
    /// </summary>
    public class UnitEvent : FlattiverseEvent
    {
        /// <summary>
        /// The unit which is new.
        /// </summary>
        public readonly Units.Unit Unit;

        /// <summary>
        /// This specifies the kind of this event.
        /// </summary>
        public override FlattiverseEventGroup Group => FlattiverseEventGroup.Scan;

        internal UnitEvent(Units.Unit unit)
        {
            Unit = unit;
        }
    }
}
