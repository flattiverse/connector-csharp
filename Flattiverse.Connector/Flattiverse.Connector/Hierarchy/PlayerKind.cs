namespace Flattiverse.Connector.Hierarchy
{
    /// <summary>
    /// The type/kind/role of a player.
    /// </summary>
    public enum PlayerKind
    {
        /// <summary>
        /// A regular player can spawn ships and control them.
        /// </summary>
        Player = 0x01,

        /// <summary>
        /// A spectator can only watch the game.
        /// </summary>
        Spectator = 0x02,

        /// <summary>
        /// Functions similar to a spectator but can also control the game.
        /// </summary>
        Admin = 0x04,
    }
}
