using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace Flattiverse.Connector;

public class Galaxy
{
    private int id;
    private string name;
    private string description;
    private GameType gameType;
    private byte maxPlayers;

    private ushort maxPlatformsUniverse;
    private ushort maxProbesUniverse;
    private ushort maxDronesUniverse;
    private ushort maxShipsUniverse;
    private ushort maxBasesUniverse;

    private ushort maxPlatformsTeam;
    private ushort maxProbesTeam;
    private ushort maxDronesTeam;
    private ushort maxShipsTeam;
    private ushort maxBasesTeam;

    private byte maxPlatformsPlayer;
    private byte maxProbesPlayer;
    private byte maxDronesPlayer;
    private byte maxShipsPlayer;
    private byte maxBasesPlayer;

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
    public byte MaxPlayers => maxPlayers;

    public ushort MaxPlatformsUniverse => maxPlatformsUniverse;
    public ushort MaxProbesUniverse => maxProbesUniverse;
    public ushort MaxDronesUniverse => maxDronesUniverse;
    public ushort MaxShipsUniverse => maxShipsUniverse;
    public ushort MaxBasesUniverse => maxBasesUniverse;
           
    public ushort MaxPlatformsTeam => maxPlatformsTeam;
    public ushort MaxProbesTeam => maxProbesTeam;
    public ushort MaxDronesTeam => maxDronesTeam;
    public ushort MaxShipsTeam => maxShipsTeam;
    public ushort MaxBasesTeam => maxBasesTeam;

    public byte MaxPlatformsPlayer => maxPlatformsPlayer;
    public byte MaxProbesPlayer => maxProbesPlayer;
    public byte MaxDronesPlayer => maxDronesPlayer;
    public byte MaxShipsPlayer => maxShipsPlayer;
    public byte MaxBasesPlayer => maxBasesPlayer;

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

                break;
            case 0x11://Cluster info
                clusters[packet.Header.Param0] = new Cluster(packet.Header.Param0, this, reader);

                // JAM TODO: Hier arbeiten wir nicht mit so was wie clusterMax, sondern mit etwas dass ich im
                //           Flattiverse 2014 eingebaut habe und UniversalHolder heißt. Habe Mal die Klasse
                //           Beispielhaft implementiert.
                
                break;
            case 0x12://Team info
                teams[packet.Header.Param0] = new Team(packet.Header.Param0, reader);

                if (teamMax < packet.Header.Param0 + 1)
                    teamMax = packet.Header.Param0 + 1;

                break;
            case 0x13://Ship info
                ships[packet.Header.Param0] = new Ship(packet.Header.Param0, this, reader);

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