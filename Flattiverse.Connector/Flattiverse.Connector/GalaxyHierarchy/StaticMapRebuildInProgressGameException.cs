namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// The galaxy is currently rebuilding its static segment data.
/// </summary>
public class StaticMapRebuildInProgressGameException : GameException
{
    internal StaticMapRebuildInProgressGameException() : base(0x3C, "[0x3C] The galaxy is currently rebuilding its static map data.")
    {
    }
}
