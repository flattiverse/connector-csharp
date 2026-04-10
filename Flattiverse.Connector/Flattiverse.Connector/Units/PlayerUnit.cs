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
    private float _effectiveStructureLoad;
    
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
        _effectiveStructureLoad = 0f;
    }

    internal PlayerUnit(PlayerUnit unit) : base(unit)
    {
        Player = unit.Player;
        ControllableInfo = unit.ControllableInfo;
        _energyBattery = new BatterySubsystemInfo(unit._energyBattery);
        _ionBattery = new BatterySubsystemInfo(unit._ionBattery);
        _neutrinoBattery = new BatterySubsystemInfo(unit._neutrinoBattery);
        _energyCell = new EnergyCellSubsystemInfo(unit._energyCell);
        _ionCell = new EnergyCellSubsystemInfo(unit._ionCell);
        _neutrinoCell = new EnergyCellSubsystemInfo(unit._neutrinoCell);
        _hull = new HullSubsystemInfo(unit._hull);
        _shield = new ShieldSubsystemInfo(unit._shield);
        _armor = new ArmorSubsystemInfo(unit._armor);
        _repair = new RepairSubsystemInfo(unit._repair);
        _cargo = new CargoSubsystemInfo(unit._cargo);
        _resourceMiner = new ResourceMinerSubsystemInfo(unit._resourceMiner);
        _effectiveStructureLoad = unit._effectiveStructureLoad;
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

    /// <summary>
    /// Exact effective structural load of the visible unit as reported by the server.
    /// </summary>
    public float EffectiveStructureLoad
    {
        get { return _effectiveStructureLoad; }
    }

    private protected bool TryGetOwnControllable(out Controllable? controllable)
    {
        if (Player != Cluster.Galaxy.Player)
        {
            controllable = null;
            return false;
        }

        Controllable? ownControllable = Cluster.Galaxy.Controllables[ControllableInfo.Id];

        if (ownControllable is null || ownControllable.Kind != ControllableInfo.Kind)
        {
            controllable = null;
            return false;
        }

        controllable = ownControllable;
        return true;
    }

    internal override void UpdateMovement(PacketReader reader)
    {
        base.UpdateMovement(reader);
    }

    internal override void UpdateState(PacketReader reader)
    {
        base.UpdateState(reader);
        if (!_energyBattery.Update(reader) ||
            !_ionBattery.Update(reader) ||
            !_neutrinoBattery.Update(reader) ||
            !_energyCell.Update(reader) ||
            !_ionCell.Update(reader) ||
            !_neutrinoCell.Update(reader) ||
            !_hull.Update(reader) ||
            !_shield.Update(reader) ||
            !_armor.Update(reader) ||
            !_repair.Update(reader) ||
            !_cargo.Update(reader) ||
            !_resourceMiner.Update(reader) ||
            !reader.Read(out _effectiveStructureLoad))
            throw new InvalidDataException("Couldn't read Unit.");
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
               $"ResourceMinerRate={_resourceMiner.Rate:0.###}, ResourceMinerStatus={_resourceMiner.Status}, ResourceMinerConsumed=({_resourceMiner.ConsumedEnergyThisTick:0.###},{_resourceMiner.ConsumedIonsThisTick:0.###},{_resourceMiner.ConsumedNeutrinosThisTick:0.###}), ResourceMinerMined=({_resourceMiner.MinedMetalThisTick:0.###},{_resourceMiner.MinedCarbonThisTick:0.###},{_resourceMiner.MinedHydrogenThisTick:0.###},{_resourceMiner.MinedSiliconThisTick:0.###}), EffectiveStructureLoad={_effectiveStructureLoad:0.###}";
    }
}
