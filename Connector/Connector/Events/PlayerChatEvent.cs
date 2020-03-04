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
        public readonly Player Sender;

        public PlayerChatEvent(string message, Player sender) : base(message)
        {
            Sender = sender;
        }

        /// <summary>
        /// This event as string.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString()
        {
            return $"[{Sender.Name}->YOU]: {Message}";
        }
    }
}
