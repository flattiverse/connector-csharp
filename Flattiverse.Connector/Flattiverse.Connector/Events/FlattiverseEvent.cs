namespace Flattiverse.Connector.Events;

/// <summary>
/// Base type of all connector events returned by <c>Galaxy.NextEvent()</c>.
/// </summary>
public class FlattiverseEvent
{
    /// <summary>
    /// UTC timestamp when this event instance was created inside the connector.
    /// This is a local connector timestamp, not the authoritative server tick time.
    /// </summary>
    public readonly DateTime Stamp;

    internal FlattiverseEvent()
    {
        Stamp = DateTime.UtcNow;
    }

    /// <summary>
    /// Connector-side event classification used for event dispatch and switch statements.
    /// </summary>
    public virtual EventKind Kind => throw new NotImplementedException("Kind must be overwritten.");

    /// <summary>
    /// Human-readable diagnostic representation of the event payload.
    /// </summary>
    /// <returns>This event formatted as a string.</returns>
    public override string ToString() => throw new NotImplementedException("ToString must be overwritten.");
}
