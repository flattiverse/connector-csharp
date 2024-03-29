﻿using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.UnitConfigurations;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Hierarchy;

/// <summary>
/// This is a sub map of the Galaxy which is usually used to create logical groups of game units
/// within galaxies to optimize unit movement and collision processing.
/// </summary>
public class Cluster : INamedUnit
{
    /// <summary>
    /// The galaxy this cluster is part of.
    /// </summary>
    public readonly Galaxy Galaxy;

    private Dictionary<string, Unit> units;

    private bool active;
    
    internal byte id;
    private ClusterConfig config;

    internal readonly Region?[] regions = new Region?[256];

    /// <summary>
    /// The subregions of the cluster.
    /// </summary>
    public readonly UniversalHolder<Region> Regions;

    internal Cluster(Galaxy galaxy, byte id, PacketReader reader)
    {
        Galaxy = galaxy;

        units = new Dictionary<string, Unit>();

        this.id = id;
        active = true;

        config = new ClusterConfig(reader);

        Regions = new UniversalHolder<Region>(regions);
    }

    internal void Update(PacketReader reader)
    {
        config = new ClusterConfig(reader);
    }

    internal void Deactivate()
    {
        active = false;
    }
    
    /// <summary>
    /// This flag indicates if the cluster is active and part of the simulation.
    /// </summary>
    public bool IsActive => active;
    
    /// <summary>
    /// TODO JOW: Öffentliche ID's entfernen.
    /// </summary>
    public int Id => id;

    /// <summary>
    /// The name of the cluster.
    /// </summary>
    public string Name => config.Name;

    /// <summary>
    /// The configuration the cluster will use
    /// </summary>
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
        packet.Header.Id0 = id;

        using (PacketWriter writer = packet.Write())
            changes.Write(writer);

        await session.SendWait(packet);
    }

    internal Unit SeeNewUnit(UnitKind kind, PacketReader reader)
    {
        Unit unit = Unit.FromPacket(this, kind, reader);

        units.Add(unit.Name, unit);

        return unit;
    }

    internal Unit SeeUpdatedUnit(PacketReader reader)
    {
        string name = reader.PeekString();
        Unit? unit;

        if (!units.TryGetValue(name, out unit))
            Debug.Fail($"Requested unit \"{name}\" should be know but isn't in my units dictionary.");

        unit.Update(reader);

        return unit;
    }

    internal Unit SeeUnitNoMore(string name)
    {
        Unit? unit;

        if (!units.TryGetValue(name, out unit))
            Debug.Fail($"Requested unit \"{name}\" should be know but isn't in my units dictionary.");

        units.Remove(name);

        unit.Deactivate();

        return unit;
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
        packet.Header.Id0 = id;

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
        packet.Header.Id0 = id;

        using (PacketWriter writer = packet.Write())
            changes.Write(writer);

        await session.SendWait(packet);

        if (regions[packet.Header.Id0] is not Region region)
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

        if (!units.TryGetValue(name, out Unit? unit) || unit is not Sun sun)
            throw new GameException(0x35);

        return sun;
    }

    /// <summary>
    /// Creates a black hole with given values in this cluster.
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    public async Task<BlackHole> CreateBlackhole(Action<BlackHoleConfiguration> config)
    {
        BlackHoleConfiguration changes = BlackHoleConfiguration.Default;
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

        if (!units.TryGetValue(name, out Unit? unit) || unit is not BlackHole blackHole)
            throw new GameException(0x35);

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

        if (!units.TryGetValue(name, out Unit? unit) || unit is not Planet planet)
            throw new GameException(0x35);

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

        if (!units.TryGetValue(name, out Unit? unit) || unit is not Moon moon)
            throw new GameException(0x35);

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

        if (!units.TryGetValue(name, out Unit? unit) || unit is not Meteoroid meteoroid)
            throw new GameException(0x35);

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

        if (!units.TryGetValue(name, out Unit? unit) || unit is not Buoy buoy)
            throw new GameException(0x35);

        return buoy;
    }

    internal void ReadRegion(byte id, PacketReader reader)
    {
        regions[id] = new Region(Galaxy, this, id, reader);
    }

    internal bool TryGetUnit(string name, [NotNullWhen(true)] out Unit? unit) => units.TryGetValue(name, out unit);
}