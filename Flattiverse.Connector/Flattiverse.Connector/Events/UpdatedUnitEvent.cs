using Flattiverse.Connector.Hierarchy;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events
{
    public class UpdatedUnitEvent : UnitEvent
    {
        public readonly Unit Unit;

        internal UpdatedUnitEvent(Galaxy galaxy, Unit unit) : base(galaxy)
        {
            Unit = unit;
        }

        public override EventKind Kind => EventKind.UnitUpdated;

        public override string ToString()
        {
            return $"{Stamp:HH:mm:ss.fff} UNIUP The unit {Unit} in cluster {Cluster.Name} was updated.";
        }
    }
}
