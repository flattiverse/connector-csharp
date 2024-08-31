namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// This is a subset of the galaxy. Each cluster is a map.
/// </summary>
public class Cluster : INamedUnit
{
    /// <summary>
    /// The id within the galaxy of the cluster.
    /// </summary>
    public readonly byte Id;

    private string _name;
    private bool _active;

    internal Cluster(byte id, string name)
    {
        Id = id;
        _name = name;
    }

    internal void Update(string name)
    {
        _name = name;
    }
    
    internal void Deactivate()
    {
        _active = false;
    }
    
    /// <summary>
    /// The name of the Cluster.
    /// </summary>
    public string Name => _name;

    /// <summary>
    /// If false, you have been disconnected or the cluster has been removed and therefore disabled.
    /// </summary>
    public bool Active => _active;
}