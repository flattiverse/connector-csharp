namespace Flattiverse.Connector.Network;

/// <summary>
/// Thrown, if a given Parameter is wrong.
/// </summary>
public class InvalidArgumentGameException : GameException
{
    /// <summary>
    /// What was the issue with the argument?
    /// </summary>
    public readonly InvalidArgumentKind Reason;
    
    /// <summary>
    /// The name of the parameter.
    /// </summary>
    public readonly string Parameter;
    
    internal InvalidArgumentGameException(InvalidArgumentKind reason, string parameter) : base(0x12, Formulate(reason, parameter))
    {
        Reason = reason;
        Parameter = parameter;
    }

    private static string Formulate(InvalidArgumentKind reason, string parameter)
    {
        switch (reason)
        {
            default: // Unknown (and valid)
                return $"[0x12] Parameter \"{parameter}\" is wrong due to an invalid value.";
            case InvalidArgumentKind.TooSmall:
                return $"[0x12] Parameter \"{parameter}\" is wrong due to an too small value.";
            case InvalidArgumentKind.TooLarge:
                return $"[0x12] Parameter \"{parameter}\" is wrong due to an too large value.";
            case InvalidArgumentKind.NameConstraint:
                return $"[0x12] Parameter \"{parameter}\" doesn't match name constraint.";
            case InvalidArgumentKind.EntityNotFound:
                return $"[0x12] Parameter \"{parameter}\" doesn't point to an existing entity.";
            case InvalidArgumentKind.ChatConstraint:
                return $"[0x12] Parameter \"{parameter}\" doesn't match chat message constraint.";
            case InvalidArgumentKind.ContainedNaN:
                return $"[0x12] Parameter \"{parameter}\" contained a \"Not a Number\" value.";
            case InvalidArgumentKind.ContainedInfinity:
                return $"[0x12] Parameter \"{parameter}\" contained a \"Infinite\" value.";
        }
    }
}