using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// A meteoroid.
/// </summary>
public class Meteoroid : SteadyUnit
{
    /// <summary>
    /// Visual type of the meteoroid.
    /// </summary>
    public readonly MeteoroidType Type;

    /// <summary>
    /// Metal richness of this meteoroid.
    /// </summary>
    public readonly float Metal;

    /// <summary>
    /// Carbon richness of this meteoroid.
    /// </summary>
    public readonly float Carbon;

    /// <summary>
    /// Hydrogen richness of this meteoroid.
    /// </summary>
    public readonly float Hydrogen;

    /// <summary>
    /// Silicon richness of this meteoroid.
    /// </summary>
    public readonly float Silicon;

    internal Meteoroid(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
        if (!reader.Read(out byte typeId) || !reader.Read(out Metal) || !reader.Read(out Carbon) || !reader.Read(out Hydrogen) || !reader.Read(out Silicon))
            throw new System.IO.InvalidDataException("Couldn't read Unit.");

        Type = (MeteoroidType)typeId;
    }

    internal Meteoroid(Meteoroid meteoroid) : base(meteoroid)
    {
        Type = meteoroid.Type;

        Metal = meteoroid.Metal;
        Carbon = meteoroid.Carbon;
        Hydrogen = meteoroid.Hydrogen;
        Silicon = meteoroid.Silicon;
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.Meteoroid;

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new Meteoroid(this);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{base.ToString()}, Type={Type}, Metal={Metal:0.000}, Carbon={Carbon:0.000}, Hydrogen={Hydrogen:0.000}, Silicon={Silicon:0.000}";
    }
}
