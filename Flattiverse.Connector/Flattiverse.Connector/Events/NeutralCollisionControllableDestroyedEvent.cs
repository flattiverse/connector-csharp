using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events;

public class NeutralCollisionControllableDestroyedEvent : ControllableDestroyedEvent
{
    public readonly UnitKind UnitKind;
    public readonly string UnitName;
    
    internal NeutralCollisionControllableDestroyedEvent(PacketReader reader) : base(reader)
    {
        UnitKind = (UnitKind)reader.ReadByte();
        UnitName = reader.ReadString();
    }
    
    
}