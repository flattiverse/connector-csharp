using System.Text.Json;

namespace Flattiverse.Connector.Events
{
    public class PlayerEvent : FlattiverseEvent
    {
        /// <summary>
        /// The ID of the player, representing his slot in the universegroup's player array.
        /// </summary>
        public readonly Player Player;

        internal PlayerEvent(UniverseGroup group, JsonElement element) : base()
        {
            Utils.Traverse(element, out int ID, "id");
            Player = group.playersId[ID];
        }
    }
}