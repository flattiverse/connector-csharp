using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Galaxy-wide system chat announcing that a flag became active again.
/// </summary>
public class FlagReactivatedChatEvent : SystemChatEvent
{
    /// <summary>
    /// Team configured on the flag.
    /// </summary>
    public readonly Team FlagTeam;

    /// <summary>
    /// Name of the reactivated flag.
    /// </summary>
    public readonly string FlagName;

    internal FlagReactivatedChatEvent(Team flagTeam, string flagName)
        : base($"Flag \"{flagName}\" of team {flagTeam.Name} is active again.")
    {
        FlagTeam = flagTeam;
        FlagName = flagName;
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.FlagReactivatedChat;

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} [SYSTEM] Flag \"{FlagName}\" of team {FlagTeam.Name} is active again.";
    }
}
