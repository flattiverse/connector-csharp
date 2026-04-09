using Flattiverse.Connector.Events;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Persistent shot-fabricator subsystem configuration and runtime state of a controllable.
/// </summary>
public class DynamicShotFabricatorSubsystem : Subsystem
{
    private const float MinimumRateValue = 0f;

    private float _maximumRate;
    private bool _active;
    private float _rate;
    private float _consumedEnergyThisTick;
    private float _consumedIonsThisTick;
    private float _consumedNeutrinosThisTick;

    internal DynamicShotFabricatorSubsystem(Controllable controllable, string name, bool exists, SubsystemSlot slot) :
        base(controllable, name, exists, slot)
    {
        _maximumRate = exists ? 0.025f : 0f;
        _active = false;
        _rate = 0f;
        _consumedEnergyThisTick = 0f;
        _consumedIonsThisTick = 0f;
        _consumedNeutrinosThisTick = 0f;
    }

    internal DynamicShotFabricatorSubsystem(Controllable controllable, string name, PacketReader reader, SubsystemSlot slot) :
        base(controllable, name, false, slot)
    {
        _maximumRate = 0f;
        _active = false;
        _rate = 0f;
        _consumedEnergyThisTick = 0f;
        _consumedIonsThisTick = 0f;
        _consumedNeutrinosThisTick = 0f;

        if (!reader.Read(out byte exists))
            throw new InvalidDataException($"Couldn't read controllable {name} state.");

        SetExists(exists != 0);

        if (!Exists)
            return;

        if (!reader.Read(out byte tier) ||
            !reader.Read(out float maximumRate) ||
            !reader.Read(out byte active) ||
            !reader.Read(out float rate) ||
            !reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick))
            throw new InvalidDataException($"Couldn't read controllable {name} state.");

        SetMaximumRate(maximumRate);
        UpdateRuntime(active != 0, rate, (SubsystemStatus)status, consumedEnergyThisTick, consumedIonsThisTick,
            consumedNeutrinosThisTick);
        SetReportedTier(tier);
    }

    /// <summary>
    /// The minimum configurable shot fabrication rate.
    /// </summary>
    public float MinimumRate
    {
        get { return MinimumRateValue; }
    }

    /// <summary>
    /// The maximum configurable shot fabrication rate.
    /// </summary>
    public float MaximumRate
    {
        get { return _maximumRate; }
    }

    /// <summary>
    /// True when the fabricator was active during the latest reported server tick.
    /// </summary>
    public bool Active
    {
        get { return _active; }
    }

    /// <summary>
    /// The configured shot fabrication rate.
    /// </summary>
    public float Rate
    {
        get { return _rate; }
    }

    /// <summary>
    /// The energy consumed by the fabricator during the current server tick.
    /// </summary>
    public float ConsumedEnergyThisTick
    {
        get { return _consumedEnergyThisTick; }
    }

    /// <summary>
    /// The ions consumed by the fabricator during the current server tick.
    /// </summary>
    public float ConsumedIonsThisTick
    {
        get { return _consumedIonsThisTick; }
    }

    /// <summary>
    /// The neutrinos consumed by the fabricator during the current server tick.
    /// </summary>
    public float ConsumedNeutrinosThisTick
    {
        get { return _consumedNeutrinosThisTick; }
    }

    /// <summary>
    /// Calculates the current placeholder resource costs for one fabrication tick at the specified rate.
    /// The current model consumes only energy.
    /// </summary>
    public bool CalculateCost(float rate, out float energy, out float ions, out float neutrinos)
    {
        energy = 0f;
        ions = 0f;
        neutrinos = 0f;

        if (!Exists)
            return false;

        if (RangeTolerance.ClampRange(rate, MinimumRateValue, MaximumRate, out rate) != InvalidArgumentKind.Valid)
            return false;

        energy = ShipBalancing.CalculateEngineEnergy(rate, MaximumRate, FullCostFromMaximumRate(MaximumRate));

        if (float.IsNaN(energy) || float.IsInfinity(energy))
        {
            energy = 0f;
            return false;
        }

        return true;
    }

    private static float FullCostFromMaximumRate(float maximumRate)
    {
        if (maximumRate <= 0.00331f)
            return 2.64f;

        if (maximumRate <= 0.00496f)
            return 3.76f;

        if (maximumRate <= 0.00688f)
            return 5.50f;

        if (maximumRate <= 0.00908f)
            return 8.62f;

        if (maximumRate <= 0.01156f)
            return 13.86f;

        if (maximumRate <= 0.0121f)
            return 8f;

        if (maximumRate <= 0.0181f)
            return 11f;

        if (maximumRate <= 0.0251f)
            return 16f;

        if (maximumRate <= 0.0331f)
            return 25f;

        return 39f;
    }

    /// <summary>
    /// Sets the shot fabrication rate on the server.
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

        InvalidArgumentKind rateValidity = RangeTolerance.ClampRange(rate, MinimumRateValue, MaximumRate, out rate);

        if (rateValidity != InvalidArgumentKind.Valid)
            throw new InvalidArgumentGameException(rateValidity, "rate");

        await Controllable.Cluster.Galaxy.Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0x8C;
            writer.Write(Controllable.Id);
            writer.Write(rate);
        });
    }

    /// <summary>
    /// Turns the shot fabricator on.
    /// </summary>
    /// <exception cref="SpecifiedElementNotFoundGameException">Thrown, if the controllable or subsystem does not exist.</exception>
    /// <exception cref="YouNeedToContinueFirstGameException">Thrown, if the controllable is dead.</exception>
    public async Task On()
    {
        if (!Controllable.Active || !Exists)
            throw new SpecifiedElementNotFoundGameException();

        if (!Controllable.Alive)
            throw new YouNeedToContinueFirstGameException();

        await Controllable.Cluster.Galaxy.Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0x8D;
            writer.Write(Controllable.Id);
        });
    }

    /// <summary>
    /// Turns the shot fabricator off.
    /// </summary>
    /// <exception cref="SpecifiedElementNotFoundGameException">Thrown, if the controllable or subsystem does not exist.</exception>
    /// <exception cref="YouNeedToContinueFirstGameException">Thrown, if the controllable is dead.</exception>
    public async Task Off()
    {
        if (!Controllable.Active || !Exists)
            throw new SpecifiedElementNotFoundGameException();

        if (!Controllable.Alive)
            throw new YouNeedToContinueFirstGameException();

        await Controllable.Cluster.Galaxy.Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0x8E;
            writer.Write(Controllable.Id);
        });
    }

    internal void SetMaximumRate(float maximumRate)
    {
        _maximumRate = Exists ? maximumRate : 0f;

        if (_rate > _maximumRate)
            _rate = _maximumRate;

        RefreshTier();
    }

    internal void ResetRuntime()
    {
        _active = false;
        _rate = 0f;
        _consumedEnergyThisTick = 0f;
        _consumedIonsThisTick = 0f;
        _consumedNeutrinosThisTick = 0f;
        ResetRuntimeStatus();
    }

    internal void UpdateRuntime(bool active, float rate, SubsystemStatus status, float consumedEnergyThisTick, float consumedIonsThisTick,
        float consumedNeutrinosThisTick)
    {
        _active = active;
        _rate = rate;
        _consumedEnergyThisTick = consumedEnergyThisTick;
        _consumedIonsThisTick = consumedIonsThisTick;
        _consumedNeutrinosThisTick = consumedNeutrinosThisTick;
        UpdateRuntimeStatus(status);
    }

    internal bool Update(PacketReader reader)
    {
        if (!Exists)
            return true;

        if (!reader.Read(out byte active) ||
            !reader.Read(out float rate) ||
            !reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick))
            return false;

        UpdateRuntime(active != 0, rate, (SubsystemStatus)status, consumedEnergyThisTick, consumedIonsThisTick,
            consumedNeutrinosThisTick);
        return true;
    }

    internal FlattiverseEvent? CreateRuntimeEvent()
    {
        if (!Exists || !ShouldEmitRuntimeEvent())
            return null;

        return new DynamicShotFabricatorSubsystemEvent(Controllable, Slot, Status, _active, _rate, _consumedEnergyThisTick,
            _consumedIonsThisTick, _consumedNeutrinosThisTick);
    }

    protected override void RefreshTier()
    {
        if (!Exists)
        {
            SetTier(0);
            return;
        }

        byte maximumTier = (byte)(TierInfos.Count - 1);

        for (byte tier = 1; tier <= maximumTier; tier++)
        {
            float maximumRate;
            float fullCost;
            float load;

            if (Slot is SubsystemSlot.DynamicShotFabricator or SubsystemSlot.DynamicInterceptorFabricator)
            {
                if (this is DynamicInterceptorFabricatorSubsystem or StaticInterceptorFabricatorSubsystem)
                    ShipBalancing.GetDynamicInterceptorFabricator(tier, out maximumRate, out fullCost, out load);
                else
                    ShipBalancing.GetDynamicShotFabricator(tier, out maximumRate, out fullCost, out load);
            }
            else
            {
                if (this is DynamicInterceptorFabricatorSubsystem or StaticInterceptorFabricatorSubsystem)
                    ShipBalancing.GetStaticInterceptorFabricator(tier, out maximumRate, out fullCost, out load);
                else
                    ShipBalancing.GetStaticShotFabricator(tier, out maximumRate, out fullCost, out load);
            }

            if (Matches(_maximumRate, maximumRate))
            {
                SetTier(tier);
                return;
            }
        }

        SetTier(0);
    }
}
