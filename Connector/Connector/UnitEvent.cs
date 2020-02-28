using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// An event indicating that something happend to a unit.
    /// </summary>
    public class UnitEvent : FlattiverseEvent
    {
        /// <summary>
        /// The unit which is new.
        /// </summary>
        public readonly Unit Unit;

        /// <summary>
        /// This specifies the kind of this event.
        /// </summary>
        public override FlattiverseEventKind Kind => FlattiverseEventKind.Scan;

        internal UnitEvent(Unit unit)
        {
            Unit = unit;
        }
    }
}
