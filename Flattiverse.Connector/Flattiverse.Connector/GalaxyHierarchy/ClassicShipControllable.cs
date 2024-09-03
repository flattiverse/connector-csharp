using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// The controllable of a classic ship.
/// </summary>
public class ClassicShipControllable : Controllable
{
    internal ClassicShipControllable(Cluster cluster, byte id, string name, PacketReader reader) : base(id, name,
        cluster, reader)
    {
    }
}