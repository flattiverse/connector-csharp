using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// Mobile NPC probe without weapons.
/// </summary>
public class AiProbe : MobileNpcUnit
{
    internal AiProbe(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
    }

    internal AiProbe(AiProbe unit) : base(unit)
    {
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.AiProbe;

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new AiProbe(this);
    }
}
