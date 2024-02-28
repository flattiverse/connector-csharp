using Flattiverse.Connector.Hierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Event that is raised when a controllable has left the cluster.
/// </summary>
public class PartedControllableEvent : ControllableEvent
{
    internal PartedControllableEvent(Player player, ControllableInfo info) : base(player, info)
    {
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.PartedControllable;


    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} {Player.Name}'s controllable \"{Info.Name}\" based on design {Info.ShipDesign.Name} left the galaxy.";
    }
}