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

    internal ClusterSnapshot(byte id, string name, bool active)
    {
        Id = id;
        Name = name;
        Active = active;
    }
}
