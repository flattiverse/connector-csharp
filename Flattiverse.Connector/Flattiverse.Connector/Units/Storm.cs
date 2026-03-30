using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// A storm source that periodically spawns announcing and active whirls.
/// </summary>
public class Storm : SteadyUnit
{
    private float _spawnChancePerTick;
    private ushort _minAnnouncementTicks;
    private ushort _maxAnnouncementTicks;
    private ushort _minActiveTicks;
    private ushort _maxActiveTicks;
    private float _minWhirlRadius;
    private float _maxWhirlRadius;
    private float _minWhirlSpeed;
    private float _maxWhirlSpeed;
    private float _minWhirlGravity;
    private float _maxWhirlGravity;
    private float _damage;

    /// <summary>
    /// Probability in the range `[0; 1]` that the storm spawns one announcing whirl in a tick.
    /// </summary>
    public float SpawnChancePerTick
    {
        get { return _spawnChancePerTick; }
    }

    /// <summary>
    /// Minimum announcement duration for newly spawned whirls.
    /// </summary>
    public ushort MinAnnouncementTicks
    {
        get { return _minAnnouncementTicks; }
    }

    /// <summary>
    /// Maximum announcement duration for newly spawned whirls.
    /// </summary>
    public ushort MaxAnnouncementTicks
    {
        get { return _maxAnnouncementTicks; }
    }

    /// <summary>
    /// Minimum active duration for newly spawned whirls.
    /// </summary>
    public ushort MinActiveTicks
    {
        get { return _minActiveTicks; }
    }

    /// <summary>
    /// Maximum active duration for newly spawned whirls.
    /// </summary>
    public ushort MaxActiveTicks
    {
        get { return _maxActiveTicks; }
    }

    /// <summary>
    /// Minimum radius used for newly spawned whirls.
    /// </summary>
    public float MinWhirlRadius
    {
        get { return _minWhirlRadius; }
    }

    /// <summary>
    /// Maximum radius used for newly spawned whirls.
    /// </summary>
    public float MaxWhirlRadius
    {
        get { return _maxWhirlRadius; }
    }

    /// <summary>
    /// Minimum initial speed used for newly spawned whirls.
    /// </summary>
    public float MinWhirlSpeed
    {
        get { return _minWhirlSpeed; }
    }

    /// <summary>
    /// Maximum initial speed used for newly spawned whirls.
    /// </summary>
    public float MaxWhirlSpeed
    {
        get { return _maxWhirlSpeed; }
    }

    /// <summary>
    /// Minimum gravity used for active whirls.
    /// </summary>
    public float MinWhirlGravity
    {
        get { return _minWhirlGravity; }
    }

    /// <summary>
    /// Maximum gravity used for active whirls.
    /// </summary>
    public float MaxWhirlGravity
    {
        get { return _maxWhirlGravity; }
    }

    /// <summary>
    /// Damage applied by each active-whirl hit.
    /// </summary>
    public float Damage
    {
        get { return _damage; }
    }

    internal Storm(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
        _spawnChancePerTick = 0f;
        _minAnnouncementTicks = 0;
        _maxAnnouncementTicks = 0;
        _minActiveTicks = 0;
        _maxActiveTicks = 0;
        _minWhirlRadius = 0f;
        _maxWhirlRadius = 0f;
        _minWhirlSpeed = 0f;
        _maxWhirlSpeed = 0f;
        _minWhirlGravity = 0f;
        _maxWhirlGravity = 0f;
        _damage = 0f;
    }

    internal Storm(Storm storm) : base(storm)
    {
        _spawnChancePerTick = storm._spawnChancePerTick;
        _minAnnouncementTicks = storm._minAnnouncementTicks;
        _maxAnnouncementTicks = storm._maxAnnouncementTicks;
        _minActiveTicks = storm._minActiveTicks;
        _maxActiveTicks = storm._maxActiveTicks;
        _minWhirlRadius = storm._minWhirlRadius;
        _maxWhirlRadius = storm._maxWhirlRadius;
        _minWhirlSpeed = storm._minWhirlSpeed;
        _maxWhirlSpeed = storm._maxWhirlSpeed;
        _minWhirlGravity = storm._minWhirlGravity;
        _maxWhirlGravity = storm._maxWhirlGravity;
        _damage = storm._damage;
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.Storm;

    /// <inheritdoc/>
    public override bool IsMasking => false;

    /// <inheritdoc/>
    public override bool IsSolid => false;

    /// <inheritdoc/>
    public override bool CanBeEdited => true;

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new Storm(this);
    }

    internal override void UpdateState(PacketReader reader)
    {
        base.UpdateState(reader);

        if (!reader.Read(out _spawnChancePerTick) ||
            !reader.Read(out _minAnnouncementTicks) ||
            !reader.Read(out _maxAnnouncementTicks) ||
            !reader.Read(out _minActiveTicks) ||
            !reader.Read(out _maxActiveTicks) ||
            !reader.Read(out _minWhirlRadius) ||
            !reader.Read(out _maxWhirlRadius) ||
            !reader.Read(out _minWhirlSpeed) ||
            !reader.Read(out _maxWhirlSpeed) ||
            !reader.Read(out _minWhirlGravity) ||
            !reader.Read(out _maxWhirlGravity) ||
            !reader.Read(out _damage))
            throw new InvalidDataException("Couldn't read Unit.");
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{base.ToString()}, SpawnChancePerTick={_spawnChancePerTick:0.###}, AnnouncementTicks={_minAnnouncementTicks}..{_maxAnnouncementTicks}, " +
               $"ActiveTicks={_minActiveTicks}..{_maxActiveTicks}, WhirlRadius={_minWhirlRadius:0.###}..{_maxWhirlRadius:0.###}, " +
               $"WhirlSpeed={_minWhirlSpeed:0.###}..{_maxWhirlSpeed:0.###}, WhirlGravity={_minWhirlGravity:0.###}..{_maxWhirlGravity:0.###}, Damage={_damage:0.###}";
    }
}
