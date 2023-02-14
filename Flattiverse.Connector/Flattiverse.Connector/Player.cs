using Flattiverse.Connector.Network;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Flattiverse.Connector
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

        /// <summary>
        /// The team the player is in.
        /// </summary>
        public readonly Team Team;

        public readonly UniverseGroup Group;

        private double pvpScore;
        private int rank;
        private long kills;
        private long deaths;
        private long collisions;

        private TimeSpan ping;

        internal Player(UniverseGroup group, JsonElement element)
        {
            Group = group;

            Utils.Traverse(element, out ID, "id");
            Utils.Traverse(element, out Name, "name");
            Utils.Traverse(element, out Admin, "admin");
            Utils.Traverse(element, out pvpScore, "pvpScore");
            Utils.Traverse(element, out deaths, "deaths");
            Utils.Traverse(element, out collisions, "collisions");
            Utils.Traverse(element, out kills, "kills");
            Utils.Traverse(element, out rank, "rank");
            Utils.Traverse(element, out string playerKind, "playerKind");
            Utils.Traverse(element, out int teamID, "team");
            Team = group.teamsId[teamID];
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

        public async Task Chat(string message)
        {
            if (!Utils.CheckMessage(message))
                throw new GameException(0xB5);

            using (Query query = Group.connection.Query("chatUnicast"))
            {
                query.Write("player", ID);

                query.Write("message", message);

                await query.Send().ConfigureAwait(false);

                await query.Wait().ConfigureAwait(false);
            }
        }
    }
}
