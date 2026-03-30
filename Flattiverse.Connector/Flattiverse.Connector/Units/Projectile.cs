using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// Base type for visible projectile units such as shots and interceptors.
/// The owning player information can be absent for neutral or system-generated projectiles.
/// </summary>
public class Projectile : MobileUnit
{
    /// <summary>
    /// Player that launched the projectile, or <see langword="null" /> if no player-owned source is known.
    /// </summary>
    public readonly Player? Player;

    /// <summary>
    /// Controllable entry that launched the projectile, or <see langword="null" /> if no player-owned source is known.
    /// </summary>
    public readonly ControllableInfo? ControllableInfo;

    private ushort _ticks;
    private float _load;
    private float _damage;

    internal Projectile(Cluster cluster, string name, PacketReader reader) : base(cluster, name)
    {
        if (!reader.Read(out byte playerId) || !reader.Read(out byte controllableId) || !reader.Read(out _ticks))
            throw new InvalidDataException("Couldn't read Unit.");

        ReadPositionAndMovement(reader);

        _load = 0f;
        _damage = 0f;

        if (playerId < 192)
        {
            Player = cluster.Galaxy.Players[playerId];
            ControllableInfo = Player.ControllableInfos[controllableId];
        }
    }

    internal Projectile(Projectile unit) : base(unit)
    {
        Player = unit.Player;
        ControllableInfo = unit.ControllableInfo;
        _ticks = unit._ticks;
        _load = unit._load;
        _damage = unit._damage;
    }

    /// <inheritdoc/>
    public override float Radius => 1f;

    /// <inheritdoc/>
    public override Team? Team => Player?.Team;

    /// <inheritdoc/>
    public override bool IsMasking => false;

    /// <summary>
    /// The remaining projectile lifetime in ticks.
    /// </summary>
    public ushort Ticks
    {
        get { return _ticks; }
    }

    /// <summary>
    /// Explosion load of the projectile.
    /// This becomes meaningful once the full projectile state is known.
    /// </summary>
    public float Load
    {
        get { return _load; }
    }

    /// <summary>
    /// Direct damage of the projectile.
    /// This becomes meaningful once the full projectile state is known.
    /// </summary>
    public float Damage
    {
        get { return _damage; }
    }

    internal override void UpdateMovement(PacketReader reader)
    {
        if (!reader.Read(out _ticks))
            throw new InvalidDataException("Couldn't read Unit.");

        ReadPositionAndMovement(reader);
    }

    internal override void UpdateState(PacketReader reader)
    {
        base.UpdateState(reader);

        if (!reader.Read(out _load) || !reader.Read(out _damage))
            throw new InvalidDataException("Couldn't read Unit.");
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        string playerName = Player is null ? "-" : Player.Name;
        string controllableName = ControllableInfo is null ? "-" : ControllableInfo.Name;

        return $"{base.ToString()}, Player=\"{playerName}\", Controllable=\"{controllableName}\", Ticks={_ticks}, Load={_load:0.000}, Damage={_damage:0.000}";
    }
}
