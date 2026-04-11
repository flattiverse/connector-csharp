using System.Linq;
using Flattiverse.Connector.Events;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Engine subsystem of a classic ship controllable.
/// </summary>
public class ClassicShipEngineSubsystem : Subsystem
{
    private float _maximum;

    private Vector _current;
    private Vector _target;
    private float _consumedEnergyThisTick;
    private float _consumedIonsThisTick;
    private float _consumedNeutrinosThisTick;

    internal ClassicShipEngineSubsystem(Controllable controllable) :
        base(controllable, "Engine", true, SubsystemSlot.PrimaryEngine)
    {
        _maximum = 0.1f;
        _current = new Vector();
        _target = new Vector();
        _consumedEnergyThisTick = 0f;
        _consumedIonsThisTick = 0f;
        _consumedNeutrinosThisTick = 0f;
    }

    internal ClassicShipEngineSubsystem(Controllable controllable, PacketReader reader) :
        base(controllable, "Engine", false, SubsystemSlot.PrimaryEngine)
    {
        _maximum = 0f;
        _current = new Vector();
        _target = new Vector();
        _consumedEnergyThisTick = 0f;
        _consumedIonsThisTick = 0f;
        _consumedNeutrinosThisTick = 0f;

        if (!reader.Read(out byte exists))
            throw new InvalidDataException("Couldn't read controllable engine state.");

        SetExists(exists != 0);

        if (!Exists)
            return;

        if (!reader.Read(out byte tier) ||
            !reader.Read(out float maximum) ||
            !Vector.FromReader(reader, out Vector? current) ||
            !Vector.FromReader(reader, out Vector? target) ||
            !reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick))
            throw new InvalidDataException("Couldn't read controllable engine state.");

        SetMaximum(maximum);
        UpdateRuntime(current, target, (SubsystemStatus)status, consumedEnergyThisTick, consumedIonsThisTick,
            consumedNeutrinosThisTick);
        SetReportedTier(tier);
    }

    /// <summary>
    /// The maximum configurable movement vector length.
    /// </summary>
    public float Maximum
    {
        get { return _maximum; }
    }

    public override IReadOnlyList<SubsystemTierInfo> TierInfos
    {
        get
        {
            IReadOnlyList<SubsystemTierInfo> baseInfos = base.TierInfos;
            SubsystemTierInfo[] result = new SubsystemTierInfo[baseInfos.Count];

            for (int index = 0; index < baseInfos.Count; index++)
            {
                SubsystemTierInfo baseInfo = baseInfos[index];

                if (!baseInfo.TryGetProperty("maximumThrust", out SubsystemPropertyInfo? property) || property is null)
                {
                    result[index] = baseInfo;
                    continue;
                }

                float effectiveStructureLoad = Controllable.CalculateProjectedEffectiveStructureLoad(Slot, baseInfo.StructuralLoad);
                float adjustedMaximum = property.MaximumValue * SubsystemTierInfo.CalculateEngineEfficiency(effectiveStructureLoad);
                SubsystemPropertyInfo[] properties = ReplaceMaximumThrust(baseInfo.Properties, adjustedMaximum);
                result[index] = new SubsystemTierInfo(baseInfo.SubsystemKind, baseInfo.Tier, baseInfo.StructuralLoad,
                    baseInfo.ResourceUsages.ToArray(), baseInfo.UpgradeCost, baseInfo.DowngradeCost, properties, baseInfo.Description);
            }

            return result;
        }
    }

    internal void SetMaximum(float maximum)
    {
        _maximum = Exists ? maximum : 0f;
        RefreshTier();
    }

    internal void CopyFrom(ClassicShipEngineSubsystem other)
    {
        CopyBaseFrom(other);
        _maximum = other._maximum;
        _current = other._current;
        _target = other._target;
        _consumedEnergyThisTick = other._consumedEnergyThisTick;
        _consumedIonsThisTick = other._consumedIonsThisTick;
        _consumedNeutrinosThisTick = other._consumedNeutrinosThisTick;
    }

    /// <summary>
    /// The current server-applied movement impulse.
    /// </summary>
    public Vector Current
    {
        get { return new Vector(_current); }
    }

    /// <summary>
    /// The current target movement impulse.
    /// </summary>
    public Vector Target
    {
        get { return new Vector(_target); }
    }

    /// <summary>
    /// The energy consumed by the engine during the current server tick.
    /// </summary>
    public float ConsumedEnergyThisTick
    {
        get { return _consumedEnergyThisTick; }
    }

    /// <summary>
    /// The ions consumed by the engine during the current server tick.
    /// </summary>
    public float ConsumedIonsThisTick
    {
        get { return _consumedIonsThisTick; }
    }

    /// <summary>
    /// The neutrinos consumed by the engine during the current server tick.
    /// </summary>
    public float ConsumedNeutrinosThisTick
    {
        get { return _consumedNeutrinosThisTick; }
    }

    /// <summary>
    /// Calculates the current placeholder engine tick costs for the requested movement vector.
    /// The current formula is <c>energy = 12000 * movement.Length^3</c>.
    /// Returns false when the subsystem does not exist or the movement is outside the valid range.
    /// Values just above the maximum are clipped to the maximum before the cost is calculated.
    /// </summary>
    public bool CalculateCost(Vector movement, out float energy, out float ions, out float neutrinos)
    {
        energy = 0f;
        ions = 0f;
        neutrinos = 0f;

        if (!Exists)
            return false;

        if (RangeTolerance.ClampMaximum(movement, _maximum, out Vector clampedMovement) != InvalidArgumentKind.Valid)
            return false;

        energy = ShipBalancing.CalculateEngineEnergy(clampedMovement.Length, _maximum, FullCostFromMaximum(_maximum));

        if (float.IsNaN(energy) || float.IsInfinity(energy))
        {
            energy = 0f;
            return false;
        }

        return true;
    }

    private static float FullCostFromMaximum(float maximum)
    {
        if (maximum <= 0.0381f)
            return 8f;

        if (maximum <= 0.0551f)
            return 13f;

        if (maximum <= 0.0731f)
            return 18f;

        if (maximum <= 0.0921f)
            return 25f;

        return 35f;
    }

    /// <summary>
    /// Sets the target movement impulse on the server.
    /// Values just above the maximum are clipped to the maximum before they are sent.
    /// </summary>
    /// <exception cref="SpecifiedElementNotFoundGameException">Thrown, if the controllable or subsystem does not exist.</exception>
    /// <exception cref="YouNeedToContinueFirstGameException">Thrown, if the controllable is dead.</exception>
    /// <exception cref="InvalidArgumentGameException">Thrown, if an argument is invalid.</exception>
    public async Task Set(Vector movement)
    {
        if (!Controllable.Active || !Exists)
            throw new SpecifiedElementNotFoundGameException();

        if (!Controllable.Alive)
            throw new YouNeedToContinueFirstGameException();

        InvalidArgumentKind movementValidity = RangeTolerance.ClampMaximum(movement, _maximum, out movement);

        if (movementValidity != InvalidArgumentKind.Valid)
            throw new InvalidArgumentGameException(movementValidity, "movement");

        await Controllable.Cluster.Galaxy.Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0x87;
            writer.Write(Controllable.Id);
            movement.Write(ref writer);
        });
    }

    /// <summary>
    /// Turns the engine off by requesting a zero movement vector.
    /// </summary>
    public async Task Off()
    {
        await Set(new Vector());
    }

    internal void ResetRuntime()
    {
        _current = new Vector();
        _target = new Vector();
        _consumedEnergyThisTick = 0f;
        _consumedIonsThisTick = 0f;
        _consumedNeutrinosThisTick = 0f;
        ResetRuntimeStatus();
    }

    internal void UpdateRuntime(Vector current, Vector target, SubsystemStatus status, float consumedEnergyThisTick,
        float consumedIonsThisTick, float consumedNeutrinosThisTick)
    {
        _current = current;
        _target = target;
        _consumedEnergyThisTick = consumedEnergyThisTick;
        _consumedIonsThisTick = consumedIonsThisTick;
        _consumedNeutrinosThisTick = consumedNeutrinosThisTick;
        UpdateRuntimeStatus(status);
    }

    internal bool Update(PacketReader reader)
    {
        if (!Exists)
            return true;

        if (!Vector.FromReader(reader, out Vector? current) ||
            !Vector.FromReader(reader, out Vector? target) ||
            !reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick))
            return false;

        UpdateRuntime(current, target, (SubsystemStatus)status, consumedEnergyThisTick, consumedIonsThisTick,
            consumedNeutrinosThisTick);
        return true;
    }

    internal FlattiverseEvent? CreateRuntimeEvent()
    {
        if (!Exists || !ShouldEmitRuntimeEvent())
            return null;

        return new ClassicShipEngineSubsystemEvent(Controllable, Slot, Status, _current, _target,
            _consumedEnergyThisTick, _consumedIonsThisTick, _consumedNeutrinosThisTick);
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
            ShipBalancing.GetClassicEngine(tier, out float maximum, out float fullCost, out float load);

            if (Matches(_maximum, maximum))
            {
                SetTier(tier);
                return;
            }
        }

        SetTier(0);
    }

    private static SubsystemPropertyInfo[] ReplaceMaximumThrust(IReadOnlyList<SubsystemPropertyInfo> source, float maximumThrust)
    {
        SubsystemPropertyInfo[] result = new SubsystemPropertyInfo[source.Count];

        for (int index = 0; index < source.Count; index++)
        {
            SubsystemPropertyInfo property = source[index];

            if (property.Key == "maximumThrust")
                result[index] = new SubsystemPropertyInfo(property.Key, property.Label, property.Unit, maximumThrust, maximumThrust);
            else
                result[index] = property;
        }

        return result;
    }
}
