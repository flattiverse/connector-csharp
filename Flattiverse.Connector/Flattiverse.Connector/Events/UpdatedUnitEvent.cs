using Flattiverse.Connector.Hierarchy;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events
{
    /// <summary>
    /// Event that is raised when a unit has been updated in the cluster.
    /// </summary>
    public class UpdatedUnitEvent : UnitEvent
    {
        internal UpdatedUnitEvent(Unit unit) : base(unit)
        {
        }

        /// <inheritdoc/>
        public override EventKind Kind => EventKind.UnitUpdated;

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{Stamp:HH:mm:ss.fff} Unit {Unit.Name} of kind {Unit.Kind} in the cluster {Unit.Cluster.Name} has been updated.";
        }
    }
}
