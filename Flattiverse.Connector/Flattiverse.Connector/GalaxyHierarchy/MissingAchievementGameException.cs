namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Protocol error <c>0x23</c>: thrown when a normal player login is denied because a required galaxy achievement is missing.
/// </summary>
public class MissingAchievementGameException : GameException
{
    public readonly string AchievementName;

    internal MissingAchievementGameException(string achievementName)
        : base(0x23, $"[0x23] Missing required achievement \"{achievementName}\".")
    {
        AchievementName = achievementName;
    }
}
