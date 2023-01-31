using System.Text.Json;

namespace Flattiverse.Connector.Events
{
    /// <summary>
    /// This event informs of the disconnect of a player from the universeGroup.
    /// </summary>
    [FlattiverseEventIdentifier("playerRemoved")]
    public class RemovedPlayerEvent : PlayerEvent
    {
        internal RemovedPlayerEvent(JsonElement element) : base(element) { }

        internal override void Process(UniverseGroup group)
        {
            group.players[ID] = null;
        }

        public override EventKind Kind => EventKind.PlayerRemoved;
    }
}