using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Raised when a public controllable entry is finally closed and removed.
/// This is the final close, not the initial close request.
/// </summary>
public class ClosedControllableInfoPlayerEvent : ControllableInfoPlayerEvent
{
    internal ClosedControllableInfoPlayerEvent(Player player, ControllableInfo controllableInfo) :
        base(player, controllableInfo)
    {
    }

    /// <inheritdoc />
    public override EventKind Kind => EventKind.ControllableInfoClosed;

    /// <summary>
    /// Returns a compact diagnostic representation of the event.
    /// </summary>
    public override string ToString() => $"{Stamp:HH:mm:ss.fff} Player {Player.Name} of Team {Player.Team.Name} closed controllable {ControllableInfo.Name} of type {ControllableInfo.Kind}.";
}
