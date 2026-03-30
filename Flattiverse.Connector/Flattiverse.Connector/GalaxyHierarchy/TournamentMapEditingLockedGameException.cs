namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Protocol error <c>0x34</c>: thrown when galaxy, region, or editable-unit map editing is attempted while any
/// tournament is configured.
/// </summary>
public class TournamentMapEditingLockedGameException : GameException
{
    internal TournamentMapEditingLockedGameException() : base(0x34, "[0x34] Map editing is locked while a tournament exists.")
    {
    }
}
