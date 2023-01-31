using Flattiverse.Connector.Accounts;
using System.Text.Json;

namespace Flattiverse.Connector.Events
{
    /// <summary>
    /// This event contains all information abaout a player.
    /// </summary>
    [FlattiverseEventIdentifier("playerFullUpdate")]
    public class FullUpdatePlayerEvent : PlayerEvent
    {
        public readonly Player Player;

        internal FullUpdatePlayerEvent(JsonElement element) : base(element)
        {
            Player = new Player(element);
        }

        internal override void Process(UniverseGroup group)
        {
            group.players[ID] = Player;
        }

        public override EventKind Kind => EventKind.PlayerFullUpdate;
    }
}