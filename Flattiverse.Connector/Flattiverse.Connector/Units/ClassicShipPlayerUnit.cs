using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// A classic ship for noobs.
/// </summary>
public class ClassicShipPlayerUnit : PlayerUnit
{
    internal ClassicShipPlayerUnit(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
    }

    internal ClassicShipPlayerUnit(ClassicShipPlayerUnit unit) : base(unit)
    {
    }

    /// <inheritdoc/>
    public override float Gravity => 0.0012f;

    /// <inheritdoc/>
    public override float Radius => 14f;

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.ClassicShipPlayerUnit;
    
    public override Unit Clone()
    {
        return new ClassicShipPlayerUnit(this);
    }
}