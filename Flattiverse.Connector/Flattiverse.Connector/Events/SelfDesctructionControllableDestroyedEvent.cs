using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events;

public class SelfDesctructionControllableDestroyedEvent : ControllableDestroyedEvent
{
    public SelfDesctructionControllableDestroyedEvent(Controllable controllable) : base(controllable)
    {
    }

    public override EventKind Kind => EventKind.DeathBySelfDestruction;

    public override DestructionReason Reason => DestructionReason.SelfDestruction;
}