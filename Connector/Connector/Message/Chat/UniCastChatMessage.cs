using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Flattiverse.Message.Chat
{
    internal class UniCastChatMessage : ChatMessage
    {
        public User ToUser;

        public readonly string Message;

        internal override ChatMessageType type => ChatMessageType.Uni;

        public UniCastChatMessage(User toUser, string message) : base()
        {
            ToUser = toUser;
            Message = message;
        }

        internal UniCastChatMessage(JsonElement data, Connection connection) 
        { 
        
        }

        internal override string ToJson()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (Utf8JsonWriter writer = new Utf8JsonWriter(ms, Connection.jsonOptions))
                {
                    writer.WriteStartObject();

                    writer.WriteNumber("userId", ToUser.Id);

                    writer.WritePropertyName("message");

                    writer.WriteRawValue(Message);

                    writer.WriteEndObject();
                }

                ms.Position = 0;
                return new StreamReader(ms).ReadToEnd();
            }
        }
    }
}
