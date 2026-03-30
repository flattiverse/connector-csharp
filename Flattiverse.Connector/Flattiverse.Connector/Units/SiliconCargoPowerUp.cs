using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// A visible silicon cargo power-up.
/// </summary>
public class SiliconCargoPowerUp : PowerUp
{
    internal SiliconCargoPowerUp(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
    }

    internal SiliconCargoPowerUp(SiliconCargoPowerUp powerUp) : base(powerUp)
    {
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.SiliconCargoPowerUp;

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new SiliconCargoPowerUp(this);
    }
}
