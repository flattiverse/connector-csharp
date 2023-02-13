using Flattiverse.Connector.Accounts;
using System.Text.Json;

namespace Flattiverse.Connector.Events
{
    /// <summary>
    /// This event informs of a chatmessage to a player.
    /// </summary>
    [FlattiverseEventIdentifier("chatUnicast")]
    public class ChatUnicastEvent : ChatEvent
    {
        public readonly Player Destination;
        internal ChatUnicastEvent(UniverseGroup group, JsonElement element) : base(group, element)
        {
            Utils.Traverse(element, out int playerID, "destination");
            Destination = group.players[playerID];
        }

        public override EventKind Kind => EventKind.ChatUnicast;
    }
}