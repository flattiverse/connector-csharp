using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Represents a chat message sent to the universe.
/// </summary>
public class GalaxyChatPlayerEvent : ChatPlayerEvent
{
    /// <summary>
    /// The destination where this message was sent to.
    /// </summary>
    public readonly Galaxy Destination;

    internal GalaxyChatPlayerEvent(Player player, string message, Galaxy destination) : base(player, message)
    {
        Destination = destination;
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.ChatGalaxy;
    
    /// <inheritdoc/>
    public override string ToString() => $"{Stamp:HH:mm:ss.fff} <[{Player.Team.Name}]{Player.Name}> {Message}";
}