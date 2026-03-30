using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Raised when a visible unit becomes newly known to the local visibility mirror.
/// </summary>
public class NewUnitFlattiverseEvent : UnitFlattiverseEvent
{
    internal NewUnitFlattiverseEvent(Unit unit) : base(unit)
    {
    }
    
    /// <inheritdoc />
    public override EventKind Kind => EventKind.NewUnit;

    /// <summary>
    /// Returns a compact diagnostic representation of the event.
    /// </summary>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} New Unit: {Unit}.";
    }
}
