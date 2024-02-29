using System.Collections.ObjectModel;
using System.Text.Json;
using Flattiverse.Connector.Hierarchy;

namespace Flattiverse.Connector.MissionSelection;

/// <summary>
/// 
public class GalaxyInfo
{
    public readonly Universe Universe;
    
    public readonly int Id;
    public readonly string Name;
    public readonly bool SpectatorsAllowed;
    public readonly GameMode GameMode;
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

    public GalaxyInfo(Universe universe, JsonElement element)
    {
        Universe = universe;
        
        if(
            !Utils.Traverse(element, out Id, "id") ||
            !Utils.Traverse(element, out Name, "name") ||
            !Utils.Traverse(element, out SpectatorsAllowed, "allowSpectating") ||
            !Utils.Traverse(element, out string gm, "gameType") ||
            !Enum.TryParse(gm, out GameMode) ||
            !Utils.Traverse(element, out MaxPlayers, "maxPlayers") ||
            !Utils.Traverse(element, out MaxPlatformsUniverse, "maxPlatformsUniverse") ||
            !Utils.Traverse(element, out MaxProbesUniverse, "maxProbesUniverse") ||
            !Utils.Traverse(element, out MaxDronesUniverse, "maxDronesUniverse") ||
            !Utils.Traverse(element, out MaxShipsUniverse, "maxShipsUniverse") ||
            !Utils.Traverse(element, out MaxBasesUniverse, "maxBasesUniverse") ||
            !Utils.Traverse(element, out MaxPlatformsTeam, "maxPlatformsTeam") ||
            !Utils.Traverse(element, out MaxProbesTeam, "maxProbesTeam") ||
            !Utils.Traverse(element, out MaxDronesTeam, "maxDronesTeam") ||
            !Utils.Traverse(element, out MaxShipsTeam, "maxShipsTeam") ||
            !Utils.Traverse(element, out MaxBasesTeam, "maxBasesTeam") ||
            !Utils.Traverse(element, out MaxPlatformsPlayer, "maxPlatformsPlayer") ||
            !Utils.Traverse(element, out MaxProbesPlayer, "maxProbesPlayer") ||
            !Utils.Traverse(element, out MaxDronesPlayer, "maxDronesPlayer") ||
            !Utils.Traverse(element, out MaxShipsPlayer, "maxShipsPlayer") ||
            !Utils.Traverse(element, out MaxBasesPlayer, "maxBasesPlayer")
        )
        {
            throw new GameException(0xF3);
        } 

        JsonElement teams;

        Dictionary<string, TeamInfo> teamsResult = new Dictionary<string, TeamInfo>();

        if (Utils.Traverse(element, out teams, "teams"))
        {
            foreach (JsonElement team in teams.EnumerateArray())
                if (team.ValueKind == JsonValueKind.Object)
                {
                    TeamInfo teamResult = new TeamInfo(team);

                    teamsResult.Add(teamResult.Name, teamResult);
                }
        }
        else
        {
            throw new GameException(0xF3);
        }

        Teams = new ReadOnlyDictionary<string, TeamInfo>(teamsResult);


        JsonElement players;

        Dictionary<string, PlayerInfo> playersResult = new Dictionary<string, PlayerInfo>();

        if (Utils.Traverse(element, out players, "players"))
        {
            foreach (JsonElement player in players.EnumerateArray())
                if (player.ValueKind == JsonValueKind.Object)
                {
                    PlayerInfo playerResult = new PlayerInfo(player, Teams);

                    playersResult.Add(playerResult.Name, playerResult);
                }
        }
        else
        {
            throw new GameException(0xF3);
        }

        Players = new ReadOnlyDictionary<string, PlayerInfo>(playersResult);
    }
    
    /// <summary>
    /// Joins the galaxy with the specified team.
    /// </summary>
    /// <returns>The corresponding connection to the Galaxy. (Maintained by the Galaxy class.)</returns>
    public async Task<Galaxy> Join(string auth, TeamInfo team)
    {
        Galaxy galaxy = new Galaxy(Universe);
        
        if (Universe.UseSSL)
            await galaxy.Connect($"wss://{Universe.BaseURI}/game/galaxies/{Id}", auth, (byte)team.Id);
        else
            await galaxy.Connect($"ws://{Universe.BaseURI}/game/galaxies/{Id}", auth, (byte)team.Id);

        // We wait so that we are sure that we have all meta data.
        await galaxy.WaitLoginCompleted();
        
        return galaxy;
    }
}