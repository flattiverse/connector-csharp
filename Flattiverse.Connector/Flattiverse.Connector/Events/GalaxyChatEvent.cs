using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Represents a chat message sent to the universe.
/// </summary>
public class GalaxyChatEvent : ChatEvent
{
    /// <summary>
    /// The destination where this message was sent to.
    /// </summary>
    public readonly Galaxy Destination;

    internal GalaxyChatEvent(Player player, string message, Galaxy destination) : base(player, message)
    {
        Destination = destination;
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.GalaxyChat;
    
    /// <inheritdoc/>
    public override string ToString() => $"{Stamp:HH:mm:ss.fff} <[{Player.Team.Name}]{Player.Name}> {Message}";
}