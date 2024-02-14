using Flattiverse.Connector.Network;
using System.Text;

namespace Flattiverse.Connector;

public class Galaxy
{
    private Connection connection;

    internal Galaxy(Universe universe)
    {
        connection = new Connection(universe);
    }

    internal async Task Connect(string uri, string auth, byte team)
    {
        await connection.Connect(uri, auth, team);
    }

    //TODO MALUK CHECK
    /// <summary>
    /// Sends given message to the player with given id.
    /// </summary>
    /// <param name="playerId">ID of the target player.</param>
    /// <param name="message">Message of upto 1040 characters.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if length of message is outside of 1 to 1040.</exception>
    public async Task SendMessage(byte playerId, string message)
    {
        if (message is null || message.Length > 1040)
            throw new ArgumentOutOfRangeException(nameof(message), "Length of message has to be between including 1 and 1040.");

        Packet packet = new Packet(new PacketHeader(0x30, 0x00, playerId, 0x00, 0x00, 0x00, 0x00));

        using (PacketWriter pw = packet.Write())
            pw.Write(message);

        await connection.Send(packet);
    }
}