using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Base type for events about visible units.
/// </summary>
public class UnitEvent : FlattiverseEvent
{
    /// <summary>
    /// Snapshot copy of the visible unit this event is about.
    /// The connector clones the unit when the event is created, so this object does not track later live updates.
    /// </summary>
    public readonly Unit Unit;
    
    internal UnitEvent(Unit unit) : base()
    {
        Unit = unit.Clone();
    }
}
