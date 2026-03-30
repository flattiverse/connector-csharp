namespace Flattiverse.Connector.Units;

/// <summary>
/// Determines how a current field induces movement.
/// </summary>
public enum CurrentFieldMode : byte
{
    /// <summary>
    /// Applies a fixed world-space movement vector.
    /// </summary>
    Directional = 0x00,
    /// <summary>
    /// Applies radial and tangential movement relative to the field center.
    /// </summary>
    Relative = 0x01
}
