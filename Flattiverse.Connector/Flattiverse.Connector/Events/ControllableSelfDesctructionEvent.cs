using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events;

public class ControllableSelfDesctructionEvent : ControllableDestroyedEvent
{
    public ControllableSelfDesctructionEvent(Controllable controllable) : base(controllable)
    {
    }

    public override EventKind Kind => EventKind.DeathBySelfDestruction;

    public override DestructionReason Reason => DestructionReason.SelfDestruction;
}