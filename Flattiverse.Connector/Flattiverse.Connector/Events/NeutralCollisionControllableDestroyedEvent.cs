using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events;

public class NeutralCollisionControllableDestroyedEvent : ControllableDestroyedEvent
{
    /// <summary>
    /// The UnitKind your Controllable collided with.
    /// </summary>
    public readonly UnitKind UnitKind;
    
    /// <summary>
    /// The Name of the Unit your Controllable collided with.
    /// </summary>
    public readonly string UnitName;

    internal NeutralCollisionControllableDestroyedEvent(Controllable controllable, UnitKind unitKind, PacketReader reader) : base(controllable)
    {
        UnitKind = unitKind;
        UnitName = reader.ReadString();
    }
    public override EventKind Kind => EventKind.DeathByNeutralCollision;
    public override DestructionReason Reason => DestructionReason.Collision;

}