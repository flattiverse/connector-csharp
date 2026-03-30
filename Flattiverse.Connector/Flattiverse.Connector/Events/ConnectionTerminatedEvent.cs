namespace Flattiverse.Connector.Events;

/// <summary>
/// Raised when the galaxy connection has terminated and no further protocol traffic will arrive.
/// </summary>
public class ConnectionTerminatedEvent : FlattiverseEvent
{
    /// <inheritdoc/>
    public override EventKind Kind => EventKind.ConnectionTerminated;
    
    /// <summary>
    /// Optional close reason supplied by the local connector or the remote endpoint.
    /// </summary>
    public readonly string? Message;

    internal ConnectionTerminatedEvent()
    { }

    internal ConnectionTerminatedEvent(string message)
    {
        Message = message;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        if (Message is null)
            return $"{Stamp:HH:mm:ss.fff} Connection terminated.";
        
        return $"{Stamp:HH:mm:ss.fff} Connection terminated: {Message}";
    }
}
