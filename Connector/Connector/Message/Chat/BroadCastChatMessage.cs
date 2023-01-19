using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Flattiverse.Message.Chat
{
    public class BroadCastChatMessage : ChatMessage
    {
        public readonly string Message;

        public readonly User FromUser;

        internal override ChatMessageType type => ChatMessageType.Broad;

        public BroadCastChatMessage(User fromUser, string message) : base()
        {
            Message = message;
            FromUser = fromUser;
        }

        internal BroadCastChatMessage(JsonElement data, Connection connection)
        {

        }

    }
}
