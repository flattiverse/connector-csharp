namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Thrown, if a specified element like a player isn't found.
/// </summary>
public class SpecifiedElementNotFoundGameException : GameException
{
    internal SpecifiedElementNotFoundGameException() : base(0x05, "[0x05] No or non-existent team specified.")
    {
    }
}