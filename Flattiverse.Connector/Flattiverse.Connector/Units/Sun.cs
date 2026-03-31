using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// Stellar map unit that acts as a major energy intake source and environmental hazard.
/// </summary>
public class Sun : SteadyUnit
{
    private float _energy;
    private float _ions;
    private float _neutrinos;
    private float _heat;
    private float _drain;

    /// <summary>
    /// Photon flux emitted by this sun.
    /// Energy cells can harvest this field.
    /// </summary>
    public float Energy
    {
        get { return _energy; }
    }

    /// <summary>
    /// Plasma wind emitted by this sun.
    /// Ion cells can harvest this field.
    /// </summary>
    public float Ions
    {
        get { return _ions; }
    }

    /// <summary>
    /// Neutrino radiation emitted by this sun. Neutrinos are not blocked by other celestial bodies.
    /// </summary>
    public float Neutrinos
    {
        get { return _neutrinos; }
    }

    /// <summary>
    /// Thermal radiation. Each point drains 15 energy per tick before any remaining overflow turns into radiation damage.
    /// </summary>
    public float Heat
    {
        get { return _heat; }
    }

    /// <summary>
    /// Ionizing radiation component. Each point causes 0.125 hull damage per tick after armor reduction.
    /// </summary>
    public float Drain
    {
        get { return _drain; }
    }

    internal Sun(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
        _energy = 0f;
        _ions = 0f;
        _neutrinos = 0f;
        _heat = 0f;
        _drain = 0f;
    }

    internal Sun(Sun sun) : base(sun)
    {
        _energy = sun._energy;
        _ions = sun._ions;
        _neutrinos = sun._neutrinos;
        _heat = sun._heat;
        _drain = sun._drain;
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.Sun;

    /// <inheritdoc/>
    public override bool CanBeEdited => true;

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new Sun(this);
    }

    internal override void UpdateState(PacketReader reader)
    {
        base.UpdateState(reader);

        if (!reader.Read(out _energy) || !reader.Read(out _ions) || !reader.Read(out _neutrinos) || !reader.Read(out _heat) || !reader.Read(out _drain))
            throw new System.IO.InvalidDataException("Couldn't read Unit.");
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{base.ToString()}, Energy={_energy:0.000}, Ions={_ions:0.000}, Neutrinos={_neutrinos:0.000}, Heat={_heat:0.000}, Drain={_drain:0.000}";
    }
}
