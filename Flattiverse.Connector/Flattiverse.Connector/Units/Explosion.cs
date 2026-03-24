using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// Represents an explosion.
/// </summary>
public class Explosion : Unit
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
    private float _size;
    private float _damage;
    
    internal Explosion(Cluster cluster, string name, PacketReader reader) : base(cluster, name)
    {
        if (!reader.Read(out byte playerId) || !reader.Read(out byte controllableId) || !reader.Read(out _size) || !reader.Read(out _damage) || !Vector.FromReader(reader, out _position))
            throw new InvalidDataException("Couldn't read Unit.");

        if (playerId < 192)
        {
            Player = cluster.Galaxy.Players[playerId];
            ControllableInfo = Player.ControllableInfos[controllableId];
        }

        MarkFullStateKnown();
    }
    
    internal Explosion(Explosion unit) : base(unit)
    {
        Player = unit.Player;
        ControllableInfo = unit.ControllableInfo;
        
        _size = unit._size;
        _damage = unit._damage;
        
        _position = new Vector(unit._position);
    }

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new Explosion(this);
    }

    /// <inheritdoc/>
    public override Vector Position => _position;
    
    /// <inheritdoc/>
    public override float Radius => _size;

    /// <inheritdoc/>
    public override bool IsMasking => false;
    
    /// <inheritdoc/>
    public override bool IsSolid => false;
    
    /// <inheritdoc/>
    public override float Gravity => 0f;
    
    /// <inheritdoc/>
    public override Team? Team => Player?.Team;

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.Explosion;

    /// <summary>
    /// Defines whether this explosion is in the damage phase or not.
    /// </summary>
    public bool DamagePhase => true;
    
    /// <summary>
    /// Defines whether this explosion is in the shockwave phase or not.
    /// </summary>
    public bool ShockWavePhase => false;

    /// <summary>
    /// The damage this explosion inflicts.
    /// </summary>
    public float Damage => _damage;

    internal override void UpdateMovement(PacketReader reader)
    {
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        string playerName = Player is null ? "-" : Player.Name;
        string controllableName = ControllableInfo is null ? "-" : ControllableInfo.Name;

        return $"{base.ToString()}, Player=\"{playerName}\", Controllable=\"{controllableName}\", DamagePhase={DamagePhase}, ShockWavePhase={ShockWavePhase}, Damage={Damage:0.000}";
    }
}
