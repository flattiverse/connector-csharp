using Flattiverse.Connector.Hierarchy;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events;

/// <summary>
/// This event is raised, if a player destroyed a controllable by self destruction.
/// </summary>
public class ShutdownControllableDestroyedEvent : ControllableDestroyedEvent
{
    internal ShutdownControllableDestroyedEvent(Player player, ControllableInfo controllable) : base(player, controllable)
    {
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.DeathByShutdown;

    /// <inheritdoc/>
    public override DestructionReason Reason => DestructionReason.Shutdown;
}