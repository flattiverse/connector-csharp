using Flattiverse.Connector.Hierarchy;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events;

public class ShutdownControllableDestroyedEvent : ControllableDestroyedEvent
{
    public ShutdownControllableDestroyedEvent(Player player, ControllableInfo controllable) : base(player, controllable)
    {
    }

    public override EventKind Kind => EventKind.DeathByShutdown;

    public override DestructionReason Reason => DestructionReason.Shutdown;
}