using Flattiverse.Connector.Hierarchy;

namespace Flattiverse.Connector.Events
{
    public class PartedPlayerEvent : PlayerEvent
    {
        internal PartedPlayerEvent(Player player) : base(player)
        {
        }

        public override EventKind Kind => EventKind.PartedPlayer;

        public override string ToString()
        {
            return $"{Stamp:HH:mm:ss.fff} {Player.Name} on team {Player.Team.Name} left the galaxy.";
        }
    }
}
