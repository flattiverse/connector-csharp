using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Raised when a previously known visible unit leaves the local visibility mirror.
/// </summary>
public class RemovedUnitEvent : UnitEvent
{
    internal RemovedUnitEvent(Unit unit) : base(unit)
    {
    }

    /// <inheritdoc />
    public override EventKind Kind => EventKind.UnitRemoved;
    
    /// <summary>
    /// Returns a compact diagnostic representation of the event.
    /// </summary>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} Removed Unit: {Unit}.";
    }
}
