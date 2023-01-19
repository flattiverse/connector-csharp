using Flattiverse.Message.Chat;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Xml.Linq;

namespace Flattiverse.Message
{
    public class Message
    {
        public readonly DateTime TimeStamp;

        internal virtual ChatMessageType type => throw new NotImplementedException();

        internal Message()
        {
            TimeStamp = DateTime.Now;
        }

        internal virtual string ToJson() => throw new NotImplementedException();

        internal static Message FromJson(JsonElement data, Connection connection)
        {
            JsonElement element;

            string? type;

            if (!data.TryGetProperty("type", out element))
                throw new Exception($"Type property not found.");

            if (element.ValueKind != JsonValueKind.String)
                throw new Exception($"Type property must be a string. Received {element.ValueKind}.");

            type = element.GetString();

            if (string.IsNullOrWhiteSpace(type))
                throw new Exception($"Type can't be null or empty.");

            if (!Enum.TryParse<ChatMessageType>(type, out ChatMessageType messageType))
                throw new Exception($"Unkown message type {type}");

            if (!data.TryGetProperty("message", out element))
                throw new Exception($"Message property not found.");

            if (element.ValueKind != JsonValueKind.Object)
                throw new Exception($"Type property must be a object. Received {element.ValueKind}.");

            Message message;

            switch(messageType)
            {
                case ChatMessageType.Uni:
                    return new UniCastChatMessage(element, connection);
                case ChatMessageType.Broad:
                    return new BroadCastChatMessage(element, connection);
                case ChatMessageType.Team:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
