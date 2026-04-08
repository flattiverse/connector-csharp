using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// Mobile hostile jellyfish NPC.
/// </summary>
public class SpaceJellyFish : MobileNpcUnit
{
    internal SpaceJellyFish(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
    }

    internal SpaceJellyFish(SpaceJellyFish unit) : base(unit)
    {
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.SpaceJellyFish;

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new SpaceJellyFish(this);
    }
}
