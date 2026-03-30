using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// A visible hull repair power-up.
/// </summary>
public class HullRepairPowerUp : PowerUp
{
    internal HullRepairPowerUp(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
    }

    internal HullRepairPowerUp(HullRepairPowerUp powerUp) : base(powerUp)
    {
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.HullRepairPowerUp;

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new HullRepairPowerUp(this);
    }
}
