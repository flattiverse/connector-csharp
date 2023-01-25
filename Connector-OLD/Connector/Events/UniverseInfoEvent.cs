using System.Text.Json;

namespace Flattiverse.Events
{
    public class UniverseInfoEvent : FlattiverseEvent
    {
        public readonly short UniverseId;

        internal UniverseInfoEvent(JsonElement element)
        {
            int universeId;

            if (!Utils.Traverse(element, out universeId, "universe"))
                throw new InvalidDataException("Event does not contain valid universe property.");

            UniverseId = (short)universeId;
        }
    }
}
