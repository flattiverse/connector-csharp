namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// The game mode of a galaxy.
/// </summary>
public enum GameMode
{
    /// <summary>
    /// In this game mode players try to complete mission objectives.
    /// </summary>
    Mission = 0x00,

    /// <summary>
    /// In this game mode players try to shoot the enemy flag.
    /// </summary>
    ShootTheFlag = 0x01,

    /// <summary>
    /// In this game mode players fight over control points.
    /// </summary>
    Domination = 0x02,
        
    /// <summary>
    /// In this game mode players try to get the fastest time on a track.
    /// </summary>
    Race = 0x03
}