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

    internal MobileUnit(Cluster cluster, string name) : base(cluster, name)
    {
        _position = new Vector();
        _movement = new Vector();
    }

    internal MobileUnit(MobileUnit unit) : base(unit)
    {
        _position = new Vector(unit._position);
        _movement = new Vector(unit._movement);
    }

    /// <inheritdoc/>
    public override Vector Position => _position;

    /// <inheritdoc/>
    public override Vector Movement => _movement;

    /// <inheritdoc/>
    public override float Angle => _movement.Angle;

    /// <inheritdoc/>
    public override Mobility Mobility => Mobility.Mobile;

    private protected void ReadPositionAndMovement(PacketReader reader)
    {
        if (!Vector.FromReader(reader, out _position) || !Vector.FromReader(reader, out _movement))
            throw new InvalidDataException("Couldn't read Unit.");
    }

    internal override void UpdateMovement(PacketReader reader)
    {
        base.UpdateMovement(reader);
        ReadPositionAndMovement(reader);
    }
}
