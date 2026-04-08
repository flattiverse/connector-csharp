using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// Stationary NPC turret.
/// </summary>
public class AiTurret : NpcUnit
{
    internal AiTurret(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
    }

    internal AiTurret(AiTurret unit) : base(unit)
    {
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.AiTurret;

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new AiTurret(this);
    }
}
