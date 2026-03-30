using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// A visible neutrino charge power-up.
/// </summary>
public class NeutrinoChargePowerUp : PowerUp
{
    internal NeutrinoChargePowerUp(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
    }

    internal NeutrinoChargePowerUp(NeutrinoChargePowerUp powerUp) : base(powerUp)
    {
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.NeutrinoChargePowerUp;

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new NeutrinoChargePowerUp(this);
    }
}
