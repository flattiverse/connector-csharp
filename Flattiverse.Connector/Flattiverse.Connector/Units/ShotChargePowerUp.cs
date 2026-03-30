using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// A visible shot charge power-up.
/// </summary>
public class ShotChargePowerUp : PowerUp
{
    internal ShotChargePowerUp(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
    }

    internal ShotChargePowerUp(ShotChargePowerUp powerUp) : base(powerUp)
    {
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.ShotChargePowerUp;

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new ShotChargePowerUp(this);
    }
}
