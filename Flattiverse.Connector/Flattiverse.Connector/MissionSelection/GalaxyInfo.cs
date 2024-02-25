using System.Collections.ObjectModel;
using System.Text.Json;

namespace Flattiverse.Connector.MissionSelection;

public class GalaxyInfo
{
    public readonly int Id;
    public readonly string Name;
    public readonly bool SpectatorsAllowed;

    public readonly ReadOnlyDictionary<string, TeamInfo> Teams;

    public GalaxyInfo(JsonElement element)
    {
        Utils.Traverse(element, out Id, "id");
        Utils.Traverse(element, out Name, "name");
        Utils.Traverse(element, out SpectatorsAllowed, "allowSpectating");

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
    }
}