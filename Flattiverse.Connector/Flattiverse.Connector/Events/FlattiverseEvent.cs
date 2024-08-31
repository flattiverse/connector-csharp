namespace Flattiverse.Connector.Events;

/// <summary>
/// The base class for FlattiverseEvents.
/// </summary>
public class FlattiverseEvent
{
    /// <summary>
    /// The UTC timestamp when this event has been received or generated.
    /// </summary>
    public readonly DateTime Stamp;

    internal FlattiverseEvent()
    {
        Stamp = DateTime.UtcNow;
    }

    /// <summary>
    /// The kind of the event.
    /// </summary>
    public virtual EventKind Kind => throw new NotImplementedException("Kind must be overwritten.");

    /// <summary>
    /// A user readable message.
    /// </summary>
    /// <returns>This event formatted as string.</returns>
    public override string ToString() => throw new NotImplementedException("ToString must be overwritten.");
}