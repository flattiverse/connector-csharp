using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Represents one private binary chat message from another player to you.
/// </summary>
public class BinaryChatPlayerEvent : PlayerEvent
{
    /// <summary>
    /// The destination where this message was sent to.
    /// </summary>
    public readonly Player Destination;

    /// <summary>
    /// Raw binary message payload.
    /// </summary>
    public readonly byte[] Message;

    internal BinaryChatPlayerEvent(Player player, byte[] message, Player destination) : base(player)
    {
        Destination = destination;
        Message = message;
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.BinaryChatPlayer;

    /// <inheritdoc/>
    public override string ToString()
    {
        int previewLength = Math.Min(Message.Length, 16);
        string preview = Convert.ToHexString(Message.AsSpan(0, previewLength));
        string suffix = Message.Length > previewLength ? "..." : string.Empty;
        return $"{Stamp:HH:mm:ss.fff} <[{Player.Team.Name}]{Player.Name}->{Destination.Name}> 0x{preview}{suffix}";
    }
}
