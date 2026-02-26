namespace Flattiverse.Connector.Events;

/// <summary>
/// Event that is raised when the server has processed a tick.
/// </summary>
public class GalaxyTickEvent : FlattiverseEvent
{
    /// <inheritdoc/>
    public override EventKind Kind => EventKind.GalaxyTick;

    /// <summary>
    /// Specifies 
    /// </summary>
    public readonly uint Tick;

    internal GalaxyTickEvent(uint tick)
    {
        Tick = tick;
    }

    /// <inheritdoc/>
    public override string ToString() => $"{Stamp:HH:mm:ss.fff} Tick/Tack #{Tick}.";
}
