using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// Represents a shot.
/// </summary>
public class Shot : Unit
{
    /// <summary>
    /// Represents the player which invoked the shot or null, if the shot hasn't been invoked by a player.
    /// </summary>
    public readonly Player? Player;
    
    /// <summary>
    /// Represents the ControllableInfo which invoked the shot or null, if the shot hasn't been invoked by a player.
    /// </summary>
    public readonly ControllableInfo? ControllableInfo;
    
    private Vector _position;
    private Vector _movement;

    private ushort _ticks;
    
    /// <summary>
    /// The size of the generated explosion.
    /// </summary>
    public readonly float Load;
    
    /// <summary>
    /// The damage inflicted.
    /// </summary>
    public readonly float Damage;
    
    internal Shot(Cluster cluster, string name, PacketReader reader) : base(cluster, name)
    {
        if (!reader.Read(out byte playerId) || !reader.Read(out byte controllableId) || !reader.Read(out _ticks) || !reader.Read(out Load) || !reader.Read(out Damage) || !Vector.FromReader(reader, out _position) || !Vector.FromReader(reader, out _movement))
            throw new InvalidDataException("Couldn't read Unit.");

        if (playerId < 192)
        {
            Player = cluster.Galaxy.Players[playerId];
            ControllableInfo = Player.ControllableInfos[controllableId];
        }
    }
    
    internal Shot(Shot unit) : base(unit)
    {
        Player = unit.Player;
        ControllableInfo = unit.ControllableInfo;
        
        _ticks = unit._ticks;
        
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
    public override float Radius => 1f;

    /// <inheritdoc/>
    public override Mobility Mobility => Mobility.Mobile;
    
    /// <inheritdoc/>
    public override Team? Team => Player?.Team;
    
    /// <summary>
    /// The countdown of when the shot explodes.
    /// </summary>
    public ushort Ticks => _ticks;

    internal override void UpdateMovement(PacketReader reader)
    {
        base.UpdateMovement(reader);
        
        if (!reader.Read(out _ticks) || !Vector.FromReader(reader, out _position) || !Vector.FromReader(reader, out _movement))
            throw new InvalidDataException("Couldn't read Unit.");
    }
    
    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new Shot(this);
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.Shot;
}