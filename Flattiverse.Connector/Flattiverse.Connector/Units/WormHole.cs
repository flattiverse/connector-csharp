using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// A worm hole with a visible jump target after full disclosure.
/// </summary>
public class WormHole : SteadyUnit
{
    private Cluster? _targetCluster;
    private float _targetLeft;
    private float _targetTop;
    private float _targetRight;
    private float _targetBottom;

    internal WormHole(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
        _targetCluster = null;
        _targetLeft = 0f;
        _targetTop = 0f;
        _targetRight = 0f;
        _targetBottom = 0f;
    }

    internal WormHole(WormHole wormHole) : base(wormHole)
    {
        _targetCluster = wormHole._targetCluster;
        _targetLeft = wormHole._targetLeft;
        _targetTop = wormHole._targetTop;
        _targetRight = wormHole._targetRight;
        _targetBottom = wormHole._targetBottom;
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.WormHole;

    /// <inheritdoc/>
    public override bool IsMasking => false;

    /// <inheritdoc/>
    public override bool IsSolid => false;

    /// <summary>
    /// The cluster a jump leads to, once the full state is known.
    /// </summary>
    public Cluster? TargetCluster
    {
        get { return _targetCluster; }
    }

    /// <summary>
    /// Left boundary of the target region inside <see cref="TargetCluster" />.
    /// </summary>
    public float TargetLeft
    {
        get { return _targetLeft; }
    }

    /// <summary>
    /// Top boundary of the target region inside <see cref="TargetCluster" />.
    /// </summary>
    public float TargetTop
    {
        get { return _targetTop; }
    }

    /// <summary>
    /// Right boundary of the target region inside <see cref="TargetCluster" />.
    /// </summary>
    public float TargetRight
    {
        get { return _targetRight; }
    }

    /// <summary>
    /// Bottom boundary of the target region inside <see cref="TargetCluster" />.
    /// </summary>
    public float TargetBottom
    {
        get { return _targetBottom; }
    }

    internal override void UpdateState(PacketReader reader)
    {
        base.UpdateState(reader);

        if (!reader.Read(out byte targetClusterId) ||
            !reader.Read(out _targetLeft) ||
            !reader.Read(out _targetTop) ||
            !reader.Read(out _targetRight) ||
            !reader.Read(out _targetBottom))
            throw new InvalidDataException("Couldn't read WormHole.");

        _targetCluster = Cluster.Galaxy.Clusters[targetClusterId];

        if (_targetCluster is null)
            throw new InvalidDataException($"WormHole references unknown target cluster {targetClusterId}.");
    }

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new WormHole(this);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        string targetClusterName = _targetCluster is null ? "-" : _targetCluster.Name;
        return $"{base.ToString()}, TargetCluster=\"{targetClusterName}\", TargetRegion=({_targetLeft:0.###},{_targetTop:0.###})..({_targetRight:0.###},{_targetBottom:0.###})";
    }
}
