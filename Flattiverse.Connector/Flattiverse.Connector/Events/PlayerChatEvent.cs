using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Represents a chat message from a player only to you.
/// </summary>
public class PlayerChatEvent : ChatEvent
{
    /// <summary>
    /// The destination where this message was sent to.
    /// </summary>
    public readonly Player Destination;

    internal PlayerChatEvent(Player player, string message, Player destination) : base(player, message)
    {
        Destination = destination;
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.PlayerChat;
    
    /// <inheritdoc/>
    public override string ToString() => $"{Stamp:HH:mm:ss.fff} <[{Player.Team.Name}]{Player.Name}->{Destination.Name}> {Message}";
}