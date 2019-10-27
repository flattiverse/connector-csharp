﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// An event specifying a player joinned an universe.
    /// </summary>
    public class PlayerJoinnedEvent : PlayerEvent
    {
        /// <summary>
        /// The universe.
        /// </summary>
        public readonly Universe Universe;

        /// <summary>
        /// The team with which the player joined.
        /// </summary>
        public readonly Team Team;

        internal PlayerJoinnedEvent(Player player) : base(player)
        {
            Universe = player.Universe;
            Team = player.Team;
        }
    }
}
