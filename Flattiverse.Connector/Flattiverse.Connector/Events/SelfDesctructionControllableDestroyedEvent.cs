using Flattiverse.Connector.Hierarchy;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events;

public class SelfDesctructionControllableDestroyedEvent : ControllableDestroyedEvent
{
    public SelfDesctructionControllableDestroyedEvent(Player player, ControllableInfo controllableInfo) : base(player, controllableInfo)
    {
    }

    public override EventKind Kind => EventKind.DeathBySelfDestruction;

    public override DestructionReason Reason => DestructionReason.SelfDestruction;
}