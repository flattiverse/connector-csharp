using Flattiverse.Connector.Hierarchy;

namespace Flattiverse.Connector.Events
{
    /// <summary>
    /// Event that is raised when a player has left the galaxy.
    /// </summary>
    public class PartedPlayerEvent : PlayerEvent
    {
        internal PartedPlayerEvent(Player player) : base(player)
        {
        }

        /// <inheritdoc/>
        public override EventKind Kind => EventKind.PartedPlayer;

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{Stamp:HH:mm:ss.fff} {Player.Name} on team {Player.Team.Name} left the galaxy.";
        }
    }
}
