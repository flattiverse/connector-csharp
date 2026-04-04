using Flattiverse.Connector.Events;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Resource miner subsystem of a controllable.
/// </summary>
public class ResourceMinerSubsystem : Subsystem
{
    private float _minimumRate;
    private float _maximumRate;
    private float _rate;
    private float _consumedEnergyThisTick;
    private float _consumedIonsThisTick;
    private float _consumedNeutrinosThisTick;
    private float _minedMetalThisTick;
    private float _minedCarbonThisTick;
    private float _minedHydrogenThisTick;
    private float _minedSiliconThisTick;

    internal ResourceMinerSubsystem(Controllable controllable, bool exists, SubsystemSlot slot) :
        base(controllable, "ResourceMiner", exists, slot)
    {
        _minimumRate = 0f;
        _maximumRate = 0.01f;
        _rate = 0f;
        _consumedEnergyThisTick = 0f;
        _consumedIonsThisTick = 0f;
        _consumedNeutrinosThisTick = 0f;
        _minedMetalThisTick = 0f;
        _minedCarbonThisTick = 0f;
        _minedHydrogenThisTick = 0f;
        _minedSiliconThisTick = 0f;
    }

    internal static ResourceMinerSubsystem CreateClassicShipResourceMiner(Controllable controllable)
    {
        return new ResourceMinerSubsystem(controllable, true, SubsystemSlot.ResourceMiner);
    }

    /// <summary>
    /// Minimum configurable mining rate.
    /// </summary>
    public float MinimumRate
    {
        get { return _minimumRate; }
    }

    /// <summary>
    /// Maximum configurable mining rate.
    /// </summary>
    public float MaximumRate
    {
        get { return _maximumRate; }
    }

    internal void SetCapabilities(float minimumRate, float maximumRate)
    {
        _minimumRate = Exists ? minimumRate : 0f;
        _maximumRate = Exists ? maximumRate : 0f;
        RefreshTier();
    }

    /// <summary>
    /// Configured mining rate for the tick.
    /// </summary>
    public float Rate
    {
        get { return _rate; }
    }

    /// <summary>
    /// Energy consumed during the current server tick.
    /// </summary>
    public float ConsumedEnergyThisTick
    {
        get { return _consumedEnergyThisTick; }
    }

    /// <summary>
    /// Ions consumed during the current server tick.
    /// </summary>
    public float ConsumedIonsThisTick
    {
        get { return _consumedIonsThisTick; }
    }

    /// <summary>
    /// Neutrinos consumed during the current server tick.
    /// </summary>
    public float ConsumedNeutrinosThisTick
    {
        get { return _consumedNeutrinosThisTick; }
    }

    /// <summary>
    /// Metal mined during the current server tick.
    /// </summary>
    public float MinedMetalThisTick
    {
        get { return _minedMetalThisTick; }
    }

    /// <summary>
    /// Carbon mined during the current server tick.
    /// </summary>
    public float MinedCarbonThisTick
    {
        get { return _minedCarbonThisTick; }
    }

    /// <summary>
    /// Hydrogen mined during the current server tick.
    /// </summary>
    public float MinedHydrogenThisTick
    {
        get { return _minedHydrogenThisTick; }
    }

    /// <summary>
    /// Silicon mined during the current server tick.
    /// </summary>
    public float MinedSiliconThisTick
    {
        get { return _minedSiliconThisTick; }
    }

    /// <summary>
    /// Calculates the resource costs for one mining tick at the specified rate.
    /// </summary>
    public bool CalculateCost(float rate, out float energy, out float ions, out float neutrinos)
    {
        energy = 0f;
        ions = 0f;
        neutrinos = 0f;

        if (!Exists)
            return false;

        if (RangeTolerance.ClampRange(rate, _minimumRate, _maximumRate, out rate) != InvalidArgumentKind.Valid)
            return false;

        energy = ShipBalancing.CalculateEngineEnergy(rate, _maximumRate, FullCostFromMaximumRate(_maximumRate));

        if (float.IsNaN(energy) || float.IsInfinity(energy))
        {
            energy = 0f;
            return false;
        }

        return true;
    }

    private static float FullCostFromMaximumRate(float maximumRate)
    {
        if (maximumRate <= 0.00221f)
            return 10f;

        if (maximumRate <= 0.00331f)
            return 14f;

        if (maximumRate <= 0.00461f)
            return 20f;

        if (maximumRate <= 0.00611f)
            return 30f;

        return 44f;
    }

    /// <summary>
    /// Sets the mining rate on the server.
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

        InvalidArgumentKind rateValidity = RangeTolerance.ClampRange(rate, _minimumRate, _maximumRate, out rate);

        if (rateValidity != InvalidArgumentKind.Valid)
            throw new InvalidArgumentGameException(rateValidity, "rate");

        await Controllable.Cluster.Galaxy.Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0x94;
            writer.Write(Controllable.Id);
            writer.Write(rate);
        });
    }

    /// <summary>
    /// Turns the resource miner off by setting the rate to zero.
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
        _minedMetalThisTick = 0f;
        _minedCarbonThisTick = 0f;
        _minedHydrogenThisTick = 0f;
        _minedSiliconThisTick = 0f;
        ResetRuntimeStatus();
    }

    internal void UpdateRuntime(float rate, SubsystemStatus status, float consumedEnergyThisTick, float consumedIonsThisTick,
        float consumedNeutrinosThisTick, float minedMetalThisTick, float minedCarbonThisTick, float minedHydrogenThisTick,
        float minedSiliconThisTick)
    {
        _rate = rate;
        _consumedEnergyThisTick = consumedEnergyThisTick;
        _consumedIonsThisTick = consumedIonsThisTick;
        _consumedNeutrinosThisTick = consumedNeutrinosThisTick;
        _minedMetalThisTick = minedMetalThisTick;
        _minedCarbonThisTick = minedCarbonThisTick;
        _minedHydrogenThisTick = minedHydrogenThisTick;
        _minedSiliconThisTick = minedSiliconThisTick;
        UpdateRuntimeStatus(status);
    }

    internal FlattiverseEvent? CreateRuntimeEvent()
    {
        if (!Exists || !ShouldEmitRuntimeEvent())
            return null;

        return new ResourceMinerSubsystemEvent(Controllable, Slot, Status, _rate, _consumedEnergyThisTick, _consumedIonsThisTick,
            _consumedNeutrinosThisTick, _minedMetalThisTick, _minedCarbonThisTick, _minedHydrogenThisTick, _minedSiliconThisTick);
    }

    protected override void RefreshTier()
    {
        if (!Exists)
        {
            SetTier(0);
            return;
        }

        for (byte tier = 1; tier <= ShipUpgradeBalancing.GetMaximumTier(Slot); tier++)
        {
            ShipBalancing.GetResourceMiner(tier, ModernShip, out float maximumRate, out float fullCost, out float load);

            if (Matches(_maximumRate, maximumRate))
            {
                SetTier(tier);
                return;
            }
        }

        SetTier(0);
    }
}
