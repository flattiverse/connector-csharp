using Flattiverse.Connector.GalaxyHierarchy;

namespace Flattiverse.Connector.Events;

/// <summary>
/// Runtime update of a resource miner subsystem on your own controllable.
/// </summary>
public class ResourceMinerSubsystemEvent : ControllableSubsystemEvent
{
    /// <summary>
    /// Configured mining rate for the tick.
    /// </summary>
    public readonly float Rate;

    /// <summary>
    /// Energy consumed during the current server tick.
    /// </summary>
    public readonly float ConsumedEnergyThisTick;

    /// <summary>
    /// Ions consumed during the current server tick.
    /// </summary>
    public readonly float ConsumedIonsThisTick;

    /// <summary>
    /// Neutrinos consumed during the current server tick.
    /// </summary>
    public readonly float ConsumedNeutrinosThisTick;

    /// <summary>
    /// Metal mined during the current tick.
    /// </summary>
    public readonly float MinedMetalThisTick;

    /// <summary>
    /// Carbon mined during the current tick.
    /// </summary>
    public readonly float MinedCarbonThisTick;

    /// <summary>
    /// Hydrogen mined during the current tick.
    /// </summary>
    public readonly float MinedHydrogenThisTick;

    /// <summary>
    /// Silicon mined during the current tick.
    /// </summary>
    public readonly float MinedSiliconThisTick;

    internal ResourceMinerSubsystemEvent(Controllable controllable, SubsystemSlot slot, SubsystemStatus status, float rate,
        float consumedEnergyThisTick, float consumedIonsThisTick, float consumedNeutrinosThisTick, float minedMetalThisTick,
        float minedCarbonThisTick, float minedHydrogenThisTick, float minedSiliconThisTick) : base(controllable, slot, status)
    {
        Rate = rate;
        ConsumedEnergyThisTick = consumedEnergyThisTick;
        ConsumedIonsThisTick = consumedIonsThisTick;
        ConsumedNeutrinosThisTick = consumedNeutrinosThisTick;
        MinedMetalThisTick = minedMetalThisTick;
        MinedCarbonThisTick = minedCarbonThisTick;
        MinedHydrogenThisTick = minedHydrogenThisTick;
        MinedSiliconThisTick = minedSiliconThisTick;
    }

    /// <inheritdoc/>
    public override EventKind Kind => EventKind.ResourceMinerSubsystem;

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Stamp:HH:mm:ss.fff} Resource miner subsystem event: Controllable=\"{Controllable.Name}\", Slot={Slot}, Status={Status}, Rate={Rate:0.###}, Mined=({MinedMetalThisTick:0.###},{MinedCarbonThisTick:0.###},{MinedHydrogenThisTick:0.###},{MinedSiliconThisTick:0.###}), Consumed=({ConsumedEnergyThisTick:0.###},{ConsumedIonsThisTick:0.###},{ConsumedNeutrinosThisTick:0.###}).";
    }
}
