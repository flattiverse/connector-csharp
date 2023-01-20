using System.Text.Json;

namespace Flattiverse.Events
{
    public class MessageEvent : FlattiverseEvent
    {
        public readonly DateTime Stamp;

        internal MessageEvent(JsonElement element)
        {
            if (!Utils.Traverse(element, out Stamp, "timestamp"))
                throw new InvalidDataException("Event does not contain valid timestamp property.");
        }

    }
}
