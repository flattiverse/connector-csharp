namespace Flattiverse.Connector.Units;

/// <summary>
/// Thrown, if you try to shoot too often.
/// </summary>
public class CanOnlyShootOncePerTickGameException : GameException
{
    internal CanOnlyShootOncePerTickGameException() : base(0x30, "[0x30] You tried to register too much units of a specific kind.")
    {
    }
}