using System.Text.Json;

namespace Flattiverse.Connector.Events
{
    /// <summary>
    /// This event informs of the addition of a unit to the universeGroup.
    /// </summary>
    [FlattiverseEventIdentifier("unitAdded")]
    public class AddedUnitEvent : UnitEvent
    {
        internal AddedUnitEvent(UniverseGroup group, JsonElement element) : base(element)
        {

        }

        public override EventKind Kind => EventKind.UnitAdded;
    }
}
