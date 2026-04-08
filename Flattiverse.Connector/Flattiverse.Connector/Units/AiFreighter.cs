using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// Mobile NPC freighter with visible loot values.
/// </summary>
public class AiFreighter : MobileNpcUnit
{
    private float _metal;
    private float _carbon;
    private float _hydrogen;
    private float _silicon;

    internal AiFreighter(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
        _metal = 0f;
        _carbon = 0f;
        _hydrogen = 0f;
        _silicon = 0f;
    }

    internal AiFreighter(AiFreighter unit) : base(unit)
    {
        _metal = unit._metal;
        _carbon = unit._carbon;
        _hydrogen = unit._hydrogen;
        _silicon = unit._silicon;
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.AiFreighter;

    /// <summary>
    /// Metal loot value.
    /// </summary>
    public float Metal
    {
        get { return _metal; }
    }

    /// <summary>
    /// Carbon loot value.
    /// </summary>
    public float Carbon
    {
        get { return _carbon; }
    }

    /// <summary>
    /// Hydrogen loot value.
    /// </summary>
    public float Hydrogen
    {
        get { return _hydrogen; }
    }

    /// <summary>
    /// Silicon loot value.
    /// </summary>
    public float Silicon
    {
        get { return _silicon; }
    }

    internal override void UpdateState(PacketReader reader)
    {
        base.UpdateState(reader);

        if (!reader.Read(out _metal) || !reader.Read(out _carbon) || !reader.Read(out _hydrogen) || !reader.Read(out _silicon))
            throw new InvalidDataException("Couldn't read Unit.");
    }

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new AiFreighter(this);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{base.ToString()}, Loot=({_metal:0.###},{_carbon:0.###},{_hydrogen:0.###},{_silicon:0.###})";
    }
}
