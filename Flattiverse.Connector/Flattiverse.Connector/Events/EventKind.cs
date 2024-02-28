namespace Flattiverse.Connector.Events
{
    /// <summary>
    /// Specifies the various event kinds for a better switch() experience.
    /// </summary>
    public enum EventKind : byte
    {
        /// <summary>
        /// A unit was added to the universe.
        /// </summary>
        UnitAdded = 0x00,

        /// <summary>
        /// You received an update for a unit.
        /// </summary>
        UnitUpdated = 0x01,

        /// <summary>
        /// The unit went ouside scanning cone.
        /// </summary>
        UnitVanished = 0x02,

        /// <summary>
        /// A player joined the universe.
        /// </summary>
        JoinedPlayer = 0x04,

        /// <summary>
        /// A player left the universe.
        /// </summary>
        PartedPlayer = 0x05,

        /// <summary>
        /// Someone registerd a ship in the cluster.
        /// </summary>
        JoinedControllable = 0x08,

        /// <summary>
        /// Someone unregistered a ship in the cluster.
        /// </summary>
        PartedControllable = 0x09,
        
        /// <summary>
        /// Someone wrote a message in the chat.
        /// </summary>
        PlayerChat = 0x0C,

        /// <summary>
        /// Someone wrote a message in the team chat.
        /// </summary>
        TeamChat = 0x0D,

        /// <summary>
        /// Someone wrote a message in the galaxy chat.
        /// </summary>
        GalaxyChat = 0x0E,
        
        /// <summary>
        /// The server processed a tick.
        /// </summary>
        GalaxyTick = 0xFF
    }
}
