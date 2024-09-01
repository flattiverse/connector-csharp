using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Represents a chat message from a player to the team.
/// </summary>
public class TeamChatPlayerEvent : ChatPlayerEvent
{
    /// <summary>
    /// The destination where this message was sent to.
    /// </summary>
    public readonly Team Destination;

    internal TeamChatPlayerEvent(Player player, string message, Team destination) : base(player, message)
    {
        Destination = destination;
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.ChatTeam;
    
    /// <inheritdoc/>
    public override string ToString() => $"{Stamp:HH:mm:ss.fff} <[{Player.Team.Name}]{Player.Name}->[{Destination.Name}]> {Message}";
}