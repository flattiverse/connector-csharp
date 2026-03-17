using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// A buoy.
/// </summary>
public class Buoy : SteadyUnit
{
    private string? _message;

    internal Buoy(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
        _message = null;
    }

    internal Buoy(Buoy buoy) : base(buoy)
    {
        _message = buoy._message;
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.Buoy;

    /// <inheritdoc/>
    public override bool IsMasking => false;

    /// <inheritdoc/>
    public override bool IsSolid => false;

    /// <summary>
    /// Optional buoy message. Null means no message.
    /// </summary>
    public string? Message => _message;

    internal override void UpdateState(PacketReader reader)
    {
        base.UpdateState(reader);

        if (!reader.Read(out string message))
            throw new InvalidDataException("Couldn't read Buoy.");

        _message = string.IsNullOrEmpty(message) ? null : message;
    }

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new Buoy(this);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        string message = _message is null ? "-" : _message;
        return $"{base.ToString()}, Message=\"{message}\"";
    }
}
