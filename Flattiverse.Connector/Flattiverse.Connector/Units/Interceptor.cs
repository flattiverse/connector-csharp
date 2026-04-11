using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// Represents an interceptor projectile.
/// </summary>
public class Interceptor : Projectile
{
    private const float FixedSpeedLimit = 10f;

    internal Interceptor(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
    }

    internal Interceptor(Interceptor unit) : base(unit)
    {
    }

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new Interceptor(this);
    }

    /// <inheritdoc/>
    public override float SpeedLimit => FixedSpeedLimit;

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.Interceptor;
}
