namespace Flattiverse.Connector.Events;

/// <summary>
/// Is raised when a cluster has been removed.
/// </summary>
public class RemovedClusterEvent : FlattiverseEvent
{
    /// <summary>
    /// Snapshot of the removed cluster.
    /// </summary>
    public readonly ClusterSnapshot Cluster;

    internal RemovedClusterEvent(ClusterSnapshot cluster)
    {
        Cluster = cluster;
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.ClusterRemoved;

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} Cluster removed: Id={Cluster.Id}, Name=\"{Cluster.Name}\", Active={Cluster.Active}, Start={Cluster.Start}, Respawn={Cluster.Respawn}.";
    }
}
