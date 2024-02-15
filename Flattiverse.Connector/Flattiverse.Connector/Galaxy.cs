using Flattiverse.Connector.Network;
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
    private int clusterMax = 0;

    private readonly Ship?[] ships = new Ship?[256];
    private int shipMax = 0;

    private readonly Team?[] teams = new Team?[33];
    private int teamMax = 0;

    private readonly SessionHandler sessions;
    private readonly Connection connection;

    internal Galaxy(Universe universe)
    {
        connection = new Connection(universe, ConnectionClosed, PacketRecevied);
        sessions = new SessionHandler(connection);
    }

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
                id = packet.Header.Param;

                name = reader.ReadString();
                description = reader.ReadString();
                gameType = (GameType)reader.ReadByte();
                maxPlayers = reader.ReadInt32();
                maxPlatformsUniverse = reader.ReadInt32();
                maxProbesUniverse = reader.ReadInt32();
                maxDronesUniverse = reader.ReadInt32();
                maxShipsUniverse = reader.ReadInt32();
                maxBasesUniverse = reader.ReadInt32();
                maxPlatformsTeam = reader.ReadInt32();
                maxProbesTeam = reader.ReadInt32();
                maxDronesTeam = reader.ReadInt32();
                maxShipsTeam = reader.ReadInt32();
                maxBasesTeam = reader.ReadInt32();
                maxPlatformsPlayer = reader.ReadInt32();
                maxProbesPlayer = reader.ReadInt32();
                maxDronesPlayer = reader.ReadInt32();
                maxShipsPlayer = reader.ReadInt32();
                maxBasesPlayer = reader.ReadInt32();

                break;
            case 0x11://Cluster info
                clusters[packet.Header.Param0] = new Cluster(packet.Header.Param0, this, reader);

                if (clusterMax < packet.Header.Param0 + 1)
                    clusterMax = packet.Header.Param0 + 1;

                break;
            case 0x12://Team info
                teams[packet.Header.Param0] = new Team(packet.Header.Param0, this, reader);

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
                    ships[packet.Header.Param1].ReadUpgrade(packet.Header.Param0, this, reader);

                break;
            case 0x15://New player joined info
                break;
        }
    }
}