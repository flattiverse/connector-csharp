using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse.Events
{
    /// <summary>
    /// All players of the current universe receive this chat message.
    /// </summary>
    public class UniverseChatEvent : ChatEvent
    {
        /// <summary>
        /// The source of the message.
        /// </summary>
        public readonly Player Source;

        /// <summary>
        /// The destination of the message.
        /// </summary>
        public readonly Universe Destination;

        internal UniverseChatEvent(string message, Player source, Universe destination) : base(message)
        {
            Source = source;
            Destination = destination;
        }

        /// <summary>
        /// This event as string.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString()
        {
            return $"[{Source.Name}->{Destination.Name}]: {Message}";
        }
    }
}
