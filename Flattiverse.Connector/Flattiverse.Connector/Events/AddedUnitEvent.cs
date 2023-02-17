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

        internal AddedUnitEvent(UniverseGroup group, JsonElement element) : base(group, element)
        {
            // TOG: Wirklich überall wollen wir entsprechend Failure Events senden, wenn etwas schief geht und es keine andere Möglichkeit des Reports gibt.
            // In den meisten Fällen muss der Rückgabewert von Utils.Traverse also überprüft werden.
            Utils.Traverse(element, out JsonElement unit, "unit");

            Unit = Unit.CreateFromJson(group, unit);
        }

        public override EventKind Kind => EventKind.UnitAdded;

        public override string ToString()
        {
            return $"{Stamp:HH:mm:ss.fff} ADDED The unit {Unit} was added to the universe {Universe.Name}.";
        }
    }
}