using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events;

/// <summary>
/// This event informs about an removed unit.
/// </summary>
public class RemovedUnitFlattiverseEvent : UnitFlattiverseEvent
{
    internal RemovedUnitFlattiverseEvent(Unit unit) : base(unit)
    {
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.RemovedUnit;
    
    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} Removed Unit: {Unit}.";
    }
}
