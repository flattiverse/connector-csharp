using Flattiverse.Connector.Hierarchy;

namespace Flattiverse.Connector.Events;

public class ControllableEvent : PlayerEvent
{
    public readonly ControllableInfo Info;

    internal ControllableEvent(Player player, ControllableInfo info) : base(player)
    {
        Info = info;
    }
}