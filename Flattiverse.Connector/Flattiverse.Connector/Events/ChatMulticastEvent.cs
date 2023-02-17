using System.Text.Json;

namespace Flattiverse.Connector.Events
{
    /// <summary>
    /// This event informs of a chatmessage to everyone.
    /// </summary>
    [FlattiverseEventIdentifier("chatMulticast")]
    public class ChatMulticastEvent : ChatEvent
    {
        internal ChatMulticastEvent(UniverseGroup group, JsonElement element) : base(group, element)
        {

        }

        public override EventKind Kind => EventKind.ChatMulticast;

        public override string ToString()
        {
            return $"{Stamp:HH:mm:ss.fff} MSG4A Player {Source.Name} has sent a message to everyone: \"{Message}\".";
        }
    }
}