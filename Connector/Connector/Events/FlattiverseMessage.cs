using System.Text.Json;

namespace Flattiverse.Events
{
    public class FlattiverseMessage : FlattiverseEvent 
    {
        public readonly User? From;

        public readonly DateTime TimeStamp;

        public readonly string Message;

        internal FlattiverseMessage(Connection connection, JsonElement element)
        {
            element.TryGetProperty("message", out JsonElement messageElement);
            if (messageElement.ValueKind != JsonValueKind.Object)
                throw new InvalidDataException("Message property needs to be an object.");

            messageElement.TryGetProperty("sender", out JsonElement senderElement);
            if (senderElement.ValueKind != JsonValueKind.String)
                throw new InvalidDataException("Sender property needs to be a ´string.");

            string sender = senderElement.GetString()!;

            if (string.IsNullOrEmpty(sender))
                throw new InvalidDataException($"Message Sender must not be null or empty");

            connection.tryGetUser(sender, out From);

            messageElement.TryGetProperty("timestamp", out JsonElement stampElement);
            if (stampElement.ValueKind != JsonValueKind.String)
                throw new InvalidDataException("Timestamp property needs to be a ´string.");

            if(!stampElement.TryGetDateTime(out TimeStamp))
                throw new InvalidDataException("Timestamp property could not be parsed to datetime.");

            messageElement.TryGetProperty("text", out JsonElement textElement);
            if (textElement.ValueKind != JsonValueKind.String)
                throw new InvalidDataException("Text property needs to be a ´string.");

            string text = senderElement.GetString()!;

            if (string.IsNullOrEmpty(text))
                throw new InvalidDataException($"Message text must not be null or empty");

            Message = text;
        }
    }
}
