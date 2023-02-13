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
    }
}