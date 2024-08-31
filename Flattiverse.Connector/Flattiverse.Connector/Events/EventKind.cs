namespace Flattiverse.Connector.Events;

/// <summary>
/// Specifies the various event kinds for a better switch() experience.
/// </summary>
public enum EventKind : byte
{
    ConnectionTerminated = 0xF0,
    GalaxyTick = 0xFF
}