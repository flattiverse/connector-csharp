using Flattiverse.Connector.Hierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Event that is raised when a chat message is received.
/// </summary>
public class ChatEvent : FlattiverseEvent
{
    /// <summary>
    /// The player who sent the message.
    /// </summary>
    public readonly Player Player;
    
    /// <summary>
    /// The message that was sent from the player.
    /// </summary>
    public readonly string Message;

    internal ChatEvent(Player player, Packet packet)
    {
        Player = player;
        Message = System.Text.Encoding.UTF8.GetString(packet.Payload.AsSpan(packet.Offset, packet.Header.Size));
    }
}