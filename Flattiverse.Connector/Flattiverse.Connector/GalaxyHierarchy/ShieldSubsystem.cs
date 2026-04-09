using Flattiverse.Connector.Events;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Shield subsystem of a controllable.
/// </summary>
public class ShieldSubsystem : Subsystem
{
    private float _maximum;
    private float _minimumRate;
    private float _maximumRate;
    private float _current;
    private bool _active;
    private float _rate;
    private float _consumedEnergyThisTick;
    private float _consumedIonsThisTick;
    private float _consumedNeutrinosThisTick;

    internal ShieldSubsystem(Controllable controllable, string name, bool exists, SubsystemSlot slot) :
        base(controllable, name, exists, slot)
    {
        _maximum = exists ? 20f : 0f;
        _minimumRate = 0f;
        _maximumRate = 0.125f;
        _current = 0f;
        _active = false;
        _rate = 0f;
        _consumedEnergyThisTick = 0f;
        _consumedIonsThisTick = 0f;
        _consumedNeutrinosThisTick = 0f;
    }

    internal ShieldSubsystem(Controllable controllable, string name, PacketReader reader, SubsystemSlot slot) :
        base(controllable, name, false, slot)
    {
        _maximum = 0f;
        _minimumRate = 0f;
        _maximumRate = 0f;
        _current = 0f;
        _active = false;
        _rate = 0f;
        _consumedEnergyThisTick = 0f;
        _consumedIonsThisTick = 0f;
        _consumedNeutrinosThisTick = 0f;

        if (!reader.Read(out byte exists))
            throw new InvalidDataException("Couldn't read controllable shield state.");

        SetExists(exists != 0);

        if (!Exists)
            return;

        if (!reader.Read(out byte tier) ||
            !reader.Read(out float maximum) ||
            !reader.Read(out float minimumRate) ||
            !reader.Read(out float maximumRate) ||
            !reader.Read(out float current) ||
            !reader.Read(out byte active) ||
            !reader.Read(out float rate) ||
            !reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick))
            throw new InvalidDataException("Couldn't read controllable shield state.");

        SetMaximum(maximum);
        SetRateCapabilities(minimumRate, maximumRate);
        UpdateRuntime(current, active != 0, rate, (SubsystemStatus)status, consumedEnergyThisTick, consumedIonsThisTick,
            consumedNeutrinosThisTick);
        SetReportedTier(tier);
    }

    internal static ShieldSubsystem CreateClassicShipShield(Controllable controllable)
    {
        return new ShieldSubsystem(controllable, "Shield", true, SubsystemSlot.Shield);
    }

    /// <summary>
    /// The maximum shield integrity.
    /// </summary>
    public float Maximum
    {
        get { return _maximum; }
    }

    /// <summary>
    /// The current shield integrity.
    /// </summary>
    public float Current
    {
        get { return _current; }
    }

    /// <summary>
    /// The minimum configurable shield load rate.
    /// </summary>
    public float MinimumRate
    {
        get { return _minimumRate; }
    }

    /// <summary>
    /// The maximum configurable shield load rate.
    /// </summary>
    public float MaximumRate
    {
        get { return _maximumRate; }
    }

    internal void SetMaximum(float maximum)
    {
        _maximum = Exists ? maximum : 0f;
        RefreshTier();

        if (_current > _maximum)
            _current = _maximum;
    }

    internal void SetRateCapabilities(float minimumRate, float maximumRate)
    {
        _minimumRate = Exists ? minimumRate : 0f;
        _maximumRate = Exists ? maximumRate : 0f;
        RefreshTier();
    }

    /// <summary>
    /// Whether shield loading is active.
    /// </summary>
    public bool Active
    {
        get { return _active; }
    }

    /// <summary>
    /// The configured shield load rate per tick.
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
    /// Calculates the resource costs for one shield loading tick at the specified rate.
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

        if (RateToTier(_maximum, _maximumRate) == 5)
        {
            ions = _maximumRate > 0f ? 0.9f * rate / _maximumRate : 0f;
            return true;
        }

        energy = ShipBalancing.CalculateShieldEnergy((byte)RateToTier(_maximum, _maximumRate), rate, _maximumRate,
            FullCostFromCapabilities(_maximum, _maximumRate));

        if (float.IsNaN(energy) || float.IsInfinity(energy))
        {
            energy = 0f;
            return false;
        }

        return true;
    }

    private static int RateToTier(float maximum, float maximumRate)
    {
        if (maximum <= 20.5f && maximumRate <= 0.101f)
            return 1;

        if (maximum <= 35.5f && maximumRate <= 0.141f)
            return 2;

        if (maximum <= 50.5f && maximumRate <= 0.181f)
            return 3;

        if (maximum <= 65.5f && maximumRate <= 0.231f)
            return 4;

        return 5;
    }

    private static float FullCostFromCapabilities(float maximum, float maximumRate)
    {
        return RateToTier(maximum, maximumRate) switch
        {
            1 => 16f,
            2 => 26f,
            3 => 39f,
            4 => 58f,
            5 => 82f,
            _ => 0f
        };
    }

    /// <summary>
    /// Sets the shield load rate on the server.
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
            writer.Command = 0x90;
            writer.Write(Controllable.Id);
            writer.Write(rate);
        });
    }

    /// <summary>
    /// Turns shield loading on.
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
            writer.Command = 0x91;
            writer.Write(Controllable.Id);
        });
    }

    /// <summary>
    /// Turns shield loading off.
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
            writer.Command = 0x92;
            writer.Write(Controllable.Id);
        });
    }

    internal void ResetRuntime()
    {
        _current = 0f;
        _active = false;
        _rate = 0f;
        _consumedEnergyThisTick = 0f;
        _consumedIonsThisTick = 0f;
        _consumedNeutrinosThisTick = 0f;
        ResetRuntimeStatus();
    }

    internal void UpdateRuntime(float current, bool active, float rate, SubsystemStatus status, float consumedEnergyThisTick,
        float consumedIonsThisTick, float consumedNeutrinosThisTick)
    {
        _current = current;
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

        if (!reader.Read(out float current) ||
            !reader.Read(out byte active) ||
            !reader.Read(out float rate) ||
            !reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick))
            return false;

        UpdateRuntime(current, active != 0, rate, (SubsystemStatus)status, consumedEnergyThisTick, consumedIonsThisTick,
            consumedNeutrinosThisTick);
        return true;
    }

    internal FlattiverseEvent? CreateRuntimeEvent()
    {
        if (!Exists || !ShouldEmitRuntimeEvent())
            return null;

        return new ShieldSubsystemEvent(Controllable, Slot, Status, _current, _active, _rate, _consumedEnergyThisTick,
            _consumedIonsThisTick, _consumedNeutrinosThisTick);
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
            ShipBalancing.GetShield(tier, out float maximum, out float maximumRate, out float fullCost, out float load);

            if (Matches(_maximum, maximum) && Matches(_maximumRate, maximumRate))
            {
                SetTier(tier);
                return;
            }
        }

        SetTier(0);
    }
}
