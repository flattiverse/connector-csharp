using Flattiverse.Connector.Units;
using System.Text.Json;

namespace Flattiverse.Connector.Events
{
    /// <summary>
    /// This event informs of the addition of a unit to the universeGroup.
    /// </summary>
    [FlattiverseEventIdentifier("unitAdded")]
    public class AddedUnitEvent : UnitEvent
    {
        public readonly Unit Unit;

        internal AddedUnitEvent(UniverseGroup group, JsonElement element) : base(element)
        {
            Utils.Traverse(element, out JsonElement unit, "unit");

            Unit = Unit.CreateFromJson(unit);
        }

        public override EventKind Kind => EventKind.UnitAdded;
    }
}