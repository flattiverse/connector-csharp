using Flattiverse.Connector.Hierarchy;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Xml.Linq;

namespace Flattiverse.Connector.MissionSelection
{
    public class PlayerInfo
    {
        public readonly int Id;
        public readonly string Name;
        public readonly bool HasAvatar;
        public readonly TeamInfo Team;
        public readonly int Kills;
        public readonly int Deaths;
        public readonly int Collisions;
        public readonly int SessionKills;
        public readonly int SessionDeaths;
        public readonly int SessionCollisions;
        public readonly int Rank;
        public readonly int PvPScore;
        public readonly int DatePlayedStart;
        public readonly int DatePlayedEnd;



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