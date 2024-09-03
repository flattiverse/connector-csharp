using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

public class SteadyUnit : Unit
{
    private float _gravity;

    private float _radius;

    private Vector _position;
    
    internal SteadyUnit(Cluster cluster, string name, PacketReader reader) : base(cluster, name)
    {
        if (!Vector.FromReader(reader, out _position) || !reader.Read(out _radius) || !reader.Read(out _gravity))
            throw new InvalidDataException("Couldn't read Unit.");
    }

    internal SteadyUnit(SteadyUnit unit) : base(unit)
    {
        _position = new Vector(unit._position);
        _radius = unit._radius;
        _gravity = unit._gravity;
    }

    /// <inheritdoc/>
    public override float Gravity => _gravity;

    /// <inheritdoc/>
    public override float Radius => _radius;

    /// <inheritdoc/>
    public override Vector Position => _position;
}