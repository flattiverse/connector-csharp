using System.Text.Json;

namespace Flattiverse.Events
{
    public class ChatMessageEvent : MessageEvent
    {
        public readonly string SenderUserName;
        public readonly string Text;

        internal ChatMessageEvent(JsonElement element) : base(element) 
        {
            if (!Utils.Traverse(element, out SenderUserName, false, "sender"))
                throw new InvalidDataException("Message does not contain valid sender property.");

            if (!Utils.Traverse(element, out Text, false, "text"))
                throw new InvalidDataException("Message does not contain valid text property.");
        }
    }
}
