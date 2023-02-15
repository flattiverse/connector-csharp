using System.Text.Json;

namespace Flattiverse.Connector.Events
{
    /// <summary>
    /// This event informs of the disconnect of a player from the universeGroup.
    /// </summary>
    [FlattiverseEventIdentifier("playerRemoved")]
    public class RemovedPlayerEvent : PlayerEvent
    {
        internal RemovedPlayerEvent(UniverseGroup group, JsonElement element) : base(group, element) 
        {
            group.playersId[Player.ID] = null;
        }

        public override EventKind Kind => EventKind.PlayerRemoved;
    }
}