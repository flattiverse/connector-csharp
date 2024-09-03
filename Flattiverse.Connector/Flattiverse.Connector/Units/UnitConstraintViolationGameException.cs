namespace Flattiverse.Connector.Units;

/// <summary>
/// Thrown, if you try to register too many units.
/// </summary>
public class UnitConstraintViolationGameException : GameException
{
    internal UnitConstraintViolationGameException() : base(0x15, "[0x15] You tried to register too much units of a specific kind.")
    {
    }
}