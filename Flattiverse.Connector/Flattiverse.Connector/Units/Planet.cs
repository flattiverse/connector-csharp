using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// A planet.
/// </summary>
public class Planet : SteadyUnit
{
    internal Planet(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
    }

    internal Planet(Planet planet) : base(planet)
    {
    }
    
    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.Planet;

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new Planet(this);
    }
}