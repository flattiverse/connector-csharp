using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// A triggerable switch that affects linked gates.
/// </summary>
public class Switch : SteadyUnit
{
    private Team _team;
    private ushort _linkId;
    private float _range;
    private ushort _cooldownTicks;
    private ushort _cooldownRemainingTicks;
    private SwitchMode _mode;
    private bool _switched;

    internal Switch(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
        if (!reader.Read(out byte teamId) ||
            !cluster.Galaxy.Teams.TryGet(teamId, out Team? team) ||
            !reader.Read(out _linkId) ||
            !reader.Read(out _range) ||
            !reader.Read(out _cooldownTicks) ||
            !reader.Read(out byte mode))
            throw new InvalidDataException("Couldn't read Switch.");

        _team = team;
        _mode = (SwitchMode)mode;
        _cooldownRemainingTicks = 0;
        _switched = false;
    }

    internal Switch(Switch switchUnit) : base(switchUnit)
    {
        _team = switchUnit._team;
        _linkId = switchUnit._linkId;
        _range = switchUnit._range;
        _cooldownTicks = switchUnit._cooldownTicks;
        _cooldownRemainingTicks = switchUnit._cooldownRemainingTicks;
        _mode = switchUnit._mode;
        _switched = switchUnit._switched;
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.Switch;

    /// <inheritdoc/>
    public override bool CanBeEdited => true;

    /// <inheritdoc/>
    public override Team? Team => _team;

    /// <summary>
    /// Link id shared with linked gates.
    /// </summary>
    public ushort LinkId => _linkId;

    /// <summary>
    /// Search radius for linked gates.
    /// </summary>
    public float Range => _range;

    /// <summary>
    /// Configured switch cooldown in ticks.
    /// </summary>
    public ushort CooldownTicks => _cooldownTicks;

    /// <summary>
    /// Remaining runtime cooldown in ticks.
    /// </summary>
    public ushort CooldownRemainingTicks => _cooldownRemainingTicks;

    /// <summary>
    /// Switch output mode.
    /// </summary>
    public SwitchMode Mode => _mode;

    /// <summary>
    /// Runtime toggled state of the switch itself.
    /// </summary>
    public bool Switched => _switched;

    internal override void UpdateState(PacketReader reader)
    {
        base.UpdateState(reader);

        if (!reader.Read(out _cooldownRemainingTicks) || !reader.Read(out byte switched))
            throw new InvalidDataException("Couldn't read Switch state.");

        _switched = switched != 0x00;
    }

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new Switch(this);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{base.ToString()}, Team={_team.Name}, LinkId={_linkId}, Range={_range:0.###}, CooldownTicks={_cooldownTicks}, CooldownRemainingTicks={_cooldownRemainingTicks}, Mode={_mode}, Switched={_switched}";
    }
}
