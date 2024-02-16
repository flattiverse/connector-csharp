using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Hierarchy;

public class Cluster : INamedUnit
{
    public readonly Galaxy Galaxy;
    public readonly byte ID;

    private string name;

    private readonly Region?[] regions = new Region?[256];
    public readonly UniversalHolder<Region> Regions;

    internal Cluster(byte id, Galaxy galaxy, PacketReader reader)
    {
        ID = id;
        Galaxy = galaxy;

        name = reader.ReadString();

        Regions = new UniversalHolder<Region>(regions);
    }

    /// <summary>
    /// The name of the cluster.
    /// </summary>
    public string Name => name;
}