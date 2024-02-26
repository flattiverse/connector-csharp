using Flattiverse.Connector.Hierarchy;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events
{
    public class AddedUnitEvent : UnitEvent
    {
        internal AddedUnitEvent(Unit unit) : base(unit) 
        {
        }

        public override EventKind Kind => EventKind.UnitAdded;

        public override string ToString()
        {
            return $"{Stamp:HH:mm:ss.fff} Unit {Unit.Name} of kind {Unit.Kind} has been added to the cluster {Unit.Cluster.Name}.";
        }
    }
}
