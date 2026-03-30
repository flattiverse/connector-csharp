using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// A gate that may block movement and restore automatically.
/// </summary>
public class Gate : SteadyUnit
{
    private ushort _linkId;
    private bool _defaultClosed;
    private int? _restoreTicks;
    private bool _closed;
    private int? _restoreRemainingTicks;

    internal Gate(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
        if (!reader.Read(out _linkId) ||
            !reader.Read(out byte defaultClosed) ||
            !reader.Read(out int restoreTicks))
            throw new InvalidDataException("Couldn't read Gate.");

        _defaultClosed = defaultClosed != 0x00;
        _restoreTicks = restoreTicks < 0 ? null : restoreTicks;
        _closed = _defaultClosed;
        _restoreRemainingTicks = null;
    }

    internal Gate(Gate gate) : base(gate)
    {
        _linkId = gate._linkId;
        _defaultClosed = gate._defaultClosed;
        _restoreTicks = gate._restoreTicks;
        _closed = gate._closed;
        _restoreRemainingTicks = gate._restoreRemainingTicks;
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.Gate;

    /// <inheritdoc/>
    public override bool CanBeEdited => true;

    /// <inheritdoc/>
    public override bool IsMasking => _closed;

    /// <inheritdoc/>
    public override bool IsSolid => _closed;

    /// <summary>
    /// Link id shared with switches.
    /// </summary>
    public ushort LinkId => _linkId;

    /// <summary>
    /// Default closed state to which the gate may restore.
    /// </summary>
    public bool DefaultClosed => _defaultClosed;

    /// <summary>
    /// Optional configured restore delay in ticks.
    /// </summary>
    public int? RestoreTicks => _restoreTicks;

    /// <summary>
    /// Current gate state.
    /// </summary>
    public bool Closed => _closed;

    /// <summary>
    /// Remaining restore delay in ticks while a restore is armed.
    /// </summary>
    public int? RestoreRemainingTicks => _restoreRemainingTicks;

    internal override void UpdateState(PacketReader reader)
    {
        base.UpdateState(reader);

        if (!reader.Read(out byte closed) || !reader.Read(out int restoreRemainingTicks))
            throw new InvalidDataException("Couldn't read Gate state.");

        _closed = closed != 0x00;
        _restoreRemainingTicks = restoreRemainingTicks < 0 ? null : restoreRemainingTicks;
    }

    /// <inheritdoc/>
    public override Unit Clone()
    {
        return new Gate(this);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        string restoreText = _restoreTicks is null ? "-" : _restoreTicks.Value.ToString();
        string remainingText = _restoreRemainingTicks is null ? "-" : _restoreRemainingTicks.Value.ToString();
        return $"{base.ToString()}, LinkId={_linkId}, DefaultClosed={_defaultClosed}, Closed={_closed}, RestoreTicks={restoreText}, RestoreRemainingTicks={remainingText}";
    }
}
