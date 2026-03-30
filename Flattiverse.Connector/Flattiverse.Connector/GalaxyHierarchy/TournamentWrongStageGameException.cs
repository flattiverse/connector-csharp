namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Protocol error <c>0x33</c>: thrown when a tournament lifecycle action is requested in a stage that does not permit
/// it.
/// </summary>
public class TournamentWrongStageGameException : GameException
{
    internal TournamentWrongStageGameException() : base(0x33, "[0x33] This tournament action is not allowed in the current stage.")
    {
    }
}
