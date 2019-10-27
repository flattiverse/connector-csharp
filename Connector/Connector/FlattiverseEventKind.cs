using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// Specifies the kind of an flattiverse event.
    /// </summary>
    public enum FlattiverseEventKind
    {
        /// <summary>
        /// Meta events are usually no direct game events. Meta events are used to inform you about
        /// players joinning our universe or about newly registered ships of your teammates, etc.
        /// Meta events also gives you the heartbeat of the universe (when internal ticks have been
        /// finished.)
        /// </summary>
        Meta,
        /// <summary>
        /// Status updates are updates which inform you about a status change of one of your units.
        /// This includes hull, shield, energy or configuration changes.
        /// </summary>
        Status,
        /// <summary>
        /// Scan events are updates to scanned units: Units appreaing in your event horizon, units
        /// which change something within your eventhorizon or units which leave your eventhorizon.
        /// </summary>
        Scan,
        /// <summary>
        /// Chat events contain chat messages from other players sent to you or the universe group
        /// you are in or the team you are in. This also includes map pings or binary messages.
        /// </summary>
        Chat
    }
}
