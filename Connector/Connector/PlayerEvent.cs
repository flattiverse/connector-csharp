using System;
using System.Collections.Generic;
using System.Text;

namespace Flattiverse
{
    /// <summary>
    /// A helper event managin player stuff.
    /// </summary>
    public class PlayerEvent : FlattiverseEvent
    {
        /// <summary>
        /// The player.
        /// </summary>
        /// <remarks>
        /// The information(s) in this field may be already updated. If the gathered event propagates
        /// a change, like the PlayerPartedEvent the team the player had can't be read from this property.
        /// However the PlayerPartedEvent will have a Universe and Team field for you to gather those
        /// information(s).
        /// </remarks>
        public readonly Player Player;

        internal PlayerEvent(Player player)
        {
            Player = player;
        }
    }
}
