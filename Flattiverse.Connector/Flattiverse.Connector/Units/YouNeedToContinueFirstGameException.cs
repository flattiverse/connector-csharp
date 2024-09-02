namespace Flattiverse.Connector.Units;

/// <summary>
/// Thrown, if the controllable you want to control is dead.
/// </summary>
public class YouNeedToContinueFirstGameException : GameException
{
    internal YouNeedToContinueFirstGameException() : base(0x20, "[0x20] This controllable is dead. You need to Continue() first.")
    {
    }
}