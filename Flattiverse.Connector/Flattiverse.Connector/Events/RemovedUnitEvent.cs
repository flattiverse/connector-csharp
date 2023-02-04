using System.Text.Json;

namespace Flattiverse.Connector.Events
{
    /// <summary>
    /// This event informs of the removal of a unit from the universeGroup.
    /// </summary>
    [FlattiverseEventIdentifier("unitRemoved")]
    public class RemovedUnitEvent : UnitEvent
    {
        /// <summary>
        /// The name of the unit, representing his slot in the universegroup's player array.
        /// </summary>
        public readonly string Name;

        internal RemovedUnitEvent(UniverseGroup group, JsonElement element) : base(element)
        {
            Utils.Traverse(element, out Name, "name");
        }

        public override EventKind Kind => EventKind.UnitRemoved;
    }
}