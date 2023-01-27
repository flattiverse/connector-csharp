using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Flattiverse.Connector.Accounts
{
    /// <summary>
    /// Specifies a player which is currently connected to the UniverseGroup.
    /// </summary>
    public class Player
    {
        /// <summary>
        /// The internal ID of the player.
        /// </summary>
        public readonly int ID;

        /// <summary>
        /// The name of the player.
        /// </summary>
        public readonly string Name;

        private int rank;
        private long kills;
        private long deaths;
        private TimeSpan ping;

        internal Player(JsonElement element)
        {

        }

        internal void Update(JsonElement element)
        {

        }

        /// <summary>
        /// The rank of the player.
        /// </summary>
        public int Rank => rank;

        /// <summary>
        /// All-Time-Kills of the player.
        /// </summary>
        public long Kills => kills;

        /// <summary>
        /// All-Time-Deaths of the player.
        /// </summary>
        public long Deaths => deaths;

        /// <summary>
        /// The ping of the player.
        /// </summary>
        public TimeSpan Ping => ping;
    }
}
