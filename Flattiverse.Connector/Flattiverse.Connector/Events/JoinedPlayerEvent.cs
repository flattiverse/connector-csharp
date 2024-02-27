using Flattiverse.Connector.Hierarchy;

namespace Flattiverse.Connector.Events
{
    public class JoinedPlayerEvent : PlayerEvent
    {
        internal JoinedPlayerEvent(Player player) : base(player)
        {
        }

        public override EventKind Kind => EventKind.JoinedPlayer;

        public override string ToString()
        {
            return $"{Stamp:HH:mm:ss.fff} {Player.Name} joined the galaxy on team {Player.Team.Name}.";
        }
    }
}
