using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// A visible energy charge power-up.
/// </summary>
public class EnergyChargePowerUp : PowerUp
{
    internal EnergyChargePowerUp(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
    }

    internal EnergyChargePowerUp(EnergyChargePowerUp powerUp) : base(powerUp)
    {
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.EnergyChargePowerUp;

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new EnergyChargePowerUp(this);
    }
}
