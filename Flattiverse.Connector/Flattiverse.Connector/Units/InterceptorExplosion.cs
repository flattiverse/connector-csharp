using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// Represents an interceptor explosion.
/// </summary>
public class InterceptorExplosion : Explosion
{
    internal InterceptorExplosion(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
    }

    internal InterceptorExplosion(InterceptorExplosion unit) : base(unit)
    {
    }

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new InterceptorExplosion(this);
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.InterceptorExplosion;
}
