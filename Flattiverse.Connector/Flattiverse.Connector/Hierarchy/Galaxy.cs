using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Hierarchy;

public class Galaxy
{
    private ushort id;
    private string name;
    private string description;
    private GameType gameType;
    private int maxPlayers;

    private int maxPlatformsUniverse;
    private int maxProbesUniverse;
    private int maxDronesUniverse;
    private int maxShipsUniverse;
    private int maxBasesUniverse;

    private int maxPlatformsTeam;
    private int maxProbesTeam;
    private int maxDronesTeam;
    private int maxShipsTeam;
    private int maxBasesTeam;

    private int maxPlatformsPlayer;
    private int maxProbesPlayer;
    private int maxDronesPlayer;
    private int maxShipsPlayer;
    private int maxBasesPlayer;

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
    public string Name => name;
    public string Description => description;
    public GameType GameType => gameType;
    public int MaxPlayers => maxPlayers;

    public int MaxPlatformsUniverse => maxPlatformsUniverse;
    public int MaxProbesUniverse => maxProbesUniverse;
    public int MaxDronesUniverse => maxDronesUniverse;
    public int MaxShipsUniverse => maxShipsUniverse;
    public int MaxBasesUniverse => maxBasesUniverse;

    public int MaxPlatformsTeam => maxPlatformsTeam;
    public int MaxProbesTeam => maxProbesTeam;
    public int MaxDronesTeam => maxDronesTeam;
    public int MaxShipsTeam => maxShipsTeam;
    public int MaxBasesTeam => maxBasesTeam;

    public int MaxPlatformsPlayer => maxPlatformsPlayer;
    public int MaxProbesPlayer => maxProbesPlayer;
    public int MaxDronesPlayer => maxDronesPlayer;
    public int MaxShipsPlayer => maxShipsPlayer;
    public int MaxBasesPlayer => maxBasesPlayer;

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
        GalaxyConfig changes = new GalaxyConfig(this);
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
            throw GameException.TODO;

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
            throw GameException.TODO;

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
            throw GameException.TODO;

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
                    players[packet.Header.Player] = new Player(packet.Header.Player, (PlayerKind)packet.Header.Param0, team, reader);
                    Console.WriteLine($"Received player {players[packet.Header.Player]!.Name} update");
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

        name = reader.ReadString();
        description = reader.ReadString();
        gameType = (GameType)reader.ReadByte();
        maxPlayers = reader.ReadByte();
        maxPlatformsUniverse = reader.ReadUInt16();
        maxProbesUniverse = reader.ReadUInt16();
        maxDronesUniverse = reader.ReadUInt16();
        maxShipsUniverse = reader.ReadUInt16();
        maxBasesUniverse = reader.ReadUInt16();
        maxPlatformsTeam = reader.ReadUInt16();
        maxProbesTeam = reader.ReadUInt16();
        maxDronesTeam = reader.ReadUInt16();
        maxShipsTeam = reader.ReadUInt16();
        maxBasesTeam = reader.ReadUInt16();
        maxPlatformsPlayer = reader.ReadByte();
        maxProbesPlayer = reader.ReadByte();
        maxDronesPlayer = reader.ReadByte();
        maxShipsPlayer = reader.ReadByte();
        maxBasesPlayer = reader.ReadByte();
    }
}