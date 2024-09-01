namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Thrown, if you did forget to specify a proper team.
/// </summary>
public class InvalidOrMissingTeamGameException : GameException
{
    internal InvalidOrMissingTeamGameException() : base(0x05, "[0x05] No or non-existent team specified.")
    {
    }
}