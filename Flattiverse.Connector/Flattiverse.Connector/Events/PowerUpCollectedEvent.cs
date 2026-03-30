using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Your controllable has collected a power-up.
/// </summary>
public class PowerUpCollectedEvent : FlattiverseEvent
{
    /// <summary>
    /// The controllable that collected the power-up.
    /// </summary>
    public readonly Controllable Controllable;

    /// <summary>
    /// The kind of the collected power-up.
    /// </summary>
    public readonly UnitKind PowerUpKind;

    /// <summary>
    /// The configured unit name of the collected power-up.
    /// </summary>
    public readonly string PowerUpName;

    /// <summary>
    /// The configured amount carried by the power-up.
    /// </summary>
    public readonly float Amount;

    /// <summary>
    /// The amount that was actually applied to the controllable.
    /// </summary>
    public readonly float AppliedAmount;

    internal PowerUpCollectedEvent(Controllable controllable, UnitKind powerUpKind, string powerUpName, float amount, float appliedAmount)
    {
        Controllable = controllable;
        PowerUpKind = powerUpKind;
        PowerUpName = powerUpName;
        Amount = amount;
        AppliedAmount = appliedAmount;
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.PowerUpCollected;

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} PowerUp collected: Controllable=\"{Controllable.Name}\", Kind={PowerUpKind}, Name=\"{PowerUpName}\", Amount={Amount:0.###}, Applied={AppliedAmount:0.###}.";
    }
}
