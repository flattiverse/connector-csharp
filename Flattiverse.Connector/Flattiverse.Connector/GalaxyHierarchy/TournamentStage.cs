namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Lifecycle stage of a configured tournament.
/// </summary>
public enum TournamentStage : byte
{
    /// <summary>
    /// Tournament exists but has not been commenced yet.
    /// </summary>
    Preparation = 0x00,

    /// <summary>
    /// Tournament has been commenced and is in its pre-run stage.
    /// </summary>
    Commencing = 0x01,

    /// <summary>
    /// Tournament is currently running.
    /// </summary>
    Running = 0x02
}
