using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// An event indicating that an unit left your scan horizon.
    /// </summary>
    public class GoneUnitEvent : FlattiverseEvent
    {
        /// <summary>
        /// The name of the unit which is gane.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// This specifies the kind of this event.
        /// </summary>
        public override FlattiverseEventKind Kind => FlattiverseEventKind.Scan;

        internal GoneUnitEvent(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The string representation of this object.
        /// </summary>
        /// <returns>The String.</returns>
        public override string ToString()
        {
            return $"Gone Unit: {Name}";
        }
    }
}
