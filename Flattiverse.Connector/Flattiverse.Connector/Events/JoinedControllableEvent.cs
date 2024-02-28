using Flattiverse.Connector.Hierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Event that is raised when a controllable has been registerd in the cluster.
/// </summary>
public class JoinedControllableEvent : ControllableEvent
{
    internal JoinedControllableEvent(Player player, ControllableInfo info) : base(player, info)
    {
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.JoinedControllable;

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} {Player.Name}'s controllable \"{Info.Name}\" based on design {Info.ShipDesign.Name} joined the galaxy.";
    }
}