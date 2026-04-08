namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Protocol error <c>0x3E</c>: thrown when another binary message is sent before the target player has acknowledged the
/// binary channel with a binary reply.
/// </summary>
public class BinaryChatAckRequiredGameException : GameException
{
    internal BinaryChatAckRequiredGameException() : base(0x3E, "[0x3E] Target player has not yet acknowledged binary chat.")
    {
    }
}
