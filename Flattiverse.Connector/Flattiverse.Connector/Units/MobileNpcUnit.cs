using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// Base type for visible mobile NPC units.
/// </summary>
public class MobileNpcUnit : MobileUnit
{
    private readonly Team _team;
    private float _radius;
    private float _hull;
    private float _hullMaximum;

    internal MobileNpcUnit(Cluster cluster, string name, PacketReader reader) : base(cluster, name)
    {
        if (!reader.Read(out byte teamId))
            throw new InvalidDataException("Couldn't read Unit.");

        _team = cluster.Galaxy.Teams[teamId];
        ReadPositionAndMovement(reader);

        if (!reader.Read(out _radius))
            throw new InvalidDataException("Couldn't read Unit.");

        _hull = 0f;
        _hullMaximum = 0f;
    }

    internal MobileNpcUnit(MobileNpcUnit unit) : base(unit)
    {
        _team = unit._team;
        _radius = unit._radius;
        _hull = unit._hull;
        _hullMaximum = unit._hullMaximum;
    }

    /// <inheritdoc/>
    public override float Radius => _radius;

    /// <inheritdoc/>
    public override Team? Team => _team;

    /// <inheritdoc/>
    public override bool CanBeEdited => true;

    /// <summary>
    /// Current hull value.
    /// </summary>
    public float Hull
    {
        get { return _hull; }
    }

    /// <summary>
    /// Maximum hull value.
    /// </summary>
    public float HullMaximum
    {
        get { return _hullMaximum; }
    }

    internal override void UpdateState(PacketReader reader)
    {
        base.UpdateState(reader);

        if (!reader.Read(out _hull) || !reader.Read(out _hullMaximum))
            throw new InvalidDataException("Couldn't read Unit.");
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{base.ToString()}, Hull={_hull:0.###}/{_hullMaximum:0.###}";
    }
}
