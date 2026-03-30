using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// A mission target with sequence number and configurable waypoint vectors.
/// </summary>
public class MissionTarget : Target
{
    private ushort _sequenceNumber;
    private Vector[] _vectors;

    internal MissionTarget(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
        _sequenceNumber = 0;
        _vectors = Array.Empty<Vector>();
    }

    internal MissionTarget(MissionTarget missionTarget) : base(missionTarget)
    {
        _sequenceNumber = missionTarget._sequenceNumber;
        _vectors = new Vector[missionTarget._vectors.Length];

        for (int vectorIndex = 0; vectorIndex < _vectors.Length; vectorIndex++)
            _vectors[vectorIndex] = new Vector(missionTarget._vectors[vectorIndex]);
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.MissionTarget;

    /// <summary>
    /// Sequence number of this mission target within the scenario.
    /// </summary>
    public ushort SequenceNumber => _sequenceNumber;

    /// <summary>
    /// Number of configured waypoint vectors.
    /// </summary>
    public int VectorCount => _vectors.Length;

    /// <summary>
    /// Returns a copy of all configured mission vectors.
    /// Their exact scenario-specific meaning depends on the mission script or map logic.
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

    internal override void UpdateState(PacketReader reader)
    {
        base.UpdateState(reader);

        if (!reader.Read(out _sequenceNumber) || !reader.Read(out ushort vectorCount))
            throw new InvalidDataException("Couldn't read MissionTarget.");

        _vectors = new Vector[vectorCount];

        for (int vectorIndex = 0; vectorIndex < vectorCount; vectorIndex++)
        {
            if (!Vector.FromReader(reader, out Vector vector))
                throw new InvalidDataException("Couldn't read MissionTarget.");

            _vectors[vectorIndex] = vector;
        }
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{base.ToString()}, SequenceNumber={_sequenceNumber}, VectorCount={_vectors.Length}";
    }
}
