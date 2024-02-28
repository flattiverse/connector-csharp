using Flattiverse.Connector.Hierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Flattiverse.Connector.Events
{
    /// <summary>
    /// Event that is raised when the server updates you about other players.
    /// </summary>
    public class PlayerEvent : FlattiverseEvent
    {
        /// <summary>
        /// The player that the infos are about.
        /// </summary>
        public readonly Player Player;

        internal PlayerEvent(Player player)
        {
            Player = player;
        }

    }
}
