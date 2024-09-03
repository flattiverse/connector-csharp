using System.Diagnostics.CodeAnalysis;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

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

    /// <summary>
    /// The galaxy this cluster is in.
    /// </summary>
    public readonly Galaxy Galaxy;
    
    private string _name;
    private bool _active;
    
    private Dictionary<string, Unit> _units;

    internal Cluster(Galaxy galaxy, byte id, string name)
    {
        Galaxy = galaxy;
        Id = id;
        _name = name;
        
        _units = new Dictionary<string, Unit>();
    }

    internal void Update(string name)
    {
        _name = name;
    }
    
    internal void Deactivate()
    {
        _active = false;
    }

    internal bool UpdateUnit(string name, PacketReader reader, [NotNullWhen(true)] out Unit? unit)
    {
        if (!_units.TryGetValue(name, out unit))
            return false;

        unit.UpdateMovement(reader);

        return true;
    }
    
    internal void AddUnit(Unit unit)
    {
        _units.Add(unit.Name, unit);
    }

    internal bool RemoveUnit(string name, [NotNullWhen(true)] out Unit? unit)
    {
        return _units.Remove(name, out unit);
    }

    internal bool GetUnit([NotNullWhen(true)] out Unit? unit)
    {
        return _units.TryGetValue(_name, out unit);
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