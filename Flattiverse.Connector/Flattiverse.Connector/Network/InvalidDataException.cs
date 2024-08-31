namespace Flattiverse.Connector.Network;

/// <summary>
/// This exception is thrown, when the server sends ambiguous data.
/// </summary>
public class InvalidDataException : GameException
{
    internal InvalidDataException() : base(0x03, "[0x0D] Invalid data received: Terminating connection.")
    {
    }
}