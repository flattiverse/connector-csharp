namespace Flattiverse.Connector.Network;

/// <summary>
/// Server-side validation category transported with <see cref="InvalidArgumentGameException" />.
/// </summary>
public enum InvalidArgumentKind : byte
{
    /// <summary>
    /// The server rejected the value but did not provide a more specific category.
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
    /// The value violates the server-side naming rules.
    /// </summary>
    NameConstraint = 0x03,
    /// <summary>
    /// The value violates the server-side chat-message rules.
    /// </summary>
    ChatConstraint = 0x04,
    /// <summary>
    /// The supplied XML data is empty, unreadable, or malformed.
    /// </summary>
    AmbiguousXmlData = 0x05,
    /// <summary>
    /// The referenced entity could not be resolved by the server.
    /// </summary>
    EntityNotFound = 0xFB,
    /// <summary>
    /// The requested name is already in use.
    /// </summary>
    NameInUse = 0xFC,
    /// <summary>
    /// The supplied value contained a floating-point <c>NaN</c>.
    /// </summary>
    ContainedNaN = 0xFD,
    /// <summary>
    /// The supplied value contained positive or negative infinity.
    /// </summary>
    ContainedInfinity = 0xFE,
    /// <summary>
    /// Sentinel value meaning that no validation error exists.
    /// </summary>
    Valid = 0xFF
}
