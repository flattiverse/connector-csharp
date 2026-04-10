using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// Base type for storm whirls.
/// </summary>
public abstract class StormWhirl : MobileUnit
{
    private float _radius;
    private float _gravity;
    private ushort _remainingTicks;

    /// <summary>
    /// Remaining ticks for the current whirl phase, once the unit became fully visible.
    /// </summary>
    public ushort RemainingTicks
    {
        get { return _remainingTicks; }
    }

    internal StormWhirl(Cluster cluster, string name, PacketReader reader) : base(cluster, name)
    {
        if (!Vector.FromReader(reader, out _position) ||
            !Vector.FromReader(reader, out _movement) ||
            !reader.Read(out _radius) ||
            !reader.Read(out _gravity))
            throw new InvalidDataException("Couldn't read Unit.");
    }

    internal StormWhirl(StormWhirl stormWhirl) : base(stormWhirl)
    {
        _radius = stormWhirl._radius;
        _gravity = stormWhirl._gravity;
        _remainingTicks = stormWhirl._remainingTicks;
    }

    /// <inheritdoc/>
    public override float Radius => _radius;

    /// <inheritdoc/>
    public override float Gravity => _gravity;

    /// <inheritdoc/>
    public override bool IsSolid => false;

    internal override void UpdateMovement(PacketReader reader)
    {
        if (!Vector.FromReader(reader, out _position) || !Vector.FromReader(reader, out _movement))
            throw new InvalidDataException("Couldn't read Unit.");
    }

    private protected void ReadRemainingTicks(PacketReader reader)
    {
        if (!reader.Read(out _remainingTicks))
            throw new InvalidDataException("Couldn't read Unit.");
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{base.ToString()}, RemainingTicks={_remainingTicks}";
    }
}
