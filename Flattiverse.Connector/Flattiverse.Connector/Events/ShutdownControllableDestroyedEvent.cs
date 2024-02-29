using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events;

public class ShutdownControllableDestroyedEvent : ControllableDestroyedEvent
{
    public ShutdownControllableDestroyedEvent(Controllable controllable) : base(controllable)
    {
    }

    public override EventKind Kind => EventKind.DeathByShutdown;

    public override DestructionReason Reason => DestructionReason.Shutdown;
}