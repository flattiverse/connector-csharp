namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// The requested action targets a controllable that is currently rebuilding one subsystem.
/// </summary>
public class ControllableIsRebuildingGameException : GameException
{
    internal ControllableIsRebuildingGameException() :
        base(0x3F, "[0x3F] This controllable is currently rebuilding and cannot execute commands.")
    {
    }
}
