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
        public readonly Player Sender;
        public readonly Team Team;

        internal TeamChatEvent(string message, Player sender, Team team) : base(message)
        {
            Sender = sender;
            Team = team;
        }

        /// <summary>
        /// This event as string.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString()
        {
            return $"[{Sender.Name}->{Team.Universe.Name}\\{Team.Name}]: {Message}";
        }
    }
}
