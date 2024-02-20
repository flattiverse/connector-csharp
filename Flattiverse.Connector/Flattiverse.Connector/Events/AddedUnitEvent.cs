using Flattiverse.Connector.Hierarchy;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events
{
    public class AddedUnitEvent : UnitEvent
    {
        public readonly Unit Unit;

        internal AddedUnitEvent(Galaxy galaxy, Unit unit) : base(galaxy) 
        {
            Unit = unit;
        }

        public override EventKind Kind => EventKind.UnitAdded;

        public override string ToString()
        {
            return $"{Stamp:HH:mm:ss.fff} UNIADD The unit {Unit} was added to the cluster {Cluster.Name}.";
        }
    }
}
