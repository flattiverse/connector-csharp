using System.Text.Json;

namespace Flattiverse.Connector.Events
{
    /// <summary>
    /// This event informs of the disconnect of a player from the universeGroup.
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