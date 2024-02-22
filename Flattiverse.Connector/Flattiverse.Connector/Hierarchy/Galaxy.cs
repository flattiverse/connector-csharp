using System.Diagnostics;
using Flattiverse.Connector.Events;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Hierarchy;

public class Galaxy
{
    private ushort id;

    private GalaxyConfig config;

    private readonly Cluster?[] clusters = new Cluster?[256];
    public readonly UniversalHolder<Cluster> Clusters;

    private readonly ShipDesign?[] ships = new ShipDesign?[256];
    public readonly UniversalHolder<ShipDesign> Ships;

    private readonly Team?[] teams = new Team?[33];
    public readonly UniversalHolder<Team> Teams;

    private Dictionary<byte, Player> players = new Dictionary<byte, Player>();

    private readonly SessionHandler sessions;
    private readonly Connection connection;

    private TaskCompletionSource? loginCompleted;

    private readonly Queue<FlattiverseEvent> pendingEvents = new Queue<FlattiverseEvent>();
    private readonly Queue<TaskCompletionSource<FlattiverseEvent>> pendingEventWaiters = new Queue<TaskCompletionSource<FlattiverseEvent>>();
    private readonly object syncEvents = new object();

    internal Galaxy(Universe universe)
    {
        //TODO universal holder are public and can be written to
        Clusters = new UniversalHolder<Cluster>(clusters);
        Ships = new UniversalHolder<ShipDesign>(ships);
        Teams = new UniversalHolder<Team>(teams);

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
            throw new GameException("Creation successfull, but connector didn't receive update yet.");//Should never happen

        return team;
    }

    /// <summary>
    /// Creates a ship with given values.
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    public async Task<ShipDesign> CreateShip(Action<ShipDesignConfig> config)
    {
        ShipDesignConfig changes = ShipDesignConfig.Default;
        config(changes);

        Session session = await sessions.Get();

        Packet packet = new Packet();
        packet.Header.Command = 0x4A;

        using (PacketWriter writer = packet.Write())
            changes.Write(writer);

        packet = await session.SendWait(packet);

        if (ships[packet.Header.Param0] is not ShipDesign ship)
            throw new GameException("Creation successfull, but connector didn't receive update yet.");//Should never happen

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


            case 0x10://Galaxy info
                Update(packet.Header, reader);
                Console.WriteLine($"Received galaxy {Name} update");

                break;
            case 0x11://Cluster info
                clusters[packet.Header.Id0] = new Cluster(this, packet.Header.Id0, reader);
                Console.WriteLine($"Received cluster {clusters[packet.Header.Id0]!.Name} update");

                break;
            case 0x12://Region info
                if (clusters[packet.Header.Id1] is Cluster cluster)
                    cluster.ReadRegion(packet.Header.Id0, reader);

                break;
            case 0x13://Team info
                teams[packet.Header.Id0] = new Team(this, packet.Header.Id0, reader);
                Console.WriteLine($"Received team {teams[packet.Header.Id0]!.Name} update");

                break;
            case 0x14://Ship info
                ships[packet.Header.Id0] = new ShipDesign(this, packet.Header.Id0, reader);
                Console.WriteLine($"Received ship(id={packet.Header.Id0}) {ships[packet.Header.Id0]!.Name} update");

                break;
            case 0x15://Upgrade info
                if (ships[packet.Header.Id1] is ShipDesign ship)
                    ship.ReadUpgrade(packet.Header.Id0, reader);

                break;
            case 0x16://New player joined info
                { 
                    if (teams[packet.Header.Id1] is Team team)
                    {
                        // TODO MALUK (PlayerKind)packet.Header.Id0 <- Id0 ?? Param0? 
                        players[packet.Header.Id0] = new Player(packet.Header.Id0, (PlayerKind)packet.Header.Id0, team, reader);
                        Console.WriteLine($"Received player {players[packet.Header.Id0]!.Name} update");
                        pushEvent(new PlayerAddedEvent(this, players[packet.Header.Id0]!));
                    }
                }
                break;
            case 0x17://Player removed info
                { 
                    if (teams[packet.Header.Id1] is Team && players.TryGetValue(packet.Header.Id0, out Player? p))
                    {
                        p.Deactivate();
                        players.Remove(packet.Header.Id0);
                        Console.WriteLine($"Received player {p.Name} removed");
                        pushEvent(new PlayerRemovedEvent(this, p));
                    }
                }
                break;
            case 0x18://Controllable info
                {
                    if(players.TryGetValue(packet.Header.Id1, out Player? player) && clusters[packet.Header.Id0] is Cluster cl)
                    {
                        // TODO MALUK param0 (schon für die Player-Id verwendet) für den reduced flag ??
                        ControllableInfo info = new ControllableInfo(cl, player, reader, packet.Header.Param0 == 1);
                        player.AddControllableInfo(info);
                        Console.WriteLine($"Received controllable info");
                    }
                }
                break;
            case 0x19://Controllable removed info
                {
                    if (players.TryGetValue(packet.Header.Id1, out Player? player) && clusters[packet.Header.Id0] is Cluster)
                    {
                        player.RemoveControllableInfo(packet.Read().ReadString());
                        Console.WriteLine($"Received controllable remove info");
                    }
                }
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
}