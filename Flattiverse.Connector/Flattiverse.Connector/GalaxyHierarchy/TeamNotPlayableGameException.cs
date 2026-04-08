namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Thrown, if the requested team exists but is not playable for regular player logins.
/// </summary>
public class TeamNotPlayableGameException : GameException
{
    /// <summary>
    /// Team name received from the server.
    /// </summary>
    public readonly string TeamName;

    internal TeamNotPlayableGameException(string teamName) : base(0x24, $"[0x24] The requested team \"{teamName}\" is not playable.")
    {
        TeamName = teamName;
    }
}
