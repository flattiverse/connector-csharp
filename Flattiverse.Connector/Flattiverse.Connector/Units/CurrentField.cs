using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// A non-solid field that induces movement on mobile units.
/// </summary>
public class CurrentField : SteadyUnit
{
    private CurrentFieldMode _mode;
    private Vector _flow;
    private float _radialForce;
    private float _tangentialForce;

    /// <summary>
    /// Current-field mode.
    /// </summary>
    public CurrentFieldMode Mode
    {
        get { return _mode; }
    }

    /// <summary>
    /// Fixed world-space movement vector for directional fields.
    /// </summary>
    public Vector Flow
    {
        get { return _flow; }
    }

    /// <summary>
    /// Radial movement component for relative fields.
    /// </summary>
    public float RadialForce
    {
        get { return _radialForce; }
    }

    /// <summary>
    /// Tangential movement component for relative fields.
    /// </summary>
    public float TangentialForce
    {
        get { return _tangentialForce; }
    }

    internal CurrentField(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
        _mode = CurrentFieldMode.Directional;
        _flow = new Vector();
        _radialForce = 0f;
        _tangentialForce = 0f;
    }

    internal CurrentField(CurrentField currentField) : base(currentField)
    {
        _mode = currentField._mode;
        _flow = new Vector(currentField._flow);
        _radialForce = currentField._radialForce;
        _tangentialForce = currentField._tangentialForce;
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.CurrentField;

    /// <inheritdoc/>
    public override bool IsMasking => false;

    /// <inheritdoc/>
    public override bool IsSolid => false;

    /// <inheritdoc/>
    public override bool CanBeEdited => true;

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new CurrentField(this);
    }

    internal override void UpdateState(PacketReader reader)
    {
        base.UpdateState(reader);

        if (!reader.Read(out byte modeValue) || modeValue > (byte)CurrentFieldMode.Relative ||
            !Vector.FromReader(reader, out Vector flow) || !reader.Read(out _radialForce) || !reader.Read(out _tangentialForce))
            throw new System.IO.InvalidDataException("Couldn't read Unit.");

        _mode = (CurrentFieldMode)modeValue;
        _flow = flow;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{base.ToString()}, Mode={_mode}, Flow={_flow}, RadialForce={_radialForce:0.000}, TangentialForce={_tangentialForce:0.000}";
    }
}
