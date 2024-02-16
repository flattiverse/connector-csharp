using Flattiverse.Connector.Network;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace Flattiverse.Connector.Hierarchy;

public class Galaxy
{
    private int id;
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
    
    // JAM TODO: Clusters ist jetzt beispielhaft mit UniversalHolder implementiert. Wir brauchen maxClusters
    //           praktisch nie und uns die die Performance des Clients praktisch egal. Der kann sich das theoretisch
    //           einmal abholen und dann glücklich sein. Wenn er inperformante calls macht ist uns das egal.
    //
    //           Bitte den Holder auch für alles andere nutzen.

    private readonly Ship?[] ships = new Ship?[256];
    private int shipMax = 0;

    private readonly Team?[] teams = new Team?[33];
    private int teamMax = 0;

    private Dictionary<byte, Player> players = new Dictionary<byte, Player>();

    private readonly SessionHandler sessions;
    private readonly Connection connection;

    internal Galaxy(Universe universe)
    {
        Clusters = new UniversalHolder<Cluster>(clusters);
        
        connection = new Connection(universe, ConnectionClosed, PacketRecevied);
        sessions = new SessionHandler(connection);
    }

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

    /// <summary>
    /// Requests from the server if given number is even.
    /// This is a method to test the connector-server communication.
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public async Task<bool> IsEven(int number)
    {
        Session session = await sessions.Get();

        Packet packet = new Packet();

        packet.Header.Command = 0x55;
        
        using (PacketWriter writer = packet.Write())
            writer.Write(number);

        packet = await session.SendWait(packet);

        return packet.Header.Param0 != 0;
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
                clusters[packet.Header.Param0] = new Cluster(packet.Header.Param0, this, reader);
                Console.WriteLine($"Received cluster {clusters[packet.Header.Param0]!.Name} update");

                // JAM TODO: Hier arbeiten wir nicht mit so was wie clusterMax, sondern mit etwas dass ich im
                //           Flattiverse 2014 eingebaut habe und UniversalHolder heißt. Habe Mal die Klasse
                //           Beispielhaft implementiert.

                break;
            case 0x12://Team info
                teams[packet.Header.Param0] = new Team(packet.Header.Param0, reader);
                Console.WriteLine($"Received team {teams[packet.Header.Param0]!.Name} update");

                if (teamMax < packet.Header.Param0 + 1)
                    teamMax = packet.Header.Param0 + 1;

                break;
            case 0x13://Ship info
                ships[packet.Header.Param0] = new Ship(packet.Header.Param0, this, reader);
                Console.WriteLine($"Received ship {ships[packet.Header.Param0]!.Name} update");

                if (shipMax < packet.Header.Param0 + 1)
                    shipMax = packet.Header.Param0 + 1;

                break;
            case 0x14://Upgrade info
                if (ships[packet.Header.Param1] is Ship ship)
                    ship.ReadUpgrade(packet.Header.Param0, reader);

                break;
            case 0x15://New player joined info
                if (teams[packet.Header.Param1] is Team team)
                    players[packet.Header.Player] = new Player(packet.Header.Player, (PlayerKind)packet.Header.Param0, team, reader);
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