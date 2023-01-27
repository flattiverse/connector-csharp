using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flattiverse.Connector.Players
{
    /// <summary>
    /// States the kind of player.
    /// </summary>
    public enum PlayerKind
    {
        /// <summary>
        /// The player is a player participating in the game and also blocking a player slot.
        /// </summary>
        Player,
        /// <summary>
        /// The player is a spectator which used the Api-Key 0x0000000000000000000000000000000000000000000000000000000000000000.
        /// This is only allowed in universe groups which allow spectators.
        /// </summary>
        Spectator,
        /// <summary>
        /// The player is an admin. Admins can't participate in the game but they can alter gamestates via admin commands.
        /// </summary>
        Admin
    }
}
