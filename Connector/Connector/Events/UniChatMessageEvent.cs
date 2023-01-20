using System.Text.Json;

namespace Flattiverse.Events
{
    public class UniChatMessageEvent : ChatMessageEvent
    {
        internal UniChatMessageEvent(JsonElement element) : base(element)
        {

        }
    }
}
