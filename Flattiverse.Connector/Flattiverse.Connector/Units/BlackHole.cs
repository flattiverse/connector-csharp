using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// A black hole.
/// </summary>
public class BlackHole : SteadyUnit
{
    /// <summary>
    /// Radius of the intensified gravity well.
    /// </summary>
    public readonly float GravityWellRadius;

    /// <summary>
    /// Additional attraction force inside the gravity well.
    /// </summary>
    public readonly float GravityWellForce;

    internal BlackHole(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
        if (!reader.Read(out GravityWellRadius) || !reader.Read(out GravityWellForce))
            throw new System.IO.InvalidDataException("Couldn't read Unit.");
    }

    internal BlackHole(BlackHole blackHole) : base(blackHole)
    {
        GravityWellRadius = blackHole.GravityWellRadius;
        GravityWellForce = blackHole.GravityWellForce;
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.BlackHole;

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new BlackHole(this);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{base.ToString()}, GravityWellRadius={GravityWellRadius:0.000}, GravityWellForce={GravityWellForce:0.000}";
    }
}
