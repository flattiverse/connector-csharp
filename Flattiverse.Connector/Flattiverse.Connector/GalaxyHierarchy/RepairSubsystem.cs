using Flattiverse.Connector.Events;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Hull repair subsystem of a controllable.
/// </summary>
public class RepairSubsystem : Subsystem
{
    private float _minimumRate;
    private float _maximumRate;
    private float _rate;
    private float _consumedEnergyThisTick;
    private float _consumedIonsThisTick;
    private float _consumedNeutrinosThisTick;
    private float _repairedHullThisTick;

    internal RepairSubsystem(Controllable controllable, string name, bool exists, SubsystemSlot slot) :
        base(controllable, name, exists, slot)
    {
        _minimumRate = 0f;
        _maximumRate = 0.1f;
        _rate = 0f;
        _consumedEnergyThisTick = 0f;
        _consumedIonsThisTick = 0f;
        _consumedNeutrinosThisTick = 0f;
        _repairedHullThisTick = 0f;
    }

    internal RepairSubsystem(Controllable controllable, string name, PacketReader reader, SubsystemSlot slot) :
        base(controllable, name, false, slot)
    {
        _minimumRate = 0f;
        _maximumRate = 0f;
        _rate = 0f;
        _consumedEnergyThisTick = 0f;
        _consumedIonsThisTick = 0f;
        _consumedNeutrinosThisTick = 0f;
        _repairedHullThisTick = 0f;

        if (!reader.Read(out byte exists))
            throw new InvalidDataException("Couldn't read controllable repair state.");

        SetExists(exists != 0);

        if (!Exists)
            return;

        if (!reader.Read(out byte tier) ||
            !reader.Read(out float minimumRate) ||
            !reader.Read(out float maximumRate) ||
            !reader.Read(out float rate) ||
            !reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick) ||
            !reader.Read(out float repairedHullThisTick))
            throw new InvalidDataException("Couldn't read controllable repair state.");

        SetCapabilities(minimumRate, maximumRate);
        UpdateRuntime(rate, (SubsystemStatus)status, consumedEnergyThisTick, consumedIonsThisTick, consumedNeutrinosThisTick,
            repairedHullThisTick);
        SetReportedTier(tier);
    }

    internal static RepairSubsystem CreateClassicShipRepair(Controllable controllable)
    {
        return new RepairSubsystem(controllable, "Repair", true, SubsystemSlot.Repair);
    }

    /// <summary>
    /// The minimum configurable repair rate.
    /// <c>0</c> means the repair subsystem is off.
    /// </summary>
    public float MinimumRate
    {
        get { return _minimumRate; }
    }

    /// <summary>
    /// The maximum configurable repair rate.
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

    internal void CopyFrom(RepairSubsystem other)
    {
        CopyBaseFrom(other);
        _minimumRate = other._minimumRate;
        _maximumRate = other._maximumRate;
        _rate = other._rate;
        _consumedEnergyThisTick = other._consumedEnergyThisTick;
        _consumedIonsThisTick = other._consumedIonsThisTick;
        _consumedNeutrinosThisTick = other._consumedNeutrinosThisTick;
        _repairedHullThisTick = other._repairedHullThisTick;
    }

    /// <summary>
    /// The repair rate currently mirrored from the server.
    /// The server may clear this value back to <c>0</c>, for example after movement.
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

        if (RangeTolerance.ClampRange(rate, _minimumRate, _maximumRate, out rate) != InvalidArgumentKind.Valid)
            return false;

        energy = ShipBalancing.CalculateRepairEnergy(RateToTier(_maximumRate), rate, _maximumRate);

        if (float.IsNaN(energy) || float.IsInfinity(energy))
        {
            energy = 0f;
            return false;
        }

        return true;
    }

    private static byte RateToTier(float maximumRate)
    {
        if (maximumRate <= 0.051f)
            return 1;

        if (maximumRate <= 0.071f)
            return 2;

        if (maximumRate <= 0.101f)
            return 3;

        if (maximumRate <= 0.141f)
            return 4;

        return 5;
    }

    /// <summary>
    /// Sets the repair rate on the server.
    /// </summary>
    /// <remarks>
    /// The current classic ship uses <c>rate in [0; 0.1]</c> with placeholder tick cost
    /// <c>energy = 1600 * rate^2</c>. The server executes repair authoritatively: it only repairs hull and may clear the
    /// mirrored rate back to <c>0</c> when the ship movement reaches <c>&gt;= 0.1</c>.
    /// </remarks>
    /// <param name="rate">Requested repair rate. <c>0</c> turns the repair subsystem off.</param>
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
            writer.Command = 0x93;
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

    internal bool Update(PacketReader reader)
    {
        if (!Exists)
            return true;

        if (!reader.Read(out float rate) ||
            !reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick) ||
            !reader.Read(out float repairedHullThisTick))
            return false;

        UpdateRuntime(rate, (SubsystemStatus)status, consumedEnergyThisTick, consumedIonsThisTick, consumedNeutrinosThisTick,
            repairedHullThisTick);
        return true;
    }

    internal FlattiverseEvent? CreateRuntimeEvent()
    {
        if (!Exists || !ShouldEmitRuntimeEvent())
            return null;

        return new RepairSubsystemEvent(Controllable, Slot, Status, _rate, _consumedEnergyThisTick, _consumedIonsThisTick,
            _consumedNeutrinosThisTick, _repairedHullThisTick);
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
            ShipBalancing.GetRepair(tier, out float maximumRate, out float load);

            if (Matches(_maximumRate, maximumRate))
            {
                SetTier(tier);
                return;
            }
        }

        SetTier(0);
    }
}
