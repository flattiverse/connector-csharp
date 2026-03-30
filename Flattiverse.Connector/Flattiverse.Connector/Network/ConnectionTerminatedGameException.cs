namespace Flattiverse.Connector.Network;

/// <summary>
/// Thrown when an operation requires an active galaxy connection but the connector has already lost or closed it.
/// </summary>
public class ConnectionTerminatedGameException : GameException
{
    /// <summary>
    /// Best locally known textual reason for the terminated connection.
    /// This text may come from the remote endpoint or from local connector diagnostics.
    /// </summary>
    public readonly string Reason;
    
    internal ConnectionTerminatedGameException(string? reason) : base(0x0F, $"[0x0F] Connection has been terminated: {reason ?? "Unknown reason."}")
    {
        Reason = reason ?? "Unknown reason.";
    }
}
