namespace Flattiverse.Connector.Network;

/// <summary>
/// Specifies how much an argument is wrong.
/// </summary>
public enum InvalidArgumentKind : byte
{
    /// <summary>
    /// It's not specified how the parameter is invalid.
    /// </summary>
    Unknown = 0x00,
    /// <summary>
    /// The argument was too small.
    /// </summary>
    TooSmall = 0x01,
    /// <summary>
    /// The argument was too large.
    /// </summary>
    TooLarge = 0x02,
    /// <summary>
    /// The arguments value doesn't match name constraint.
    /// </summary>
    NameConstraint = 0x03,
    /// <summary>
    /// The arguments value doesn't match chat message constraiont.
    /// </summary>
    ChatConstraint = 0x04,
    /// <summary>
    /// The specified entity has not been found.
    /// </summary>
    EntityNotFound = 0xFC,
    /// <summary>
    /// The arguments value did contain a Not a Number value.
    /// </summary>
    ContainedNaN = 0xFD,
    /// <summary>
    /// The argument contains an infinite value.
    /// </summary>
    ContainedInfinity = 0xFE,
    /// <summary>
    /// The argument is actually valid.
    /// </summary>
    Valid = 0xFF
}