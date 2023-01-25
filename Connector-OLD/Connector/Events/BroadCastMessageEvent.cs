using System.Text.Json;

namespace Flattiverse.Events
{
    public class BroadCastMessageEvent : ChatMessageEvent
    {
        internal BroadCastMessageEvent(JsonElement element) : base(element)
        {
        }
    }
}
