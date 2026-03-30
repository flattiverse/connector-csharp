using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// A visible metal cargo power-up.
/// </summary>
public class MetalCargoPowerUp : PowerUp
{
    internal MetalCargoPowerUp(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
    }

    internal MetalCargoPowerUp(MetalCargoPowerUp powerUp) : base(powerUp)
    {
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.MetalCargoPowerUp;

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new MetalCargoPowerUp(this);
    }
}
