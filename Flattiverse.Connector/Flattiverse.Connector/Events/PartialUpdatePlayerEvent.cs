using System.Text.Json;

namespace Flattiverse.Connector.Events
{
    /// <summary>
    /// This event contains only mutable information about a player.
    /// </summary>
    [FlattiverseEventIdentifier("playerPartialUpdate")]
    public class PartialUpdatePlayerEvent : PlayerEvent
    {
        internal JsonElement element;

        internal PartialUpdatePlayerEvent(UniverseGroup group, JsonElement element) : base(element)
        {
            this.element = element;
            group.playersId[ID].Update(element);
        }

        public override EventKind Kind => EventKind.PlayerPartialUpdate;
    }
}