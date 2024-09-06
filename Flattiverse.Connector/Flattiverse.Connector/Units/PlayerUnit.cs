using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// Represents a player unit.
/// </summary>
public class PlayerUnit : Unit
{
    /// <summary>
    /// Represents the player which controlls the PlayerUnit.
    /// </summary>
    public readonly Player Player;
    
    /// <summary>
    /// Represents the ControllableInfo of this PlayerUnit.
    /// </summary>
    public readonly ControllableInfo ControllableInfo;
    
    private Vector _position;
    private Vector _movement;
    
    internal PlayerUnit(Cluster cluster, string name, PacketReader reader) : base(cluster, name)
    {
        if (!reader.Read(out byte playerId) || !reader.Read(out byte controllableId) || !Vector.FromReader(reader, out _position) || !Vector.FromReader(reader, out _movement))
            throw new InvalidDataException("Couldn't read Unit.");

        Player = cluster.Galaxy.Players[playerId];
        ControllableInfo = Player.ControllableInfos[controllableId];
    }

    internal PlayerUnit(PlayerUnit unit) : base(unit)
    {
        Player = unit.Player;
        ControllableInfo = unit.ControllableInfo;
        
        _position = new Vector(unit._position);
        _movement = new Vector(unit._movement);
    }
    
    /// <inheritdoc/>
    public override Vector Position => _position;
    
    /// <inheritdoc/>
    public override Vector Movement => _movement;
        
    /// <inheritdoc/>
    public override float Angle => _movement.Angle;
    
    /// <inheritdoc/>
    public override Mobility Mobility => Mobility.Mobile;

    internal override void UpdateMovement(PacketReader reader)
    {
        base.UpdateMovement(reader);
        
        Vector.FromReader(reader, out _position);
        Vector.FromReader(reader, out _movement);
    }
}