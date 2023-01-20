using System.Text.Json;

namespace Flattiverse.Events
{
    public class BroadCastMessageEvent : ChatMessageEvent
    {
        public readonly string ReceiverUserName;

        internal BroadCastMessageEvent(JsonElement element) : base(element)
        {
            if (!Utils.Traverse(element, out ReceiverUserName, false, "receiver"))
                throw new InvalidDataException("Message does not contain valid receiver property.");
        }
    }
}
