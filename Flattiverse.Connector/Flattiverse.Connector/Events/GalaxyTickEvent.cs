namespace Flattiverse.Connector.Events;

/// <summary>
/// Event that is raised when the server has processed a tick.
/// </summary>
public class GalaxyTickEvent : FlattiverseEvent
{
    /// <inheritdoc/>
    public override EventKind Kind => EventKind.GalaxyTick;

    /// <summary>
    /// Tick number processed by the server.
    /// </summary>
    public readonly uint Tick;
    public readonly float ScanMs;
    public readonly float SteadyMs;
    public readonly float GravityMs;
    public readonly float EnginesMs;
    public readonly float LimitMs;
    public readonly float MovementMs;
    public readonly float CollisionsMs;
    public readonly float ActionsMs;
    public readonly float VisibilityMs;
    public readonly float TotalMs;
    public readonly int RemainingStaticSegments;

    internal GalaxyTickEvent(uint tick, float scanMs, float steadyMs, float gravityMs, float enginesMs, float limitMs,
        float movementMs, float collisionsMs, float actionsMs, float visibilityMs, float totalMs, int remainingStaticSegments)
    {
        Tick = tick;
        ScanMs = scanMs;
        SteadyMs = steadyMs;
        GravityMs = gravityMs;
        EnginesMs = enginesMs;
        LimitMs = limitMs;
        MovementMs = movementMs;
        CollisionsMs = collisionsMs;
        ActionsMs = actionsMs;
        VisibilityMs = visibilityMs;
        TotalMs = totalMs;
        RemainingStaticSegments = remainingStaticSegments;
    }

    /// <inheritdoc/>
    public override string ToString() =>
        $"{Stamp:HH:mm:ss.fff} Tick/Tack #{Tick} total={TotalMs:0.000}ms remainingStaticSegments={RemainingStaticSegments}.";
}
