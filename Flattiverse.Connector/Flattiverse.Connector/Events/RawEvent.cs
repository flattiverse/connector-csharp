using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Flattiverse.Connector.Events
{
    /// <summary>
    /// A Fallback event for debugging purposes, if the event sent from the server is unknown to the connector.
    /// </summary>
    public class RawEvent : FlattiverseEvent
    {
        /// <summary>
        /// RAW data this event consists of.
        /// </summary>
        public readonly string RawData;

        internal RawEvent(JsonElement element)
        {
            RawData = element.GetRawText();
        }

        /// <summary>
        /// Specifies the kind of the event for a better switch() experience.
        /// </summary>
        public override EventKind Kind => EventKind.RAW;
    }
}
