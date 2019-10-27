using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// Represents a flattiverse event
    /// </summary>
    public class FlattiverseEvent
    {
        /// <summary>
        /// This specifies the kind of this event.
        /// </summary>
        public virtual FlattiverseEventKind Kind => FlattiverseEventKind.Meta;

        /// <summary>
        /// This event as string.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString()
        {
            return $"Sadly {this.GetType()} has no valid .ToString().";
        }
    }
}
