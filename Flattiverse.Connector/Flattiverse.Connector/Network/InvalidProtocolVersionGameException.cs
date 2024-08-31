namespace Flattiverse.Connector.Network;

/// <summary>
/// Thrown, if you try to connect to a galaxy which uses another protocol version.
/// You should consider updating the connector.
/// </summary>
public class InvalidProtocolVersionGameException : GameException
{
    internal InvalidProtocolVersionGameException() : base(0x02, "[0x02] Invalid protocol version. Consider up(- or down)grading the connector.")
    {
    }
}