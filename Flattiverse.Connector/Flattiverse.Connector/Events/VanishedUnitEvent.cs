using Flattiverse.Connector.Hierarchy;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events
{
    /// <summary>
    /// Event that is raised when a unit has been removed from the cluster.
    /// </summary>
    public class VanishedUnitEvent : UnitEvent
    {
        internal VanishedUnitEvent(Unit unit) : base(unit)
        {
        }

        /// <inheritdoc/>
        public override EventKind Kind => EventKind.UnitVanished;

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{Stamp:HH:mm:ss.fff} Unit {Unit.Name} of kind {Unit.Kind} has been removed from cluster {Unit.Cluster.Name}.";
        }
    }
}
