using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse.Events
{
    /// <summary>
    /// All players of the same team receive this chat message.
    /// </summary>
    public class TeamChatEvent : ChatEvent
    {
        /// <summary>
        /// The source of the message.
        /// </summary>
        public readonly Player Source;

        /// <summary>
        /// The destination of the message.
        /// </summary>
        public readonly Team Destination;

        internal TeamChatEvent(string message, Player source, Team destination) : base(message)
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
            return $"[{Source.Name}->{Destination.Universe.Name}\\{Destination.Name}]: {Message}";
        }
    }
}
