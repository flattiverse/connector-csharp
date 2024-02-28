using Flattiverse.Connector.Hierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Event that is raised when an update about a controllable is received.
/// </summary>
public class ControllableEvent : PlayerEvent
{
    /// <summary>
    /// The controllable that was affected by the event.
    /// </summary>
    public readonly ControllableInfo Info;

    internal ControllableEvent(Player player, ControllableInfo info) : base(player)
    {
        Info = info;
    }
}