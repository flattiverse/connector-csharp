namespace Flattiverse.Connector.Events;

/// <summary>
/// Generic server-originated system message.
/// </summary>
public class SystemMessageEvent : SystemChatEvent
{
    internal SystemMessageEvent(string message) : base(message)
    {
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.SystemMessage;

    /// <inheritdoc/>
    public override string ToString() => Message;
}
