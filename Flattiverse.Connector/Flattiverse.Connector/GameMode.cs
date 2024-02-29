namespace Flattiverse.Connector
{
    /// <summary>
    /// The game mode of a galaxy.
    /// </summary>
    public enum GameMode
    {
        /// <summary>
        /// In this game mode players try to complete mission objectives.
        /// </summary>
        Mission,

        /// <summary>
        /// In this game mode players try to destroy the enemy flag.
        /// </summary>
        STF,

        /// <summary>
        /// In this game mode players fight over control points.
        /// </summary>
        Domination,
        
        /// <summary>
        /// In this game mode players try to get the fastest time on a track.
        /// </summary>
        Race
    }
}
