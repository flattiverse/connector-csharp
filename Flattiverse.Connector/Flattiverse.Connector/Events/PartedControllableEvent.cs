using Flattiverse.Connector.Hierarchy;

namespace Flattiverse.Connector.Events;

public class PartedControllableEvent : ControllableEvent
{
    internal PartedControllableEvent(Player player, ControllableInfo info) : base(player, info)
    {
    }

    public override EventKind Kind => EventKind.PartedControllable;

    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} {Player.Name}'s controllable \"{Info.Name}\" based on design {Info.ShipDesign.Name} left the galaxy.";
    }
}