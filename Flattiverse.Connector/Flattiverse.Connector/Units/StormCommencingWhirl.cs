using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// A storm whirl that is still announcing itself and does not deal damage yet.
/// </summary>
public class StormCommencingWhirl : StormWhirl
{
    internal StormCommencingWhirl(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
    }

    internal StormCommencingWhirl(StormCommencingWhirl stormCommencingWhirl) : base(stormCommencingWhirl)
    {
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.StormCommencingWhirl;

    /// <inheritdoc/>
    public override bool IsMasking => false;

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new StormCommencingWhirl(this);
    }

    internal override void UpdateState(PacketReader reader)
    {
        base.UpdateState(reader);
        ReadRemainingTicks(reader);
    }
}
