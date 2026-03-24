using Flattiverse.Connector.Events;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// With this class you actually can control a player unit.
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
    private protected BatterySubsystem _energyBattery;
    private protected BatterySubsystem _ionBattery;
    private protected BatterySubsystem _neutrinoBattery;
    private protected EnergyCellSubsystem _energyCell;
    private protected EnergyCellSubsystem _ionCell;
    private protected EnergyCellSubsystem _neutrinoCell;

    internal Controllable(byte id, string name, Cluster cluster, PacketReader reader)
    {
        _cluster = cluster;

        Id = id;
        _name = name;
        
        _active = true;
        _hull = null!;
        _shield = null!;
        _energyBattery = null!;
        _ionBattery = null!;
        _neutrinoBattery = null!;
        _energyCell = null!;
        _ionCell = null!;
        _neutrinoCell = null!;

        if (!Vector.FromReader(reader, out _position) || !Vector.FromReader(reader, out _movement))
            throw new InvalidDataException("Couldn't read controllable.");
    }

    /// <summary>
    /// The name of the controllable.
    /// </summary>
    public string Name => _name;
    
    /// <summary>
    /// Specifies the kind of the controllable.
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
    /// true, if the unit is alive.
    /// </summary>
    public bool Alive => _alive;
    
    /// <summary>
    /// true if this object still can be used. If the unit has been finally closed this is false.
    /// </summary>
    public bool Active => _active;

    /// <summary>
    /// Call this to continue the game with this unit after you are dead or when you have created the unit.
    /// </summary>
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
    /// Call this to suicide (=self destroy).
    /// </summary>
    public async Task Suicide()
    {
        await _cluster.Galaxy.Connection.SendSessionRequestAndGetReply(delegate (ref PacketWriter writer)
        {
            writer.Command = 0x85;
            writer.Write(Id);
        });
    }
    
    /// <summary>
    /// Call this to request closing the unit. The server may keep it alive for a grace period before it is finally removed.
    /// </summary>
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
    /// The gravity this controllable has.
    /// </summary>
    public virtual float Gravity => 0.0012f;
    
    /// <summary>
    /// The size (Radius) of the controllable.
    /// </summary>
    public virtual float Size => 14f;
    
    internal void Deceased()
    {
        _alive = false;
        
        _position = new Vector();
        _movement = new Vector();
        ResetRuntime();
    }

    internal void Updated(PacketReader reader)
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
            !reader.Read(out shieldConsumedIonsThisTick) || !reader.Read(out shieldConsumedNeutrinosThisTick))
            throw new InvalidDataException("Couldan't read ControllableUpdate.");

        _energyBattery.UpdateRuntime(energyBatteryCurrent, energyBatteryConsumedThisTick, (SubsystemStatus)energyBatteryStatus);
        _ionBattery.UpdateRuntime(ionBatteryCurrent, ionBatteryConsumedThisTick, (SubsystemStatus)ionBatteryStatus);
        _neutrinoBattery.UpdateRuntime(neutrinoBatteryCurrent, neutrinoBatteryConsumedThisTick, (SubsystemStatus)neutrinoBatteryStatus);
        _energyCell.UpdateRuntime(energyCellCollectedThisTick, (SubsystemStatus)energyCellStatus);
        _ionCell.UpdateRuntime(ionCellCollectedThisTick, (SubsystemStatus)ionCellStatus);
        _neutrinoCell.UpdateRuntime(neutrinoCellCollectedThisTick, (SubsystemStatus)neutrinoCellStatus);
        _hull.UpdateRuntime(hullCurrent, (SubsystemStatus)hullStatus);
        _shield.UpdateRuntime(shieldCurrent, shieldActive != 0, shieldRate, (SubsystemStatus)shieldStatus, shieldConsumedEnergyThisTick,
            shieldConsumedIonsThisTick, shieldConsumedNeutrinosThisTick);
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
    }

    private void PushRuntimeEvent(FlattiverseEvent? @event)
    {
        if (@event is not null)
            _cluster.Galaxy.PushEvent(@event);
    }
}
