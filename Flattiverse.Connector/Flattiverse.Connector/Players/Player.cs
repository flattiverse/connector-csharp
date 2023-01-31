using Flattiverse.Connector.Players;
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

        /// <summary>
        /// The kind of the player.
        /// </summary>
        public readonly PlayerKind playerKind;

        /// <summary>
        /// Whether the player is an admin.
        /// </summary>
        public readonly bool Admin;

        private double pvpScore;
        private int rank;
        private long kills;
        private long deaths;
        private long collisions;
        
        private TimeSpan ping;

        internal Player(JsonElement element)
        {
            Utils.Traverse(element, out ID, "id");
            Utils.Traverse(element, out Name, "name");
            Utils.Traverse(element, out Admin, "admin");
            Utils.Traverse(element, out pvpScore, "pvpScore");
            Utils.Traverse(element, out deaths, "deaths");
            Utils.Traverse(element, out collisions, "collisions");
            Utils.Traverse(element, out kills, "kills");
            Utils.Traverse(element, out rank, "rank");
            Utils.Traverse(element, out string playerKind, "playerKind");
            Enum.TryParse(playerKind, true, out this.playerKind);
        }

        internal void Update(JsonElement element)
        {
            Utils.Traverse(element, out pvpScore, "pvpScore");
            Utils.Traverse(element, out deaths, "deaths");
            Utils.Traverse(element, out collisions, "collisions");
            Utils.Traverse(element, out kills, "kills");
            Utils.Traverse(element, out rank, "rank");
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
        /// All-Time-Collisions of the player.
        /// </summary>
        public long Collisions => collisions;

        /// <summary>
        /// The ELO ranking of the player.
        /// </summary>
        public double PVPScore => pvpScore;

        /// <summary>
        /// The ping of the player.
        /// </summary>
        public TimeSpan Ping => ping;
    }
}
