using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse.Events
{
    /// <summary>
    /// ChatMessages.
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class ChatEvent : FlattiverseEvent
    {
        /// <summary>
        /// The chat message as string.
        /// </summary>
        public readonly string Message;

        internal ChatEvent(string message)
        {
            Message = message;
        }

        /// <summary>
        /// This event as string.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString()
        {
            return Message;
        }

        /// <summary>
        /// The kind of the event.
        /// </summary>
        public override FlattiverseEventKind Kind => FlattiverseEventKind.PlayerChat;
    }
}
