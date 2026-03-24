using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Runtime update of a dynamic shot launcher subsystem on your own controllable.
/// </summary>
public class DynamicShotLauncherSubsystemEvent : ControllableSubsystemEvent
{
    /// <summary>
    /// The shot movement processed for the current server tick.
    /// </summary>
    public readonly Vector RelativeMovement;

    /// <summary>
    /// The shot lifetime processed for the current server tick.
    /// </summary>
    public readonly ushort Ticks;

    /// <summary>
    /// The shot load processed for the current server tick.
    /// </summary>
    public readonly float Load;

    /// <summary>
    /// The shot damage processed for the current server tick.
    /// </summary>
    public readonly float Damage;

    /// <summary>
    /// The energy consumed during the current server tick.
    /// </summary>
    public readonly float ConsumedEnergyThisTick;

    /// <summary>
    /// The ions consumed during the current server tick.
    /// </summary>
    public readonly float ConsumedIonsThisTick;

    /// <summary>
    /// The neutrinos consumed during the current server tick.
    /// </summary>
    public readonly float ConsumedNeutrinosThisTick;

    internal DynamicShotLauncherSubsystemEvent(Controllable controllable, SubsystemSlot slot, SubsystemStatus status, Vector relativeMovement,
        ushort ticks, float load, float damage, float consumedEnergyThisTick, float consumedIonsThisTick, float consumedNeutrinosThisTick) :
        base(controllable, slot, status)
    {
        RelativeMovement = new Vector(relativeMovement);
        Ticks = ticks;
        Load = load;
        Damage = damage;
        ConsumedEnergyThisTick = consumedEnergyThisTick;
        ConsumedIonsThisTick = consumedIonsThisTick;
        ConsumedNeutrinosThisTick = consumedNeutrinosThisTick;
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.DynamicShotLauncherSubsystem;

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} Dynamic shot launcher subsystem event: Controllable=\"{Controllable.Name}\", Slot={Slot}, Status={Status}, RelativeMovement={RelativeMovement}, Ticks={Ticks}, Load={Load:0.###}, Damage={Damage:0.###}, Consumed=({ConsumedEnergyThisTick:0.###},{ConsumedIonsThisTick:0.###},{ConsumedNeutrinosThisTick:0.###}).";
    }
}
