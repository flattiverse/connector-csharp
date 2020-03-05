using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse.Events
{
    /// <summary>
    /// The player is informed by a new message sent directly to him.
    /// </summary>
    public class PlayerChatEvent : ChatEvent
    {
        /// <summary>
        /// The source of the message.
        /// </summary>
        public readonly Player Source;

        /// <summary>
        /// The destination of the message.
        /// </summary>
        public readonly Player Destination;

        internal PlayerChatEvent(string message, Player source, Player destination) : base(message)
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
            return $"[{Source.Name}->YOU]: {Message}";
        }
    }
}
