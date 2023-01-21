using System.Text.Json;

namespace Flattiverse.Events
{
    public class FlattiverseMessage : FlattiverseEvent 
    {
        public readonly string FromUserName;

        public readonly DateTime TimeStamp;

        public readonly string Message;

        internal FlattiverseMessage(Connection connection, JsonElement element)
        {
            JsonElement subElement;

            if (!Utils.Traverse(element, out subElement, JsonValueKind.Object, "message"))
                throw new InvalidDataException("Event does not contain a valid message property.");

            if(!Utils.Traverse(subElement, out FromUserName, false, "sender"))
                throw new InvalidDataException("Message does not contain a valid sender property.");

            if (!Utils.Traverse(subElement, out TimeStamp, "timestamp"))
                throw new InvalidDataException("Message does not contain a valid timestamp property.");


            if (!Utils.Traverse(subElement, out Message, false, "text"))
                throw new InvalidDataException("Message does not contain a valid text property.");
        }
    }
}
