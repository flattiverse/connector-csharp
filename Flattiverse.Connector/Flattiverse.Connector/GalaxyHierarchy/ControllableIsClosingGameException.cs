namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// The requested continue action targets a controllable that is already closing.
/// </summary>
public class ControllableIsClosingGameException : GameException
{
    internal ControllableIsClosingGameException() : base(0x17, "[0x17] Can't continue a controllable that is already closing.")
    {
    }
}
