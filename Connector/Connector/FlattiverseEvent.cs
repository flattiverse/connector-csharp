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
    }
}
