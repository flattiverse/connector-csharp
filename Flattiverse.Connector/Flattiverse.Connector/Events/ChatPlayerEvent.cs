using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// This event is received when some kind of chat is received.
/// </summary>
public class ChatPlayerEvent : PlayerEvent
{
    /// <summary>
    /// The message of the chat.
    /// </summary>
    public readonly string Message;

    internal ChatPlayerEvent(Player player, string message) : base(player)
    {
        Message = message;
    }
}