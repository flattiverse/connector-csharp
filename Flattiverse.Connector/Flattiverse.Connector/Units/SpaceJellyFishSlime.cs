using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// Homing biological projectile spawned by a space jellyfish.
/// </summary>
public class SpaceJellyFishSlime : Projectile
{
    private byte _targetClusterId;
    private string _targetUnitName;
    private UnitKind? _targetUnitKind;

    internal SpaceJellyFishSlime(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
        _targetClusterId = byte.MaxValue;
        _targetUnitName = string.Empty;
        _targetUnitKind = null;
    }

    internal SpaceJellyFishSlime(SpaceJellyFishSlime unit) : base(unit)
    {
        _targetClusterId = unit._targetClusterId;
        _targetUnitName = unit._targetUnitName;
        _targetUnitKind = unit._targetUnitKind;
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.SpaceJellyFishSlime;

    /// <summary>
    /// Target cluster id, or 255 if no target is currently known.
    /// </summary>
    public byte TargetClusterId
    {
        get { return _targetClusterId; }
    }

    /// <summary>
    /// Target unit name, or empty if no target is currently known.
    /// </summary>
    public string TargetUnitName
    {
        get { return _targetUnitName; }
    }

    /// <summary>
    /// Target unit kind, or <see langword="null" /> if no target is currently known.
    /// </summary>
    public UnitKind? TargetUnitKind
    {
        get { return _targetUnitKind; }
    }

    internal override void UpdateState(PacketReader reader)
    {
        base.UpdateState(reader);

        if (!reader.Read(out _targetClusterId) || !reader.Read(out _targetUnitName) || !reader.Read(out byte targetKindValue))
            throw new InvalidDataException("Couldn't read Unit.");

        if (_targetClusterId == byte.MaxValue || targetKindValue == byte.MaxValue || _targetUnitName.Length == 0)
        {
            _targetClusterId = byte.MaxValue;
            _targetUnitName = string.Empty;
            _targetUnitKind = null;
            return;
        }

        _targetUnitKind = (UnitKind)targetKindValue;
    }

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new SpaceJellyFishSlime(this);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        string targetKind = _targetUnitKind is null ? "-" : _targetUnitKind.Value.ToString();
        string targetName = _targetUnitName.Length == 0 ? "-" : _targetUnitName;
        string targetCluster = _targetClusterId == byte.MaxValue ? "-" : _targetClusterId.ToString();

        return $"{base.ToString()}, TargetCluster={targetCluster}, TargetKind={targetKind}, TargetName=\"{targetName}\"";
    }
}
