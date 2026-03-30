using Flattiverse.Connector.Events;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Hull repair subsystem of a controllable.
/// </summary>
public class RepairSubsystem : Subsystem
{
    private const float MinimumRateValue = 0f;
    private const float MaximumRateValue = 0.1f;
    private const float EnergyScale = 1600f;

    private float _rate;
    private float _consumedEnergyThisTick;
    private float _consumedIonsThisTick;
    private float _consumedNeutrinosThisTick;
    private float _repairedHullThisTick;

    internal RepairSubsystem(Controllable controllable, string name, bool exists, SubsystemSlot slot) :
        base(controllable, name, exists, slot)
    {
        _rate = 0f;
        _consumedEnergyThisTick = 0f;
        _consumedIonsThisTick = 0f;
        _consumedNeutrinosThisTick = 0f;
        _repairedHullThisTick = 0f;
    }

    internal static RepairSubsystem CreateClassicShipRepair(Controllable controllable)
    {
        return new RepairSubsystem(controllable, "Repair", true, SubsystemSlot.Repair);
    }

    /// <summary>
    /// The minimum configurable repair rate.
    /// </summary>
    public float MinimumRate
    {
        get { return MinimumRateValue; }
    }

    /// <summary>
    /// The maximum configurable repair rate.
    /// </summary>
    public float MaximumRate
    {
        get { return MaximumRateValue; }
    }

    /// <summary>
    /// The configured hull repair rate per tick.
    /// </summary>
    public float Rate
    {
        get { return _rate; }
    }

    /// <summary>
    /// The energy consumed during the current server tick.
    /// </summary>
    public float ConsumedEnergyThisTick
    {
        get { return _consumedEnergyThisTick; }
    }

    /// <summary>
    /// The ions consumed during the current server tick.
    /// </summary>
    public float ConsumedIonsThisTick
    {
        get { return _consumedIonsThisTick; }
    }

    /// <summary>
    /// The neutrinos consumed during the current server tick.
    /// </summary>
    public float ConsumedNeutrinosThisTick
    {
        get { return _consumedNeutrinosThisTick; }
    }

    /// <summary>
    /// The amount of hull repaired during the current server tick.
    /// </summary>
    public float RepairedHullThisTick
    {
        get { return _repairedHullThisTick; }
    }

    /// <summary>
    /// Calculates the resource costs for one repair tick at the specified rate.
    /// </summary>
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
    /// Sets the repair rate on the server.
    /// </summary>
    /// <exception cref="SpecifiedElementNotFoundGameException">Thrown, if the controllable or subsystem does not exist.</exception>
    /// <exception cref="YouNeedToContinueFirstGameException">Thrown, if the controllable is dead.</exception>
    /// <exception cref="InvalidArgumentGameException">Thrown, if an argument is invalid.</exception>
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
            writer.Command = 0x93;
            writer.Write(Controllable.Id);
            writer.Write(rate);
        });
    }

    internal void ResetRuntime()
    {
        _rate = 0f;
        _consumedEnergyThisTick = 0f;
        _consumedIonsThisTick = 0f;
        _consumedNeutrinosThisTick = 0f;
        _repairedHullThisTick = 0f;
        ResetRuntimeStatus();
    }

    internal void UpdateRuntime(float rate, SubsystemStatus status, float consumedEnergyThisTick, float consumedIonsThisTick,
        float consumedNeutrinosThisTick, float repairedHullThisTick)
    {
        _rate = rate;
        _consumedEnergyThisTick = consumedEnergyThisTick;
        _consumedIonsThisTick = consumedIonsThisTick;
        _consumedNeutrinosThisTick = consumedNeutrinosThisTick;
        _repairedHullThisTick = repairedHullThisTick;
        UpdateRuntimeStatus(status);
    }

    internal FlattiverseEvent? CreateRuntimeEvent()
    {
        if (!Exists || !ShouldEmitRuntimeEvent())
            return null;

        return new RepairSubsystemEvent(Controllable, Slot, Status, _rate, _consumedEnergyThisTick, _consumedIonsThisTick,
            _consumedNeutrinosThisTick, _repairedHullThisTick);
    }
}
