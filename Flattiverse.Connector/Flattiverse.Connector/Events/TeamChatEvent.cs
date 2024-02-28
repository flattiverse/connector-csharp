using Flattiverse.Connector.Hierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Event that is raised when a chat message is received.
/// </summary>
public class TeamChatEvent : ChatEvent
{
    internal TeamChatEvent(Packet packet, Player player) : base(player, packet)
    { }

    /// <inheritdoc/>    
    public override EventKind Kind => EventKind.TeamChat;

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} «{Player.Name}» {Message}";
    }
}