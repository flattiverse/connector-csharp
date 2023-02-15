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

        internal FullUpdatePlayerEvent(UniverseGroup group, JsonElement element) : base(group, element)
        {
            Player = new Player(group, element);
            group.playersId[Player.ID] = Player;
        }

        public override EventKind Kind => EventKind.PlayerFullUpdate;
    }
}