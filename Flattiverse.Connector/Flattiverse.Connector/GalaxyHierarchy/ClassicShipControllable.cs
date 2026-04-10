using Flattiverse.Connector.Events;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Owner-side handle for one registered classic-ship controllable.
/// </summary>
public class ClassicShipControllable : Controllable
{
    private readonly NebulaCollectorSubsystem _nebulaCollector;
    private readonly ClassicShipEngineSubsystem _engine;
    private readonly DynamicShotLauncherSubsystem _shotLauncher;
    private readonly DynamicShotMagazineSubsystem _shotMagazine;
    private readonly DynamicShotFabricatorSubsystem _shotFabricator;
    private readonly DynamicInterceptorLauncherSubsystem _interceptorLauncher;
    private readonly DynamicInterceptorMagazineSubsystem _interceptorMagazine;
    private readonly DynamicInterceptorFabricatorSubsystem _interceptorFabricator;
    private readonly ClassicRailgunSubsystem _railgun;
    private readonly DynamicScannerSubsystem _mainScanner;
    private readonly DynamicScannerSubsystem _secondaryScanner;
    private readonly JumpDriveSubsystem _jumpDrive;
    private readonly string[] _equippedCrystals;

    internal ClassicShipControllable(Cluster cluster, byte id, string name, PacketReader reader) : base(id, name, cluster, reader)
    {
        _energyBattery = new BatterySubsystem(this, "EnergyBattery", reader, SubsystemSlot.EnergyBattery);
        _ionBattery = new BatterySubsystem(this, "IonBattery", reader, SubsystemSlot.IonBattery);
        _neutrinoBattery = new BatterySubsystem(this, "NeutrinoBattery", reader, SubsystemSlot.NeutrinoBattery);
        _energyCell = new EnergyCellSubsystem(this, "EnergyCell", reader, SubsystemSlot.EnergyCell);
        _ionCell = new EnergyCellSubsystem(this, "IonCell", reader, SubsystemSlot.IonCell);
        _neutrinoCell = new EnergyCellSubsystem(this, "NeutrinoCell", reader, SubsystemSlot.NeutrinoCell);
        _hull = new HullSubsystem(this, "Hull", reader, SubsystemSlot.Hull);
        _shield = new ShieldSubsystem(this, "Shield", reader, SubsystemSlot.Shield);
        _armor = new ArmorSubsystem(this, "Armor", reader, SubsystemSlot.Armor);
        _repair = new RepairSubsystem(this, "Repair", reader, SubsystemSlot.Repair);
        _cargo = new CargoSubsystem(this, reader, SubsystemSlot.Cargo);
        _resourceMiner = new ResourceMinerSubsystem(this, reader, SubsystemSlot.ResourceMiner);
        _structureOptimizer = new StructureOptimizerSubsystem(this, reader);
        _nebulaCollector = new NebulaCollectorSubsystem(this, reader, SubsystemSlot.NebulaCollector);
        _mainScanner = new DynamicScannerSubsystem(this, "MainScanner", 0, reader, SubsystemSlot.PrimaryScanner);
        _secondaryScanner = new DynamicScannerSubsystem(this, "SecondaryScanner", 1, reader, SubsystemSlot.SecondaryScanner);
        _engine = new ClassicShipEngineSubsystem(this, reader);
        _shotLauncher = new DynamicShotLauncherSubsystem(this, "ShotLauncher", reader, SubsystemSlot.DynamicShotLauncher);
        _shotMagazine = new DynamicShotMagazineSubsystem(this, "ShotMagazine", reader, SubsystemSlot.DynamicShotMagazine);
        _shotFabricator = new DynamicShotFabricatorSubsystem(this, "ShotFabricator", reader, SubsystemSlot.DynamicShotFabricator);
        _interceptorLauncher = new DynamicInterceptorLauncherSubsystem(this, "InterceptorLauncher", reader,
            SubsystemSlot.DynamicInterceptorLauncher);
        _interceptorMagazine = new DynamicInterceptorMagazineSubsystem(this, "InterceptorMagazine", reader,
            SubsystemSlot.DynamicInterceptorMagazine);
        _interceptorFabricator = new DynamicInterceptorFabricatorSubsystem(this, "InterceptorFabricator", reader,
            SubsystemSlot.DynamicInterceptorFabricator);
        _railgun = new ClassicRailgunSubsystem(this, "Railgun", reader, SubsystemSlot.Railgun);
        _jumpDrive = new JumpDriveSubsystem(this, reader);
        _equippedCrystals = new string[3];

        if (!reader.Read(out _equippedCrystals[0]) ||
            !reader.Read(out _equippedCrystals[1]) ||
            !reader.Read(out _equippedCrystals[2]))
            throw new InvalidDataException("Couldn't read ClassicShipControllable create state.");
    }

    /// <summary>
    /// The engine subsystem of the classic ship.
    /// </summary>
    public ClassicShipEngineSubsystem Engine
    {
        get { return _engine; }
    }

    /// <summary>
    /// The shot launcher subsystem of the classic ship.
    /// </summary>
    public DynamicShotLauncherSubsystem ShotLauncher
    {
        get { return _shotLauncher; }
    }

    /// <summary>
    /// The shot magazine subsystem of the classic ship.
    /// </summary>
    public DynamicShotMagazineSubsystem ShotMagazine
    {
        get { return _shotMagazine; }
    }

    /// <summary>
    /// The shot fabricator subsystem of the classic ship.
    /// </summary>
    public DynamicShotFabricatorSubsystem ShotFabricator
    {
        get { return _shotFabricator; }
    }

    /// <summary>
    /// The interceptor launcher subsystem of the classic ship.
    /// </summary>
    public DynamicInterceptorLauncherSubsystem InterceptorLauncher
    {
        get { return _interceptorLauncher; }
    }

    /// <summary>
    /// The interceptor magazine subsystem of the classic ship.
    /// </summary>
    public DynamicInterceptorMagazineSubsystem InterceptorMagazine
    {
        get { return _interceptorMagazine; }
    }

    /// <summary>
    /// The interceptor fabricator subsystem of the classic ship.
    /// </summary>
    public DynamicInterceptorFabricatorSubsystem InterceptorFabricator
    {
        get { return _interceptorFabricator; }
    }

    /// <summary>
    /// The railgun subsystem of the classic ship.
    /// </summary>
    public ClassicRailgunSubsystem Railgun
    {
        get { return _railgun; }
    }

    /// <summary>
    /// The primary scanner subsystem of the classic ship.
    /// </summary>
    public DynamicScannerSubsystem MainScanner
    {
        get { return _mainScanner; }
    }

    /// <summary>
    /// The secondary scanner subsystem of the classic ship.
    /// </summary>
    public DynamicScannerSubsystem SecondaryScanner
    {
        get { return _secondaryScanner; }
    }

    /// <summary>
    /// The nebula collector subsystem of the classic ship.
    /// </summary>
    public NebulaCollectorSubsystem NebulaCollector
    {
        get { return _nebulaCollector; }
    }

    /// <summary>
    /// The jump-drive subsystem of the classic ship.
    /// </summary>
    public JumpDriveSubsystem JumpDrive
    {
        get { return _jumpDrive; }
    }
    
    /// <inheritdoc/>
    public override float Gravity => 0.0012f;
    
    /// <inheritdoc/>
    public override float Size => 14f;

    /// <summary>
    /// The three equipped crystal names. Empty slots are reported as empty strings.
    /// </summary>
    public IReadOnlyList<string> EquippedCrystals
    {
        get { return _equippedCrystals; }
    }

    internal override float GetProjectedRawStructuralLoad(SubsystemSlot slot, float projectedStructuralLoad)
    {
        return GetCommonProjectedStructuralLoad(slot, projectedStructuralLoad) +
            StructuralLoadFor(_nebulaCollector, slot, projectedStructuralLoad) +
            StructuralLoadFor(_engine, slot, projectedStructuralLoad) +
            StructuralLoadFor(_shotLauncher, slot, projectedStructuralLoad) +
            StructuralLoadFor(_shotMagazine, slot, projectedStructuralLoad) +
            StructuralLoadFor(_shotFabricator, slot, projectedStructuralLoad) +
            StructuralLoadFor(_interceptorLauncher, slot, projectedStructuralLoad) +
            StructuralLoadFor(_interceptorMagazine, slot, projectedStructuralLoad) +
            StructuralLoadFor(_interceptorFabricator, slot, projectedStructuralLoad) +
            StructuralLoadFor(_railgun, slot, projectedStructuralLoad) +
            StructuralLoadFor(_mainScanner, slot, projectedStructuralLoad) +
            StructuralLoadFor(_secondaryScanner, slot, projectedStructuralLoad) +
            StructuralLoadFor(_jumpDrive, slot, projectedStructuralLoad);
    }

    internal override void ApplyCreateRefresh(Controllable refreshed)
    {
        base.ApplyCreateRefresh(refreshed);

        ClassicShipControllable classicRefreshed = (ClassicShipControllable)refreshed;
        _nebulaCollector.CopyFrom(classicRefreshed._nebulaCollector);
        _engine.CopyFrom(classicRefreshed._engine);
        _shotLauncher.CopyFrom(classicRefreshed._shotLauncher);
        _shotMagazine.CopyFrom(classicRefreshed._shotMagazine);
        _shotFabricator.CopyFrom(classicRefreshed._shotFabricator);
        _interceptorLauncher.CopyFrom(classicRefreshed._interceptorLauncher);
        _interceptorMagazine.CopyFrom(classicRefreshed._interceptorMagazine);
        _interceptorFabricator.CopyFrom(classicRefreshed._interceptorFabricator);
        _railgun.CopyFrom(classicRefreshed._railgun);
        _mainScanner.CopyFrom(classicRefreshed._mainScanner);
        _secondaryScanner.CopyFrom(classicRefreshed._secondaryScanner);
        _jumpDrive.CopyFrom(classicRefreshed._jumpDrive);
        _equippedCrystals[0] = classicRefreshed._equippedCrystals[0];
        _equippedCrystals[1] = classicRefreshed._equippedCrystals[1];
        _equippedCrystals[2] = classicRefreshed._equippedCrystals[2];
    }

    private protected override void ResetRuntime()
    {
        base.ResetRuntime();
        _nebulaCollector.ResetRuntime();
        _engine.ResetRuntime();
        _shotLauncher.ResetRuntime();
        _shotMagazine.ResetRuntime();
        _shotFabricator.ResetRuntime();
        _interceptorLauncher.ResetRuntime();
        _interceptorMagazine.ResetRuntime();
        _interceptorFabricator.ResetRuntime();
        _railgun.ResetRuntime();
        _mainScanner.ResetRuntime();
        _secondaryScanner.ResetRuntime();
        _jumpDrive.ResetRuntime();
    }

    private protected override void ReadRuntime(PacketReader reader)
    {
        if (!_nebulaCollector.Update(reader) ||
            !_mainScanner.Update(reader) ||
            !_secondaryScanner.Update(reader) ||
            !_engine.Update(reader) ||
            !_shotLauncher.Update(reader) ||
            !_shotMagazine.Update(reader) ||
            !_shotFabricator.Update(reader) ||
            !_interceptorLauncher.Update(reader) ||
            !_interceptorMagazine.Update(reader) ||
            !_interceptorFabricator.Update(reader) ||
            !_railgun.Update(reader) ||
            !_jumpDrive.Update(reader))
            throw new InvalidDataException("Couldn't read ClassicShipControllable runtime.");
    }

    private protected override void EmitRuntimeEvents()
    {
        base.EmitRuntimeEvents();
        PushRuntimeEvent(_nebulaCollector.CreateRuntimeEvent());
        PushRuntimeEvent(_mainScanner.CreateRuntimeEvent());
        PushRuntimeEvent(_secondaryScanner.CreateRuntimeEvent());
        PushRuntimeEvent(_engine.CreateRuntimeEvent());
        PushRuntimeEvent(_shotLauncher.CreateRuntimeEvent());
        PushRuntimeEvent(_shotMagazine.CreateRuntimeEvent());
        PushRuntimeEvent(_shotFabricator.CreateRuntimeEvent());
        PushRuntimeEvent(_interceptorLauncher.CreateRuntimeEvent());
        PushRuntimeEvent(_interceptorMagazine.CreateRuntimeEvent());
        PushRuntimeEvent(_interceptorFabricator.CreateRuntimeEvent());
        PushRuntimeEvent(_railgun.CreateRuntimeEvent());
    }

    private void PushRuntimeEvent(FlattiverseEvent? @event)
    {
        if (@event is not null)
            _cluster.Galaxy.PushEvent(@event);
    }
}
