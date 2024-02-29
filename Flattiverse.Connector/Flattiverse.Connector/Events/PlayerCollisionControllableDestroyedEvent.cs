using System.Diagnostics;
using Flattiverse.Connector.Hierarchy;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events;

/// <summary>
/// This event is raised, if a player collided with a controllable and destroyed it.
/// </summary>
public class PlayerCollisionControllableDestroyedEvent: ControllableDestroyedEvent
{

    /// <summary>
    /// The other Player that collided with the Controllable.
    /// </summary>
    public readonly Player OtherPlayer;
    
    /// <summary>
    /// The other Controllable that collided with the Controllable.
    /// </summary>
    public readonly ControllableInfo OtherControllableInfo;
    
    internal PlayerCollisionControllableDestroyedEvent(Player player, ControllableInfo controllableInfo, Galaxy galaxy, PacketReader reader) : base(player, controllableInfo)
    {
        byte playerId = reader.ReadByte();
        byte controllableInfoId = reader.ReadByte();
        
        Debug.Assert(galaxy.GetPlayer(playerId) is not null, $"players[{playerId}] not populated.");
        Debug.Assert(galaxy.GetPlayer(playerId).controllableInfos[controllableInfoId] is not null, $"players[{playerId}].controllableInfos[{controllableInfoId}] not populated.");
        
        OtherPlayer = galaxy.GetPlayer(playerId);
        OtherControllableInfo = OtherPlayer.controllableInfos[controllableInfoId]!;
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.DeathByControllableCollision;
    
    /// <inheritdoc/>
    public override DestructionReason Reason => DestructionReason.Collision;
}