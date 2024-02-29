namespace Flattiverse.Connector.Units;

/// <summary>
/// The reason why a unit was destroyed.
/// </summary>
public enum DestructionReason
{
    /// <summary>
    /// The unit was destroyed because the galaxy was shut down.
    /// </summary>
    Shutdown,

    /// <summary>
    /// The unit was destroyed because it self destructed.
    /// </summary>
    SelfDestruction,

    /// <summary>
    /// The unit was destroyed because it was destroyed by another unit.
    /// </summary>
    Collision
}