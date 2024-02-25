using System.Data.Common;
using System.Text.Json;

namespace Flattiverse.Connector.MissionSelection;

public class TeamInfo
{
    public readonly int Id;
    public readonly string Name;

    public TeamInfo(JsonElement element)
    {
        
    }
}