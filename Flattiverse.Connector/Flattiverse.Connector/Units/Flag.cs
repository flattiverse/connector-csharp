using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// Flag target used by capture-style scenarios.
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
    /// Grace or cooldown time in ticks configured for this flag.
    /// </summary>
    public int GraceTicks => _graceTicks;

    /// <summary>
    /// True while the flag is currently active and can be interacted with normally.
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
