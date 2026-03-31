using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// Meteoroid map unit that can act as a mining target.
/// </summary>
public class Meteoroid : SteadyUnit
{
    private MeteoroidType _type;
    private float _metal;
    private float _carbon;
    private float _hydrogen;
    private float _silicon;

    /// <summary>
    /// Visual type of the meteoroid.
    /// </summary>
    public MeteoroidType Type
    {
        get { return _type; }
    }

    /// <summary>
    /// Metal richness of this meteoroid for the current mining model.
    /// </summary>
    public float Metal
    {
        get { return _metal; }
    }

    /// <summary>
    /// Carbon richness of this meteoroid for the current mining model.
    /// </summary>
    public float Carbon
    {
        get { return _carbon; }
    }

    /// <summary>
    /// Hydrogen richness of this meteoroid for the current mining model.
    /// </summary>
    public float Hydrogen
    {
        get { return _hydrogen; }
    }

    /// <summary>
    /// Silicon richness of this meteoroid for the current mining model.
    /// </summary>
    public float Silicon
    {
        get { return _silicon; }
    }

    internal Meteoroid(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
        if (!reader.Read(out byte typeId))
            throw new System.IO.InvalidDataException("Couldn't read Unit.");

        _type = (MeteoroidType)typeId;
        _metal = 0f;
        _carbon = 0f;
        _hydrogen = 0f;
        _silicon = 0f;
    }

    internal Meteoroid(Meteoroid meteoroid) : base(meteoroid)
    {
        _type = meteoroid._type;
        _metal = meteoroid._metal;
        _carbon = meteoroid._carbon;
        _hydrogen = meteoroid._hydrogen;
        _silicon = meteoroid._silicon;
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.Meteoroid;

    /// <inheritdoc/>
    public override bool CanBeEdited => true;

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new Meteoroid(this);
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
