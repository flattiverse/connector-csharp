using Flattiverse.Connector.Events;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Owner-side handle for one registered controllable of the local player.
/// A controllable survives deaths until it is finally closed and is distinct from the visible
/// <see cref="PlayerUnit" /> snapshots in <see cref="Cluster.Units" />.
/// </summary>
public class Controllable : INamedUnit
{
    private readonly string _name;

    /// <summary>
    /// The id of the controllable.
    /// </summary>
    public readonly byte Id;
    
    /// <summary>
    /// The cluster this controllable belongs to.
    /// </summary>
    protected Cluster _cluster;
    
    private bool _active;
    
    private bool _alive;

    private Vector _position;
    private Vector _movement;
    private protected HullSubsystem _hull;
    private protected ShieldSubsystem _shield;
    private protected ArmorSubsystem _armor;
    private protected RepairSubsystem _repair;
    private protected CargoSubsystem _cargo;
    private protected ResourceMinerSubsystem _resourceMiner;
    private protected BatterySubsystem _energyBattery;
    private protected BatterySubsystem _ionBattery;
    private protected BatterySubsystem _neutrinoBattery;
    private protected EnergyCellSubsystem _energyCell;
    private protected EnergyCellSubsystem _ionCell;
    private protected EnergyCellSubsystem _neutrinoCell;
    private float _environmentHeatThisTick;
    private float _environmentHeatEnergyCostThisTick;
    private float _environmentHeatEnergyOverflowThisTick;
    private float _environmentRadiationThisTick;
    private float _environmentRadiationDamageBeforeArmorThisTick;
    private float _environmentArmorBlockedDamageThisTick;
    private float _environmentHullDamageThisTick;

    internal Controllable(byte id, string name, Cluster cluster, PacketReader reader)
    {
        _cluster = cluster;

        Id = id;
        _name = name;
        
        _active = true;
        _hull = null!;
        _shield = null!;
        _armor = null!;
        _repair = null!;
        _cargo = null!;
        _resourceMiner = null!;
        _energyBattery = null!;
        _ionBattery = null!;
        _neutrinoBattery = null!;
        _energyCell = null!;
        _ionCell = null!;
        _neutrinoCell = null!;
        _environmentHeatThisTick = 0f;
        _environmentHeatEnergyCostThisTick = 0f;
        _environmentHeatEnergyOverflowThisTick = 0f;
        _environmentRadiationThisTick = 0f;
        _environmentRadiationDamageBeforeArmorThisTick = 0f;
        _environmentArmorBlockedDamageThisTick = 0f;
        _environmentHullDamageThisTick = 0f;

        if (!Vector.FromReader(reader, out _position) || !Vector.FromReader(reader, out _movement))
            throw new InvalidDataException("Couldn't read controllable.");
    }

    /// <summary>
    /// The name of the controllable.
    /// </summary>
    public string Name => _name;
    
    /// <summary>
    /// Declared runtime unit kind that this controllable uses while it is alive in the world.
    /// </summary>
    public virtual UnitKind Kind => UnitKind.ClassicShipPlayerUnit;
    
    /// <summary>
    /// The cluster this unit currently is in.
    /// </summary>
    public Cluster Cluster => _cluster;
    
    /// <summary>
    /// The position of the unit.
    /// </summary>
    public Vector Position => new Vector(_position);
    
    /// <summary>
    /// The movement of the unit.
    /// </summary>
    public Vector Movement => new Vector(_movement);

    /// <summary>
    /// The energy battery subsystem of the controllable.
    /// </summary>
    public BatterySubsystem EnergyBattery
    {
        get { return _energyBattery; }
    }

    /// <summary>
    /// The hull subsystem of the controllable.
    /// </summary>
    public HullSubsystem Hull
    {
        get { return _hull; }
    }

    /// <summary>
    /// The shield subsystem of the controllable.
    /// </summary>
    public ShieldSubsystem Shield
    {
        get { return _shield; }
    }

    /// <summary>
    /// The armor subsystem of the controllable.
    /// </summary>
    public ArmorSubsystem Armor
    {
        get { return _armor; }
    }

    /// <summary>
    /// The ion battery subsystem of the controllable.
    /// </summary>
    public BatterySubsystem IonBattery
    {
        get { return _ionBattery; }
    }

    /// <summary>
    /// The neutrino battery subsystem of the controllable.
    /// </summary>
    public BatterySubsystem NeutrinoBattery
    {
        get { return _neutrinoBattery; }
    }

    /// <summary>
    /// The energy cell subsystem of the controllable.
    /// </summary>
    public EnergyCellSubsystem EnergyCell
    {
        get { return _energyCell; }
    }

    /// <summary>
    /// The ion cell subsystem of the controllable.
    /// </summary>
    public EnergyCellSubsystem IonCell
    {
        get { return _ionCell; }
    }

    /// <summary>
    /// The neutrino cell subsystem of the controllable.
    /// </summary>
    public EnergyCellSubsystem NeutrinoCell
    {
        get { return _neutrinoCell; }
    }

    /// <summary>
    /// The repair subsystem of the controllable.
    /// </summary>
    public RepairSubsystem Repair
    {
        get { return _repair; }
    }

    /// <summary>
    /// The cargo subsystem of the controllable.
    /// </summary>
    public CargoSubsystem Cargo
    {
        get { return _cargo; }
    }

    /// <summary>
    /// The resource miner subsystem of the controllable.
    /// </summary>
    public ResourceMinerSubsystem ResourceMiner
    {
        get { return _resourceMiner; }
    }

    /// <summary>
    /// Aggregated environment heat applied during the current server tick.
    /// </summary>
    public float EnvironmentHeatThisTick
    {
        get { return _environmentHeatThisTick; }
    }

    /// <summary>
    /// Energy drained by environment heat during the current server tick.
    /// </summary>
    public float EnvironmentHeatEnergyCostThisTick
    {
        get { return _environmentHeatEnergyCostThisTick; }
    }

    /// <summary>
    /// Heat energy that could not be paid and overflowed into radiation during the current server tick.
    /// </summary>
    public float EnvironmentHeatEnergyOverflowThisTick
    {
        get { return _environmentHeatEnergyOverflowThisTick; }
    }

    /// <summary>
    /// Aggregated environment radiation applied during the current server tick.
    /// </summary>
    public float EnvironmentRadiationThisTick
    {
        get { return _environmentRadiationThisTick; }
    }

    /// <summary>
    /// Radiation damage before armor reduction during the current server tick.
    /// </summary>
    public float EnvironmentRadiationDamageBeforeArmorThisTick
    {
        get { return _environmentRadiationDamageBeforeArmorThisTick; }
    }

    /// <summary>
    /// Environment damage blocked by armor during the current server tick.
    /// </summary>
    public float EnvironmentArmorBlockedDamageThisTick
    {
        get { return _environmentArmorBlockedDamageThisTick; }
    }

    /// <summary>
    /// Environment damage that reached the hull during the current server tick.
    /// </summary>
    public float EnvironmentHullDamageThisTick
    {
        get { return _environmentHullDamageThisTick; }
    }
    
    /// <summary>
    /// True while this controllable currently has an active in-world runtime.
    /// </summary>
    public bool Alive => _alive;
    
    /// <summary>
    /// True while this controllable still exists on the server and can still be addressed by commands.
    /// After the final close, this becomes <see langword="false" /> permanently.
    /// </summary>
    public bool Active => _active;

    /// <summary>
    /// Requests that this controllable enters the world again after initial registration or after a previous death.
    /// </summary>
    /// <remarks>
    /// <para>
    /// In Flattiverse, owning a <see cref="Controllable" /> and currently flying it are different lifecycle states.
    /// A newly registered ship exists as an owner-side controllable before it has an active in-world runtime, and a
    /// dead controllable also remains registered until it is finally closed. <see cref="Continue()" /> is the command
    /// that asks the server to spawn or respawn that registered controllable.
    /// </para>
    /// <para>
    /// Spawn cluster, spawn position, revived runtime values, and all subsystem state are chosen authoritatively by
    /// the server. Do not infer the post-continue state locally. Instead, read the updated owner-side mirror
    /// afterwards via properties such as <see cref="Alive" />, <see cref="Cluster" />, <see cref="Position" />, and
    /// the subsystem objects on this controllable.
    /// </para>
    /// <para>
    /// A dead controllable can usually be continued repeatedly until <see cref="RequestClose()" /> has been issued and
    /// the server has performed the final close. Calling this on an already alive controllable is invalid.
    /// </para>
    /// </remarks>
    /// <exception cref="SpecifiedElementNotFoundGameException">
    /// Thrown, if this controllable no longer exists on the server, for example because it has already been finally
    /// closed.
    /// </exception>
    /// <exception cref="YouNeedToDieFirstGameException">
    /// Thrown, if the controllable is still alive and therefore does not need a continue request.
    /// </exception>
    /// <exception cref="ControllableIsClosingGameException">
    /// Thrown, if the controllable has already entered the closing phase and can no longer be continued.
    /// </exception>
    /// <exception cref="AllStartLocationsAreOvercrowded">
    /// Thrown, if the galaxy currently cannot find a free spawn position for this continue request.
    /// </exception>
    /// <exception cref="TournamentRegistrationClosedGameException">
    /// Thrown, if the current tournament stage forbids ship registration or respawn.
    /// </exception>
    /// <exception cref="TournamentParticipantRequiredGameException">
    /// Thrown, if the account does not participate in the configured tournament.
    /// </exception>
    /// <exception cref="TournamentTeamMismatchGameException">
    /// Thrown, if the controllable belongs to a team that conflicts with the configured tournament team of the
    /// account.
    /// </exception>
    public async Task Continue()
    {
        await _cluster.Galaxy.Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0x84;
            writer.Write(Id);
        });
    }

    internal void Deactivate()
    {
        _active = false;
        _alive = false;
        ResetRuntime();
    }
    
    /// <summary>
    /// Requests self-destruction of the currently alive runtime of this controllable.
    /// The registration itself remains available afterwards until it is explicitly closed.
    /// </summary>
    /// <exception cref="SpecifiedElementNotFoundGameException">
    /// Thrown, if this controllable no longer exists on the server.
    /// </exception>
    /// <exception cref="YouNeedToContinueFirstGameException">
    /// Thrown, if this controllable is already dead and therefore has no active runtime to destroy.
    /// </exception>
    public async Task Suicide()
    {
        await _cluster.Galaxy.Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0x85;
            writer.Write(Id);
        });
    }
    
    /// <summary>
    /// Requests final closure of this controllable registration.
    /// The server may keep the controllable alive for a grace period before it is actually removed.
    /// </summary>
    /// <remarks>
    /// This request is fire-and-forget and therefore does not wait for a reply. Observe subsequent events and
    /// <see cref="Active" /> to detect when the close has completed.
    /// </remarks>
    public void RequestClose()
    {
        _cluster.Galaxy.Connection.Send(delegate (ref PacketWriter writer)
        {
            writer.Command = 0x8F;
            writer.Write(Id);
        });
        _cluster.Galaxy.Connection.Flush();
    }

    internal static bool New(UnitKind kind, Cluster cluster, byte id, string name, PacketReader reader, out Controllable? controllable)
    {
        switch (kind)
        {
            case UnitKind.ClassicShipPlayerUnit:
                controllable = new ClassicShipControllable(cluster, id, name, reader);
                return true;
            default:
                controllable = null;
                return false;
        }
    }

    /// <summary>
    /// Gravity emitted by the live runtime of this controllable.
    /// </summary>
    public virtual float Gravity => 0.0012f;
    
    /// <summary>
    /// Collision radius of the live runtime of this controllable.
    /// </summary>
    public virtual float Size => 14f;
    
    internal void Deceased()
    {
        _alive = false;
        
        _position = new Vector();
        _movement = new Vector();
        ResetRuntime();
    }

    private protected void ReadInitialState(PacketReader reader)
    {
        byte cargoExists;
        float cargoMaximumMetal;
        float cargoMaximumCarbon;
        float cargoMaximumHydrogen;
        float cargoMaximumSilicon;
        float cargoMaximumNebula;
        float cargoCurrentMetal;
        float cargoCurrentCarbon;
        float cargoCurrentHydrogen;
        float cargoCurrentSilicon;
        float cargoCurrentNebula;
        float cargoNebulaHue;
        byte cargoStatus;
        byte resourceMinerExists;
        float resourceMinerMinimumRate;
        float resourceMinerMaximumRate;
        float resourceMinerRate;
        byte resourceMinerStatus;
        float resourceMinerConsumedEnergyThisTick;
        float resourceMinerConsumedIonsThisTick;
        float resourceMinerConsumedNeutrinosThisTick;
        float resourceMinerMinedMetalThisTick;
        float resourceMinerMinedCarbonThisTick;
        float resourceMinerMinedHydrogenThisTick;
        float resourceMinerMinedSiliconThisTick;

        if (!reader.Read(out byte energyBatteryExists) ||
            !reader.Read(out float energyBatteryMaximum) ||
            !reader.Read(out float energyBatteryCurrent) ||
            !reader.Read(out float energyBatteryConsumedThisTick) ||
            !reader.Read(out byte energyBatteryStatus) ||
            !reader.Read(out byte ionBatteryExists) ||
            !reader.Read(out float ionBatteryMaximum) ||
            !reader.Read(out float ionBatteryCurrent) ||
            !reader.Read(out float ionBatteryConsumedThisTick) ||
            !reader.Read(out byte ionBatteryStatus) ||
            !reader.Read(out byte neutrinoBatteryExists) ||
            !reader.Read(out float neutrinoBatteryMaximum) ||
            !reader.Read(out float neutrinoBatteryCurrent) ||
            !reader.Read(out float neutrinoBatteryConsumedThisTick) ||
            !reader.Read(out byte neutrinoBatteryStatus) ||
            !reader.Read(out byte energyCellExists) ||
            !reader.Read(out float energyCellEfficiency) ||
            !reader.Read(out float energyCellCollectedThisTick) ||
            !reader.Read(out byte energyCellStatus) ||
            !reader.Read(out byte ionCellExists) ||
            !reader.Read(out float ionCellEfficiency) ||
            !reader.Read(out float ionCellCollectedThisTick) ||
            !reader.Read(out byte ionCellStatus) ||
            !reader.Read(out byte neutrinoCellExists) ||
            !reader.Read(out float neutrinoCellEfficiency) ||
            !reader.Read(out float neutrinoCellCollectedThisTick) ||
            !reader.Read(out byte neutrinoCellStatus) ||
            !reader.Read(out byte hullExists) ||
            !reader.Read(out float hullMaximum) ||
            !reader.Read(out float hullCurrent) ||
            !reader.Read(out byte hullStatus) ||
            !reader.Read(out byte shieldExists) ||
            !reader.Read(out float shieldMaximum) ||
            !reader.Read(out float shieldCurrent) ||
            !reader.Read(out byte shieldActive) ||
            !reader.Read(out float shieldRate) ||
            !reader.Read(out byte shieldStatus) ||
            !reader.Read(out float shieldConsumedEnergyThisTick) ||
            !reader.Read(out float shieldConsumedIonsThisTick) ||
            !reader.Read(out float shieldConsumedNeutrinosThisTick) ||
            !reader.Read(out byte armorExists) ||
            !reader.Read(out float armorReduction) ||
            !reader.Read(out byte armorStatus) ||
            !reader.Read(out float armorBlockedDirectDamageThisTick) ||
            !reader.Read(out float armorBlockedRadiationDamageThisTick) ||
            !reader.Read(out byte repairExists) ||
            !reader.Read(out float repairMinimumRate) ||
            !reader.Read(out float repairMaximumRate) ||
            !reader.Read(out float repairRate) ||
            !reader.Read(out byte repairStatus) ||
            !reader.Read(out float repairConsumedEnergyThisTick) ||
            !reader.Read(out float repairConsumedIonsThisTick) ||
            !reader.Read(out float repairConsumedNeutrinosThisTick) ||
            !reader.Read(out float repairRepairedHullThisTick) ||
            !reader.Read(out cargoExists) ||
            !reader.Read(out cargoMaximumMetal) ||
            !reader.Read(out cargoMaximumCarbon) ||
            !reader.Read(out cargoMaximumHydrogen) ||
            !reader.Read(out cargoMaximumSilicon) ||
            !reader.Read(out cargoMaximumNebula) ||
            !reader.Read(out cargoCurrentMetal) ||
            !reader.Read(out cargoCurrentCarbon) ||
            !reader.Read(out cargoCurrentHydrogen) ||
            !reader.Read(out cargoCurrentSilicon) ||
            !reader.Read(out cargoCurrentNebula) ||
            !reader.Read(out cargoNebulaHue) ||
            !reader.Read(out cargoStatus) ||
            !reader.Read(out resourceMinerExists) ||
            !reader.Read(out resourceMinerMinimumRate) ||
            !reader.Read(out resourceMinerMaximumRate) ||
            !reader.Read(out resourceMinerRate) ||
            !reader.Read(out resourceMinerStatus) ||
            !reader.Read(out resourceMinerConsumedEnergyThisTick) ||
            !reader.Read(out resourceMinerConsumedIonsThisTick) ||
            !reader.Read(out resourceMinerConsumedNeutrinosThisTick) ||
            !reader.Read(out resourceMinerMinedMetalThisTick) ||
            !reader.Read(out resourceMinerMinedCarbonThisTick) ||
            !reader.Read(out resourceMinerMinedHydrogenThisTick) ||
            !reader.Read(out resourceMinerMinedSiliconThisTick))
            throw new InvalidDataException("Couldn't read controllable create state.");

        _energyBattery.SetMaximum(energyBatteryMaximum);
        _energyBattery.UpdateRuntime(energyBatteryCurrent, energyBatteryConsumedThisTick, (SubsystemStatus)energyBatteryStatus);
        _ionBattery.SetMaximum(ionBatteryMaximum);
        _ionBattery.UpdateRuntime(ionBatteryCurrent, ionBatteryConsumedThisTick, (SubsystemStatus)ionBatteryStatus);
        _neutrinoBattery.SetMaximum(neutrinoBatteryMaximum);
        _neutrinoBattery.UpdateRuntime(neutrinoBatteryCurrent, neutrinoBatteryConsumedThisTick, (SubsystemStatus)neutrinoBatteryStatus);
        _energyCell.SetEfficiency(energyCellEfficiency);
        _energyCell.UpdateRuntime(energyCellCollectedThisTick, (SubsystemStatus)energyCellStatus);
        _ionCell.SetEfficiency(ionCellEfficiency);
        _ionCell.UpdateRuntime(ionCellCollectedThisTick, (SubsystemStatus)ionCellStatus);
        _neutrinoCell.SetEfficiency(neutrinoCellEfficiency);
        _neutrinoCell.UpdateRuntime(neutrinoCellCollectedThisTick, (SubsystemStatus)neutrinoCellStatus);
        _hull.SetMaximum(hullMaximum);
        _hull.UpdateRuntime(hullCurrent, (SubsystemStatus)hullStatus);
        _shield.SetMaximum(shieldMaximum);
        _shield.UpdateRuntime(shieldCurrent, shieldActive != 0, shieldRate, (SubsystemStatus)shieldStatus, shieldConsumedEnergyThisTick,
            shieldConsumedIonsThisTick, shieldConsumedNeutrinosThisTick);
        _armor.SetReduction(armorReduction);
        _armor.UpdateRuntime(armorBlockedDirectDamageThisTick, armorBlockedRadiationDamageThisTick, (SubsystemStatus)armorStatus);
        _repair.UpdateRuntime(repairRate, (SubsystemStatus)repairStatus, repairConsumedEnergyThisTick, repairConsumedIonsThisTick,
            repairConsumedNeutrinosThisTick, repairRepairedHullThisTick);
        _cargo.SetMaximumNebula(cargoMaximumNebula);
        _cargo.UpdateRuntime(cargoCurrentMetal, cargoCurrentCarbon, cargoCurrentHydrogen, cargoCurrentSilicon, cargoCurrentNebula,
            cargoNebulaHue, (SubsystemStatus)cargoStatus);
        _resourceMiner.UpdateRuntime(resourceMinerRate, (SubsystemStatus)resourceMinerStatus, resourceMinerConsumedEnergyThisTick,
            resourceMinerConsumedIonsThisTick, resourceMinerConsumedNeutrinosThisTick, resourceMinerMinedMetalThisTick,
            resourceMinerMinedCarbonThisTick, resourceMinerMinedHydrogenThisTick, resourceMinerMinedSiliconThisTick);
    }

    internal void Updated(Cluster cluster, PacketReader reader)
    {
        float energyBatteryCurrent;
        float energyBatteryConsumedThisTick;
        byte energyBatteryStatus;
        float ionBatteryCurrent;
        float ionBatteryConsumedThisTick;
        byte ionBatteryStatus;
        float neutrinoBatteryCurrent;
        float neutrinoBatteryConsumedThisTick;
        byte neutrinoBatteryStatus;
        float energyCellCollectedThisTick;
        byte energyCellStatus;
        float ionCellCollectedThisTick;
        byte ionCellStatus;
        float neutrinoCellCollectedThisTick;
        byte neutrinoCellStatus;
        float hullCurrent;
        byte hullStatus;
        float shieldCurrent;
        byte shieldActive;
        float shieldRate;
        byte shieldStatus;
        float shieldConsumedEnergyThisTick;
        float shieldConsumedIonsThisTick;
        float shieldConsumedNeutrinosThisTick;
        float armorBlockedDirectDamageThisTick;
        float armorBlockedRadiationDamageThisTick;
        byte armorStatus;
        float repairRate;
        byte repairStatus;
        float repairConsumedEnergyThisTick;
        float repairConsumedIonsThisTick;
        float repairConsumedNeutrinosThisTick;
        float repairRepairedHullThisTick;
        float cargoCurrentMetal;
        float cargoCurrentCarbon;
        float cargoCurrentHydrogen;
        float cargoCurrentSilicon;
        float cargoCurrentNebula;
        float cargoNebulaHue;
        byte cargoStatus;
        float resourceMinerRate;
        byte resourceMinerStatus;
        float resourceMinerConsumedEnergyThisTick;
        float resourceMinerConsumedIonsThisTick;
        float resourceMinerConsumedNeutrinosThisTick;
        float resourceMinerMinedMetalThisTick;
        float resourceMinerMinedCarbonThisTick;
        float resourceMinerMinedHydrogenThisTick;
        float resourceMinerMinedSiliconThisTick;
        float environmentHeatThisTick;
        float environmentHeatEnergyCostThisTick;
        float environmentHeatEnergyOverflowThisTick;
        float environmentRadiationThisTick;
        float environmentRadiationDamageBeforeArmorThisTick;
        float environmentArmorBlockedDamageThisTick;
        float environmentHullDamageThisTick;

        if (!Vector.FromReader(reader, out _position) || !Vector.FromReader(reader, out _movement) ||
            !reader.Read(out energyBatteryCurrent) || !reader.Read(out energyBatteryConsumedThisTick) || !reader.Read(out energyBatteryStatus) ||
            !reader.Read(out ionBatteryCurrent) || !reader.Read(out ionBatteryConsumedThisTick) || !reader.Read(out ionBatteryStatus) ||
            !reader.Read(out neutrinoBatteryCurrent) || !reader.Read(out neutrinoBatteryConsumedThisTick) || !reader.Read(out neutrinoBatteryStatus) ||
            !reader.Read(out energyCellCollectedThisTick) || !reader.Read(out energyCellStatus) ||
            !reader.Read(out ionCellCollectedThisTick) || !reader.Read(out ionCellStatus) ||
            !reader.Read(out neutrinoCellCollectedThisTick) || !reader.Read(out neutrinoCellStatus) ||
            !reader.Read(out hullCurrent) || !reader.Read(out hullStatus) ||
            !reader.Read(out shieldCurrent) || !reader.Read(out shieldActive) || !reader.Read(out shieldRate) ||
            !reader.Read(out shieldStatus) || !reader.Read(out shieldConsumedEnergyThisTick) ||
            !reader.Read(out shieldConsumedIonsThisTick) || !reader.Read(out shieldConsumedNeutrinosThisTick) ||
            !reader.Read(out armorBlockedDirectDamageThisTick) || !reader.Read(out armorBlockedRadiationDamageThisTick) ||
            !reader.Read(out armorStatus) ||
            !reader.Read(out repairRate) || !reader.Read(out repairStatus) || !reader.Read(out repairConsumedEnergyThisTick) ||
            !reader.Read(out repairConsumedIonsThisTick) || !reader.Read(out repairConsumedNeutrinosThisTick) ||
            !reader.Read(out repairRepairedHullThisTick) ||
            !reader.Read(out cargoCurrentMetal) || !reader.Read(out cargoCurrentCarbon) || !reader.Read(out cargoCurrentHydrogen) ||
            !reader.Read(out cargoCurrentSilicon) || !reader.Read(out cargoCurrentNebula) || !reader.Read(out cargoNebulaHue) ||
            !reader.Read(out cargoStatus) ||
            !reader.Read(out resourceMinerRate) || !reader.Read(out resourceMinerStatus) ||
            !reader.Read(out resourceMinerConsumedEnergyThisTick) || !reader.Read(out resourceMinerConsumedIonsThisTick) ||
            !reader.Read(out resourceMinerConsumedNeutrinosThisTick) || !reader.Read(out resourceMinerMinedMetalThisTick) ||
            !reader.Read(out resourceMinerMinedCarbonThisTick) || !reader.Read(out resourceMinerMinedHydrogenThisTick) ||
            !reader.Read(out resourceMinerMinedSiliconThisTick) ||
            !reader.Read(out environmentHeatThisTick) || !reader.Read(out environmentHeatEnergyCostThisTick) ||
            !reader.Read(out environmentHeatEnergyOverflowThisTick) || !reader.Read(out environmentRadiationThisTick) ||
            !reader.Read(out environmentRadiationDamageBeforeArmorThisTick) || !reader.Read(out environmentArmorBlockedDamageThisTick) ||
            !reader.Read(out environmentHullDamageThisTick))
            throw new InvalidDataException("Couldn't read ControllableUpdate.");

        _cluster = cluster;
        _energyBattery.UpdateRuntime(energyBatteryCurrent, energyBatteryConsumedThisTick, (SubsystemStatus)energyBatteryStatus);
        _ionBattery.UpdateRuntime(ionBatteryCurrent, ionBatteryConsumedThisTick, (SubsystemStatus)ionBatteryStatus);
        _neutrinoBattery.UpdateRuntime(neutrinoBatteryCurrent, neutrinoBatteryConsumedThisTick, (SubsystemStatus)neutrinoBatteryStatus);
        _energyCell.UpdateRuntime(energyCellCollectedThisTick, (SubsystemStatus)energyCellStatus);
        _ionCell.UpdateRuntime(ionCellCollectedThisTick, (SubsystemStatus)ionCellStatus);
        _neutrinoCell.UpdateRuntime(neutrinoCellCollectedThisTick, (SubsystemStatus)neutrinoCellStatus);
        _hull.UpdateRuntime(hullCurrent, (SubsystemStatus)hullStatus);
        _shield.UpdateRuntime(shieldCurrent, shieldActive != 0, shieldRate, (SubsystemStatus)shieldStatus, shieldConsumedEnergyThisTick,
            shieldConsumedIonsThisTick, shieldConsumedNeutrinosThisTick);
        _armor.UpdateRuntime(armorBlockedDirectDamageThisTick, armorBlockedRadiationDamageThisTick, (SubsystemStatus)armorStatus);
        _repair.UpdateRuntime(repairRate, (SubsystemStatus)repairStatus, repairConsumedEnergyThisTick, repairConsumedIonsThisTick,
            repairConsumedNeutrinosThisTick, repairRepairedHullThisTick);
        _cargo.UpdateRuntime(cargoCurrentMetal, cargoCurrentCarbon, cargoCurrentHydrogen, cargoCurrentSilicon, cargoCurrentNebula, cargoNebulaHue,
            (SubsystemStatus)cargoStatus);
        _resourceMiner.UpdateRuntime(resourceMinerRate, (SubsystemStatus)resourceMinerStatus, resourceMinerConsumedEnergyThisTick,
            resourceMinerConsumedIonsThisTick, resourceMinerConsumedNeutrinosThisTick, resourceMinerMinedMetalThisTick,
            resourceMinerMinedCarbonThisTick, resourceMinerMinedHydrogenThisTick, resourceMinerMinedSiliconThisTick);
        _environmentHeatThisTick = environmentHeatThisTick;
        _environmentHeatEnergyCostThisTick = environmentHeatEnergyCostThisTick;
        _environmentHeatEnergyOverflowThisTick = environmentHeatEnergyOverflowThisTick;
        _environmentRadiationThisTick = environmentRadiationThisTick;
        _environmentRadiationDamageBeforeArmorThisTick = environmentRadiationDamageBeforeArmorThisTick;
        _environmentArmorBlockedDamageThisTick = environmentArmorBlockedDamageThisTick;
        _environmentHullDamageThisTick = environmentHullDamageThisTick;
        ReadRuntime(reader);
        _alive = true;
        EmitRuntimeEvents();
    }

    private protected virtual void ResetRuntime()
    {
        _energyBattery.ResetRuntime();
        _ionBattery.ResetRuntime();
        _neutrinoBattery.ResetRuntime();
        _energyCell.ResetRuntime();
        _ionCell.ResetRuntime();
        _neutrinoCell.ResetRuntime();
        _hull.ResetRuntime();
        _shield.ResetRuntime();
        _armor.ResetRuntime();
        _repair.ResetRuntime();
        _cargo.ResetRuntime();
        _resourceMiner.ResetRuntime();
        _environmentHeatThisTick = 0f;
        _environmentHeatEnergyCostThisTick = 0f;
        _environmentHeatEnergyOverflowThisTick = 0f;
        _environmentRadiationThisTick = 0f;
        _environmentRadiationDamageBeforeArmorThisTick = 0f;
        _environmentArmorBlockedDamageThisTick = 0f;
        _environmentHullDamageThisTick = 0f;
    }

    private protected virtual void ReadRuntime(PacketReader reader)
    {
    }

    private protected virtual void EmitRuntimeEvents()
    {
        PushRuntimeEvent(_energyBattery.CreateRuntimeEvent());
        PushRuntimeEvent(_ionBattery.CreateRuntimeEvent());
        PushRuntimeEvent(_neutrinoBattery.CreateRuntimeEvent());
        PushRuntimeEvent(_energyCell.CreateRuntimeEvent());
        PushRuntimeEvent(_ionCell.CreateRuntimeEvent());
        PushRuntimeEvent(_neutrinoCell.CreateRuntimeEvent());
        PushRuntimeEvent(_hull.CreateRuntimeEvent());
        PushRuntimeEvent(_shield.CreateRuntimeEvent());
        PushRuntimeEvent(_armor.CreateRuntimeEvent());
        PushRuntimeEvent(_repair.CreateRuntimeEvent());
        PushRuntimeEvent(_cargo.CreateRuntimeEvent());
        PushRuntimeEvent(_resourceMiner.CreateRuntimeEvent());
        PushRuntimeEvent(CreateEnvironmentRuntimeEvent());
    }

    private void PushRuntimeEvent(FlattiverseEvent? @event)
    {
        if (@event is not null)
            _cluster.Galaxy.PushEvent(@event);
    }

    private FlattiverseEvent? CreateEnvironmentRuntimeEvent()
    {
        if (_environmentHeatThisTick == 0f && _environmentHeatEnergyCostThisTick == 0f && _environmentHeatEnergyOverflowThisTick == 0f &&
            _environmentRadiationThisTick == 0f && _environmentRadiationDamageBeforeArmorThisTick == 0f &&
            _environmentArmorBlockedDamageThisTick == 0f && _environmentHullDamageThisTick == 0f)
            return null;

        return new EnvironmentDamageEvent(this, _environmentHeatThisTick, _environmentHeatEnergyCostThisTick,
            _environmentHeatEnergyOverflowThisTick, _environmentRadiationThisTick, _environmentRadiationDamageBeforeArmorThisTick,
            _environmentArmorBlockedDamageThisTick, _environmentHullDamageThisTick);
    }
}
