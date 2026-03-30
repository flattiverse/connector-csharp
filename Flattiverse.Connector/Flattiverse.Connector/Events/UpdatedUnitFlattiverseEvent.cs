using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Raised when the connector updates the snapshot of a currently visible unit.
/// </summary>
public class UpdatedUnitFlattiverseEvent : UnitFlattiverseEvent
{
    internal UpdatedUnitFlattiverseEvent(Unit unit) : base(unit)
    {
    }

    /// <inheritdoc />
    public override EventKind Kind => EventKind.UpdatedUnit;
    
    /// <summary>
    /// Returns a compact diagnostic representation of the event.
    /// </summary>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} Updated Unit: {Unit}.";
    }
}
