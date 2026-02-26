namespace Flattiverse.Connector.Units;

/// <summary>
/// Thrown, if you try to shoot too often.
/// </summary>
public class CanOnlyShootOncePerTickGameException : GameException
{
    internal CanOnlyShootOncePerTickGameException() : base(0x30, "[0x30] Please, only shoot once a tick with the same unit.")
    {
    }
}