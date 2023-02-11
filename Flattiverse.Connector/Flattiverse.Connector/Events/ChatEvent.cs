using Flattiverse.Connector.Accounts;
using System.Text.Json;

namespace Flattiverse.Connector.Events
{
    public class ChatEvent : FlattiverseEvent
    {
        /// <summary>
        /// The player that sent the message.
        /// </summary>
        public readonly Player Source;

        /// <summary>
        /// The chat message received.
        /// </summary>
        public readonly string Message;

        internal ChatEvent(UniverseGroup group, JsonElement element) : base()
        {
            Utils.Traverse(element, out Message, "message");
            Utils.Traverse(element, out int playerID, "source");
            Source = group.players[playerID];
        }
    }
}