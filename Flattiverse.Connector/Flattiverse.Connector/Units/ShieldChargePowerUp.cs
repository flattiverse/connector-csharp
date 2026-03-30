using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// A visible shield charge power-up.
/// </summary>
public class ShieldChargePowerUp : PowerUp
{
    internal ShieldChargePowerUp(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
    }

    internal ShieldChargePowerUp(ShieldChargePowerUp powerUp) : base(powerUp)
    {
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.ShieldChargePowerUp;

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new ShieldChargePowerUp(this);
    }
}
