using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

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