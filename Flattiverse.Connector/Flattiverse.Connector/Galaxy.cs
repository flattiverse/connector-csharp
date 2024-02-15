using Flattiverse.Connector.Network;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Text;

namespace Flattiverse.Connector;

public class Galaxy
{
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

        switch (packet.Header.Command)
        {
            case 0x00: // This and that command.
                // JAM TODO: Hier das Empfangen eines Pakets, welches keine Session gesetzt hat testen.
                break;
        }
    }
}