using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flattiverse.Connector.Events
{
    /// <summary>
    /// This event indicates some critical out-of-game failure like a problem with the
    /// data-transport, etc.. Consider upgrading the connector if this happens and it
    /// is not due to a lost connection.
    /// </summary>
    public class FailureEvent : FlattiverseEvent
    {
        /// <summary>
        /// The message which indicates the issue happened.
        /// </summary>
        public readonly string Message;

        internal FailureEvent(string message)
        {
            Message = message;
        }

        /// <summary>
        /// Specifies the kind of the event for a better switch() experience.
        /// </summary>
        public override EventKind Kind => EventKind.Failure;
    }
}
