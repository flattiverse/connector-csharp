namespace Flattiverse.Connector.Events;

/// <summary>
/// Base type for server-originated gameplay or tournament system messages.
/// These are not normal player chat messages.
/// </summary>
public abstract class SystemChatEvent : FlattiverseEvent
{
    /// <summary>
    /// Raw system message text sent by the server.
    /// </summary>
    public readonly string Message;

    internal SystemChatEvent(string message)
    {
        Message = message;
    }
}
