using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events;

public class ControllableShutdownEvent : ControllableDestroyedEvent
{
    public ControllableShutdownEvent(Controllable controllable) : base(controllable)
    {
    }

    public override EventKind Kind => EventKind.DeathByShutdown;

    public override DestructionReason Reason => DestructionReason.Shutdown;
}