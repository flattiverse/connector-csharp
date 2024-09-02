using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Someone continued a PlayerUnit.
/// </summary>
public class ContinuedControllableInfoPlayerEvent : ControllableInfoPlayerEvent
{
    internal ContinuedControllableInfoPlayerEvent(Player player, ControllableInfo controllableInfo) :
        base(player, controllableInfo)
    {
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.ControllableInfoContinued;

    /// <inheritdoc/>
    public override string ToString() => $"{Stamp:HH:mm:ss.fff} Player {Player.Name} of Team {Player.Team.Name} continued controllable {ControllableInfo.Name} of type {ControllableInfo.Kind}.";
}