using Flattiverse.Connector.Events;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Nebula collector subsystem of a controllable.
/// </summary>
public class NebulaCollectorSubsystem : Subsystem
{
    private const float MinimumRateValue = 0f;
    private const float MaximumRateValue = 0.1f;
    private const float EnergyScale = 1600f;

    private float _rate;
    private float _consumedEnergyThisTick;
    private float _consumedIonsThisTick;
    private float _consumedNeutrinosThisTick;
    private float _collectedThisTick;
    private float _collectedHueThisTick;

    internal NebulaCollectorSubsystem(Controllable controllable, bool exists, SubsystemSlot slot) :
        base(controllable, "NebulaCollector", exists, slot)
    {
        _rate = 0f;
        _consumedEnergyThisTick = 0f;
        _consumedIonsThisTick = 0f;
        _consumedNeutrinosThisTick = 0f;
        _collectedThisTick = 0f;
        _collectedHueThisTick = 0f;
    }

    internal static NebulaCollectorSubsystem CreateClassicShipNebulaCollector(Controllable controllable)
    {
        return new NebulaCollectorSubsystem(controllable, true, SubsystemSlot.NebulaCollector);
    }

    /// <summary>
    /// Minimum configurable collection rate.
    /// <c>0</c> means the collector is off.
    /// </summary>
    public float MinimumRate
    {
        get { return MinimumRateValue; }
    }

    /// <summary>
    /// Maximum configurable collection rate supported by the current controllable kind.
    /// </summary>
    public float MaximumRate
    {
        get { return MaximumRateValue; }
    }

    /// <summary>
    /// Rate currently mirrored from the server.
    /// The server may clear this value back to <c>0</c>, for example after movement or a paid zero-yield tick.
    /// </summary>
    public float Rate
    {
        get { return _rate; }
    }

    /// <summary>
    /// Energy consumed by the collector during the current server tick.
    /// </summary>
    public float ConsumedEnergyThisTick
    {
        get { return _consumedEnergyThisTick; }
    }

    /// <summary>
    /// Ions consumed by the collector during the current server tick.
    /// </summary>
    public float ConsumedIonsThisTick
    {
        get { return _consumedIonsThisTick; }
    }

    /// <summary>
    /// Neutrinos consumed by the collector during the current server tick.
    /// </summary>
    public float ConsumedNeutrinosThisTick
    {
        get { return _consumedNeutrinosThisTick; }
    }

    /// <summary>
    /// Nebula amount collected during the current server tick.
    /// </summary>
    public float CollectedThisTick
    {
        get { return _collectedThisTick; }
    }

    /// <summary>
    /// Hue of the nebula material collected during the current server tick.
    /// </summary>
    public float CollectedHueThisTick
    {
        get { return _collectedHueThisTick; }
    }

    /// <summary>
    /// Calculates the current placeholder tick cost for the given collection rate.
    /// </summary>
    /// <param name="rate">Requested collector rate in the inclusive range <c>[0; 0.1]</c>.</param>
    /// <param name="energy">Calculated energy cost for one tick.</param>
    /// <param name="ions">Calculated ion cost for one tick, currently always <c>0</c>.</param>
    /// <param name="neutrinos">Calculated neutrino cost for one tick, currently always <c>0</c>.</param>
    /// <returns>
    /// <see langword="true" /> if the subsystem exists and the input is within the accepted range;
    /// otherwise <see langword="false" />.
    /// </returns>
    public bool CalculateCost(float rate, out float energy, out float ions, out float neutrinos)
    {
        energy = 0f;
        ions = 0f;
        neutrinos = 0f;

        if (!Exists)
            return false;

        if (RangeTolerance.ClampRange(rate, MinimumRateValue, MaximumRateValue, out rate) != InvalidArgumentKind.Valid)
            return false;

        energy = rate * rate * EnergyScale;

        if (float.IsNaN(energy) || float.IsInfinity(energy))
        {
            energy = 0f;
            return false;
        }

        return true;
    }

    /// <summary>
    /// Sets the target nebula-collection rate on the server.
    /// </summary>
    /// <remarks>
    /// The current classic ship uses <c>rate in [0; 0.1]</c> with placeholder tick cost
    /// <c>energy = 1600 * rate^2</c>. The server executes the collector authoritatively: it requires low movement,
    /// searches for an in-range nebula, and may clear the mirrored rate back to <c>0</c> after a paid zero-yield tick.
    /// </remarks>
    /// <param name="rate">Requested collection rate. <c>0</c> turns the collector off.</param>
    /// <exception cref="SpecifiedElementNotFoundGameException">
    /// Thrown, if the controllable or subsystem does not exist anymore.
    /// </exception>
    /// <exception cref="YouNeedToContinueFirstGameException">Thrown, if the controllable is currently dead.</exception>
    /// <exception cref="InvalidArgumentGameException">Thrown, if <paramref name="rate" /> is outside the valid range.</exception>
    public async Task Set(float rate)
    {
        if (!Controllable.Active || !Exists)
            throw new SpecifiedElementNotFoundGameException();

        if (!Controllable.Alive)
            throw new YouNeedToContinueFirstGameException();

        InvalidArgumentKind rateValidity = RangeTolerance.ClampRange(rate, MinimumRateValue, MaximumRateValue, out rate);

        if (rateValidity != InvalidArgumentKind.Valid)
            throw new InvalidArgumentGameException(rateValidity, "rate");

        await Controllable.Cluster.Galaxy.Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0x9C;
            writer.Write(Controllable.Id);
            writer.Write(rate);
        });
    }

    /// <summary>
    /// Convenience wrapper for <see cref="Set" /><c>(0f)</c>.
    /// </summary>
    public Task Off()
    {
        return Set(0f);
    }

    internal void ResetRuntime()
    {
        _rate = 0f;
        _consumedEnergyThisTick = 0f;
        _consumedIonsThisTick = 0f;
        _consumedNeutrinosThisTick = 0f;
        _collectedThisTick = 0f;
        _collectedHueThisTick = 0f;
        ResetRuntimeStatus();
    }

    internal void UpdateRuntime(float rate, SubsystemStatus status, float consumedEnergyThisTick, float consumedIonsThisTick,
        float consumedNeutrinosThisTick, float collectedThisTick, float collectedHueThisTick)
    {
        _rate = rate;
        _consumedEnergyThisTick = consumedEnergyThisTick;
        _consumedIonsThisTick = consumedIonsThisTick;
        _consumedNeutrinosThisTick = consumedNeutrinosThisTick;
        _collectedThisTick = collectedThisTick;
        _collectedHueThisTick = collectedHueThisTick;
        UpdateRuntimeStatus(status);
    }

    internal FlattiverseEvent? CreateRuntimeEvent()
    {
        if (!Exists || !ShouldEmitRuntimeEvent())
            return null;

        return new NebulaCollectorSubsystemEvent(Controllable, Slot, Status, _rate, _consumedEnergyThisTick,
            _consumedIonsThisTick, _consumedNeutrinosThisTick, _collectedThisTick, _collectedHueThisTick);
    }
}
