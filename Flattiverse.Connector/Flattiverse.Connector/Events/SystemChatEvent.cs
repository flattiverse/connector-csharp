namespace Flattiverse.Connector.Events;

/// <summary>
/// Base type for server-originated system chat messages.
/// </summary>
public abstract class SystemChatEvent : FlattiverseEvent
{
    /// <summary>
    /// Human-readable system message text.
    /// </summary>
    public readonly string Message;

    internal SystemChatEvent(string message)
    {
        Message = message;
    }
}
