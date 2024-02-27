using Flattiverse.Connector.Hierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Events;

public class PlayerChatEvent : ChatEvent
{
    internal PlayerChatEvent(Packet packet, Player player) : base(player, packet)
    { }
    
    public override EventKind Kind => EventKind.PlayerChat;

    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} <{Player.Name}> {Message}";
    }
}