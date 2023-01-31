using System.Text.Json;

namespace Flattiverse.Connector.Events
{
    /// <summary>
    /// This event informs of the disconnect of a player from the universeGroup.
    /// </summary>
    [FlattiverseEventIdentifier("playerRemoved")]
    internal class RemovedPlayerEvent : PlayerEvent
    {
        internal RemovedPlayerEvent(JsonElement element) : base(element) { }

        public override EventKind Kind => EventKind.PlayerRemoved;

        internal override void Process(UniverseGroup group)
        {
            group.players[ID] = null;
        }
    }
}