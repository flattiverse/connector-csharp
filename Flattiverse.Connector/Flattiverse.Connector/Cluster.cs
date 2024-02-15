using Flattiverse.Connector.Network;

namespace Flattiverse.Connector;

class Cluster
{
    public readonly Galaxy Galaxy;
    public readonly byte ID;

    private string name;

    public Cluster(byte id, Galaxy galaxy, PacketReader reader)
    {
        ID = id;
        Galaxy = galaxy;

        name = reader.ReadString();
    }

    public string Name => name;
}