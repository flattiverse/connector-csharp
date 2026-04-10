namespace Flattiverse.Connector.Events;

/// <summary>
/// Describes why a public controllable-registration entry died.
/// </summary>
public enum PlayerUnitDestroyedReason
{
    /// <summary>
    /// Destroyed by global server rules, for example disconnect cleanup or maintenance transitions.
    /// </summary>
    ByRules = 0x00,

    /// <summary>
    /// Destroyed because the owner explicitly called <c>Suicide()</c>.
    /// </summary>
    Suicided = 0x10,

    /// <summary>
    /// Destroyed because the containing cluster was removed.
    /// </summary>
    ByClusterRemoval = 0x01,

    /// <summary>
    /// Destroyed after leaving the activated map area and getting lost in deep space.
    /// </summary>
    LostInDeepSpace = 0x02,

    /// <summary>
    /// Destroyed by collision with a non-player unit.
    /// </summary>
    CollidedWithNeutralUnit = 0x20,

    /// <summary>
    /// Destroyed by collision with an enemy player-controlled unit.
    /// </summary>
    CollidedWithEnemyPlayerUnit = 0x28,

    /// <summary>
    /// Destroyed by collision with a friendly player-controlled unit.
    /// </summary>
    CollidedWithFriendlyPlayerUnit = 0x29,

    /// <summary>
    /// Destroyed by hostile player-originated weapon damage.
    /// </summary>
    ShotByEnemyPlayerUnit = 0x38,

    /// <summary>
    /// Destroyed by friendly-fire weapon damage.
    /// </summary>
    ShotByFriendlyPlayerUnit = 0x39
}
