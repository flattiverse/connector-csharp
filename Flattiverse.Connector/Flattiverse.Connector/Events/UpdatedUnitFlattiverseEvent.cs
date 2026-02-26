using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events;

/// <summary>
/// this event informs about an updated unit.
/// </summary>
public class UpdatedUnitFlattiverseEvent : UnitFlattiverseEvent
{
    internal UpdatedUnitFlattiverseEvent(Unit unit) : base(unit)
    {
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.UpdatedUnit;
    
    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} Updated Unit: {Unit}.";
    }
}
