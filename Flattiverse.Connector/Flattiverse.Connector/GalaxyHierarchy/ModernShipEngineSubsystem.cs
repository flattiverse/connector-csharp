using System.Diagnostics;
using System.Linq;
using Flattiverse.Connector.Events;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Engine subsystem of one modern-ship thruster slot.
/// </summary>
public class ModernShipEngineSubsystem : Subsystem
{
    private float _maximumThrust;
    private float _maximumThrustChangePerTick;
    private float _currentThrust;
    private float _targetThrust;
    private float _consumedEnergyThisTick;
    private float _consumedIonsThisTick;
    private float _consumedNeutrinosThisTick;

    internal ModernShipEngineSubsystem(Controllable controllable, string name, bool exists, SubsystemSlot slot) :
        base(controllable, name, exists, slot)
    {
        _maximumThrust = 0f;
        _maximumThrustChangePerTick = 0f;
        _currentThrust = 0f;
        _targetThrust = 0f;
        _consumedEnergyThisTick = 0f;
        _consumedIonsThisTick = 0f;
        _consumedNeutrinosThisTick = 0f;
    }

    internal ModernShipEngineSubsystem(Controllable controllable, string name, PacketReader reader, SubsystemSlot slot) :
        base(controllable, name, false, slot)
    {
        _maximumThrust = 0f;
        _maximumThrustChangePerTick = 0f;
        _currentThrust = 0f;
        _targetThrust = 0f;
        _consumedEnergyThisTick = 0f;
        _consumedIonsThisTick = 0f;
        _consumedNeutrinosThisTick = 0f;

        if (!reader.Read(out byte exists))
            throw new InvalidDataException($"Couldn't read controllable {name} state.");

        SetExists(exists != 0);

        if (!Exists)
            return;

        if (!reader.Read(out byte tier) ||
            !reader.Read(out float maximumForwardThrust) ||
            !reader.Read(out float maximumReverseThrust) ||
            !reader.Read(out float maximumThrustChangePerTick) ||
            !reader.Read(out float currentThrust) ||
            !reader.Read(out float targetThrust) ||
            !reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick))
            throw new InvalidDataException($"Couldn't read controllable {name} state.");

        SetCapabilities(maximumForwardThrust, maximumReverseThrust, maximumThrustChangePerTick);
        UpdateRuntime(currentThrust, targetThrust, (SubsystemStatus)status, consumedEnergyThisTick, consumedIonsThisTick,
            consumedNeutrinosThisTick);
        SetReportedTier(tier);
    }

    public float MaximumThrust
    {
        get { return _maximumThrust; }
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

                float effectiveStructuralLoad = Controllable.CalculateProjectedEffectiveStructuralLoad(Slot, baseInfo.StructuralLoad);
                float adjustedMaximum = property.MaximumValue * SubsystemTierInfo.CalculateEngineEfficiency(effectiveStructuralLoad);
                SubsystemPropertyInfo[] properties = ReplaceMaximumThrust(baseInfo.Properties, adjustedMaximum);
                result[index] = new SubsystemTierInfo(baseInfo.SubsystemKind, baseInfo.Tier, baseInfo.StructuralLoad,
                    baseInfo.ResourceUsages.ToArray(), baseInfo.UpgradeCost, baseInfo.DowngradeCost, properties, baseInfo.Description);
            }

            return result;
        }
    }

    public float MaximumForwardThrust
    {
        get { return _maximumThrust; }
    }

    public float MaximumReverseThrust
    {
        get { return _maximumThrust; }
    }

    public float MaximumThrustChangePerTick
    {
        get { return _maximumThrustChangePerTick; }
    }

    public float CurrentThrust
    {
        get { return _currentThrust; }
    }

    public float TargetThrust
    {
        get { return _targetThrust; }
    }

    public float ConsumedEnergyThisTick
    {
        get { return _consumedEnergyThisTick; }
    }

    public float ConsumedIonsThisTick
    {
        get { return _consumedIonsThisTick; }
    }

    public float ConsumedNeutrinosThisTick
    {
        get { return _consumedNeutrinosThisTick; }
    }

    public bool CalculateCost(float thrust, out float energy, out float ions, out float neutrinos)
    {
        energy = 0f;
        ions = 0f;
        neutrinos = 0f;

        if (!Exists)
            return false;

        InvalidArgumentKind thrustValidity = RangeTolerance.ClampRange(thrust, -_maximumThrust, _maximumThrust, out thrust);

        if (thrustValidity != InvalidArgumentKind.Valid)
            return false;

        energy = ShipBalancing.CalculateEngineEnergy(MathF.Abs(thrust), _maximumThrust, FullCostFromMaximumThrust(_maximumThrust));

        if (float.IsNaN(energy) || float.IsInfinity(energy))
        {
            energy = 0f;
            return false;
        }

        return true;
    }

    public async Task SetThrust(float thrust)
    {
        if (!Controllable.Active || !Exists)
            throw new SpecifiedElementNotFoundGameException();

        if (!Controllable.Alive)
            throw new YouNeedToContinueFirstGameException();

        InvalidArgumentKind thrustValidity = RangeTolerance.ClampRange(thrust, -_maximumThrust, _maximumThrust, out thrust);

        if (thrustValidity != InvalidArgumentKind.Valid)
            throw new InvalidArgumentGameException(thrustValidity, "thrust");

        await Controllable.Cluster.Galaxy.Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0xA1;
            writer.Write(Controllable.Id);
            writer.Write((byte)Slot);
            writer.Write(thrust);
        });
    }

    public Task Off()
    {
        return SetThrust(0f);
    }

    internal void SetCapabilities(float maximumForwardThrust, float maximumReverseThrust, float maximumThrustChangePerTick)
    {
        Debug.Assert(MathF.Abs(maximumForwardThrust - maximumReverseThrust) < 0.0001f,
            "Modern engine capabilities are expected to be symmetric.");
        _maximumThrust = Exists ? MathF.Max(maximumForwardThrust, maximumReverseThrust) : 0f;
        _maximumThrustChangePerTick = Exists ? maximumThrustChangePerTick : 0f;
        RefreshTier();
    }

    private static float FullCostFromMaximumThrust(float maximumThrust)
    {
        if (maximumThrust <= 0.0161f)
            return 3.8f;

        if (maximumThrust <= 0.0231f)
            return 5.6f;

        if (maximumThrust <= 0.0311f)
            return 7.2f;

        return 10f;
    }

    internal void ResetRuntime()
    {
        _currentThrust = 0f;
        _targetThrust = 0f;
        _consumedEnergyThisTick = 0f;
        _consumedIonsThisTick = 0f;
        _consumedNeutrinosThisTick = 0f;
        ResetRuntimeStatus();
    }

    internal void UpdateRuntime(float currentThrust, float targetThrust, SubsystemStatus status, float consumedEnergyThisTick,
        float consumedIonsThisTick, float consumedNeutrinosThisTick)
    {
        _currentThrust = currentThrust;
        _targetThrust = targetThrust;
        _consumedEnergyThisTick = consumedEnergyThisTick;
        _consumedIonsThisTick = consumedIonsThisTick;
        _consumedNeutrinosThisTick = consumedNeutrinosThisTick;
        UpdateRuntimeStatus(status);
    }

    internal bool Update(PacketReader reader)
    {
        if (!Exists)
            return true;

        if (!reader.Read(out float currentThrust) ||
            !reader.Read(out float targetThrust) ||
            !reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick))
            return false;

        UpdateRuntime(currentThrust, targetThrust, (SubsystemStatus)status, consumedEnergyThisTick, consumedIonsThisTick,
            consumedNeutrinosThisTick);
        return true;
    }

    internal FlattiverseEvent? CreateRuntimeEvent()
    {
        if (!Exists || !ShouldEmitRuntimeEvent())
            return null;

        return new ModernShipEngineSubsystemEvent(Controllable, Slot, Status, _currentThrust, _targetThrust, _consumedEnergyThisTick,
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
            ShipBalancing.GetModernEngine(tier, out float maximumThrust, out float maximumThrustChangePerTick, out float fullCost,
                out float load);

            if (Matches(_maximumThrust, maximumThrust) && Matches(_maximumThrustChangePerTick, maximumThrustChangePerTick))
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
