namespace Flattiverse.Connector.Events;

/// <summary>
/// Server-originated tournament system chat message.
/// </summary>
public class TournamentMessageEvent : SystemChatEvent
{
    internal TournamentMessageEvent(string message) : base(message)
    {
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.TournamentMessage;

    /// <inheritdoc/>
    public override string ToString() => Message;
}
