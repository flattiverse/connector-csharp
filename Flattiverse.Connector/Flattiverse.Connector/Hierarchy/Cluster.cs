using Flattiverse.Connector.Network;
using Flattiverse.Connector.UnitConfigurations;
using Flattiverse.Connector.Units;
using System.Net.Sockets;
using System.Numerics;

namespace Flattiverse.Connector.Hierarchy;

public class Cluster : INamedUnit
{
    public readonly Galaxy Galaxy;
    private Map map;// TODO: MALUK entfernen

    private byte id;
    private ClusterConfig config;

    private readonly Region?[] regions = new Region?[256];
    public readonly UniversalHolder<Region> Regions;

    private Cluster() { }

    internal Cluster(Galaxy galaxy, byte id, PacketReader reader)
    {
        Galaxy = galaxy;
        map = new Map();// TODO: MALUK entfernen
        this.id = id;

        config = new ClusterConfig(reader);

        Regions = new UniversalHolder<Region>(regions);
    }

    public int ID => id;
    /// <summary>
    /// The name of the cluster.
    /// </summary>
    public string Name => config.Name;
    public ClusterConfig Config => config;

    /// <summary>
    /// Sets given values in this cluster.
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    public async Task Configure(Action<ClusterConfig> config)
    {
        ClusterConfig changes = new ClusterConfig(this.config);
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

        await session.SendWait(packet);

        if (regions[packet.Header.Param0] is not Region region)
            throw new GameException("Creation successfull, but connector didn't receive update yet.");//Should never happen

        return region;
    }

    /// <summary>
    /// Creates a sun with given values in this cluster.
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    public async Task<Sun> CreateSun(Action<SunConfiguration> config)
    {
        SunConfiguration changes = SunConfiguration.Default;
        config(changes);

        Session session = await Galaxy.GetSession();

        Packet packet = new Packet();
        packet.Header.Command = 0x51;
        packet.Header.Id0 = id;
        packet.Header.Param0 = (byte)UnitKind.Sun;

        string name = changes.Name;

        using (PacketWriter writer = packet.Write())
            changes.Write(writer);

        await session.SendWait(packet);

        if (map.TryGet(name, out Unit? unit) || unit is not Sun sun)
            throw new GameException("Creation successfull, but connector didn't receive update yet.");//Should never happen

        return sun;
    }

    /// <summary>
    /// Creates a blackhole with given values in this cluster.
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    public async Task<BlackHole> CreateBlackhole(Action<BlackholeConfiguration> config)
    {
        BlackholeConfiguration changes = BlackholeConfiguration.Default;
        config(changes);

        Session session = await Galaxy.GetSession();

        Packet packet = new Packet();
        packet.Header.Command = 0x51;
        packet.Header.Id0 = id;
        packet.Header.Param0 = (byte)UnitKind.BlackHole;

        string name = changes.Name;

        using (PacketWriter writer = packet.Write())
            changes.Write(writer);

        await session.SendWait(packet);

        if (map.TryGet(name, out Unit? unit) || unit is not BlackHole blackHole)
            throw new GameException("Creation successfull, but connector didn't receive update yet.");//Should never happen

        return blackHole;
    }

    /// <summary>
    /// Creates a planet with given values in this cluster.
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    public async Task<Planet> CreatePlanet(Action<PlanetConfiguration> config)
    {
        PlanetConfiguration changes = PlanetConfiguration.Default;
        config(changes);

        Session session = await Galaxy.GetSession();

        Packet packet = new Packet();
        packet.Header.Command = 0x51;
        packet.Header.Id0 = id;
        packet.Header.Param0 = (byte)UnitKind.Planet;

        string name = changes.Name;

        using (PacketWriter writer = packet.Write())
            changes.Write(writer);

        await session.SendWait(packet);

        if (map.TryGet(name, out Unit? unit) || unit is not Planet planet)
            throw new GameException("Creation successfull, but connector didn't receive update yet.");//Should never happen

        return planet;
    }

    /// <summary>
    /// Creates a moon with given values in this cluster.
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    public async Task<Moon> CreateMoon(Action<MoonConfiguration> config)
    {
        MoonConfiguration changes = MoonConfiguration.Default;
        config(changes);

        Session session = await Galaxy.GetSession();

        Packet packet = new Packet();
        packet.Header.Command = 0x51;
        packet.Header.Id0 = id;
        packet.Header.Param0 = (byte)UnitKind.Moon;

        string name = changes.Name;

        using (PacketWriter writer = packet.Write())
            changes.Write(writer);

        await session.SendWait(packet);

        if (map.TryGet(name, out Unit? unit) || unit is not Moon moon)
            throw new GameException("Creation successfull, but connector didn't receive update yet.");//Should never happen

        return moon;
    }

    /// <summary>
    /// Creates a meteoroid with given values in this cluster.
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    public async Task<Meteoroid> CreateMeteoroid(Action<MeteoroidConfiguration> config)
    {
        MeteoroidConfiguration changes = MeteoroidConfiguration.Default;
        config(changes);

        Session session = await Galaxy.GetSession();

        Packet packet = new Packet();
        packet.Header.Command = 0x51;
        packet.Header.Id0 = id;
        packet.Header.Param0 = (byte)UnitKind.Meteoroid;

        string name = changes.Name;

        using (PacketWriter writer = packet.Write())
            changes.Write(writer);

        await session.SendWait(packet);

        if (map.TryGet(name, out Unit? unit) || unit is not Meteoroid meteoroid)
            throw new GameException("Creation successfull, but connector didn't receive update yet.");//Should never happen

        return meteoroid;
    }

    /// <summary>
    /// Creates a buoy with given values in this cluster.
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    public async Task<Buoy> CreateBuoy(Action<BuoyConfiguration> config)
    {
        BuoyConfiguration changes = BuoyConfiguration.Default;
        config(changes);

        Session session = await Galaxy.GetSession();

        Packet packet = new Packet();
        packet.Header.Command = 0x51;
        packet.Header.Id0 = id;
        packet.Header.Param0 = (byte)UnitKind.Buoy;

        string name = changes.Name;

        using (PacketWriter writer = packet.Write())
            changes.Write(writer);

        await session.SendWait(packet);

        if (map.TryGet(name, out Unit? unit) || unit is not Buoy buoy)
            throw new GameException("Creation successfull, but connector didn't receive update yet.");//Should never happen

        return buoy;
    }

    internal void ReadRegion(byte id, PacketReader reader)
    {
        regions[id] = new Region(Galaxy, this, id, reader);
        Console.WriteLine($"Received upgrade {regions[id]!.Name} update for cluster {Name}");
    }
}