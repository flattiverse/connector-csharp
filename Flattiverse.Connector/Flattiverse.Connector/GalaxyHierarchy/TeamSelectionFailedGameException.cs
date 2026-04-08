namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Thrown, if the requested team selection failed.
/// </summary>
public class TeamSelectionFailedGameException : GameException
{
    internal TeamSelectionFailedGameException() : base(0x05, "[0x05] Invalid team specified or no playable team available for auto-selection.")
    {
    }
}
