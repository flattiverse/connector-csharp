using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// A black hole.
/// </summary>
public class BlackHole : SteadyUnit
{
    private float _gravityWellRadius;
    private float _gravityWellForce;

    /// <summary>
    /// Radius of the intensified gravity well.
    /// </summary>
    public float GravityWellRadius
    {
        get { return _gravityWellRadius; }
    }

    /// <summary>
    /// Additional attraction force inside the gravity well.
    /// </summary>
    public float GravityWellForce
    {
        get { return _gravityWellForce; }
    }

    internal BlackHole(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
        _gravityWellRadius = 0f;
        _gravityWellForce = 0f;
    }

    internal BlackHole(BlackHole blackHole) : base(blackHole)
    {
        _gravityWellRadius = blackHole._gravityWellRadius;
        _gravityWellForce = blackHole._gravityWellForce;
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.BlackHole;

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new BlackHole(this);
    }

    internal override void UpdateState(PacketReader reader)
    {
        base.UpdateState(reader);

        if (!reader.Read(out _gravityWellRadius) || !reader.Read(out _gravityWellForce))
            throw new System.IO.InvalidDataException("Couldn't read Unit.");
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{base.ToString()}, GravityWellRadius={_gravityWellRadius:0.000}, GravityWellForce={_gravityWellForce:0.000}";
    }
}
