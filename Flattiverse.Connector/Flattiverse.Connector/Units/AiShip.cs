using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// Mobile NPC ship.
/// </summary>
public class AiShip : MobileNpcUnit
{
    internal AiShip(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
    }

    internal AiShip(AiShip unit) : base(unit)
    {
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.AiShip;

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new AiShip(this);
    }
}
