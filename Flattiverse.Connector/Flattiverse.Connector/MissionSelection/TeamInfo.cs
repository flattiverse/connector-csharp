using System.Data.Common;
using System.Text.Json;

namespace Flattiverse.Connector.MissionSelection;

/// <summary>
/// Information about a team.
/// </summary>
public class TeamInfo
{
    /// <summary>
    /// The id of the team. It is unique in the galaxy.
    /// </summary>
    public readonly int Id;

    /// <summary>
    /// The name of the team.
    /// </summary>
    /// <remarks>
    /// SAFETY: Make sure this name is unique in the galaxy.
    /// </remarks>
    public readonly string Name;

    /// <summary>
    /// The galaxy the team is in.
    /// </summary>
    public readonly int Galaxy;

    /// <summary>
    /// The red color channel of the team.
    /// </summary>
    public readonly int Red;

    /// <summary>
    /// The green color channel of the team.
    /// </summary>
    public readonly int Green;

    /// <summary>
    /// The blue color channel of the team.
    /// </summary>
    public readonly int Blue;

    /// <summary>
    /// Creates a new team info.
    /// </summary>
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