using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// Visible nebula map unit that can be harvested by nebula collectors and acts as the source material for crystals.
/// </summary>
public class Nebula : SteadyUnit
{
    private float _hue;

    /// <summary>
    /// Hue value of the nebula material.
    /// </summary>
    public float Hue
    {
        get { return _hue; }
    }

    internal Nebula(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
        _hue = 0f;
    }

    internal Nebula(Nebula nebula) : base(nebula)
    {
        _hue = nebula._hue;
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.Nebula;

    /// <inheritdoc/>
    public override bool IsMasking => false;

    /// <inheritdoc/>
    public override bool IsSolid => false;

    /// <inheritdoc/>
    public override bool CanBeEdited => true;

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new Nebula(this);
    }

    internal override void UpdateState(PacketReader reader)
    {
        base.UpdateState(reader);

        if (!reader.Read(out _hue))
            throw new System.IO.InvalidDataException("Couldn't read Unit.");
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{base.ToString()}, Hue={_hue:0.###}";
    }
}
