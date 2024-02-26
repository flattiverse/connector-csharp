using Flattiverse.Connector.Hierarchy;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events
{
    public class UpdatedUnitEvent : UnitEvent
    {
        internal UpdatedUnitEvent(Unit unit) : base(unit)
        {
        }

        public override EventKind Kind => EventKind.UnitUpdated;

        public override string ToString()
        {
            return $"{Stamp:HH:mm:ss.fff} Unit {Unit.Name} of kind {Unit.Kind} in the cluster {Unit.Cluster.Name} has been updated.";
        }
    }
}
