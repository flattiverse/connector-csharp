using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// A moon.
/// </summary>
public class Moon : SteadyUnit
{
    /// <summary>
    /// Visual type of the moon.
    /// </summary>
    public readonly MoonType Type;

    /// <summary>
    /// Metal richness of this moon.
    /// </summary>
    public readonly float Metal;

    /// <summary>
    /// Carbon richness of this moon.
    /// </summary>
    public readonly float Carbon;

    /// <summary>
    /// Hydrogen richness of this moon.
    /// </summary>
    public readonly float Hydrogen;

    /// <summary>
    /// Silicon richness of this moon.
    /// </summary>
    public readonly float Silicon;

    internal Moon(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
        if (!reader.Read(out byte typeId) || !reader.Read(out Metal) || !reader.Read(out Carbon) || !reader.Read(out Hydrogen) || !reader.Read(out Silicon))
            throw new System.IO.InvalidDataException("Couldn't read Unit.");

        Type = (MoonType)typeId;
    }

    internal Moon(Moon moon) : base(moon)
    {
        Type = moon.Type;

        Metal = moon.Metal;
        Carbon = moon.Carbon;
        Hydrogen = moon.Hydrogen;
        Silicon = moon.Silicon;
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.Moon;

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new Moon(this);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{base.ToString()}, Type={Type}, Metal={Metal:0.000}, Carbon={Carbon:0.000}, Hydrogen={Hydrogen:0.000}, Silicon={Silicon:0.000}";
    }
}
