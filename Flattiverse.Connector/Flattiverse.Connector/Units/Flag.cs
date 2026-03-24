using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// A flag target.
/// </summary>
public class Flag : Target
{
    private int _graceTicks;
    private bool _active;

    internal Flag(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
        _graceTicks = 0;
        _active = true;
    }

    internal Flag(Flag flag) : base(flag)
    {
        _graceTicks = flag._graceTicks;
        _active = flag._active;
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.Flag;

    /// <summary>
    /// Configured flag grace time in ticks.
    /// </summary>
    public int GraceTicks => _graceTicks;

    /// <summary>
    /// True while the flag can currently be scored.
    /// </summary>
    public bool Active => _active;

    internal override void UpdateState(PacketReader reader)
    {
        base.UpdateState(reader);

        if (!reader.Read(out _graceTicks) || !reader.Read(out byte active))
            throw new InvalidDataException("Couldn't read Flag.");

        _active = active != 0x00;
    }

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new Flag(this);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{base.ToString()}, GraceTicks={_graceTicks}, Active={_active}";
    }
}
