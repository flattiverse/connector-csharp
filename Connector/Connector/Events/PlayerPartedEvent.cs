using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse.Events
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

        /// <summary>
        /// This event as string.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString()
        {
            return $"{Player.Name} parted {Universe.Name}. He was on team {Team.Name}.";
        }

        /// <summary>
        /// The kind of the event.
        /// </summary>
        public override FlattiverseEventKind Kind => FlattiverseEventKind.PlayerParted;
    }
}
