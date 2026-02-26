namespace Flattiverse.Connector.Events;

/// <summary>
/// Is raised when a cluster has been created.
/// </summary>
public class ClusterCreatedEvent : FlattiverseEvent
{
    /// <summary>
    /// Snapshot of the created cluster.
    /// </summary>
    public readonly ClusterSnapshot Cluster;

    internal ClusterCreatedEvent(ClusterSnapshot cluster)
    {
        Cluster = cluster;
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.ClusterCreated;

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} Cluster created: Id={Cluster.Id}, Name=\"{Cluster.Name}\", Active={Cluster.Active}.";
    }
}
