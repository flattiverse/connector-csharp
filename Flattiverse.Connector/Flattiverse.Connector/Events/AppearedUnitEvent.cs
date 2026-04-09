using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Raised when a visible unit becomes newly known to the local visibility mirror.
/// </summary>
public class AppearedUnitEvent : UnitEvent
{
    internal AppearedUnitEvent(Unit unit) : base(unit)
    {
    }
    
    /// <inheritdoc />
    public override EventKind Kind => EventKind.UnitAppeared;

    /// <summary>
    /// Returns a compact diagnostic representation of the event.
    /// </summary>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} Appeared Unit: {Unit}.";
    }
}
