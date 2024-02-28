using Flattiverse.Connector.Hierarchy;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events
{
    /// <summary>
    /// Event that is raised when an update about a unit is received.
    /// </summary>
    public class UnitEvent : FlattiverseEvent
    {
        /// <summary>
        /// The unit that the infos are about.
        /// </summary>
        public readonly Unit Unit;

        internal UnitEvent(Unit unit)
        {
            Unit = unit;
        }
    }
}
