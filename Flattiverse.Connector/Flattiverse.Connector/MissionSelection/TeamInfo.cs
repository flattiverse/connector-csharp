using System.Data.Common;
using System.Text.Json;

namespace Flattiverse.Connector.MissionSelection;

public class TeamInfo
{
    public readonly int Id;
    public readonly string Name;
    public readonly int Galaxy;
    public readonly int Red;
    public readonly int Green;
    public readonly int Blue;

    public TeamInfo(JsonElement element)
    {
        if(
            !Utils.Traverse(element, out Id, "id") ||
            !Utils.Traverse(element, out Name, "name") ||
            !Utils.Traverse(element, out Galaxy, "galaxy") ||
            !Utils.Traverse(element, out Red, "red") ||
            !Utils.Traverse(element, out Green, "green") ||
            !Utils.Traverse(element, out Blue, "blue")
            )
        {
            throw new GameException(0xF3);
        }
    }
}