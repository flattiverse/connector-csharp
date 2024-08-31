namespace Flattiverse.Connector.GalaxyHierarchy;

public class InvalidOrMissingTeamGameException : GameException
{
    internal InvalidOrMissingTeamGameException() : base(0x05, "[0x05] No or non-existent team specified.")
    {
    }
}