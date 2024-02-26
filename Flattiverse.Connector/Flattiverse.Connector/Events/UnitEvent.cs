using Flattiverse.Connector.Hierarchy;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events
{
    public class UnitEvent : FlattiverseEvent
    {
        public readonly Unit Unit;

        internal UnitEvent(Unit unit)
        {
            Unit = unit;
        }
    }
}
