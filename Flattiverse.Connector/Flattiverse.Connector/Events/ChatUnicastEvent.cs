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
            Destination = group.playersId[playerID];
        }

        public override EventKind Kind => EventKind.ChatUnicast;

        public override string ToString()
        {
            return $"{Stamp:HH:mm:ss.fff} MSG4U Player {Source.Name} has sent a message just for you.";
        }
    }
}