using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// Visible cluster-side snapshot of one player-owned unit.
/// This is what the local player can currently see in the world, not the owner-side runtime handle used to command
/// the local player's own ships.
/// </summary>
public class PlayerUnit : MobileUnit
{
    /// <summary>
    /// Player who owns the visible unit.
    /// </summary>
    public readonly Player Player;
    
    /// <summary>
    /// Owner-side controllable roster entry associated with this visible unit.
    /// </summary>
    public readonly ControllableInfo ControllableInfo;
    
    private readonly BatterySubsystemInfo _energyBattery;
    private readonly BatterySubsystemInfo _ionBattery;
    private readonly BatterySubsystemInfo _neutrinoBattery;
    private readonly EnergyCellSubsystemInfo _energyCell;
    private readonly EnergyCellSubsystemInfo _ionCell;
    private readonly EnergyCellSubsystemInfo _neutrinoCell;
    private readonly HullSubsystemInfo _hull;
    private readonly ShieldSubsystemInfo _shield;
    private readonly ArmorSubsystemInfo _armor;
    private readonly RepairSubsystemInfo _repair;
    private readonly CargoSubsystemInfo _cargo;
    private readonly ResourceMinerSubsystemInfo _resourceMiner;
    
    internal PlayerUnit(Cluster cluster, string name, PacketReader reader) : base(cluster, name)
    {
        if (!reader.Read(out byte playerId) || !reader.Read(out byte controllableId))
            throw new InvalidDataException("Couldn't read Unit.");

        ReadPositionAndMovement(reader);

        Player = cluster.Galaxy.Players[playerId];
        ControllableInfo = Player.ControllableInfos[controllableId];
        _energyBattery = new BatterySubsystemInfo();
        _ionBattery = new BatterySubsystemInfo();
        _neutrinoBattery = new BatterySubsystemInfo();
        _energyCell = new EnergyCellSubsystemInfo();
        _ionCell = new EnergyCellSubsystemInfo();
        _neutrinoCell = new EnergyCellSubsystemInfo();
        _hull = new HullSubsystemInfo();
        _shield = new ShieldSubsystemInfo();
        _armor = new ArmorSubsystemInfo();
        _repair = new RepairSubsystemInfo();
        _cargo = new CargoSubsystemInfo();
        _resourceMiner = new ResourceMinerSubsystemInfo();
    }

    internal PlayerUnit(PlayerUnit unit) : base(unit)
    {
        Player = unit.Player;
        ControllableInfo = unit.ControllableInfo;
        _energyBattery = new BatterySubsystemInfo();
        _energyBattery.Update(unit._energyBattery.Exists, unit._energyBattery.Maximum, unit._energyBattery.Current, unit._energyBattery.ConsumedThisTick,
            unit._energyBattery.Status);
        _ionBattery = new BatterySubsystemInfo();
        _ionBattery.Update(unit._ionBattery.Exists, unit._ionBattery.Maximum, unit._ionBattery.Current, unit._ionBattery.ConsumedThisTick,
            unit._ionBattery.Status);
        _neutrinoBattery = new BatterySubsystemInfo();
        _neutrinoBattery.Update(unit._neutrinoBattery.Exists, unit._neutrinoBattery.Maximum, unit._neutrinoBattery.Current,
            unit._neutrinoBattery.ConsumedThisTick, unit._neutrinoBattery.Status);
        _energyCell = new EnergyCellSubsystemInfo();
        _energyCell.Update(unit._energyCell.Exists, unit._energyCell.Efficiency, unit._energyCell.CollectedThisTick, unit._energyCell.Status);
        _ionCell = new EnergyCellSubsystemInfo();
        _ionCell.Update(unit._ionCell.Exists, unit._ionCell.Efficiency, unit._ionCell.CollectedThisTick, unit._ionCell.Status);
        _neutrinoCell = new EnergyCellSubsystemInfo();
        _neutrinoCell.Update(unit._neutrinoCell.Exists, unit._neutrinoCell.Efficiency, unit._neutrinoCell.CollectedThisTick,
            unit._neutrinoCell.Status);
        _hull = new HullSubsystemInfo();
        _hull.Update(unit._hull.Exists, unit._hull.Maximum, unit._hull.Current, unit._hull.Status);
        _shield = new ShieldSubsystemInfo();
        _shield.Update(unit._shield.Exists, unit._shield.Maximum, unit._shield.Current, unit._shield.Active, unit._shield.Rate,
            unit._shield.Status, unit._shield.ConsumedEnergyThisTick, unit._shield.ConsumedIonsThisTick, unit._shield.ConsumedNeutrinosThisTick);
        _armor = new ArmorSubsystemInfo();
        _armor.Update(unit._armor.Exists, unit._armor.Reduction, unit._armor.Status, unit._armor.BlockedDirectDamageThisTick,
            unit._armor.BlockedRadiationDamageThisTick);
        _repair = new RepairSubsystemInfo();
        _repair.Update(unit._repair.Exists, unit._repair.MinimumRate, unit._repair.MaximumRate, unit._repair.Rate, unit._repair.Status,
            unit._repair.ConsumedEnergyThisTick, unit._repair.ConsumedIonsThisTick, unit._repair.ConsumedNeutrinosThisTick,
            unit._repair.RepairedHullThisTick);
        _cargo = new CargoSubsystemInfo();
        _cargo.Update(unit._cargo.Exists, unit._cargo.MaximumMetal, unit._cargo.MaximumCarbon, unit._cargo.MaximumHydrogen,
            unit._cargo.MaximumSilicon, unit._cargo.MaximumNebula, unit._cargo.CurrentMetal, unit._cargo.CurrentCarbon,
            unit._cargo.CurrentHydrogen, unit._cargo.CurrentSilicon, unit._cargo.CurrentNebula, unit._cargo.NebulaHue, unit._cargo.Status);
        _resourceMiner = new ResourceMinerSubsystemInfo();
        _resourceMiner.Update(unit._resourceMiner.Exists, unit._resourceMiner.MinimumRate, unit._resourceMiner.MaximumRate,
            unit._resourceMiner.Rate, unit._resourceMiner.Status, unit._resourceMiner.ConsumedEnergyThisTick,
            unit._resourceMiner.ConsumedIonsThisTick, unit._resourceMiner.ConsumedNeutrinosThisTick,
            unit._resourceMiner.MinedMetalThisTick, unit._resourceMiner.MinedCarbonThisTick, unit._resourceMiner.MinedHydrogenThisTick,
            unit._resourceMiner.MinedSiliconThisTick);
    }
    
    /// <inheritdoc/>
    public override Team? Team => Player.Team;

    /// <summary>
    /// Visible snapshot of the energy battery subsystem.
    /// </summary>
    public BatterySubsystemInfo EnergyBattery
    {
        get { return _energyBattery; }
    }

    /// <summary>
    /// Visible snapshot of the ion battery subsystem.
    /// </summary>
    public BatterySubsystemInfo IonBattery
    {
        get { return _ionBattery; }
    }

    /// <summary>
    /// Visible snapshot of the neutrino battery subsystem.
    /// </summary>
    public BatterySubsystemInfo NeutrinoBattery
    {
        get { return _neutrinoBattery; }
    }

    /// <summary>
    /// Visible snapshot of the energy cell subsystem.
    /// </summary>
    public EnergyCellSubsystemInfo EnergyCell
    {
        get { return _energyCell; }
    }

    /// <summary>
    /// Visible snapshot of the ion cell subsystem.
    /// </summary>
    public EnergyCellSubsystemInfo IonCell
    {
        get { return _ionCell; }
    }

    /// <summary>
    /// Visible snapshot of the neutrino cell subsystem.
    /// </summary>
    public EnergyCellSubsystemInfo NeutrinoCell
    {
        get { return _neutrinoCell; }
    }

    /// <summary>
    /// Visible snapshot of the hull subsystem.
    /// </summary>
    public HullSubsystemInfo Hull
    {
        get { return _hull; }
    }

    /// <summary>
    /// Visible snapshot of the shield subsystem.
    /// </summary>
    public ShieldSubsystemInfo Shield
    {
        get { return _shield; }
    }

    /// <summary>
    /// Visible snapshot of the armor subsystem.
    /// </summary>
    public ArmorSubsystemInfo Armor
    {
        get { return _armor; }
    }

    /// <summary>
    /// Visible snapshot of the repair subsystem.
    /// </summary>
    public RepairSubsystemInfo Repair
    {
        get { return _repair; }
    }

    /// <summary>
    /// Visible snapshot of the cargo subsystem.
    /// </summary>
    public CargoSubsystemInfo Cargo
    {
        get { return _cargo; }
    }

    /// <summary>
    /// Visible snapshot of the resource miner subsystem.
    /// </summary>
    public ResourceMinerSubsystemInfo ResourceMiner
    {
        get { return _resourceMiner; }
    }

    internal override void UpdateMovement(PacketReader reader)
    {
        base.UpdateMovement(reader);
    }

    internal override void UpdateState(PacketReader reader)
    {
        base.UpdateState(reader);

        byte armorExists;
        float armorReduction;
        byte armorStatus;
        float armorBlockedDirectDamageThisTick;
        float armorBlockedRadiationDamageThisTick;
        byte repairExists;
        float repairMinimumRate;
        float repairMaximumRate;
        float repairRate;
        byte repairStatus;
        float repairConsumedEnergyThisTick;
        float repairConsumedIonsThisTick;
        float repairConsumedNeutrinosThisTick;
        float repairRepairedHullThisTick;
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
            !reader.Read(out armorExists) ||
            !reader.Read(out armorReduction) ||
            !reader.Read(out armorStatus) ||
            !reader.Read(out armorBlockedDirectDamageThisTick) ||
            !reader.Read(out armorBlockedRadiationDamageThisTick) ||
            !reader.Read(out repairExists) ||
            !reader.Read(out repairMinimumRate) ||
            !reader.Read(out repairMaximumRate) ||
            !reader.Read(out repairRate) ||
            !reader.Read(out repairStatus) ||
            !reader.Read(out repairConsumedEnergyThisTick) ||
            !reader.Read(out repairConsumedIonsThisTick) ||
            !reader.Read(out repairConsumedNeutrinosThisTick) ||
            !reader.Read(out repairRepairedHullThisTick) ||
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
            throw new InvalidDataException("Couldn't read Unit.");

        _energyBattery.Update(energyBatteryExists != 0, energyBatteryMaximum, energyBatteryCurrent, energyBatteryConsumedThisTick,
            (SubsystemStatus)energyBatteryStatus);
        _ionBattery.Update(ionBatteryExists != 0, ionBatteryMaximum, ionBatteryCurrent, ionBatteryConsumedThisTick,
            (SubsystemStatus)ionBatteryStatus);
        _neutrinoBattery.Update(neutrinoBatteryExists != 0, neutrinoBatteryMaximum, neutrinoBatteryCurrent, neutrinoBatteryConsumedThisTick,
            (SubsystemStatus)neutrinoBatteryStatus);
        _energyCell.Update(energyCellExists != 0, energyCellEfficiency, energyCellCollectedThisTick, (SubsystemStatus)energyCellStatus);
        _ionCell.Update(ionCellExists != 0, ionCellEfficiency, ionCellCollectedThisTick, (SubsystemStatus)ionCellStatus);
        _neutrinoCell.Update(neutrinoCellExists != 0, neutrinoCellEfficiency, neutrinoCellCollectedThisTick, (SubsystemStatus)neutrinoCellStatus);
        _hull.Update(hullExists != 0, hullMaximum, hullCurrent, (SubsystemStatus)hullStatus);
        _shield.Update(shieldExists != 0, shieldMaximum, shieldCurrent, shieldActive != 0, shieldRate, (SubsystemStatus)shieldStatus,
            shieldConsumedEnergyThisTick, shieldConsumedIonsThisTick, shieldConsumedNeutrinosThisTick);
        _armor.Update(armorExists != 0, armorReduction, (SubsystemStatus)armorStatus, armorBlockedDirectDamageThisTick,
            armorBlockedRadiationDamageThisTick);
        _repair.Update(repairExists != 0, repairMinimumRate, repairMaximumRate, repairRate, (SubsystemStatus)repairStatus,
            repairConsumedEnergyThisTick, repairConsumedIonsThisTick, repairConsumedNeutrinosThisTick, repairRepairedHullThisTick);
        _cargo.Update(cargoExists != 0, cargoMaximumMetal, cargoMaximumCarbon, cargoMaximumHydrogen, cargoMaximumSilicon,
            cargoMaximumNebula, cargoCurrentMetal, cargoCurrentCarbon, cargoCurrentHydrogen, cargoCurrentSilicon, cargoCurrentNebula,
            cargoNebulaHue, (SubsystemStatus)cargoStatus);
        _resourceMiner.Update(resourceMinerExists != 0, resourceMinerMinimumRate, resourceMinerMaximumRate, resourceMinerRate,
            (SubsystemStatus)resourceMinerStatus, resourceMinerConsumedEnergyThisTick, resourceMinerConsumedIonsThisTick,
            resourceMinerConsumedNeutrinosThisTick, resourceMinerMinedMetalThisTick, resourceMinerMinedCarbonThisTick,
            resourceMinerMinedHydrogenThisTick, resourceMinerMinedSiliconThisTick);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{base.ToString()}, Player=\"{Player.Name}\", Controllable=\"{ControllableInfo.Name}\", " +
               $"EnergyBattery={_energyBattery.Current:0.###}/{_energyBattery.Maximum:0.###}({_energyBattery.Status}), EnergyConsumed={_energyBattery.ConsumedThisTick:0.###}, " +
               $"EnergyCellCollected={_energyCell.CollectedThisTick:0.###}({_energyCell.Status}), " +
               $"Hull={_hull.Current:0.###}/{_hull.Maximum:0.###}({_hull.Status}), " +
               $"Shield={_shield.Current:0.###}/{_shield.Maximum:0.###}({_shield.Status}), ShieldActive={_shield.Active}, ShieldRate={_shield.Rate:0.###}, ShieldConsumed=({_shield.ConsumedEnergyThisTick:0.###},{_shield.ConsumedIonsThisTick:0.###},{_shield.ConsumedNeutrinosThisTick:0.###}), " +
               $"ArmorReduction={_armor.Reduction:0.###}, ArmorBlocked=({_armor.BlockedDirectDamageThisTick:0.###},{_armor.BlockedRadiationDamageThisTick:0.###}), " +
               $"RepairRate={_repair.Rate:0.###}, RepairStatus={_repair.Status}, RepairConsumed=({_repair.ConsumedEnergyThisTick:0.###},{_repair.ConsumedIonsThisTick:0.###},{_repair.ConsumedNeutrinosThisTick:0.###}), RepairHull={_repair.RepairedHullThisTick:0.###}, " +
               $"Cargo=({_cargo.CurrentMetal:0.###}/{_cargo.MaximumMetal:0.###},{_cargo.CurrentCarbon:0.###}/{_cargo.MaximumCarbon:0.###},{_cargo.CurrentHydrogen:0.###}/{_cargo.MaximumHydrogen:0.###},{_cargo.CurrentSilicon:0.###}/{_cargo.MaximumSilicon:0.###},Nebula={_cargo.CurrentNebula:0.###}/{_cargo.MaximumNebula:0.###},Hue={_cargo.NebulaHue:0.###})({_cargo.Status}), " +
               $"ResourceMinerRate={_resourceMiner.Rate:0.###}, ResourceMinerStatus={_resourceMiner.Status}, ResourceMinerConsumed=({_resourceMiner.ConsumedEnergyThisTick:0.###},{_resourceMiner.ConsumedIonsThisTick:0.###},{_resourceMiner.ConsumedNeutrinosThisTick:0.###}), ResourceMinerMined=({_resourceMiner.MinedMetalThisTick:0.###},{_resourceMiner.MinedCarbonThisTick:0.###},{_resourceMiner.MinedHydrogenThisTick:0.###},{_resourceMiner.MinedSiliconThisTick:0.###})";
    }
}
