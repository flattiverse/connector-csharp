namespace Flattiverse.Connector.Units;

/// <summary>
/// Thrown, if you try to do something which requires that your controllable is dead, like Continue().
/// </summary>
public class YouNeedToDieFirstGameException : GameException
{
    internal YouNeedToDieFirstGameException() : base(0x21, "[0x21] This controllable is alive. The controllable needs to die first.")
    {
    }
}