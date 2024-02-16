using Flattiverse.Connector.Network;
using System.Net.Sockets;

namespace Flattiverse.Connector.Hierarchy;

public class Cluster : INamedUnit
{
    public readonly Galaxy Galaxy;

    private byte id;
    private string name;

    private readonly Region?[] regions = new Region?[256];
    public readonly UniversalHolder<Region> Regions;

    private Cluster() { }

    internal Cluster(Galaxy galaxy, byte id, PacketReader reader)
    {
        Galaxy = galaxy;
        this.id = id;

        name = reader.ReadString();

        Regions = new UniversalHolder<Region>(regions);
    }

    public int ID => id;
    /// <summary>
    /// The name of the cluster.
    /// </summary>
    public string Name => name;

    /// <summary>
    /// Sets given values in this cluster.
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    public async Task Configure(Action<ClusterConfig> config)
    {
        ClusterConfig changes = new ClusterConfig(this);
        config(changes);

        Session session = await Galaxy.GetSession();

        Packet packet = new Packet();
        packet.Header.Command = 0x42;
        packet.Header.Param0 = id;

        using (PacketWriter writer = packet.Write())
            changes.Write(writer);

        await session.SendWait(packet);
    }

    /// <summary>
    /// Removes this cluster.
    /// </summary>
    /// <returns></returns>
    public async Task Remove()
    {
        Session session = await Galaxy.GetSession();

        Packet packet = new Packet();
        packet.Header.Command = 0x43;
        packet.Header.Param0 = id;

        await session.SendWait(packet);
    }

    /// <summary>
    /// Creates a region with given values in this cluster.
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    public async Task<Region> CreateRegion(Action<RegionConfig> config)
    {
        RegionConfig changes = RegionConfig.Default;
        config(changes);

        Session session = await Galaxy.GetSession();

        Packet packet = new Packet();
        packet.Header.Command = 0x44;
        packet.Header.Param0 = id;

        using (PacketWriter writer = packet.Write())
            changes.Write(writer);

        packet = await session.SendWait(packet);

        if (regions[packet.Header.Param0] is not Region region)
            throw GameException.TODO;

        return region;
    }

    internal void ReadRegion(byte id, PacketReader reader)
    {
        regions[id] = new Region(Galaxy, this, id, reader);
        Console.WriteLine($"Received upgrade {regions[id]!.Name} update for cluster {Name}");
    }
}