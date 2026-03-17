using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// A flag target.
/// </summary>
public class Flag : Target
{
    internal Flag(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
        MarkFullStateKnown();
    }

    internal Flag(Flag flag) : base(flag)
    {
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.Flag;

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new Flag(this);
    }
}
