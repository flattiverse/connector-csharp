using Flattiverse.Connector.Hierarchy;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events;

/// <summary>
/// This event is raised, if a player destroyed a controllable by self destruction.
/// </summary>
public class SelfDesctructionControllableDestroyedEvent : ControllableDestroyedEvent
{
    public SelfDesctructionControllableDestroyedEvent(Player player, ControllableInfo controllableInfo) : base(player, controllableInfo)
    {
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.DeathBySelfDestruction;

    /// <inheritdoc/>
    public override DestructionReason Reason => DestructionReason.SelfDestruction;
}