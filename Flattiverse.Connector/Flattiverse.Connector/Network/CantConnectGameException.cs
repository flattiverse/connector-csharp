namespace Flattiverse.Connector.Network;

/// <summary>
/// This error message is thrown, if the game can't connect to a specific galaxy.
/// </summary>
public class CantConnectGameException : GameException
{
    internal CantConnectGameException() : base(0x01, "[0x01] Couldn't connect to the flattiverse galaxy.")
    {
    }
    
    internal CantConnectGameException(string message, Exception exception) : base(0x01, message, exception)
    {
    }
}