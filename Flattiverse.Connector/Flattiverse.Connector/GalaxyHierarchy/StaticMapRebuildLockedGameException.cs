namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Static segment rebuilding is currently blocked by tournament state.
/// </summary>
public class StaticMapRebuildLockedGameException : GameException
{
    internal StaticMapRebuildLockedGameException() : base(0x3D, "Static map rebuilding is currently blocked by the tournament state.")
    {
    }
}
