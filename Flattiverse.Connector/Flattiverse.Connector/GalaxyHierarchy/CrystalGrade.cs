namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Quality grade of a crystal.
/// </summary>
public enum CrystalGrade : byte
{
    /// <summary>
    /// Weak crystal with side effects.
    /// </summary>
    LowGrade = 0x00,

    /// <summary>
    /// Common crystal with small side effects.
    /// </summary>
    Regular = 0x01,

    /// <summary>
    /// Pure crystal without side effects.
    /// </summary>
    Pure = 0x02,

    /// <summary>
    /// High-grade crystal.
    /// </summary>
    Mastery = 0x03,

    /// <summary>
    /// Exceptional crystal with adjacent positive effects.
    /// </summary>
    Divine = 0x04,
}
