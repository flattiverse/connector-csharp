namespace Flattiverse.Connector.Network;

/// <summary>
/// This exception is thrown, when the server sends ambiguous data.
/// </summary>
public class InvalidDataGameException : GameException
{
    internal InvalidDataGameException() : base(0x0D, "[0x0D] Invalid data received, protocol mismatch: Terminating connection.")
    {
    }
}