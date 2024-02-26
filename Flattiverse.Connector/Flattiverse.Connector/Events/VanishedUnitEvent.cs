using Flattiverse.Connector.Hierarchy;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events
{
    public class VanishedUnitEvent : UnitEvent
    {
        internal VanishedUnitEvent(Unit unit) : base(unit)
        {
        }

        public override EventKind Kind => EventKind.UnitVanished;

        public override string ToString()
        {
            return $"{Stamp:HH:mm:ss.fff} Unit {Unit.Name} of kind {Unit.Kind} has been removed from cluster {Unit.Cluster.Name}.";
        }
    }
}
