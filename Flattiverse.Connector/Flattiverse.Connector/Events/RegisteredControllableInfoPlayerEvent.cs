using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Someone registered a PlayerUnit.
/// </summary>
public class RegisteredControllableInfoPlayerEvent : ControllableInfoPlayerEvent
{
    internal RegisteredControllableInfoPlayerEvent(Player player, ControllableInfo controllableInfo) :
        base(player, controllableInfo)
    {
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.ControllableInfoRegistered;

    /// <inheritdoc/>
    public override string ToString() => $"{Stamp:HH:mm:ss.fff} Player {Player.Name} of Team {Player.Team.Name} registered controllable {ControllableInfo.Name} of type {ControllableInfo.Kind}.";
}