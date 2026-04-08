using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// Stationary NPC base.
/// </summary>
public class AiBase : NpcUnit
{
    internal AiBase(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
    }

    internal AiBase(AiBase unit) : base(unit)
    {
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.AiBase;

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new AiBase(this);
    }
}
