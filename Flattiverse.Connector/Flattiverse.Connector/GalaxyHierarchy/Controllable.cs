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
    private bool _tierChangePending;
    private SubsystemSlot _tierChangeSlot;
    private byte _tierChangeTargetTier;
    private ushort _remainingTierChangeTicks;
    private float _effectiveStructureLoad;

    private Vector _position;
    private Vector _movement;
    private float _angle;
    private float _angularVelocity;
    private protected HullSubsystem _hull;
    private protected ShieldSubsystem _shield;
    private protected ArmorSubsystem _armor;
    private protected RepairSubsystem _repair;
    private protected CargoSubsystem _cargo;
    private protected ResourceMinerSubsystem _resourceMiner;
    private protected StructureOptimizerSubsystem _structureOptimizer;
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
        _alive = false;
        _tierChangePending = false;
        _tierChangeSlot = 0;
        _tierChangeTargetTier = 0;
        _remainingTierChangeTicks = 0;
        _effectiveStructureLoad = 0f;
        _hull = null!;
        _shield = null!;
        _armor = null!;
        _repair = null!;
        _cargo = null!;
        _resourceMiner = null!;
        _structureOptimizer = null!;
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

        if (!reader.Read(out _angle) || !reader.Read(out _angularVelocity))
            throw new InvalidDataException("Couldn't read controllable angle.");

        if (!reader.Read(out byte alive) ||
            !reader.Read(out byte tierChangePending) ||
            !reader.Read(out byte tierChangeSlot) ||
            !reader.Read(out _tierChangeTargetTier) ||
            !reader.Read(out _remainingTierChangeTicks) ||
            !reader.Read(out _effectiveStructureLoad))
            throw new InvalidDataException("Couldn't read controllable lifecycle state.");

        _alive = alive != 0;
        _tierChangePending = tierChangePending != 0;
        _tierChangeSlot = (SubsystemSlot)tierChangeSlot;
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
    /// The facing angle of the unit.
    /// </summary>
    public float Angle
    {
        get { return _angle; }
    }

    /// <summary>
    /// The angular velocity of the unit.
    /// </summary>
    public float AngularVelocity
    {
        get { return _angularVelocity; }
    }

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
    /// The structure optimizer subsystem of the controllable.
    /// </summary>
    public StructureOptimizerSubsystem StructureOptimizer
    {
        get { return _structureOptimizer; }
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

    internal virtual byte GetTierChangeTargetTier(SubsystemSlot slot, byte currentTier)
    {
        return _tierChangePending && _tierChangeSlot == slot ? _tierChangeTargetTier : currentTier;
    }

    internal virtual ushort GetRemainingTierChangeTicks(SubsystemSlot slot)
    {
        return _tierChangePending && _tierChangeSlot == slot ? _remainingTierChangeTicks : (ushort)0;
    }

    /// <summary>
    /// Effective structural load of the current owner-side configuration as reported by the server.
    /// </summary>
    public float EffectiveStructureLoad
    {
        get { return _effectiveStructureLoad; }
    }

    /// <summary>
    /// Calculates the projected effective structural load after replacing one subsystem slot with a projected raw load.
    /// Use <see cref="SubsystemTierInfo.CalculateRadius(float)" />, <see cref="SubsystemTierInfo.CalculateGravity(float)" />,
    /// <see cref="SubsystemTierInfo.CalculateClassicSpeedLimit(float)" />, <see cref="SubsystemTierInfo.CalculateModernSpeedLimit(float)" />,
    /// and <see cref="SubsystemTierInfo.CalculateEngineEfficiency(float)" /> to derive preview values from the result.
    /// </summary>
    public float CalculateProjectedEffectiveStructureLoad(SubsystemSlot slot, float projectedStructuralLoad)
    {
        float rawStructuralLoad = GetProjectedRawStructuralLoad(slot, projectedStructuralLoad);
        float reductionFactor = 1f - _structureOptimizer.ReductionPercent;
        return rawStructuralLoad * reductionFactor;
    }

    private protected float GetCommonProjectedStructuralLoad(SubsystemSlot slot, float projectedStructuralLoad)
    {
        return StructuralLoadFor(_energyBattery, slot, projectedStructuralLoad) +
            StructuralLoadFor(_ionBattery, slot, projectedStructuralLoad) +
            StructuralLoadFor(_neutrinoBattery, slot, projectedStructuralLoad) +
            StructuralLoadFor(_energyCell, slot, projectedStructuralLoad) +
            StructuralLoadFor(_ionCell, slot, projectedStructuralLoad) +
            StructuralLoadFor(_neutrinoCell, slot, projectedStructuralLoad) +
            StructuralLoadFor(_hull, slot, projectedStructuralLoad) +
            StructuralLoadFor(_shield, slot, projectedStructuralLoad) +
            StructuralLoadFor(_armor, slot, projectedStructuralLoad) +
            StructuralLoadFor(_repair, slot, projectedStructuralLoad) +
            StructuralLoadFor(_cargo, slot, projectedStructuralLoad) +
            StructuralLoadFor(_resourceMiner, slot, projectedStructuralLoad) +
            StructuralLoadFor(_structureOptimizer, slot, projectedStructuralLoad);
    }

    private protected static float StructuralLoadFor(Subsystem subsystem, SubsystemSlot slot, float projectedStructuralLoad)
    {
        return subsystem.Slot == slot ? projectedStructuralLoad : subsystem.CurrentStructuralLoad;
    }

    internal virtual float GetProjectedRawStructuralLoad(SubsystemSlot slot, float projectedStructuralLoad)
    {
        return GetCommonProjectedStructuralLoad(slot, projectedStructuralLoad);
    }

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
            case UnitKind.ModernShipPlayerUnit:
                controllable = new ModernShipControllable(cluster, id, name, reader);
                return true;
            default:
                controllable = null;
                return false;
        }
    }

    /// <summary>
    /// Gravity emitted by the live runtime of this controllable.
    /// </summary>
    public virtual float Gravity
    {
        get { return SubsystemTierInfo.CalculateGravity(EffectiveStructureLoad); }
    }
    
    /// <summary>
    /// Collision radius of the live runtime of this controllable.
    /// </summary>
    public virtual float Size
    {
        get { return SubsystemTierInfo.CalculateRadius(EffectiveStructureLoad); }
    }

    /// <summary>
    /// Maximum movement speed of the live runtime of this controllable.
    /// </summary>
    public virtual float SpeedLimit
    {
        get { return SubsystemTierInfo.CalculateClassicSpeedLimit(EffectiveStructureLoad); }
    }

    /// <summary>
    /// Engine-efficiency multiplier derived from the current effective structural load.
    /// </summary>
    public float EngineEfficiency
    {
        get { return SubsystemTierInfo.CalculateEngineEfficiency(EffectiveStructureLoad); }
    }
    
    internal void Deceased()
    {
        _alive = false;
        
        _position = new Vector();
        _movement = new Vector();
        _angle = 0f;
        _angularVelocity = 0f;
        ResetRuntime();
    }

    internal void Updated(Cluster cluster, PacketReader reader)
    {
        float environmentHeatThisTick;
        float environmentHeatEnergyCostThisTick;
        float environmentHeatEnergyOverflowThisTick;
        float environmentRadiationThisTick;
        float environmentRadiationDamageBeforeArmorThisTick;
        float environmentArmorBlockedDamageThisTick;
        float environmentHullDamageThisTick;

        if (!Vector.FromReader(reader, out _position) || !Vector.FromReader(reader, out _movement) ||
            !reader.Read(out _angle) || !reader.Read(out _angularVelocity) ||
            !reader.Read(out byte alive) || !reader.Read(out byte tierChangePending) || !reader.Read(out byte tierChangeSlot) ||
            !reader.Read(out _tierChangeTargetTier) || !reader.Read(out _remainingTierChangeTicks) ||
            !_energyBattery.Update(reader) ||
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
            !reader.Read(out environmentHeatThisTick) || !reader.Read(out environmentHeatEnergyCostThisTick) ||
            !reader.Read(out environmentHeatEnergyOverflowThisTick) || !reader.Read(out environmentRadiationThisTick) ||
            !reader.Read(out environmentRadiationDamageBeforeArmorThisTick) || !reader.Read(out environmentArmorBlockedDamageThisTick) ||
            !reader.Read(out environmentHullDamageThisTick))
            throw new InvalidDataException("Couldn't read ControllableUpdate.");

        _cluster = cluster;
        _alive = alive != 0;
        _tierChangePending = tierChangePending != 0;
        _tierChangeSlot = (SubsystemSlot)tierChangeSlot;
        _environmentHeatThisTick = environmentHeatThisTick;
        _environmentHeatEnergyCostThisTick = environmentHeatEnergyCostThisTick;
        _environmentHeatEnergyOverflowThisTick = environmentHeatEnergyOverflowThisTick;
        _environmentRadiationThisTick = environmentRadiationThisTick;
        _environmentRadiationDamageBeforeArmorThisTick = environmentRadiationDamageBeforeArmorThisTick;
        _environmentArmorBlockedDamageThisTick = environmentArmorBlockedDamageThisTick;
        _environmentHullDamageThisTick = environmentHullDamageThisTick;
        ReadRuntime(reader);
        EmitRuntimeEvents();
    }

    internal virtual void ApplyCreateRefresh(Controllable refreshed)
    {
        _cluster = refreshed._cluster;
        _active = refreshed._active;
        _alive = refreshed._alive;
        _tierChangePending = refreshed._tierChangePending;
        _tierChangeSlot = refreshed._tierChangeSlot;
        _tierChangeTargetTier = refreshed._tierChangeTargetTier;
        _remainingTierChangeTicks = refreshed._remainingTierChangeTicks;
        _effectiveStructureLoad = refreshed._effectiveStructureLoad;
        _position = refreshed._position;
        _movement = refreshed._movement;
        _angle = refreshed._angle;
        _angularVelocity = refreshed._angularVelocity;
        _environmentHeatThisTick = refreshed._environmentHeatThisTick;
        _environmentHeatEnergyCostThisTick = refreshed._environmentHeatEnergyCostThisTick;
        _environmentHeatEnergyOverflowThisTick = refreshed._environmentHeatEnergyOverflowThisTick;
        _environmentRadiationThisTick = refreshed._environmentRadiationThisTick;
        _environmentRadiationDamageBeforeArmorThisTick = refreshed._environmentRadiationDamageBeforeArmorThisTick;
        _environmentArmorBlockedDamageThisTick = refreshed._environmentArmorBlockedDamageThisTick;
        _environmentHullDamageThisTick = refreshed._environmentHullDamageThisTick;

        _energyBattery.CopyFrom(refreshed._energyBattery);
        _ionBattery.CopyFrom(refreshed._ionBattery);
        _neutrinoBattery.CopyFrom(refreshed._neutrinoBattery);
        _energyCell.CopyFrom(refreshed._energyCell);
        _ionCell.CopyFrom(refreshed._ionCell);
        _neutrinoCell.CopyFrom(refreshed._neutrinoCell);
        _hull.CopyFrom(refreshed._hull);
        _shield.CopyFrom(refreshed._shield);
        _armor.CopyFrom(refreshed._armor);
        _repair.CopyFrom(refreshed._repair);
        _cargo.CopyFrom(refreshed._cargo);
        _resourceMiner.CopyFrom(refreshed._resourceMiner);
        _structureOptimizer.CopyFrom(refreshed._structureOptimizer);
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
