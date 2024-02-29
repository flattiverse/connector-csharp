using System.Diagnostics;
using Flattiverse.Connector.Hierarchy;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events;

public class PlayerCollisionControllableDestroyedEvent: ControllableDestroyedEvent
{

    /// <summary>
    /// The other Player that collided with your Controllable.
    /// </summary>
    public readonly Player OtherPlayer;
    
    /// <summary>
    /// The name of the other players unit that collided with your Controllable.
    /// </summary>
    public readonly string OtherUnitName;
    
    internal PlayerCollisionControllableDestroyedEvent(Controllable controllable, Player otherPlayer, PacketReader reader) : base(controllable)
    {
        OtherPlayer = otherPlayer;
        OtherUnitName = reader.ReadString();
    }

    public override EventKind Kind => EventKind.DeathByControllableCollision;
    
    public override DestructionReason Reason => DestructionReason.Collision;
}