using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

public class SteadyUnit : Unit
{
    private float _gravity;
    
    internal SteadyUnit(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
        if (!reader.Read(out _gravity))
            throw new InvalidDataException("Couldn't read Unit.");
    }

    internal SteadyUnit(SteadyUnit unit) : base(unit)
    {
        _gravity = unit._gravity;
    }

    /// <inheritdoc/>
    public override float Gravity => _gravity;
}