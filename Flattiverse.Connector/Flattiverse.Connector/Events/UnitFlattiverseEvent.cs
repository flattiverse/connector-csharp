using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Base type for events about visible units.
/// </summary>
public class UnitFlattiverseEvent : FlattiverseEvent
{
    /// <summary>
    /// Snapshot copy of the visible unit this event is about.
    /// The connector clones the unit when the event is created, so this object does not track later live updates.
    /// </summary>
    public readonly Unit Unit;
    
    internal UnitFlattiverseEvent(Unit unit) : base()
    {
        Unit = unit.Clone();
    }
}
