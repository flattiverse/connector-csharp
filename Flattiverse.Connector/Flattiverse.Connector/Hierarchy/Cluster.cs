using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Hierarchy;

public class Cluster : INamedUnit
{
    public readonly Galaxy Galaxy;
    public readonly byte ID;

    private string name;

    internal Cluster(byte id, Galaxy galaxy, PacketReader reader)
    {
        ID = id;
        Galaxy = galaxy;

        name = reader.ReadString();
    }

    /// <summary>
    /// The name of the cluster.
    /// </summary>
    public string Name => name;
}