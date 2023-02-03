using System.Text.Json;

namespace Flattiverse.Connector.Events
{
    public class PlayerEvent : FlattiverseEvent
    {
        /// <summary>
        /// The ID of the player, representing his slot in the universegroup's player array.
        /// </summary>
        public readonly int ID;

        internal PlayerEvent(JsonElement element) : base()
        {
            Utils.Traverse(element, out ID, "id");
        }
    }
}