using Flattiverse.Connector.Hierarchy;

namespace Flattiverse.Connector.Events
{
    public class PlayerPartedEvent : PlayerEvent
    {
        internal PlayerPartedEvent(Player player) : base(player)
        {
        }

        public override EventKind Kind => EventKind.PlayerRemoved;

        public override string ToString()
        {
            return $"{Stamp:HH:mm:ss.fff} {Player.Name} on team {Player.Team.Name} left the galaxy.";
        }
    }
}
