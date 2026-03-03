using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// A mission target with configurable waypoint vectors.
/// </summary>
public class MissionTarget : SteadyUnit
{
    private Team _team;
    private Vector[] _vectors;

    internal MissionTarget(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
        if (!reader.Read(out byte teamId) || !cluster.Galaxy.Teams.TryGet(teamId, out Team? team) || !reader.Read(out ushort vectorCount))
            throw new InvalidDataException("Couldn't read Unit.");

        _team = team;

        _vectors = new Vector[vectorCount];

        for (int vectorIndex = 0; vectorIndex < vectorCount; vectorIndex++)
        {
            if (!Vector.FromReader(reader, out Vector vector))
                throw new InvalidDataException("Couldn't read Unit.");

            _vectors[vectorIndex] = vector;
        }
    }

    internal MissionTarget(MissionTarget missionTarget) : base(missionTarget)
    {
        _team = missionTarget._team;
        _vectors = new Vector[missionTarget._vectors.Length];

        for (int vectorIndex = 0; vectorIndex < _vectors.Length; vectorIndex++)
            _vectors[vectorIndex] = new Vector(missionTarget._vectors[vectorIndex]);
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.MissionTarget;

    /// <inheritdoc/>
    public override bool IsMasking => false;

    /// <inheritdoc/>
    public override bool IsSolid => false;

    /// <inheritdoc/>
    public override Team? Team => _team;

    /// <summary>
    /// Number of configured waypoint vectors.
    /// </summary>
    public int VectorCount => _vectors.Length;

    /// <summary>
    /// Returns a copy of all configured waypoint vectors.
    /// </summary>
    public Vector[] Vectors
    {
        get
        {
            Vector[] vectors = new Vector[_vectors.Length];

            for (int vectorIndex = 0; vectorIndex < _vectors.Length; vectorIndex++)
                vectors[vectorIndex] = new Vector(_vectors[vectorIndex]);

            return vectors;
        }
    }

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new MissionTarget(this);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{base.ToString()}, VectorCount={_vectors.Length}";
    }
}
