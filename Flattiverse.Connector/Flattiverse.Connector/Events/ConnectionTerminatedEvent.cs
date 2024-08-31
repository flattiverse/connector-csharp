namespace Flattiverse.Connector.Events;

/// <summary>
/// Is fired when the connection to the flattiverse server has been terminated.
/// </summary>
public class ConnectionTerminatedEvent : FlattiverseEvent
{
    /// <inheritdoc/>
    public override EventKind Kind => EventKind.ConnectionTerminated;
    
    /// <summary>
    /// The message which describes why the connection was terminated.
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