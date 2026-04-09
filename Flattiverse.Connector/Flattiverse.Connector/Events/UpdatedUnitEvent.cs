using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Raised when the connector updates the snapshot of a currently visible unit.
/// </summary>
public class UpdatedUnitEvent : UnitEvent
{
    internal UpdatedUnitEvent(Unit unit) : base(unit)
    {
    }

    /// <inheritdoc />
    public override EventKind Kind => EventKind.UnitUpdated;
    
    /// <summary>
    /// Returns a compact diagnostic representation of the event.
    /// </summary>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} Updated Unit: {Unit}.";
    }
}
