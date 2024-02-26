using Flattiverse.Connector.Hierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Flattiverse.Connector.Events
{
    public class PlayerEvent : FlattiverseEvent
    {
        public readonly Player Player;

        internal PlayerEvent(Player player)
        {
            Player = player;
        }

    }
}
