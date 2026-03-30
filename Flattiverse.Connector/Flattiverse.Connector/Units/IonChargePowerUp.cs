using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// A visible ion charge power-up.
/// </summary>
public class IonChargePowerUp : PowerUp
{
    internal IonChargePowerUp(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
    }

    internal IonChargePowerUp(IonChargePowerUp powerUp) : base(powerUp)
    {
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.IonChargePowerUp;

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new IonChargePowerUp(this);
    }
}
