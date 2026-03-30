using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// A visible carbon cargo power-up.
/// </summary>
public class CarbonCargoPowerUp : PowerUp
{
    internal CarbonCargoPowerUp(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
    }

    internal CarbonCargoPowerUp(CarbonCargoPowerUp powerUp) : base(powerUp)
    {
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.CarbonCargoPowerUp;

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new CarbonCargoPowerUp(this);
    }
}
