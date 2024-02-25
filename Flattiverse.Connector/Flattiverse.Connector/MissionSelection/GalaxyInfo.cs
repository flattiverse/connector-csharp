using System.Collections.ObjectModel;
using System.Text.Json;

namespace Flattiverse.Connector.MissionSelection;

public class GalaxyInfo
{
    public readonly int Id;
    public readonly string Name;
    public readonly bool SpectatorsAllowed;
    public readonly GameMode gameMode;
    public readonly int MaxPlayers;
    public readonly int MaxPlatformsUniverse;
    public readonly int MaxProbesUniverse;
    public readonly int MaxDronesUniverse;
    public readonly int MaxShipsUniverse;
    public readonly int MaxBasesUniverse;
    public readonly int MaxPlatformsTeam;
    public readonly int MaxProbesTeam;
    public readonly int MaxDronesTeam;
    public readonly int MaxShipsTeam;

    public readonly int MaxBasesTeam;
    public readonly int MaxPlatformsPlayer;
    public readonly int MaxProbesPlayer;
    public readonly int MaxDronesPlayer;
    public readonly int MaxShipsPlayer;
    public readonly int MaxBasesPlayer;

    

    public readonly ReadOnlyDictionary<string, TeamInfo> Teams;
    public readonly ReadOnlyDictionary<string, PlayerInfo> Players;

    public GalaxyInfo(JsonElement element)
    {
        Utils.Traverse(element, out Id, "id");
        Utils.Traverse(element, out Name, "name");
        Utils.Traverse(element, out SpectatorsAllowed, "allowSpectating");
        Utils.Traverse(element, out string gm, "gameType");
        Enum.TryParse(gm, out gameMode);
        Utils.Traverse(element, out MaxPlayers, "maxPlayers");
        Utils.Traverse(element, out MaxPlatformsUniverse, "maxPlatformsUniverse");
        Utils.Traverse(element, out MaxProbesUniverse, "maxProbesUniverse");
        Utils.Traverse(element, out MaxDronesUniverse, "maxDronesUniverse");
        Utils.Traverse(element, out MaxShipsUniverse, "maxShipsUniverse");
        Utils.Traverse(element, out MaxBasesUniverse, "maxBasesUniverse");
        Utils.Traverse(element, out MaxPlatformsTeam, "maxPlatformsTeam");
        Utils.Traverse(element, out MaxProbesTeam, "maxProbesTeam");
        Utils.Traverse(element, out MaxDronesTeam, "maxDronesTeam");
        Utils.Traverse(element, out MaxShipsTeam, "maxShipsTeam");
        Utils.Traverse(element, out MaxBasesTeam, "maxBasesTeam");
        Utils.Traverse(element, out MaxPlatformsPlayer, "maxPlatformsPlayer");
        Utils.Traverse(element, out MaxProbesPlayer, "maxProbesPlayer");
        Utils.Traverse(element, out MaxDronesPlayer, "maxDronesPlayer");
        Utils.Traverse(element, out MaxShipsPlayer, "maxShipsPlayer");
        Utils.Traverse(element, out MaxBasesPlayer, "maxBasesPlayer");

        JsonElement teams;

        Dictionary<string, TeamInfo> teamsResult = new Dictionary<string, TeamInfo>();

        if (Utils.Traverse(element, out teams, "teams"))
            foreach (JsonElement team in teams.EnumerateArray())
                if (team.ValueKind == JsonValueKind.Object)
                {
                    TeamInfo teamResult = new TeamInfo(team);

                    teamsResult.Add(teamResult.Name, teamResult);
                }

        Teams = new ReadOnlyDictionary<string, TeamInfo>(teamsResult);


        JsonElement players;

        Dictionary<string, PlayerInfo> playersResult = new Dictionary<string, PlayerInfo>();

        if (Utils.Traverse(element, out players, "players"))
            foreach (JsonElement player in players.EnumerateArray())
                if (player.ValueKind == JsonValueKind.Object)
                {
                    PlayerInfo playerResult = new PlayerInfo(player);

                    playersResult.Add(playerResult.Name, playerResult);
                }

        Players = new ReadOnlyDictionary<string, PlayerInfo>(playersResult);
    }
}