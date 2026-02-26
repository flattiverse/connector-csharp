using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// A sun.
/// </summary>
public class Sun : SteadyUnit
{
    /// <summary>
    /// Photon flux emitted by this sun.
    /// </summary>
    public readonly float Energy;

    /// <summary>
    /// Plasma wind emitted by this sun.
    /// </summary>
    public readonly float Ions;

    /// <summary>
    /// Neutrino radiation emitted by this sun. Neutrinos are not blocked by other celestial bodies.
    /// </summary>
    public readonly float Neutrinos;

    /// <summary>
    /// Thermal radiation. Heat raises energy costs.
    /// </summary>
    public readonly float Heat;

    /// <summary>
    /// Shield-drain radiation. Drain loads and slowly discharges shields.
    /// </summary>
    public readonly float Drain;

    internal Sun(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
        if (!reader.Read(out Energy) || !reader.Read(out Ions) || !reader.Read(out Neutrinos) || !reader.Read(out Heat) || !reader.Read(out Drain))
            throw new System.IO.InvalidDataException("Couldn't read Unit.");
    }

    internal Sun(Sun sun) : base(sun)
    {
        Energy = sun.Energy;
        Ions = sun.Ions;
        Neutrinos = sun.Neutrinos;
        Heat = sun.Heat;
        Drain = sun.Drain;
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.Sun;

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new Sun(this);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{base.ToString()}, Energy={Energy:0.000}, Ions={Ions:0.000}, Neutrinos={Neutrinos:0.000}, Heat={Heat:0.000}, Drain={Drain:0.000}";
    }
}
