using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events;

/// <summary>
/// A helper class to wrap all unit calls.
/// </summary>
public class UnitFlattiverseEvent : FlattiverseEvent
{
    /// <summary>
    /// The unit this event is about.
    /// </summary>
    public readonly Unit Unit;
    
    internal UnitFlattiverseEvent(Unit unit) : base()
    {
        Unit = unit.Clone();
    }
}