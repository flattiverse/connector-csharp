using Flattiverse.Connector.Hierarchy;

namespace Flattiverse.Connector.Events;

public class JoinedControllableEvent : ControllableEvent
{
    internal JoinedControllableEvent(Player player, ControllableInfo info) : base(player, info)
    {
    }

    public override EventKind Kind => EventKind.JoinedControllable;

    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} {Player.Name}'s controllable \"{Info.Name}\" based on design {Info.ShipDesign.Name} joined the galaxy.";
    }
}