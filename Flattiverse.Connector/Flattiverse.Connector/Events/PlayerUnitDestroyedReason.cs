namespace Flattiverse.Connector.Events;

/// <summary>
/// Specified why a PlayerUnit has been destroyed.
/// </summary>
public enum PlayerUnitDestroyedReason
{
    /// <summary>
    /// PlayerUnit got destroyed due to server rules like when the player disconnects or the Galaxy switched to
    /// maintenance mode.
    /// </summary>
    ByRules = 0x00,
    /// <summary>
    /// The player called Kill().
    /// </summary>
    Suicided = 0x10,
    /// <summary>
    /// The PlayerUnit collided with a neutral unit.
    /// </summary>
    CollidedWithNeutralUnit = 0x20,
    /// <summary>
    /// The PlayerUnit collided with an enemy PlayerUnit. 
    /// </summary>
    CollidedWithEnemyPlayerUnit = 0x28,
    /// <summary>
    /// The PlayerUnit collided with a friendly PlayerUnit.
    /// </summary>
    CollidedWithFriendlyPlayerUnit = 0x29,
    /// <summary>
    /// The PlayerUnit has been shot by an enemy PlayerUnit.
    /// </summary>
    ShotByEnemyPlayerUnit = 0x38,
    /// <summary>
    /// The PlayerUnit has been shot by a friendly PlayerUnit.
    /// </summary>
    ShotByFriendlyPlayerUnit = 0x39
}