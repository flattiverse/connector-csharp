using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Raised when a public controllable entry becomes alive in the world again.
/// </summary>
public class ContinuedControllableInfoPlayerEvent : ControllableInfoPlayerEvent
{
    internal ContinuedControllableInfoPlayerEvent(Player player, ControllableInfo controllableInfo) :
        base(player, controllableInfo)
    {
    }

    /// <inheritdoc />
    public override EventKind Kind => EventKind.ControllableInfoContinued;

    /// <summary>
    /// Returns a compact diagnostic representation of the event.
    /// </summary>
    public override string ToString() => $"{Stamp:HH:mm:ss.fff} Player {Player.Name} of Team {Player.Team.Name} continued controllable {ControllableInfo.Name} of type {ControllableInfo.Kind}.";
}
