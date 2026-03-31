using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// Base type for mobile visible units.
/// </summary>
public class MobileUnit : Unit
{
    private protected Vector _position;
    private protected Vector _movement;
    private protected float _angle;
    private protected float _angularVelocity;

    internal MobileUnit(Cluster cluster, string name) : base(cluster, name)
    {
        _position = new Vector();
        _movement = new Vector();
        _angle = 0f;
        _angularVelocity = 0f;
    }

    internal MobileUnit(MobileUnit unit) : base(unit)
    {
        _position = new Vector(unit._position);
        _movement = new Vector(unit._movement);
        _angle = unit._angle;
        _angularVelocity = unit._angularVelocity;
    }

    /// <inheritdoc/>
    public override Vector Position => _position;

    /// <inheritdoc/>
    public override Vector Movement => _movement;

    /// <inheritdoc/>
    public override float Angle => _angle;

    /// <summary>
    /// The current angular velocity of the visible unit.
    /// </summary>
    public float AngularVelocity
    {
        get { return _angularVelocity; }
    }

    /// <inheritdoc/>
    public override Mobility Mobility => Mobility.Mobile;

    private protected void ReadPositionAndMovement(PacketReader reader)
    {
        if (!Vector.FromReader(reader, out _position) || !Vector.FromReader(reader, out _movement) ||
            !reader.Read(out _angle) || !reader.Read(out _angularVelocity))
            throw new InvalidDataException("Couldn't read Unit.");
    }

    internal override void UpdateMovement(PacketReader reader)
    {
        base.UpdateMovement(reader);
        ReadPositionAndMovement(reader);
    }
}
