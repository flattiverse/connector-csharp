using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Event emitted when a gate auto-restores to its configured default state.
/// </summary>
public class RestoredGateEvent : FlattiverseEvent
{
    /// <summary>
    /// Cluster containing the gate.
    /// </summary>
    public readonly Cluster Cluster;

    /// <summary>
    /// Name of the restored gate.
    /// </summary>
    public readonly string GateName;

    /// <summary>
    /// Final closed state after the restore.
    /// </summary>
    public readonly bool Closed;

    internal RestoredGateEvent(Cluster cluster, string gateName, bool closed)
    {
        Cluster = cluster;
        GateName = gateName;
        Closed = closed;
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.GateRestored;

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} Gate \"{GateName}\" in cluster {Cluster.Name} auto-restored to {(Closed ? "closed" : "open")}.";
    }
}
