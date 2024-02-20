using Flattiverse.Connector.Hierarchy;

namespace Flattiverse.Connector.Events
{
    public class PlayerAddedEvent : PlayerEvent
    {
        internal PlayerAddedEvent(Galaxy galaxy, Player player) : base(galaxy, player)
        {
        }

        public override EventKind Kind => EventKind.PlayerAdded;

        public override string ToString()
        {
            return $"{Stamp:HH:mm:ss.fff} PLRADD The player {Player} was removed.";
        }
    }
}
