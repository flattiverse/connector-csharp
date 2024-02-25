using System.Diagnostics;
using Flattiverse.Connector.Events;
using Flattiverse.Connector.MissionSelection;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Hierarchy;

public class Galaxy
{
    private ushort id;

    private GalaxyConfig config;

    private readonly Cluster?[] clusters = new Cluster?[256];
    public readonly UniversalHolder<Cluster> Clusters;

    private readonly ShipDesign?[] shipDesigns = new ShipDesign?[256];
    public readonly UniversalHolder<ShipDesign> ShipsDesigns;

    private readonly Team?[] teams = new Team?[33];
    public readonly UniversalHolder<Team> Teams;

    private readonly Controllable?[] controllables = new Controllable?[256];
    public readonly UniversalHolder<Controllable> Controllables;

    private readonly Player?[] players = new Player?[256];
    public readonly UniversalHolder<Player> Players;

    private readonly SessionHandler sessions;
    private readonly Connection connection;

    private TaskCompletionSource? loginCompleted;

    private readonly Queue<FlattiverseEvent> pendingEvents = new Queue<FlattiverseEvent>();
    private readonly Queue<TaskCompletionSource<FlattiverseEvent>> pendingEventWaiters = new Queue<TaskCompletionSource<FlattiverseEvent>>();
    private readonly object syncEvents = new object();

    internal Galaxy(Universe universe)
    {
        Clusters = new UniversalHolder<Cluster>(clusters);
        ShipsDesigns = new UniversalHolder<ShipDesign>(shipDesigns);
        Teams = new UniversalHolder<Team>(teams);
        Controllables = new UniversalHolder<Controllable>(controllables);
        Players = new UniversalHolder<Player>(players);

        connection = new Connection(universe, ConnectionClosed, PacketRecevied);
        sessions = new SessionHandler(connection);
    }

    public int ID => id;
    public string Name => config.Name;
    public GalaxyConfig Config => config;

    internal async Task Connect(string uri, string auth, byte team)
    {
        await connection.Connect(uri, auth, team);
    }

    private void ConnectionClosed()
    {
        sessions.TerminateConnections(connection.DisconnectReason);
    }

    internal async Task<Session> GetSession()
    {
        return await sessions.Get();
    }

    public async Task WaitLoginCompleted()
    {
        TaskCompletionSource tSignal = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        loginCompleted = tSignal;

        await tSignal.Task.ConfigureAwait(false);
    }
    
    /// <summary>
    /// Sets given values in this galaxy.
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    public async Task Configure(Action<GalaxyConfig> config)
    {
        GalaxyConfig changes = new GalaxyConfig(this.config);
        config(changes);

        Session session = await sessions.Get();

        Packet packet = new Packet();
        packet.Header.Command = 0x40;
        packet.Header.Param = id;

        using (PacketWriter writer = packet.Write())
            changes.Write(writer);

        await session.SendWait(packet);
    }

    /// <summary>
    /// Creates a cluster with given values.
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    public async Task<Cluster> CreateCluster(Action<ClusterConfig> config)
    {
        ClusterConfig changes = ClusterConfig.Default;
        config(changes);

        Session session = await sessions.Get();

        Packet packet = new Packet();
        packet.Header.Command = 0x41;

        using (PacketWriter writer = packet.Write())
            changes.Write(writer);

        packet = await session.SendWait(packet);

        if (clusters[packet.Header.Param0] is not Cluster cluster)
            throw new GameException("Creation successfull, but connector didn't receive update yet.");//Should never happen

        return cluster;
    }

    /// <summary>
    /// Creates a team with given values.
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    public async Task<Team> CreateTeam(Action<TeamConfig> config)
    {
        TeamConfig changes = TeamConfig.Default;
        config(changes);

        Session session = await sessions.Get();

        Packet packet = new Packet();
        packet.Header.Command = 0x47;

        using (PacketWriter writer = packet.Write())
            changes.Write(writer);

        packet = await session.SendWait(packet);

        if (teams[packet.Header.Param0] is not Team team)
            throw new GameException("Creation successful, but connector didn't receive update yet.");//Should never happen

        return team;
    }

    /// <summary>
    /// Creates a ship with given values.
    /// </summary>
    /// <param name="config">The configuration to specify the ship design.</param>
    /// <returns>The newly created ship design.</returns>
    public async Task<ShipDesign> CreateShipDesign(Action<ShipDesignConfig> config)
    {
        ShipDesignConfig changes = ShipDesignConfig.Default;
        config(changes);

        Session session = await sessions.Get();

        Packet packet = new Packet();
        packet.Header.Command = 0x4A;

        using (PacketWriter writer = packet.Write())
            changes.Write(writer);

        packet = await session.SendWait(packet);

        if (shipDesigns[packet.Header.Param0] is not ShipDesign ship)
            throw new GameException("Creation successful, but connector didn't receive update yet.");//Should never happen

        return ship;
    }

    private void PacketRecevied(Packet packet)
    {
        if (packet.Header.Session != 0)
        {
            sessions.Answer(packet);
            return;
        }

        PacketReader reader = packet.Read();

        switch (packet.Header.Command)
        {
            //case 0x01://Event
            //    pushEvent(FlattiverseEvent.FromPacketReader(this, packet.Header, reader));
            //    break;


            case 0x40: // Galaxy created.
            case 0x50: // Galaxy updated.
                Update(packet.Header, reader);
                break;
            case 0x41: // Cluster created.
                Debug.Assert(clusters[packet.Header.Id0] is null, $"clusters[{packet.Header.Id0}] already populated by \"{clusters[packet.Header.Id0]!.Name}\".");
                clusters[packet.Header.Id0] = new Cluster(this, packet.Header.Id0, reader);
                break;
            case 0x51: // Cluster updated.
                Debug.Assert(clusters[packet.Header.Id0] is not null, $"clusters[{packet.Header.Id0}] not populated.");
                clusters[packet.Header.Id0]!.Update(reader);
                break;
            case 0x71: // Cluster removed.
                Debug.Assert(clusters[packet.Header.Id0] is not null, $"clusters[{packet.Header.Id0}] not populated.");
                clusters[packet.Header.Id0]!.Deactivate();
                clusters[packet.Header.Id0] = null;
                break;
            case 0x42: // Region created.
                Debug.Assert(clusters[packet.Header.Id0] is not null, $"clusters[{packet.Header.Id0}] not populated.");
                Debug.Assert(clusters[packet.Header.Id0]!.regions[packet.Header.Id1] is null, $"clusters[{packet.Header.Id0}].regions[{packet.Header.Id1}] already populated by \"{clusters[packet.Header.Id0]!.regions[packet.Header.Id1]!.Name}\".");
                clusters[packet.Header.Id0]!.regions[packet.Header.Id1] = new Region(this, clusters[packet.Header.Id0]!, packet.Header.Id1, reader);
                break;
            case 0x52: // Region updated.
                Debug.Assert(clusters[packet.Header.Id0] is not null, $"clusters[{packet.Header.Id0}] not populated.");
                Debug.Assert(clusters[packet.Header.Id0]!.regions[packet.Header.Id1] is not null, $"clusters[{packet.Header.Id0}].region[{packet.Header.Id1}] not populated.");
                clusters[packet.Header.Id0]!.regions[packet.Header.Id1]!.Update(reader); 
                break;
            case 0x72: // Region removed.
                Debug.Assert(clusters[packet.Header.Id0] is not null, $"clusters[{packet.Header.Id0}] not populated.");
                Debug.Assert(clusters[packet.Header.Id0]!.regions[packet.Header.Id1] is not null, $"clusters[{packet.Header.Id0}].region[{packet.Header.Id1}] not populated.");
                clusters[packet.Header.Id0]!.regions[packet.Header.Id1]!.Deactivate();
                clusters[packet.Header.Id0]!.regions[packet.Header.Id1] = null;
                break;
            case 0x43: // Team created.
                Debug.Assert(teams[packet.Header.Id0] is null, $"teams[{packet.Header.Id0}] already populated by \"{teams[packet.Header.Id0]!.Name}\".");
                teams[packet.Header.Id0] = new Team(this, packet.Header.Id0, reader);
                break;
            case 0x53: // Team updated.
                Debug.Assert(teams[packet.Header.Id0] is not null, $"teams[{packet.Header.Id0}] not populated.");
                teams[packet.Header.Id0]!.Update(reader);
                break;
            case 0x63: // Team dynamic update (Score of the team updated.)
                Debug.Assert(teams[packet.Header.Id0] is not null, $"teams[{packet.Header.Id0}] not populated.");
                teams[packet.Header.Id0]!.DynamicUpdate(reader);
                break;
            case 0x73: // Team removed.
                Debug.Assert(teams[packet.Header.Id0] is not null, $"teams[{packet.Header.Id0}] not populated.");
                teams[packet.Header.Id0]!.Deactivate();
                teams[packet.Header.Id0] = null;
                break;
            case 0x44: // ShipDesign created.
                Debug.Assert(shipDesigns[packet.Header.Id0] is null, $"shipDesigns[{packet.Header.Id0}] already populated by \"{shipDesigns[packet.Header.Id0]!.Name}\".");
                shipDesigns[packet.Header.Id0] = new ShipDesign(this, packet.Header.Id0, reader);
                break;
            case 0x54: // ShipDesign updated.
            case 0x74: // ShipDesign removed.
                // TODO JOW: Diese zwei müssten implementiert werden.
                break;
            case 0x45: // ShipUpgrade created.            
                Debug.Assert(shipDesigns[packet.Header.Id1] is not null, $"shipDesigns[{packet.Header.Id1}] not populated.");
                Debug.Assert(shipDesigns[packet.Header.Id1]!.upgrades[packet.Header.Id0] is null, $"shipDesigns[{packet.Header.Id1}].upgrades[{packet.Header.Id0}] already populated by \"{shipDesigns[packet.Header.Id1]!.upgrades[packet.Header.Id0]!.Name}\".");
                shipDesigns[packet.Header.Id1]!.upgrades[packet.Header.Id0] = new ShipUpgrade(this, shipDesigns[packet.Header.Id1]!, packet.Header.Id0, reader);
                break;
            case 0x55: // ShipUpgrade updated.
            case 0x75: // ShipUpgrade removed.
                // TODO JOW: Diese zwei müssten implementiert werden.
                break;
            case 0x46: // Player created.
                Debug.Assert(players[packet.Header.Id0] is null, $"players[{packet.Header.Id0}] already populated by \"{players[packet.Header.Id0]!.Name}\".");
                Debug.Assert(teams[packet.Header.Id1] is not null, $"teams[{packet.Header.Id1}] not populated.");
                players[packet.Header.Id0] = new Player(packet.Header.Id0, (PlayerKind)packet.Header.Param0, teams[packet.Header.Id1]!, reader);
                break;
            case 0x66: // Player dynamic update.
                // TODO JOW: Wenn wir Scores haben wird das relevant.
                break;
            case 0x76: // Player removed.
                Debug.Assert(players[packet.Header.Id0] is not null, $"players[{packet.Header.Id0}] already populated by \"{players[packet.Header.Id0]!.Name}\".");
                players[packet.Header.Id0]!.Deactivate();
                players[packet.Header.Id0] = null;
                break;
            case 0x47: // ConstrollableInfo created.
                Debug.Assert(players[packet.Header.Id0] is null, $"players[{packet.Header.Id0}] not populated.");
                Debug.Assert(players[packet.Header.Id0]!.controllableInfos[packet.Header.Id1] is null, $"players[{packet.Header.Id0}].controllableInfos[{packet.Header.Id1}] already populated by \"{players[packet.Header.Id0]!.controllableInfos[packet.Header.Id1]!.Name}\".");
                players[packet.Header.Id0]!.controllableInfos[packet.Header.Id1] = new ControllableInfo(this, players[packet.Header.Id0]!, reader, packet.Header.Id1, packet.Header.Param0 == 1);
                break;
            case 0x57: // ControllableInfo updated.
                // TODO JOW: Leben.
                break;
            case 0x67: // ControllableInfo dynamic update.
                // TODO JOW: Scores.
                break;
            case 0x77: // ControllableInfo removed.
                Debug.Assert(players[packet.Header.Id0] is not null, $"players[{packet.Header.Id0}] not populated.");
                Debug.Assert(players[packet.Header.Id0]!.controllableInfos[packet.Header.Id1] is not null, $"players[{packet.Header.Id0}].controllableInfos[{packet.Header.Id1}] not populated.");
                players[packet.Header.Id0]!.controllableInfos[packet.Header.Id1]!.Deactivate();
                players[packet.Header.Id0]!.controllableInfos[packet.Header.Id1] = null;
                break;
            case 0x48: // Controllable created.
                Debug.Assert(controllables[packet.Header.Id0] is null, $"controllables[{packet.Header.Id0}] already populated by \"{controllables[packet.Header.Id0]!.Name}\".");
                controllables[packet.Header.Id0] = new Controllable(this, packet.Header.Id0, reader);
                break;
            case 0x58: // Controllable update: List of configured upgrades, change of base-data (MaxHull, etc.).
                Debug.Assert(controllables[packet.Header.Id0] is not null, $"controllables[{packet.Header.Id0}] not populated.");
                controllables[packet.Header.Id0]!.Update(reader);
                break;
            case 0x68: // Controllable dynamics update: Position, movement, energy, hull...
                Debug.Assert(controllables[packet.Header.Id0] is not null, $"controllables[{packet.Header.Id0}] not populated.");
                controllables[packet.Header.Id0]!.DynamicUpdate(reader);
                break;
            case 0x78: // Controllable removed.
                Debug.Assert(controllables[packet.Header.Id0] is not null, $"controllables[{packet.Header.Id0}] not populated.");
                controllables[packet.Header.Id0]!.Deactivate();
                controllables[packet.Header.Id0] = null;
                break;
            case 0x1C: // We see a new unit which we didn't see before.
                {
                    Cluster? c = clusters[packet.Header.Id0];

                    Debug.Assert(c is not null, $"Cluster with ID {packet.Header.Id0} not found.");
                
                    Unit unit = c.SeeNewUnit((UnitKind)packet.Header.Param0, reader);
                
                    pushEvent(new AddedUnitEvent(this, unit));
                }
                break;
            case 0x1D: // A unit we see has been updated.
                {
                    Cluster? c = clusters[packet.Header.Id0];

                    Debug.Assert(c is not null, $"Cluster with ID {packet.Header.Id0} not found.");

                    Unit unit = c.SeeUpdatedUnit(reader);

                    pushEvent(new UpdatedUnitEvent(this, unit));
                }
                break;
            case 0x1E: // A once known unit vanished.
                {
                    Cluster? c = clusters[packet.Header.Id0];

                    Debug.Assert(c is not null, $"Cluster with ID {packet.Header.Id0} not found.");

                    Unit unit = c.SeeUnitNoMore(reader.ReadString());

                    pushEvent(new VanishedUnitEvent(this, unit));
                }
                break;
            case 0x20: //Tick completed.
                if (loginCompleted is not null)
                {
                    loginCompleted.SetResult();
                    loginCompleted = null;
                }
                break;
//            case 0x50://Unit
//                // TODO: MALUK extend
//                Console.WriteLine($"Received {(UnitKind)packet.Header.Param0} {packet.Header.Id0} update");
//
//                break;
        }
    }

    private void Update(PacketHeader header, PacketReader reader)
    {
        id = header.Param;

        config = new GalaxyConfig(reader);
    }

    internal void AddControllable(Controllable controllable)
    {
        controllables[controllable.Id] = controllable;
    }   

    internal void RemoveControllable(int id)
    {
        controllables[id] = null;
    }

    internal void pushEvent(FlattiverseEvent @event)
    {
        TaskCompletionSource<FlattiverseEvent>? tcs;

        lock (syncEvents)
        {
            if (pendingEventWaiters.TryDequeue(out tcs))
            {
                ThreadPool.QueueUserWorkItem(delegate { tcs.SetResult(@event); });
                return;
            }

            pendingEvents.Enqueue(@event);
        }
    }

    public async Task<FlattiverseEvent> NextEvent()
    {
        FlattiverseEvent? @event;
        TaskCompletionSource<FlattiverseEvent> tcs;

        lock (syncEvents)
        {
            if (pendingEvents.TryDequeue(out @event))
                return @event;

            tcs = new TaskCompletionSource<FlattiverseEvent>();
            pendingEventWaiters.Enqueue(tcs);
        }

        return await tcs.Task;
    }

    internal Player GetPlayer(int playerId) => players[(byte)playerId];

    public async Task<Controllable> RegisterShip(string name, ShipDesign design)
    {
        Session session = await GetSession();

        Packet packet = new Packet();
        packet.Header.Command = 0x30;
        packet.Header.Id0 = (byte)design.ID;

        using (PacketWriter writer = packet.Write())
            writer.Write(name);

        Packet answerPacket = await session.SendWait(packet);

        Debug.Assert(controllables[packet.Header.Id0] is not null, $"controllables[{packet.Header.Id0}] is not populated but should be after RegisterShip().");
        
        return controllables[packet.Header.Id0]!;
    }
}