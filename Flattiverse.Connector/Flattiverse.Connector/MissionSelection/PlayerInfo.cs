using Flattiverse.Connector.Hierarchy;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Xml.Linq;

namespace Flattiverse.Connector.MissionSelection
{
    /// <summary>
    /// Information about a player.
    /// </summary>
    public class PlayerInfo
    {
        /// <summary>
        /// The id of the player. It is unique in the galaxy.
        /// </summary>
        public readonly int Id;

        /// <summary>
        /// The name of the player.
        /// </summary>
        /// <remarks>
        /// SAFETY: Make sure this name is unique in the galaxy.
        /// </remarks>
        public readonly string Name;

        /// <summary>
        /// If the player has an avatar.
        /// </summary>
        public readonly bool HasAvatar;

        /// <summary>
        /// The team the player is in.
        /// </summary>
        public readonly TeamInfo Team;

        /// <summary>
        /// The amount of kills the player has.
        /// </summary>
        public readonly int Kills;

        /// <summary>
        /// The amount of deaths the player has.
        /// </summary>
        public readonly int Deaths;

        /// <summary>
        /// The amount of collisions the player has had.
        /// </summary>
        public readonly int Collisions;

        /// <summary>
        /// The amount of kills the player has in the current session.
        /// </summary>
        public readonly int SessionKills;

        /// <summary>
        /// The amount of deaths the player has in the current session.
        /// </summary>
        public readonly int SessionDeaths;

        /// <summary>
        /// The amount of collisions the player has had in the current session.
        /// </summary>
        public readonly int SessionCollisions;
        
        /// <summary>
        /// The rank of the player.
        /// </summary>
        public readonly int Rank;

        /// <summary>
        /// The PvP score of the player.
        /// </summary>
        public readonly int PvPScore;
        
        /// <summary>
        /// The date the player started playing.
        /// </summary>
        public readonly int DatePlayedStart;

        /// <summary>
        /// The date the player stopped playing.
        /// </summary>
        public readonly int DatePlayedEnd;

        /// <summary>
        /// Creates a new player info.
        /// </summary>
        public PlayerInfo(JsonElement player, ReadOnlyDictionary<string, TeamInfo> teams)
        {
            int team;
            
            if(
            !Utils.Traverse(player, out Id, "id") ||
            !Utils.Traverse(player, out Name, "name") ||
            !Utils.Traverse(player, out HasAvatar, "hasAvatar") ||
            !Utils.Traverse(player, out team, "team") ||
            !Utils.Traverse(player, out Kills, "kills") ||
            !Utils.Traverse(player, out Deaths, "deaths") ||
            !Utils.Traverse(player, out Collisions, "collisions") ||
            !Utils.Traverse(player, out SessionKills, "sessionKills") ||
            !Utils.Traverse(player, out SessionDeaths, "sessionDeaths") ||
            !Utils.Traverse(player, out SessionCollisions, "sessionCollisions") ||
            !Utils.Traverse(player, out Rank, "rank") ||
            !Utils.Traverse(player, out PvPScore, "pvPScore") ||
            !Utils.Traverse(player, out DatePlayedStart, "datePlayedStart") ||
            !Utils.Traverse(player, out DatePlayedEnd, "datePlayedEnd")
            )
            {
                throw new GameException(0xF3);
            }

            foreach (KeyValuePair<string, TeamInfo> kvp in teams)
            {
                if (kvp.Value.Id == team)
                {
                    Team = kvp.Value;
                    break;
                }
            }
        }
    }
}