using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events;

/// <summary>
/// This event is fired when a controllable is destroyed by colliding with a neutral object.
/// </summary>
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

    /// <summary>
    /// The kind of the event.
    /// </summary>
    public override EventKind Kind => EventKind.DeathByNeutralCollision;

    /// <summary>
    /// The reason for the death of your Controllable.
    /// </summary>
    public override DestructionReason Reason => DestructionReason.Collision;

}