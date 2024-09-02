using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

public class ClosedControllableInfoPlayerEvent : ControllableInfoPlayerEvent
{
    internal ClosedControllableInfoPlayerEvent(Player player, ControllableInfo controllableInfo) :
        base(player, controllableInfo)
    {
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.ControllableInfoClosed;

    /// <inheritdoc/>
    public override string ToString() => $"{Stamp:HH:mm:ss.fff} Player {Player.Name} of Team {Player.Team.Name} closed/disposed controllable {ControllableInfo.Name} of type {ControllableInfo.Kind}.";
}