using Flattiverse.Connector.Hierarchy;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events
{
    public class VanishedUnitEvent : UnitEvent
    {
        public readonly Unit Unit;

        internal VanishedUnitEvent(Galaxy galaxy, Unit unit) : base(galaxy)
        {
            Unit = unit;
        }

        public override EventKind Kind => EventKind.UnitVanished;

        public override string ToString()
        {
            return $"{Stamp:HH:mm:ss.fff} UNIVAN The unit {Unit} in cluster {Cluster.Name} vanished.";
        }
    }
}
