namespace Flattiverse.Connector.Network;

/// <summary>
/// Thrown when the connector cannot establish and activate a connection to a galaxy endpoint.
/// This covers websocket, HTTP proxy, and pre-activation failures.
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
