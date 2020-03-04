using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse.Events
{
    /// <summary>
    /// Represents a flattiverse event
    /// </summary>
    public class FlattiverseEvent
    {
        /// <summary>
        /// This specifies the kind of this event.
        /// </summary>
        public virtual FlattiverseEventGroup Group => FlattiverseEventGroup.Meta;

        /// <summary>
        /// This event as string.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString()
        {
            return $"Sadly {this.GetType()} has no valid .ToString().";
        }

        /// <summary>
        /// The kind of the event.
        /// </summary>
        public virtual FlattiverseEventKind Kind => throw new NotImplementedException("Please contact info@flattiverse.com.");
    }
}
