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
        public readonly Player Sender;
        public readonly Universe Universe;

        internal UniverseChatEvent(string message, Player sender, Universe universe) : base(message)
        {
            Sender = sender;
            Universe = universe;
        }

        /// <summary>
        /// This event as string.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString()
        {
            return $"[{Sender.Name}->{Universe.Name}]: {Message}";
        }
    }
}
