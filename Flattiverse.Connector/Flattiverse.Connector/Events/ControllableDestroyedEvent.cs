using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events;

public class ControllableDestroyedEvent : FlattiverseEvent
{
    public readonly DestructionReason Reason;

    internal ControllableDestroyedEvent(PacketReader reader)
    {
        Reason = (DestructionReason)reader.ReadByte();
    }
}