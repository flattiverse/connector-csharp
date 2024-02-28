using Flattiverse.Connector.Hierarchy;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events
{
    /// <summary>
    /// Event that is raised when a unit has been added to the cluster.
    /// </summary>
    public class AddedUnitEvent : UnitEvent
    {
        internal AddedUnitEvent(Unit unit) : base(unit) 
        {
        }

        /// <inheritdoc/>
        public override EventKind Kind => EventKind.UnitAdded;

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{Stamp:HH:mm:ss.fff} Unit {Unit.Name} of kind {Unit.Kind} has been added to the cluster {Unit.Cluster.Name}.";
        }
    }
}
