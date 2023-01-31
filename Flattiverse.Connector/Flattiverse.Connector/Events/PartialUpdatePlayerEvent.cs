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

        internal PartialUpdatePlayerEvent(JsonElement element) : base(element)
        {
            this.element = element;
        }

        internal override void Process(UniverseGroup group)
        {
            group.players[ID].Update(element);
        }

        public override EventKind Kind => EventKind.PlayerPartialUpdate;
    }
}