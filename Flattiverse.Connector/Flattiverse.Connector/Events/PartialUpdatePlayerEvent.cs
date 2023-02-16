using System.Text.Json;

namespace Flattiverse.Connector.Events
{
    /// <summary>
    /// This event contains only mutable information about a player.
    /// </summary>
    [FlattiverseEventIdentifier("playerPartialUpdate")]
    public class PartialUpdatePlayerEvent : PlayerEvent
    {
        internal PartialUpdatePlayerEvent(UniverseGroup group, JsonElement element) : base(group, element)
        {
            group.playersId[Player.ID].Update(element);
        }

        public override EventKind Kind => EventKind.PlayerPartialUpdate;

        public override string ToString()
        {
            return $"{Stamp:HH:mm:ss.fff} PLRUP Player {Player.Name} was updated.";
        }
    }
}