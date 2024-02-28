using Flattiverse.Connector.Hierarchy;

namespace Flattiverse.Connector.Events
{
    /// <summary>
    /// Event that is raised when a player has joined the galaxy.
    /// </summary>
    public class JoinedPlayerEvent : PlayerEvent
    {
        internal JoinedPlayerEvent(Player player) : base(player)
        {
        }

        /// <inheritdoc/>
        public override EventKind Kind => EventKind.JoinedPlayer;

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{Stamp:HH:mm:ss.fff} {Player.Name} joined the galaxy on team {Player.Team.Name}.";
        }
    }
}
