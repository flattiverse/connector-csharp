using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events;

/// <summary>
/// An event which informs about a new unit.
/// </summary>
public class NewUnitFlattiverseEvent : UnitFlattiverseEvent
{
    internal NewUnitFlattiverseEvent(Unit unit) : base(unit)
    {
    }
    
    /// <inheritdoc/>
    public override EventKind Kind => EventKind.NewUnit;

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} New Unit: {Unit}.";
    }
}
