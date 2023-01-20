using System.Text.Json;

namespace Flattiverse.Events
{
    public class UnitEvent : FlattiverseEvent
    {
        public readonly int UniverseId;

        internal UnitEvent(JsonElement element)
        {
            if (!Utils.Traverse(element, out UniverseId, "universe"))
                throw new InvalidDataException("Event does not contain valid universe property.");
        }
    }
}
