using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// A planet.
/// </summary>
public class Planet : SteadyUnit
{
    private PlanetType _type;
    private float _metal;
    private float _carbon;
    private float _hydrogen;
    private float _silicon;

    /// <summary>
    /// Visual type of the planet.
    /// </summary>
    public PlanetType Type
    {
        get { return _type; }
    }

    /// <summary>
    /// Metal richness of this planet.
    /// </summary>
    public float Metal
    {
        get { return _metal; }
    }

    /// <summary>
    /// Carbon richness of this planet.
    /// </summary>
    public float Carbon
    {
        get { return _carbon; }
    }

    /// <summary>
    /// Hydrogen richness of this planet.
    /// </summary>
    public float Hydrogen
    {
        get { return _hydrogen; }
    }

    /// <summary>
    /// Silicon richness of this planet.
    /// </summary>
    public float Silicon
    {
        get { return _silicon; }
    }

    internal Planet(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
        if (!reader.Read(out byte typeId))
            throw new System.IO.InvalidDataException("Couldn't read Unit.");

        _type = (PlanetType)typeId;
        _metal = 0f;
        _carbon = 0f;
        _hydrogen = 0f;
        _silicon = 0f;
    }

    internal Planet(Planet planet) : base(planet)
    {
        _type = planet._type;
        _metal = planet._metal;
        _carbon = planet._carbon;
        _hydrogen = planet._hydrogen;
        _silicon = planet._silicon;
    }
    
    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.Planet;

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new Planet(this);
    }

    internal override void UpdateState(PacketReader reader)
    {
        base.UpdateState(reader);

        if (!reader.Read(out _metal) || !reader.Read(out _carbon) || !reader.Read(out _hydrogen) || !reader.Read(out _silicon))
            throw new System.IO.InvalidDataException("Couldn't read Unit.");
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{base.ToString()}, Type={_type}, Metal={_metal:0.000}, Carbon={_carbon:0.000}, Hydrogen={_hydrogen:0.000}, Silicon={_silicon:0.000}";
    }
}
