namespace Flattiverse.Connector.Network;

/// <summary>
/// This exception is thrown when you try to invoke a command but all available session slots are in use.
/// </summary>
public class SessionsExhaustedException : GameException
{
    internal SessionsExhaustedException() : base(0x0C, "[0x0C] Sessions exhausted: You cannot have more than 255 calls in progress.")
    {
    }
}