using Flattiverse.Connector.Hierarchy;

namespace Flattiverse.Connector.Events
{
    public class PlayerRemovedEvent : PlayerEvent
    {
        internal PlayerRemovedEvent(Galaxy galaxy, Player player) : base(galaxy, player)
        {
        }

        public override EventKind Kind => EventKind.PlayerRemoved;

        public override string ToString()
        {
            return $"{Stamp:HH:mm:ss.fff} PLRREM The player {Player} was removed.";
        }
    }
}
