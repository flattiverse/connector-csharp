using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// An event specifying a player joined an universe.
    /// </summary>
    public class PlayerJoinedEvent : PlayerEvent
    {
        /// <summary>
        /// The universe.
        /// </summary>
        public readonly Universe Universe;

        /// <summary>
        /// The team with which the player joined.
        /// </summary>
        public readonly Team Team;

        internal PlayerJoinedEvent(Player player) : base(player)
        {
            Universe = player.Universe;
            Team = player.Team;
        }

        /// <summary>
        /// This event as string.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString()
        {
            return $"{Player.Name} joined {Universe.Name} with team {Team.Name}.";
        }
    }
}
