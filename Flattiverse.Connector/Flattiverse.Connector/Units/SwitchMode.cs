namespace Flattiverse.Connector.Units;

/// <summary>
/// Defines how a switch affects linked gates.
/// </summary>
public enum SwitchMode : byte
{
    /// <summary>
    /// Inverts the current gate state.
    /// </summary>
    Toggle = 0x00,

    /// <summary>
    /// Opens linked gates.
    /// </summary>
    Open = 0x01,

    /// <summary>
    /// Closes linked gates.
    /// </summary>
    Close = 0x02
}
