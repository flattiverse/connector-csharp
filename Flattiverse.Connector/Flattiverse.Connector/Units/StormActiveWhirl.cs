using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// A storm whirl that is active, masking, and damaging.
/// </summary>
public class StormActiveWhirl : StormWhirl
{
    private float _damage;

    /// <summary>
    /// Damage applied by each successful hit of this active whirl.
    /// </summary>
    public float Damage
    {
        get { return _damage; }
    }

    internal StormActiveWhirl(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
        _damage = 0f;
    }

    internal StormActiveWhirl(StormActiveWhirl stormActiveWhirl) : base(stormActiveWhirl)
    {
        _damage = stormActiveWhirl._damage;
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.StormActiveWhirl;

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new StormActiveWhirl(this);
    }

    internal override void UpdateState(PacketReader reader)
    {
        base.UpdateState(reader);
        ReadRemainingTicks(reader);

        if (!reader.Read(out _damage))
            throw new InvalidDataException("Couldn't read Unit.");
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{base.ToString()}, Damage={_damage:0.###}";
    }
}
