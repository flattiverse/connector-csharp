namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Series format of a Flattiverse tournament.
/// </summary>
public enum TournamentMode : byte
{
    /// <summary>
    /// One single decisive match.
    /// </summary>
    Solo = 0x00,

    /// <summary>
    /// First team to win two matches.
    /// </summary>
    BestOf3 = 0x01,

    /// <summary>
    /// First team to win three matches.
    /// </summary>
    BestOf5 = 0x02,

    /// <summary>
    /// First team to win four matches.
    /// </summary>
    BestOf7 = 0x03,

    /// <summary>
    /// First team to win five matches.
    /// </summary>
    BestOf9 = 0x04,

    /// <summary>
    /// First team to win six matches.
    /// </summary>
    BestOf11 = 0x05
}
