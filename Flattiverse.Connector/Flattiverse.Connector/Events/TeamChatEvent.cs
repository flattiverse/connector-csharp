using Flattiverse.Connector.Hierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Events;

public class TeamChatEvent : ChatEvent
{
    internal TeamChatEvent(Packet packet, Player player) : base(player, packet)
    { }
    
    public override EventKind Kind => EventKind.TeamChat;

    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} «{Player.Name}» {Message}";
    }
}