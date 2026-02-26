using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// A buoy.
/// </summary>
public class Buoy : SteadyUnit
{
    internal Buoy(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
    }

    internal Buoy(Buoy buoy) : base(buoy)
    {
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.Buoy;

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new Buoy(this);
    }
}
