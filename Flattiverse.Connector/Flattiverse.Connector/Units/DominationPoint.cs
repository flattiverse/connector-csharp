using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// A domination point target with live domination state.
/// </summary>
public class DominationPoint : Target
{
    private float _dominationRadius;
    private int _domination;
    private int _scoreCountdown;

    internal DominationPoint(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
        if (!reader.Read(out _dominationRadius))
            throw new InvalidDataException("Couldn't read DominationPoint.");

        _domination = 0;
        _scoreCountdown = 0;
    }

    internal DominationPoint(DominationPoint dominationPoint) : base(dominationPoint)
    {
        _dominationRadius = dominationPoint._dominationRadius;
        _domination = dominationPoint._domination;
        _scoreCountdown = dominationPoint._scoreCountdown;
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.DominationPoint;

    /// <summary>
    /// Radius in which ships influence domination.
    /// </summary>
    public float DominationRadius => _dominationRadius;

    /// <summary>
    /// Current domination progress.
    /// </summary>
    public int Domination => _domination;

    /// <summary>
    /// Current score countdown while fully controlled.
    /// </summary>
    public int ScoreCountdown => _scoreCountdown;

    internal override void UpdateState(PacketReader reader)
    {
        base.UpdateState(reader);

        if (!reader.Read(out byte teamId) ||
            !_cluster.Galaxy.Teams.TryGet(teamId, out Team? team) ||
            !reader.Read(out _domination) ||
            !reader.Read(out _scoreCountdown))
            throw new InvalidDataException("Couldn't read DominationPoint state.");

        UpdateTargetTeam(team);
    }

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new DominationPoint(this);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{base.ToString()}, DominationRadius={_dominationRadius:0.00}, Domination={_domination}, ScoreCountdown={_scoreCountdown}";
    }
}
