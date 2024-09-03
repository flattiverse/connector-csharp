namespace Flattiverse.Connector.Network;

/// <summary>
/// Thrown, if you didn't honor the flood control.
/// </summary>
public class FloodcontrolTriggeredGameException : GameException
{
    internal FloodcontrolTriggeredGameException() : base(0x14, "[0x14] You probably type too fast: Don't flood the chat.")
    {
    }
}