using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// Represents a rail projectile.
/// </summary>
public class Rail : Projectile
{
    internal Rail(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
    }

    internal Rail(Rail unit) : base(unit)
    {
    }

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new Rail(this);
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.Rail;
}
