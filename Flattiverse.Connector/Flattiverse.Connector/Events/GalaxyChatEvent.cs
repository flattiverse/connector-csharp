using Flattiverse.Connector.Hierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Event that is raised when a galaxy wide chat message is received.
/// </summary>
public class GalaxyChatEvent : ChatEvent
{
    internal GalaxyChatEvent(Packet packet, Player player) : base(player, packet)
    { }
    
    /// <inheritdoc/>
    public override EventKind Kind => EventKind.GalaxyChat;

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} [{Player.Name}] {Message}";
    }
}