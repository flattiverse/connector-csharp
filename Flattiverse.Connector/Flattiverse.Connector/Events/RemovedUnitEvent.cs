using System.Text.Json;

namespace Flattiverse.Connector.Events
{
    /// <summary>
    /// This event informs of the removal of a unit from the universeGroup.
    /// </summary>
    [FlattiverseEventIdentifier("unitRemoved")]
    public class RemovedUnitEvent : UnitEvent
    {
        internal RemovedUnitEvent(JsonElement element) : base(element) { }

        internal override void Process(UniverseGroup group)
        {

        }

        public override EventKind Kind => EventKind.UnitRemoved;
    }
}