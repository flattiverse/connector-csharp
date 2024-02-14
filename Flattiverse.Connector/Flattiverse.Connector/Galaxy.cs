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
    /// <param name="message">The message to send.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if playerId is outside of 0 to 254 or length of message is outside of 1 to 1040.</exception>
    public async Task SendMessage(int playerId, string message)
    {
        if (playerId < 0 || playerId > 254)
            throw new ArgumentOutOfRangeException(nameof(playerId), "Player ID has to be between including 0 and 254.");

        if (message is null || message.Length > 1040)
            throw new ArgumentOutOfRangeException(nameof(message), "Length of message has to be between including 1 and 1040.");

        Packet packet = new Packet(new PacketHeader(0x30, 0x00, 0x00, 0x00, (byte)playerId, 0x00, 0x00));

        using (PacketWriter pw = packet.Write())
            pw.Write(message);

        await connection.Send(packet);
    }
}