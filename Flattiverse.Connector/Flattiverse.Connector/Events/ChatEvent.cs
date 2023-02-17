using System.Text.Json;

namespace Flattiverse.Connector.Events
{
    public class ChatEvent : MessageEvent
    {
        /// <summary>
        /// The player that sent the message.
        /// </summary>
        public readonly Player Source;

        internal ChatEvent(UniverseGroup group, JsonElement element) : base(element)
        {
            Utils.Traverse(element, out int playerID, "source");
            Source = group.playersId[playerID];
        }
    }
}