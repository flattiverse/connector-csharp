using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Base type for incoming text chat events.
/// </summary>
public class ChatEvent : PlayerEvent
{
    /// <summary>
    /// The message of the chat.
    /// </summary>
    public readonly string Message;

    internal ChatEvent(Player player, string message) : base(player)
    {
        Message = message;
    }
}
