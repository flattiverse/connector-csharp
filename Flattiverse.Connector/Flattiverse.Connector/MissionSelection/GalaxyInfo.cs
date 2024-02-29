using System.Collections.ObjectModel;
using System.Text.Json;
using Flattiverse.Connector.Hierarchy;

namespace Flattiverse.Connector.MissionSelection;

/// <summary>
/// Information about a galaxy.
/// </summary>
/// <remarks>
/// A galaxy is a game instance in the universe.
/// </remarks>
public class GalaxyInfo
{
    /// <summary>
    /// The universe this galaxy belongs to.
    /// </summary>
    public readonly Universe Universe;
    
    /// <summary>
    /// The unique identifier of the galaxy.
    /// </summary>
    public readonly int Id;

    /// <summary>
    /// The name of the galaxy.
    /// </summary>
    /// <remarks>
    /// SAFETY: Make sure this name is unique in the universe.
    /// </remarks>
    public readonly string Name;

    /// <summary>
    /// If spectators are allowed in this galaxy.
    /// </summary>
    public readonly bool SpectatorsAllowed;

    /// <summary>
    /// The game mode the galaxy uses.
    /// </summary>
    public readonly GameMode GameMode;

    /// <summary>
    /// The maximum amount of players allowed in this galaxy.
    /// </summary>
    public readonly int MaxPlayers;

    /// <summary>
    /// The maximum amount of platforms allowed in this galaxy.
    /// </summary>
    public readonly int MaxPlatformsUniverse;

    /// <summary>
    /// The maximum amount of probes allowed in this galaxy.
    /// </summary>
    public readonly int MaxProbesUniverse;

    /// <summary>
    /// The maximum amount of drones allowed in this galaxy.
    /// </summary>
    public readonly int MaxDronesUniverse;

    /// <summary>
    /// The maximum amount of ships allowed in this galaxy.
    /// </summary>
    public readonly int MaxShipsUniverse;

    /// <summary>
    /// The maximum amount of bases allowed in this galaxy.
    /// </summary>
    public readonly int MaxBasesUniverse;

    /// <summary>
    /// The maximum amount of platforms allowed per team in this galaxy.
    /// </summary>
    public readonly int MaxPlatformsTeam;

    /// <summary>
    /// The maximum amount of probes allowed per team in this galaxy.
    /// </summary>
    public readonly int MaxProbesTeam;

    /// <summary>
    /// The maximum amount of drones allowed per team in this galaxy.
    /// </summary>
    public readonly int MaxDronesTeam;

    /// <summary>
    /// The maximum amount of ships allowed per team in this galaxy.
    /// </summary>
    public readonly int MaxShipsTeam;

    /// <summary>
    /// The maximum amount of bases allowed per team in this galaxy.
    /// </summary>
    public readonly int MaxBasesTeam;

    /// <summary>
    /// The maximum amount of platforms allowed per player in this galaxy.
    /// </summary>
    public readonly int MaxPlatformsPlayer;

    /// <summary>
    /// The maximum amount of probes allowed per player in this galaxy.
    /// </summary>
    public readonly int MaxProbesPlayer;

    /// <summary>
    /// The maximum amount of drones allowed per player in this galaxy.
    /// </summary>
    public readonly int MaxDronesPlayer;

    /// <summary>
    /// The maximum amount of ships allowed per player in this galaxy.
    /// </summary>
    public readonly int MaxShipsPlayer;

    /// <summary>
    /// The maximum amount of bases allowed per player in this galaxy.
    /// </summary>
    public readonly int MaxBasesPlayer;

    /// <summary>
    /// The teams in this galaxy.
    /// </summary>
    public readonly ReadOnlyDictionary<string, TeamInfo> Teams;

    /// <summary>
    /// The players in this galaxy.
    /// </summary>
    public readonly ReadOnlyDictionary<string, PlayerInfo> Players;

    /// <summary>
    /// Constructs a new galaxy info.
    /// </summary>
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