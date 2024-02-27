using Flattiverse.Connector.Hierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Events;

public class ChatEvent : FlattiverseEvent
{
    public readonly Player Player;
    
    public readonly string Message;

    internal ChatEvent(Player player, Packet packet)
    {
        Player = player;
        Message = System.Text.Encoding.UTF8.GetString(packet.Payload.AsSpan(packet.Offset, packet.Header.Size));
    }
}