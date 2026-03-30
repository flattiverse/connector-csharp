using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// A visible hydrogen cargo power-up.
/// </summary>
public class HydrogenCargoPowerUp : PowerUp
{
    internal HydrogenCargoPowerUp(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
    }

    internal HydrogenCargoPowerUp(HydrogenCargoPowerUp powerUp) : base(powerUp)
    {
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.HydrogenCargoPowerUp;

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new HydrogenCargoPowerUp(this);
    }
}
