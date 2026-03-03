namespace Flattiverse.Connector.Events;

/// <summary>
/// Snapshot of a cluster state relevant for events.
/// </summary>
public class ClusterSnapshot
{
    /// <summary>
    /// Cluster id.
    /// </summary>
    public readonly byte Id;

    /// <summary>
    /// Cluster name.
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// Cluster activity flag.
    /// </summary>
    public readonly bool Active;

    /// <summary>
    /// Start-cluster flag.
    /// </summary>
    public readonly bool Start;

    /// <summary>
    /// Respawn-cluster flag.
    /// </summary>
    public readonly bool Respawn;

    internal ClusterSnapshot(byte id, string name, bool active, bool start, bool respawn)
    {
        Id = id;
        Name = name;
        Active = active;
        Start = start;
        Respawn = respawn;
    }
}
