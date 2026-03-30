using System.Text;
using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// Map units such as suns or planets that remain present in a cluster.
/// </summary>
public class SteadyUnit : Unit
{
    private float _gravity;
    private float _radius;
    private Vector _position;
    private Vector _movement;
    private Vector _configuredPosition;
    private Orbit[] _orbitingList;

    internal SteadyUnit(Cluster cluster, string name, PacketReader reader) : base(cluster, name)
    {
        if (!Vector.FromReader(reader, out _position) || !reader.Read(out _radius) || !reader.Read(out _gravity))
            throw new InvalidDataException("Couldn't read Unit.");

        _movement = new Vector();
        _configuredPosition = new Vector(_position);
        _orbitingList = Array.Empty<Orbit>();
    }

    internal SteadyUnit(SteadyUnit unit) : base(unit)
    {
        _position = new Vector(unit._position);
        _movement = new Vector(unit._movement);
        _configuredPosition = new Vector(unit._configuredPosition);
        _radius = unit._radius;
        _gravity = unit._gravity;
        _orbitingList = new Orbit[unit._orbitingList.Length];

        for (int index = 0; index < _orbitingList.Length; index++)
        {
            Orbit orbit = unit._orbitingList[index];
            _orbitingList[index] = new Orbit(orbit.Distance, orbit.StartAngle, orbit.RotationTicks);
        }
    }

    /// <inheritdoc/>
    public override float Gravity => _gravity;

    /// <inheritdoc/>
    public override float Radius => _radius;

    /// <inheritdoc/>
    public override Vector Position => _position;

    /// <inheritdoc/>
    public override Vector Movement => _movement;

    /// <inheritdoc/>
    public override Mobility Mobility => _orbitingList.Length != 0 || _movement != 0f ? Mobility.Steady : Mobility.Still;

    /// <summary>
    /// Configured map-editor position of the unit.
    /// For orbiting units this is the center of the first orbit segment, not necessarily the current live position.
    /// </summary>
    public Vector ConfiguredPosition
    {
        get { return _configuredPosition; }
    }

    /// <summary>
    /// Orbit chain received once the unit becomes fully visible.
    /// Each entry is applied relative to the configured center or to the previous orbit segment.
    /// </summary>
    public IReadOnlyList<Orbit> OrbitingList
    {
        get { return _orbitingList; }
    }

    internal override void UpdateMovement(PacketReader reader)
    {
        if (!Vector.FromReader(reader, out _position) || !Vector.FromReader(reader, out _movement))
            throw new InvalidDataException("Couldn't read Unit.");
    }

    internal override void UpdateState(PacketReader reader)
    {
        base.UpdateState(reader);

        if (!Vector.FromReader(reader, out _configuredPosition) || !reader.Read(out byte orbitCount))
            throw new InvalidDataException("Couldn't read Unit.");

        Orbit[] orbits = new Orbit[orbitCount];

        for (int index = 0; index < orbitCount; index++)
        {
            if (!reader.Read(out float distance) || !reader.Read(out float startAngle) || !reader.Read(out int rotationTicks))
                throw new InvalidDataException("Couldn't read Unit.");

            orbits[index] = new Orbit(distance, startAngle, rotationTicks);
        }

        _orbitingList = orbits;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder(base.ToString());

        sb.Append($", ConfiguredPosition={_configuredPosition}, OrbitingList=[");

        for (int index = 0; index < _orbitingList.Length; index++)
        {
            if (index != 0)
                sb.Append(", ");

            sb.Append(_orbitingList[index]);
        }

        sb.Append(']');
        return sb.ToString();
    }
}
