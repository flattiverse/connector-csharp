namespace Flattiverse.Connector.Network;

/// <summary>
/// This exception is thrown, when you try to call a property or method which requires server connection but the connection
/// has been terminated.
/// </summary>
public class ConnectionTerminatedGameException : GameException
{
    /// <summary>
    /// The reason why the connection has been terminated.
    /// </summary>
    public readonly string Reason;
    
    internal ConnectionTerminatedGameException(string? reason) : base(0x0F, $"[0x0F] Connection has been terminated: {reason ?? "Unknown reason."}")
    {
        Reason = reason ?? "Unknown reason.";
    }
}