namespace Flattiverse.Connector.Network;

/// <summary>
/// Thrown, if a specific XML node or attribute has an invalid value.
/// </summary>
public class InvalidXmlNodeValueGameException : GameException
{
    /// <summary>
    /// Validation reason that caused the error.
    /// </summary>
    public readonly InvalidArgumentKind Reason;

    /// <summary>
    /// XML node/attribute path, for example: Galaxy&gt;Team.ColorR.
    /// </summary>
    public readonly string NodePath;

    /// <summary>
    /// Human-readable hint in English.
    /// </summary>
    public readonly string Hint;

    internal InvalidXmlNodeValueGameException(InvalidArgumentKind reason, string nodePath, string hint) : base(0x16, FormulateMessage(reason, nodePath, hint))
    {
        Reason = reason;
        NodePath = nodePath;
        Hint = hint;
    }

    private static string FormulateMessage(InvalidArgumentKind reason, string nodePath, string hint)
    {
        return $"[0x16] XML node \"{nodePath}\" is invalid ({reason}): {hint}";
    }
}
