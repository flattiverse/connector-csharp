namespace Flattiverse.Connector.Account;

/// <summary>
/// The status of an account - does it need to opt in, etc?
/// </summary>
public enum AccountStatus
{
    /// <summary>
    /// The account needs to confirm its email address.
    /// </summary>
    OptIn = 0x00,
    /// <summary>
    /// The account needs to reconfirm its email address.
    /// </summary>
    ReOptIn = 0x01,
    /// <summary>
    /// The account is opted in and can use the game.
    /// </summary>
    User = 0x10,
    /// <summary>
    /// The account is banned.
    /// </summary>
    Banned = 0x80,
    /// <summary>
    /// The account is deleted.
    /// </summary>
    Deleted = 0xF0,
    /// <summary>
    /// The account is in an unknown state.
    /// </summary>
    Unknown = 0xFF
}