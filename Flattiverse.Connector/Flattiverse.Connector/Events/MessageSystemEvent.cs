using System.Text.Json;

namespace Flattiverse.Connector.Events
{
    /// <summary>
    /// This event informs of a system message by the server.
    /// </summary>
    [FlattiverseEventIdentifier("messageSystem")]
    public class MessageSystemEvent : MessageEvent
    {
        internal MessageSystemEvent(UniverseGroup group, JsonElement element) : base(element)
        {
        }

        public override EventKind Kind => EventKind.MessageSystem;

        public override string ToString()
        {
            return $"{Stamp:HH:mm:ss.fff} MSGSY --SYSTEM-- \"{Message}\".";
        }
    }
}