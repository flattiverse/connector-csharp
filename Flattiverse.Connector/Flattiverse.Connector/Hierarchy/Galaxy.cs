using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Hierarchy;

public class Galaxy
{
    private ushort id;

    private GalaxyConfig config;

    private readonly Cluster?[] clusters = new Cluster?[256];
    public readonly UniversalHolder<Cluster> Clusters;

    private readonly Ship?[] ships = new Ship?[256];
    public readonly UniversalHolder<Ship> Ships;

    private readonly Team?[] teams = new Team?[33];
    public readonly UniversalHolder<Team> Teams;

    private Dictionary<byte, Player> players = new Dictionary<byte, Player>();

    private readonly SessionHandler sessions;
    private readonly Connection connection;

    private TaskCompletionSource? loginCompleted;

    internal Galaxy(Universe universe)
    {
        Clusters = new UniversalHolder<Cluster>(clusters);
        Ships = new UniversalHolder<Ship>(ships);
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
        
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("SIGNALLED LOGIN COMPLETED.");
        Console.ForegroundColor = ConsoleColor.Gray;
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
    public async Task<Ship> CreateShip(Action<ShipConfig> config)
    {
        ShipConfig changes = ShipConfig.Default;
        config(changes);

        Session session = await sessions.Get();

        Packet packet = new Packet();
        packet.Header.Command = 0x4A;

        using (PacketWriter writer = packet.Write())
            changes.Write(writer);

        packet = await session.SendWait(packet);

        if (ships[packet.Header.Param0] is not Ship ship)
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
            case 0x10://Galaxy info
                Update(packet.Header, reader);
                Console.WriteLine($"Received galaxy {Name} update");

                break;
            case 0x11://Cluster info
                clusters[packet.Header.Param0] = new Cluster(this, packet.Header.Param0, reader);
                Console.WriteLine($"Received cluster {clusters[packet.Header.Param0]!.Name} update");

                break;
            case 0x12://Region info
                if (clusters[packet.Header.Param1] is Cluster cluster)
                    cluster.ReadRegion(packet.Header.Param0, reader);

                break;
            case 0x13://Team info
                teams[packet.Header.Param0] = new Team(this, packet.Header.Param0, reader);
                Console.WriteLine($"Received team {teams[packet.Header.Param0]!.Name} update");

                break;
            case 0x14://Ship info
                ships[packet.Header.Param0] = new Ship(this, packet.Header.Param0, reader);
                Console.WriteLine($"Received ship {ships[packet.Header.Param0]!.Name} update");

                break;
            case 0x15://Upgrade info
                if (ships[packet.Header.Param1] is Ship ship)
                    ship.ReadUpgrade(packet.Header.Param0, reader);

                break;
            case 0x16://New player joined info
                if (teams[packet.Header.Param1] is Team team)
                {
                    players[packet.Header.Id0] = new Player(packet.Header.Id0, (PlayerKind)packet.Header.Param0, team, reader);
                    Console.WriteLine($"Received player {players[packet.Header.Id0]!.Name} update");
                }
                break;
            
            case 0x20: // Tick completed.
                if (loginCompleted is not null)
                {
                    loginCompleted.SetResult();
                    loginCompleted = null;
                }

                break;
        }
    }

    private void Update(PacketHeader header, PacketReader reader)
    {
        id = header.Param;

        config = new GalaxyConfig(reader);
    }
}