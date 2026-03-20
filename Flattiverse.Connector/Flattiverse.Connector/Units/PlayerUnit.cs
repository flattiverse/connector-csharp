using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// Represents a player unit.
/// </summary>
public class PlayerUnit : Unit
{
    /// <summary>
    /// Represents the player which controlls the PlayerUnit.
    /// </summary>
    public readonly Player Player;
    
    /// <summary>
    /// Represents the ControllableInfo of this PlayerUnit.
    /// </summary>
    public readonly ControllableInfo ControllableInfo;
    
    private Vector _position;
    private Vector _movement;
    private readonly BatterySubsystemInfo _energyBattery;
    private readonly BatterySubsystemInfo _ionBattery;
    private readonly BatterySubsystemInfo _neutrinoBattery;
    private readonly EnergyCellSubsystemInfo _energyCell;
    private readonly EnergyCellSubsystemInfo _ionCell;
    private readonly EnergyCellSubsystemInfo _neutrinoCell;
    private readonly HullSubsystemInfo _hull;
    
    internal PlayerUnit(Cluster cluster, string name, PacketReader reader) : base(cluster, name)
    {
        if (!reader.Read(out byte playerId) || !reader.Read(out byte controllableId) || !Vector.FromReader(reader, out _position) || !Vector.FromReader(reader, out _movement))
            throw new InvalidDataException("Couldn't read Unit.");

        Player = cluster.Galaxy.Players[playerId];
        ControllableInfo = Player.ControllableInfos[controllableId];
        _energyBattery = new BatterySubsystemInfo();
        _ionBattery = new BatterySubsystemInfo();
        _neutrinoBattery = new BatterySubsystemInfo();
        _energyCell = new EnergyCellSubsystemInfo();
        _ionCell = new EnergyCellSubsystemInfo();
        _neutrinoCell = new EnergyCellSubsystemInfo();
        _hull = new HullSubsystemInfo();
    }

    internal PlayerUnit(PlayerUnit unit) : base(unit)
    {
        Player = unit.Player;
        ControllableInfo = unit.ControllableInfo;
        
        _position = new Vector(unit._position);
        _movement = new Vector(unit._movement);
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
    }
    
    /// <inheritdoc/>
    public override Vector Position => _position;
    
    /// <inheritdoc/>
    public override Vector Movement => _movement;
        
    /// <inheritdoc/>
    public override float Angle => _movement.Angle;
    
    /// <inheritdoc/>
    public override Mobility Mobility => Mobility.Mobile;

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

    internal override void UpdateMovement(PacketReader reader)
    {
        base.UpdateMovement(reader);
        
        Vector.FromReader(reader, out _position);
        Vector.FromReader(reader, out _movement);
    }

    internal override void UpdateState(PacketReader reader)
    {
        base.UpdateState(reader);

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
            !reader.Read(out byte hullStatus))
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
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{base.ToString()}, Player=\"{Player.Name}\", Controllable=\"{ControllableInfo.Name}\", " +
               $"EnergyBattery={_energyBattery.Current:0.###}/{_energyBattery.Maximum:0.###}({_energyBattery.Status}), EnergyConsumed={_energyBattery.ConsumedThisTick:0.###}, " +
               $"EnergyCellCollected={_energyCell.CollectedThisTick:0.###}({_energyCell.Status}), " +
               $"Hull={_hull.Current:0.###}/{_hull.Maximum:0.###}({_hull.Status})";
    }
}
