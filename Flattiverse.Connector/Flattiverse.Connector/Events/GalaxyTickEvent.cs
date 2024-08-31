namespace Flattiverse.Connector.Events;

/// <summary>
/// Event that is raised when the server has processed a tick.
/// </summary>
public class GalaxyTickEvent : FlattiverseEvent
{
    /// <inheritdoc/>
    public override EventKind Kind => EventKind.GalaxyTick;

    internal GalaxyTickEvent()
    { }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} Tick/Tack.";
    }
}