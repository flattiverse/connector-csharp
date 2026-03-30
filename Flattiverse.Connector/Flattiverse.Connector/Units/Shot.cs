using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// Represents a shot.
/// </summary>
public class Shot : Projectile
{
    internal Shot(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
    }

    internal Shot(Shot unit) : base(unit)
    {
    }

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new Shot(this);
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.Shot;
}
