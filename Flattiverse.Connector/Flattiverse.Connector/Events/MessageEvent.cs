using System.Text.Json;

namespace Flattiverse.Connector.Events
{
    public class MessageEvent : FlattiverseEvent
    {
        /// <summary>
        /// The message received.
        /// </summary>
        public readonly string Message;

        internal MessageEvent(JsonElement element) : base()
        {
            Utils.Traverse(element, out Message, "message");
        }
    }
}