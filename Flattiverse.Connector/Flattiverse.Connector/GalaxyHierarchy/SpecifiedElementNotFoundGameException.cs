namespace Flattiverse.Connector.GalaxyHierarchy;

public class SpecifiedElementNotFoundGameException : GameException
{
    internal SpecifiedElementNotFoundGameException() : base(0x05, "[0x05] No or non-existent team specified.")
    {
    }
}