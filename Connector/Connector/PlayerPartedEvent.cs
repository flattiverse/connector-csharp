using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// An event specifying a player left an universe.
    /// </summary>
    public class PlayerPartedEvent : PlayerEvent
    {
        /// <summary>
        /// The universe the player parted from.
        /// </summary>
        public readonly Universe Universe;

        /// <summary>
        /// The team the player was on.
        /// </summary>
        public readonly Team Team;

        internal PlayerPartedEvent(Player player) : base(player)
        {
            Universe = player.Universe;
            Team = player.Team;
        }
    }
}
