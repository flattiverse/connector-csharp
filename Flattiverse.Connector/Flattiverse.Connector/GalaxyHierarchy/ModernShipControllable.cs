using Flattiverse.Connector.Events;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Owner-side handle for one registered modern-ship controllable.
/// </summary>
public class ModernShipControllable : Controllable
{
    private readonly NebulaCollectorSubsystem _nebulaCollector;
    private readonly ModernShipEngineSubsystem[] _engines;
    private readonly StaticScannerSubsystem[] _scanners;
    private readonly StaticShotLauncherSubsystem[] _shotLaunchers;
    private readonly StaticShotMagazineSubsystem[] _shotMagazines;
    private readonly StaticShotFabricatorSubsystem[] _shotFabricators;
    private readonly StaticInterceptorLauncherSubsystem[] _interceptorLaunchers;
    private readonly StaticInterceptorMagazineSubsystem[] _interceptorMagazines;
    private readonly StaticInterceptorFabricatorSubsystem[] _interceptorFabricators;
    private readonly ModernRailgunSubsystem[] _railguns;
    private readonly JumpDriveSubsystem _jumpDrive;
    private readonly string[] _equippedCrystals;

    internal ModernShipControllable(Cluster cluster, byte id, string name, PacketReader reader) : base(id, name, cluster, reader)
    {
        _hull = HullSubsystem.CreateClassicShipHull(this);
        _shield = ShieldSubsystem.CreateClassicShipShield(this);
        _armor = ArmorSubsystem.CreateClassicShipArmor(this);
        _repair = RepairSubsystem.CreateClassicShipRepair(this);
        _cargo = CargoSubsystem.CreateClassicShipCargo(this);
        _resourceMiner = ResourceMinerSubsystem.CreateClassicShipResourceMiner(this);
        _structureOptimizer = new StructureOptimizerSubsystem(this, false, 0f);
        _nebulaCollector = NebulaCollectorSubsystem.CreateClassicShipNebulaCollector(this);
        _energyBattery = BatterySubsystem.CreateClassicShipEnergyBattery(this);
        _ionBattery = BatterySubsystem.CreateMissingBattery(this, "IonBattery", SubsystemSlot.IonBattery);
        _neutrinoBattery = BatterySubsystem.CreateMissingBattery(this, "NeutrinoBattery", SubsystemSlot.NeutrinoBattery);
        _energyCell = EnergyCellSubsystem.CreateClassicShipEnergyCell(this);
        _ionCell = EnergyCellSubsystem.CreateMissingCell(this, "IonCell", SubsystemSlot.IonCell);
        _neutrinoCell = EnergyCellSubsystem.CreateMissingCell(this, "NeutrinoCell", SubsystemSlot.NeutrinoCell);
        _jumpDrive = new JumpDriveSubsystem(this, true);
        _equippedCrystals = new string[3];

        ReadInitialState(reader);

        if (!ReadNebulaCollectorInitialState(reader, out NebulaCollectorState nebulaCollectorState))
            throw new InvalidDataException("Couldn't read ModernShipControllable create state.");

        EngineState[] engineStates = new EngineState[ModernShipGeometry.EngineSlots.Length];
        ScannerState[] scannerStates = new ScannerState[ModernShipGeometry.ScannerSlots.Length];
        LauncherState[] shotLauncherStates = new LauncherState[ModernShipGeometry.ShotLauncherSlots.Length];
        MagazineState[] shotMagazineStates = new MagazineState[ModernShipGeometry.ShotMagazineSlots.Length];
        FabricatorState[] shotFabricatorStates = new FabricatorState[ModernShipGeometry.ShotFabricatorSlots.Length];
        LauncherState[] interceptorLauncherStates = new LauncherState[2];
        MagazineState[] interceptorMagazineStates = new MagazineState[2];
        FabricatorState[] interceptorFabricatorStates = new FabricatorState[2];
        RailgunState[] railgunStates = new RailgunState[ModernShipGeometry.RailgunSlots.Length];

        for (int index = 0; index < scannerStates.Length; index++)
            if (!ReadScannerState(reader, out scannerStates[index]))
                throw new InvalidDataException("Couldn't read ModernShipControllable create state.");

        for (int index = 0; index < engineStates.Length; index++)
            if (!ReadEngineState(reader, out engineStates[index]))
                throw new InvalidDataException("Couldn't read ModernShipControllable create state.");

        for (int index = 0; index < shotLauncherStates.Length; index++)
            if (!ReadLauncherState(reader, out shotLauncherStates[index]) ||
                !ReadMagazineState(reader, out shotMagazineStates[index]) ||
                !ReadFabricatorState(reader, out shotFabricatorStates[index]))
                throw new InvalidDataException("Couldn't read ModernShipControllable create state.");

        for (int index = 0; index < interceptorLauncherStates.Length; index++)
            if (!ReadLauncherState(reader, out interceptorLauncherStates[index]) ||
                !ReadMagazineState(reader, out interceptorMagazineStates[index]) ||
                !ReadFabricatorState(reader, out interceptorFabricatorStates[index]))
                throw new InvalidDataException("Couldn't read ModernShipControllable create state.");

        for (int index = 0; index < railgunStates.Length; index++)
            if (!ReadRailgunState(reader, out railgunStates[index]))
                throw new InvalidDataException("Couldn't read ModernShipControllable create state.");

        if (!reader.Read(out byte jumpDriveExists) ||
            !reader.Read(out byte jumpDriveTier) ||
            !reader.Read(out float jumpDriveEnergyCost) ||
            !reader.Read(out _equippedCrystals[0]) ||
            !reader.Read(out _equippedCrystals[1]) ||
            !reader.Read(out _equippedCrystals[2]))
            throw new InvalidDataException("Couldn't read ModernShipControllable create state.");

        _engines = new ModernShipEngineSubsystem[ModernShipGeometry.EngineSlots.Length];
        _scanners = new StaticScannerSubsystem[ModernShipGeometry.ScannerSlots.Length];
        _shotLaunchers = new StaticShotLauncherSubsystem[ModernShipGeometry.ShotLauncherSlots.Length];
        _shotMagazines = new StaticShotMagazineSubsystem[ModernShipGeometry.ShotMagazineSlots.Length];
        _shotFabricators = new StaticShotFabricatorSubsystem[ModernShipGeometry.ShotFabricatorSlots.Length];
        _interceptorLaunchers = new StaticInterceptorLauncherSubsystem[2];
        _interceptorMagazines = new StaticInterceptorMagazineSubsystem[2];
        _interceptorFabricators = new StaticInterceptorFabricatorSubsystem[2];
        _railguns = new ModernRailgunSubsystem[ModernShipGeometry.RailgunSlots.Length];

        InitializeEngines(engineStates);
        InitializeScanners(scannerStates);
        InitializeShots(shotLauncherStates, shotMagazineStates, shotFabricatorStates);
        InitializeInterceptors(interceptorLauncherStates, interceptorMagazineStates, interceptorFabricatorStates);
        InitializeRailguns(railgunStates);

        _nebulaCollector.SetExists(nebulaCollectorState.Exists);
        _nebulaCollector.SetCapabilities(nebulaCollectorState.MinimumRate, nebulaCollectorState.MaximumRate);
        _nebulaCollector.UpdateRuntime(nebulaCollectorState.Rate, nebulaCollectorState.Status, nebulaCollectorState.ConsumedEnergyThisTick,
            nebulaCollectorState.ConsumedIonsThisTick, nebulaCollectorState.ConsumedNeutrinosThisTick, nebulaCollectorState.CollectedThisTick,
            nebulaCollectorState.CollectedHueThisTick);
        _nebulaCollector.SetReportedTier(nebulaCollectorState.Tier);
        _jumpDrive.SetExists(jumpDriveExists != 0);
        _jumpDrive.SetEnergyCost(jumpDriveEnergyCost);
        _jumpDrive.SetReportedTier(jumpDriveTier);
    }

    public override UnitKind Kind => UnitKind.ModernShipPlayerUnit;
    public override float Gravity => 0.0012f;
    public override float Size => ModernShipGeometry.Radius;
    public NebulaCollectorSubsystem NebulaCollector => _nebulaCollector;
    public JumpDriveSubsystem JumpDrive => _jumpDrive;
    public IReadOnlyList<string> EquippedCrystals => _equippedCrystals;
    public IReadOnlyList<ModernShipEngineSubsystem> Engines => _engines;
    public IReadOnlyList<StaticScannerSubsystem> Scanners => _scanners;
    public IReadOnlyList<StaticShotLauncherSubsystem> ShotLaunchers => _shotLaunchers;
    public IReadOnlyList<StaticShotMagazineSubsystem> ShotMagazines => _shotMagazines;
    public IReadOnlyList<StaticShotFabricatorSubsystem> ShotFabricators => _shotFabricators;
    public IReadOnlyList<ModernRailgunSubsystem> Railguns => _railguns;

    public ModernShipEngineSubsystem EngineN => _engines[0];
    public ModernShipEngineSubsystem EngineNE => _engines[1];
    public ModernShipEngineSubsystem EngineE => _engines[2];
    public ModernShipEngineSubsystem EngineSE => _engines[3];
    public ModernShipEngineSubsystem EngineS => _engines[4];
    public ModernShipEngineSubsystem EngineSW => _engines[5];
    public ModernShipEngineSubsystem EngineW => _engines[6];
    public ModernShipEngineSubsystem EngineNW => _engines[7];

    public StaticScannerSubsystem ScannerN => _scanners[0];
    public StaticScannerSubsystem ScannerNE => _scanners[1];
    public StaticScannerSubsystem ScannerE => _scanners[2];
    public StaticScannerSubsystem ScannerSE => _scanners[3];
    public StaticScannerSubsystem ScannerS => _scanners[4];
    public StaticScannerSubsystem ScannerSW => _scanners[5];
    public StaticScannerSubsystem ScannerW => _scanners[6];
    public StaticScannerSubsystem ScannerNW => _scanners[7];

    public StaticShotLauncherSubsystem ShotLauncherN => _shotLaunchers[0];
    public StaticShotLauncherSubsystem ShotLauncherNE => _shotLaunchers[1];
    public StaticShotLauncherSubsystem ShotLauncherE => _shotLaunchers[2];
    public StaticShotLauncherSubsystem ShotLauncherSE => _shotLaunchers[3];
    public StaticShotLauncherSubsystem ShotLauncherS => _shotLaunchers[4];
    public StaticShotLauncherSubsystem ShotLauncherSW => _shotLaunchers[5];
    public StaticShotLauncherSubsystem ShotLauncherW => _shotLaunchers[6];
    public StaticShotLauncherSubsystem ShotLauncherNW => _shotLaunchers[7];

    public StaticShotMagazineSubsystem ShotMagazineN => _shotMagazines[0];
    public StaticShotMagazineSubsystem ShotMagazineNE => _shotMagazines[1];
    public StaticShotMagazineSubsystem ShotMagazineE => _shotMagazines[2];
    public StaticShotMagazineSubsystem ShotMagazineSE => _shotMagazines[3];
    public StaticShotMagazineSubsystem ShotMagazineS => _shotMagazines[4];
    public StaticShotMagazineSubsystem ShotMagazineSW => _shotMagazines[5];
    public StaticShotMagazineSubsystem ShotMagazineW => _shotMagazines[6];
    public StaticShotMagazineSubsystem ShotMagazineNW => _shotMagazines[7];

    public StaticShotFabricatorSubsystem ShotFabricatorN => _shotFabricators[0];
    public StaticShotFabricatorSubsystem ShotFabricatorNE => _shotFabricators[1];
    public StaticShotFabricatorSubsystem ShotFabricatorE => _shotFabricators[2];
    public StaticShotFabricatorSubsystem ShotFabricatorSE => _shotFabricators[3];
    public StaticShotFabricatorSubsystem ShotFabricatorS => _shotFabricators[4];
    public StaticShotFabricatorSubsystem ShotFabricatorSW => _shotFabricators[5];
    public StaticShotFabricatorSubsystem ShotFabricatorW => _shotFabricators[6];
    public StaticShotFabricatorSubsystem ShotFabricatorNW => _shotFabricators[7];

    public StaticInterceptorLauncherSubsystem InterceptorLauncherE => _interceptorLaunchers[0];
    public StaticInterceptorLauncherSubsystem InterceptorLauncherW => _interceptorLaunchers[1];
    public StaticInterceptorMagazineSubsystem InterceptorMagazineE => _interceptorMagazines[0];
    public StaticInterceptorMagazineSubsystem InterceptorMagazineW => _interceptorMagazines[1];
    public StaticInterceptorFabricatorSubsystem InterceptorFabricatorE => _interceptorFabricators[0];
    public StaticInterceptorFabricatorSubsystem InterceptorFabricatorW => _interceptorFabricators[1];

    public ModernRailgunSubsystem RailgunN => _railguns[0];
    public ModernRailgunSubsystem RailgunNE => _railguns[1];
    public ModernRailgunSubsystem RailgunE => _railguns[2];
    public ModernRailgunSubsystem RailgunSE => _railguns[3];
    public ModernRailgunSubsystem RailgunS => _railguns[4];
    public ModernRailgunSubsystem RailgunSW => _railguns[5];
    public ModernRailgunSubsystem RailgunW => _railguns[6];
    public ModernRailgunSubsystem RailgunNW => _railguns[7];

    internal override float GetProjectedRawStructuralLoad(SubsystemSlot slot, float projectedStructuralLoad)
    {
        float result = GetCommonProjectedStructuralLoad(slot, projectedStructuralLoad) +
            StructuralLoadFor(_nebulaCollector, slot, projectedStructuralLoad) +
            StructuralLoadFor(_jumpDrive, slot, projectedStructuralLoad);

        for (int index = 0; index < _engines.Length; index++)
            result += StructuralLoadFor(_engines[index], slot, projectedStructuralLoad);

        for (int index = 0; index < _scanners.Length; index++)
            result += StructuralLoadFor(_scanners[index], slot, projectedStructuralLoad);

        for (int index = 0; index < _shotLaunchers.Length; index++)
        {
            result += StructuralLoadFor(_shotLaunchers[index], slot, projectedStructuralLoad);
            result += StructuralLoadFor(_shotMagazines[index], slot, projectedStructuralLoad);
            result += StructuralLoadFor(_shotFabricators[index], slot, projectedStructuralLoad);
        }

        for (int index = 0; index < _interceptorLaunchers.Length; index++)
        {
            result += StructuralLoadFor(_interceptorLaunchers[index], slot, projectedStructuralLoad);
            result += StructuralLoadFor(_interceptorMagazines[index], slot, projectedStructuralLoad);
            result += StructuralLoadFor(_interceptorFabricators[index], slot, projectedStructuralLoad);
        }

        for (int index = 0; index < _railguns.Length; index++)
            result += StructuralLoadFor(_railguns[index], slot, projectedStructuralLoad);

        return result;
    }

    private protected override void ResetRuntime()
    {
        base.ResetRuntime();
        _nebulaCollector.ResetRuntime();

        for (int index = 0; index < _engines.Length; index++)
            _engines[index].ResetRuntime();

        for (int index = 0; index < _scanners.Length; index++)
            _scanners[index].ResetRuntime();

        for (int index = 0; index < _shotLaunchers.Length; index++)
        {
            _shotLaunchers[index].ResetRuntime();
            _shotMagazines[index].ResetRuntime();
            _shotFabricators[index].ResetRuntime();
        }

        for (int index = 0; index < _interceptorLaunchers.Length; index++)
        {
            _interceptorLaunchers[index].ResetRuntime();
            _interceptorMagazines[index].ResetRuntime();
            _interceptorFabricators[index].ResetRuntime();
        }

        for (int index = 0; index < _railguns.Length; index++)
            _railguns[index].ResetRuntime();

        _jumpDrive.ResetRuntime();
    }

    private protected override void ReadRuntime(PacketReader reader)
    {
        if (!ReadNebulaCollectorRuntime(reader, out NebulaCollectorState nebulaCollectorState))
            throw new InvalidDataException("Couldn't read ModernShipControllable runtime.");

        _nebulaCollector.UpdateRuntime(nebulaCollectorState.Rate, nebulaCollectorState.Status, nebulaCollectorState.ConsumedEnergyThisTick,
            nebulaCollectorState.ConsumedIonsThisTick, nebulaCollectorState.ConsumedNeutrinosThisTick, nebulaCollectorState.CollectedThisTick,
            nebulaCollectorState.CollectedHueThisTick);

        for (int index = 0; index < _scanners.Length; index++)
        {
            if (!ReadScannerRuntime(reader, out ScannerRuntime scannerRuntime))
                throw new InvalidDataException("Couldn't read ModernShipControllable runtime.");

            _scanners[index].UpdateRuntime(scannerRuntime.Active, scannerRuntime.CurrentWidth, scannerRuntime.CurrentLength,
                scannerRuntime.CurrentAngle, scannerRuntime.TargetWidth, scannerRuntime.TargetLength, scannerRuntime.TargetAngle,
                scannerRuntime.Status, scannerRuntime.ConsumedEnergyThisTick, scannerRuntime.ConsumedIonsThisTick,
                scannerRuntime.ConsumedNeutrinosThisTick);
        }

        for (int index = 0; index < _engines.Length; index++)
        {
            if (!ReadEngineRuntime(reader, out EngineRuntime engineRuntime))
                throw new InvalidDataException("Couldn't read ModernShipControllable runtime.");

            _engines[index].UpdateRuntime(engineRuntime.CurrentThrust, engineRuntime.TargetThrust, engineRuntime.Status,
                engineRuntime.ConsumedEnergyThisTick, engineRuntime.ConsumedIonsThisTick, engineRuntime.ConsumedNeutrinosThisTick);
        }

        for (int index = 0; index < _shotLaunchers.Length; index++)
        {
            if (!ReadLauncherRuntime(reader, out LauncherRuntime launcherRuntime) ||
                !ReadMagazineRuntime(reader, out MagazineRuntime magazineRuntime) ||
                !ReadFabricatorRuntime(reader, out FabricatorRuntime fabricatorRuntime))
                throw new InvalidDataException("Couldn't read ModernShipControllable runtime.");

            _shotLaunchers[index].UpdateRuntime(launcherRuntime.RelativeMovement, launcherRuntime.Ticks, launcherRuntime.Load,
                launcherRuntime.Damage, launcherRuntime.Status, launcherRuntime.ConsumedEnergyThisTick,
                launcherRuntime.ConsumedIonsThisTick, launcherRuntime.ConsumedNeutrinosThisTick);
            _shotMagazines[index].UpdateRuntime(magazineRuntime.CurrentShots, magazineRuntime.Status);
            _shotFabricators[index].UpdateRuntime(fabricatorRuntime.Active, fabricatorRuntime.Rate, fabricatorRuntime.Status,
                fabricatorRuntime.ConsumedEnergyThisTick, fabricatorRuntime.ConsumedIonsThisTick,
                fabricatorRuntime.ConsumedNeutrinosThisTick);
        }

        for (int index = 0; index < _interceptorLaunchers.Length; index++)
        {
            if (!ReadLauncherRuntime(reader, out LauncherRuntime launcherRuntime) ||
                !ReadMagazineRuntime(reader, out MagazineRuntime magazineRuntime) ||
                !ReadFabricatorRuntime(reader, out FabricatorRuntime fabricatorRuntime))
                throw new InvalidDataException("Couldn't read ModernShipControllable runtime.");

            _interceptorLaunchers[index].UpdateRuntime(launcherRuntime.RelativeMovement, launcherRuntime.Ticks, launcherRuntime.Load,
                launcherRuntime.Damage, launcherRuntime.Status, launcherRuntime.ConsumedEnergyThisTick,
                launcherRuntime.ConsumedIonsThisTick, launcherRuntime.ConsumedNeutrinosThisTick);
            _interceptorMagazines[index].UpdateRuntime(magazineRuntime.CurrentShots, magazineRuntime.Status);
            _interceptorFabricators[index].UpdateRuntime(fabricatorRuntime.Active, fabricatorRuntime.Rate, fabricatorRuntime.Status,
                fabricatorRuntime.ConsumedEnergyThisTick, fabricatorRuntime.ConsumedIonsThisTick,
                fabricatorRuntime.ConsumedNeutrinosThisTick);
        }

        for (int index = 0; index < _railguns.Length; index++)
        {
            if (!ReadRailgunRuntime(reader, out RailgunRuntime railgunRuntime))
                throw new InvalidDataException("Couldn't read ModernShipControllable runtime.");

            _railguns[index].UpdateRuntime(railgunRuntime.Direction, railgunRuntime.Status, railgunRuntime.ConsumedEnergyThisTick,
                railgunRuntime.ConsumedIonsThisTick, railgunRuntime.ConsumedNeutrinosThisTick);
        }

        if (!reader.Read(out byte jumpDriveStatus) ||
            !reader.Read(out float jumpDriveConsumedEnergyThisTick) ||
            !reader.Read(out float jumpDriveConsumedIonsThisTick) ||
            !reader.Read(out float jumpDriveConsumedNeutrinosThisTick))
            throw new InvalidDataException("Couldn't read ModernShipControllable runtime.");

        _jumpDrive.UpdateRuntime((SubsystemStatus)jumpDriveStatus, jumpDriveConsumedEnergyThisTick, jumpDriveConsumedIonsThisTick,
            jumpDriveConsumedNeutrinosThisTick);
    }

    private protected override void EmitRuntimeEvents()
    {
        base.EmitRuntimeEvents();
        PushRuntimeEvent(_nebulaCollector.CreateRuntimeEvent());

        for (int index = 0; index < _scanners.Length; index++)
            PushRuntimeEvent(_scanners[index].CreateRuntimeEvent());

        for (int index = 0; index < _engines.Length; index++)
            PushRuntimeEvent(_engines[index].CreateRuntimeEvent());

        for (int index = 0; index < _shotLaunchers.Length; index++)
        {
            PushRuntimeEvent(_shotLaunchers[index].CreateRuntimeEvent());
            PushRuntimeEvent(_shotMagazines[index].CreateRuntimeEvent());
            PushRuntimeEvent(_shotFabricators[index].CreateRuntimeEvent());
        }

        for (int index = 0; index < _interceptorLaunchers.Length; index++)
        {
            PushRuntimeEvent(_interceptorLaunchers[index].CreateRuntimeEvent());
            PushRuntimeEvent(_interceptorMagazines[index].CreateRuntimeEvent());
            PushRuntimeEvent(_interceptorFabricators[index].CreateRuntimeEvent());
        }

        for (int index = 0; index < _railguns.Length; index++)
            PushRuntimeEvent(_railguns[index].CreateRuntimeEvent());
    }

    private void InitializeEngines(EngineState[] engineStates)
    {
        for (int index = 0; index < _engines.Length; index++)
        {
            string suffix = GetSlotSuffix(ModernShipGeometry.EngineSlots[index]);
            ModernShipEngineSubsystem engine = new ModernShipEngineSubsystem(this, $"Engine{suffix}", engineStates[index].Exists,
                ModernShipGeometry.EngineSlots[index]);
            engine.SetCapabilities(engineStates[index].MaximumForwardThrust, engineStates[index].MaximumReverseThrust,
                engineStates[index].MaximumThrustChangePerTick);
            engine.UpdateRuntime(engineStates[index].CurrentThrust, engineStates[index].TargetThrust, engineStates[index].Status,
                engineStates[index].ConsumedEnergyThisTick, engineStates[index].ConsumedIonsThisTick, engineStates[index].ConsumedNeutrinosThisTick);
            engine.SetReportedTier(engineStates[index].Tier);
            _engines[index] = engine;
        }
    }

    private void InitializeScanners(ScannerState[] scannerStates)
    {
        for (int index = 0; index < _scanners.Length; index++)
        {
            string suffix = GetSlotSuffix(ModernShipGeometry.ScannerSlots[index]);
            StaticScannerSubsystem scanner = new StaticScannerSubsystem(this, $"Scanner{suffix}", scannerStates[index].Exists,
                scannerStates[index].MaximumWidth, scannerStates[index].MaximumLength, scannerStates[index].WidthSpeed,
                scannerStates[index].LengthSpeed, scannerStates[index].AngleSpeed, ModernShipGeometry.ScannerSlots[index]);
            scanner.UpdateRuntime(scannerStates[index].Active, scannerStates[index].CurrentWidth, scannerStates[index].CurrentLength,
                scannerStates[index].CurrentAngle, scannerStates[index].TargetWidth, scannerStates[index].TargetLength,
                scannerStates[index].TargetAngle, scannerStates[index].Status, scannerStates[index].ConsumedEnergyThisTick,
                scannerStates[index].ConsumedIonsThisTick, scannerStates[index].ConsumedNeutrinosThisTick);
            scanner.SetReportedTier(scannerStates[index].Tier);
            _scanners[index] = scanner;
        }
    }

    private void InitializeShots(LauncherState[] launcherStates, MagazineState[] magazineStates, FabricatorState[] fabricatorStates)
    {
        for (int index = 0; index < _shotLaunchers.Length; index++)
        {
            string suffix = GetSlotSuffix(ModernShipGeometry.ShotLauncherSlots[index]);
            StaticShotMagazineSubsystem magazine = new StaticShotMagazineSubsystem(this, $"ShotMagazine{suffix}",
                magazineStates[index].Exists, ModernShipGeometry.ShotMagazineSlots[index]);
            StaticShotFabricatorSubsystem fabricator = new StaticShotFabricatorSubsystem(this, $"ShotFabricator{suffix}",
                fabricatorStates[index].Exists, ModernShipGeometry.ShotFabricatorSlots[index]);
            StaticShotLauncherSubsystem launcher = new StaticShotLauncherSubsystem(this, $"ShotLauncher{suffix}",
                launcherStates[index].Exists, ModernShipGeometry.ShotLauncherSlots[index]);

            magazine.SetMaximumShots(magazineStates[index].MaximumShots);
            magazine.UpdateRuntime(magazineStates[index].CurrentShots, magazineStates[index].Status);
            fabricator.SetMaximumRate(fabricatorStates[index].MaximumRate);
            fabricator.UpdateRuntime(fabricatorStates[index].Active, fabricatorStates[index].Rate, fabricatorStates[index].Status,
                fabricatorStates[index].ConsumedEnergyThisTick, fabricatorStates[index].ConsumedIonsThisTick,
                fabricatorStates[index].ConsumedNeutrinosThisTick);
            launcher.SetCapabilities(launcherStates[index].MinimumRelativeMovement, launcherStates[index].MaximumRelativeMovement,
                launcherStates[index].MinimumTicks, launcherStates[index].MaximumTicks, launcherStates[index].MinimumLoad,
                launcherStates[index].MaximumLoad, launcherStates[index].MinimumDamage, launcherStates[index].MaximumDamage);
            launcher.UpdateRuntime(launcherStates[index].RelativeMovement, launcherStates[index].Ticks, launcherStates[index].Load,
                launcherStates[index].Damage, launcherStates[index].Status, launcherStates[index].ConsumedEnergyThisTick,
                launcherStates[index].ConsumedIonsThisTick, launcherStates[index].ConsumedNeutrinosThisTick);
            magazine.SetReportedTier(magazineStates[index].Tier);
            fabricator.SetReportedTier(fabricatorStates[index].Tier);
            launcher.SetReportedTier(launcherStates[index].Tier);

            _shotMagazines[index] = magazine;
            _shotFabricators[index] = fabricator;
            _shotLaunchers[index] = launcher;
        }
    }

    private void InitializeInterceptors(LauncherState[] launcherStates, MagazineState[] magazineStates, FabricatorState[] fabricatorStates)
    {
        SubsystemSlot[] launcherSlots = { SubsystemSlot.StaticInterceptorLauncherE, SubsystemSlot.StaticInterceptorLauncherW };
        SubsystemSlot[] magazineSlots = { SubsystemSlot.StaticInterceptorMagazineE, SubsystemSlot.StaticInterceptorMagazineW };
        SubsystemSlot[] fabricatorSlots = { SubsystemSlot.StaticInterceptorFabricatorE, SubsystemSlot.StaticInterceptorFabricatorW };

        for (int index = 0; index < _interceptorLaunchers.Length; index++)
        {
            string suffix = GetSlotSuffix(launcherSlots[index]);
            StaticInterceptorMagazineSubsystem magazine = new StaticInterceptorMagazineSubsystem(this, $"InterceptorMagazine{suffix}",
                magazineStates[index].Exists, magazineSlots[index]);
            StaticInterceptorFabricatorSubsystem fabricator = new StaticInterceptorFabricatorSubsystem(this,
                $"InterceptorFabricator{suffix}", fabricatorStates[index].Exists, fabricatorSlots[index]);
            StaticInterceptorLauncherSubsystem launcher = new StaticInterceptorLauncherSubsystem(this, $"InterceptorLauncher{suffix}",
                launcherStates[index].Exists, launcherSlots[index]);

            magazine.SetMaximumShots(magazineStates[index].MaximumShots);
            magazine.UpdateRuntime(magazineStates[index].CurrentShots, magazineStates[index].Status);
            fabricator.SetMaximumRate(fabricatorStates[index].MaximumRate);
            fabricator.UpdateRuntime(fabricatorStates[index].Active, fabricatorStates[index].Rate, fabricatorStates[index].Status,
                fabricatorStates[index].ConsumedEnergyThisTick, fabricatorStates[index].ConsumedIonsThisTick,
                fabricatorStates[index].ConsumedNeutrinosThisTick);
            launcher.SetCapabilities(launcherStates[index].MinimumRelativeMovement, launcherStates[index].MaximumRelativeMovement,
                launcherStates[index].MinimumTicks, launcherStates[index].MaximumTicks, launcherStates[index].MinimumLoad,
                launcherStates[index].MaximumLoad, launcherStates[index].MinimumDamage, launcherStates[index].MaximumDamage);
            launcher.UpdateRuntime(launcherStates[index].RelativeMovement, launcherStates[index].Ticks, launcherStates[index].Load,
                launcherStates[index].Damage, launcherStates[index].Status, launcherStates[index].ConsumedEnergyThisTick,
                launcherStates[index].ConsumedIonsThisTick, launcherStates[index].ConsumedNeutrinosThisTick);
            magazine.SetReportedTier(magazineStates[index].Tier);
            fabricator.SetReportedTier(fabricatorStates[index].Tier);
            launcher.SetReportedTier(launcherStates[index].Tier);

            _interceptorMagazines[index] = magazine;
            _interceptorFabricators[index] = fabricator;
            _interceptorLaunchers[index] = launcher;
        }
    }

    private void InitializeRailguns(RailgunState[] railgunStates)
    {
        for (int index = 0; index < _railguns.Length; index++)
        {
            string suffix = GetSlotSuffix(ModernShipGeometry.RailgunSlots[index]);
            ModernRailgunSubsystem railgun = new ModernRailgunSubsystem(this, $"Railgun{suffix}", railgunStates[index].Exists,
                ModernShipGeometry.RailgunSlots[index]);
            railgun.SetCapabilities(railgunStates[index].ProjectileSpeed, railgunStates[index].ProjectileLifetime, railgunStates[index].EnergyCost,
                railgunStates[index].MetalCost);
            railgun.UpdateRuntime(railgunStates[index].Direction, railgunStates[index].Status, railgunStates[index].ConsumedEnergyThisTick,
                railgunStates[index].ConsumedIonsThisTick, railgunStates[index].ConsumedNeutrinosThisTick);
            railgun.SetReportedTier(railgunStates[index].Tier);
            _railguns[index] = railgun;
        }
    }

    private void PushRuntimeEvent(FlattiverseEvent? @event)
    {
        if (@event is not null)
            Cluster.Galaxy.PushEvent(@event);
    }

    private static bool ReadNebulaCollectorInitialState(PacketReader reader, out NebulaCollectorState state)
    {
        state = default;

        if (!reader.Read(out byte exists) ||
            !reader.Read(out byte tier) ||
            !reader.Read(out float minimumRate) ||
            !reader.Read(out float maximumRate) ||
            !reader.Read(out float rate) ||
            !reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick) ||
            !reader.Read(out float collectedThisTick) ||
            !reader.Read(out float collectedHueThisTick))
            return false;

        state.Exists = exists != 0;
        state.Tier = tier;
        state.MinimumRate = minimumRate;
        state.MaximumRate = maximumRate;
        state.Rate = rate;
        state.Status = (SubsystemStatus)status;
        state.ConsumedEnergyThisTick = consumedEnergyThisTick;
        state.ConsumedIonsThisTick = consumedIonsThisTick;
        state.ConsumedNeutrinosThisTick = consumedNeutrinosThisTick;
        state.CollectedThisTick = collectedThisTick;
        state.CollectedHueThisTick = collectedHueThisTick;
        return true;
    }

    private static bool ReadNebulaCollectorRuntime(PacketReader reader, out NebulaCollectorState state)
    {
        state = default;

        if (!reader.Read(out float rate) ||
            !reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick) ||
            !reader.Read(out float collectedThisTick) ||
            !reader.Read(out float collectedHueThisTick))
            return false;

        state.Exists = true;
        state.Rate = rate;
        state.Status = (SubsystemStatus)status;
        state.ConsumedEnergyThisTick = consumedEnergyThisTick;
        state.ConsumedIonsThisTick = consumedIonsThisTick;
        state.ConsumedNeutrinosThisTick = consumedNeutrinosThisTick;
        state.CollectedThisTick = collectedThisTick;
        state.CollectedHueThisTick = collectedHueThisTick;
        return true;
    }

    private static bool ReadScannerState(PacketReader reader, out ScannerState state)
    {
        state = default;

        if (!reader.Read(out byte exists) ||
            !reader.Read(out byte tier) ||
            !reader.Read(out float maximumWidth) ||
            !reader.Read(out float maximumLength) ||
            !reader.Read(out float widthSpeed) ||
            !reader.Read(out float lengthSpeed) ||
            !reader.Read(out float angleSpeed) ||
            !reader.Read(out byte active) ||
            !reader.Read(out float currentWidth) ||
            !reader.Read(out float currentLength) ||
            !reader.Read(out float currentAngle) ||
            !reader.Read(out float targetWidth) ||
            !reader.Read(out float targetLength) ||
            !reader.Read(out float targetAngle) ||
            !reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick))
            return false;

        state.Exists = exists != 0;
        state.Tier = tier;
        state.MaximumWidth = maximumWidth;
        state.MaximumLength = maximumLength;
        state.WidthSpeed = widthSpeed;
        state.LengthSpeed = lengthSpeed;
        state.AngleSpeed = angleSpeed;
        state.Active = active != 0;
        state.CurrentWidth = currentWidth;
        state.CurrentLength = currentLength;
        state.CurrentAngle = currentAngle;
        state.TargetWidth = targetWidth;
        state.TargetLength = targetLength;
        state.TargetAngle = targetAngle;
        state.Status = (SubsystemStatus)status;
        state.ConsumedEnergyThisTick = consumedEnergyThisTick;
        state.ConsumedIonsThisTick = consumedIonsThisTick;
        state.ConsumedNeutrinosThisTick = consumedNeutrinosThisTick;
        return true;
    }

    private static bool ReadScannerRuntime(PacketReader reader, out ScannerRuntime runtime)
    {
        runtime = default;

        if (!reader.Read(out byte active) ||
            !reader.Read(out float currentWidth) ||
            !reader.Read(out float currentLength) ||
            !reader.Read(out float currentAngle) ||
            !reader.Read(out float targetWidth) ||
            !reader.Read(out float targetLength) ||
            !reader.Read(out float targetAngle) ||
            !reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick))
            return false;

        runtime.Active = active != 0;
        runtime.CurrentWidth = currentWidth;
        runtime.CurrentLength = currentLength;
        runtime.CurrentAngle = currentAngle;
        runtime.TargetWidth = targetWidth;
        runtime.TargetLength = targetLength;
        runtime.TargetAngle = targetAngle;
        runtime.Status = (SubsystemStatus)status;
        runtime.ConsumedEnergyThisTick = consumedEnergyThisTick;
        runtime.ConsumedIonsThisTick = consumedIonsThisTick;
        runtime.ConsumedNeutrinosThisTick = consumedNeutrinosThisTick;
        return true;
    }

    private static bool ReadEngineState(PacketReader reader, out EngineState state)
    {
        state = default;

        if (!reader.Read(out byte exists) ||
            !reader.Read(out byte tier) ||
            !reader.Read(out float maximumForwardThrust) ||
            !reader.Read(out float maximumReverseThrust) ||
            !reader.Read(out float maximumThrustChangePerTick) ||
            !reader.Read(out float currentThrust) ||
            !reader.Read(out float targetThrust) ||
            !reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick))
            return false;

        state.Exists = exists != 0;
        state.Tier = tier;
        state.MaximumForwardThrust = maximumForwardThrust;
        state.MaximumReverseThrust = maximumReverseThrust;
        state.MaximumThrustChangePerTick = maximumThrustChangePerTick;
        state.CurrentThrust = currentThrust;
        state.TargetThrust = targetThrust;
        state.Status = (SubsystemStatus)status;
        state.ConsumedEnergyThisTick = consumedEnergyThisTick;
        state.ConsumedIonsThisTick = consumedIonsThisTick;
        state.ConsumedNeutrinosThisTick = consumedNeutrinosThisTick;
        return true;
    }

    private static bool ReadEngineRuntime(PacketReader reader, out EngineRuntime runtime)
    {
        runtime = default;

        if (!reader.Read(out float currentThrust) ||
            !reader.Read(out float targetThrust) ||
            !reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick))
            return false;

        runtime.CurrentThrust = currentThrust;
        runtime.TargetThrust = targetThrust;
        runtime.Status = (SubsystemStatus)status;
        runtime.ConsumedEnergyThisTick = consumedEnergyThisTick;
        runtime.ConsumedIonsThisTick = consumedIonsThisTick;
        runtime.ConsumedNeutrinosThisTick = consumedNeutrinosThisTick;
        return true;
    }

    private static bool ReadLauncherState(PacketReader reader, out LauncherState state)
    {
        state = default;

        if (!reader.Read(out byte exists) ||
            !reader.Read(out byte tier) ||
            !reader.Read(out float minimumRelativeMovement) ||
            !reader.Read(out float maximumRelativeMovement) ||
            !reader.Read(out ushort minimumTicks) ||
            !reader.Read(out ushort maximumTicks) ||
            !reader.Read(out float minimumLoad) ||
            !reader.Read(out float maximumLoad) ||
            !reader.Read(out float minimumDamage) ||
            !reader.Read(out float maximumDamage) ||
            !Vector.FromReader(reader, out Vector relativeMovement) ||
            !reader.Read(out ushort ticks) ||
            !reader.Read(out float load) ||
            !reader.Read(out float damage) ||
            !reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick))
            return false;

        state.Exists = exists != 0;
        state.Tier = tier;
        state.MinimumRelativeMovement = minimumRelativeMovement;
        state.MaximumRelativeMovement = maximumRelativeMovement;
        state.MinimumTicks = minimumTicks;
        state.MaximumTicks = maximumTicks;
        state.MinimumLoad = minimumLoad;
        state.MaximumLoad = maximumLoad;
        state.MinimumDamage = minimumDamage;
        state.MaximumDamage = maximumDamage;
        state.RelativeMovement = relativeMovement;
        state.Ticks = ticks;
        state.Load = load;
        state.Damage = damage;
        state.Status = (SubsystemStatus)status;
        state.ConsumedEnergyThisTick = consumedEnergyThisTick;
        state.ConsumedIonsThisTick = consumedIonsThisTick;
        state.ConsumedNeutrinosThisTick = consumedNeutrinosThisTick;
        return true;
    }

    private static bool ReadLauncherRuntime(PacketReader reader, out LauncherRuntime runtime)
    {
        runtime = default;

        if (!Vector.FromReader(reader, out Vector relativeMovement) ||
            !reader.Read(out ushort ticks) ||
            !reader.Read(out float load) ||
            !reader.Read(out float damage) ||
            !reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick))
            return false;

        runtime.RelativeMovement = relativeMovement;
        runtime.Ticks = ticks;
        runtime.Load = load;
        runtime.Damage = damage;
        runtime.Status = (SubsystemStatus)status;
        runtime.ConsumedEnergyThisTick = consumedEnergyThisTick;
        runtime.ConsumedIonsThisTick = consumedIonsThisTick;
        runtime.ConsumedNeutrinosThisTick = consumedNeutrinosThisTick;
        return true;
    }

    private static bool ReadMagazineState(PacketReader reader, out MagazineState state)
    {
        state = default;

        if (!reader.Read(out byte exists) ||
            !reader.Read(out byte tier) ||
            !reader.Read(out float maximumShots) ||
            !reader.Read(out float currentShots) ||
            !reader.Read(out byte status))
            return false;

        state.Exists = exists != 0;
        state.Tier = tier;
        state.MaximumShots = maximumShots;
        state.CurrentShots = currentShots;
        state.Status = (SubsystemStatus)status;
        return true;
    }

    private static bool ReadMagazineRuntime(PacketReader reader, out MagazineRuntime runtime)
    {
        runtime = default;

        if (!reader.Read(out float currentShots) ||
            !reader.Read(out byte status))
            return false;

        runtime.CurrentShots = currentShots;
        runtime.Status = (SubsystemStatus)status;
        return true;
    }

    private static bool ReadFabricatorState(PacketReader reader, out FabricatorState state)
    {
        state = default;

        if (!reader.Read(out byte exists) ||
            !reader.Read(out byte tier) ||
            !reader.Read(out float minimumRate) ||
            !reader.Read(out float maximumRate) ||
            !reader.Read(out byte active) ||
            !reader.Read(out float rate) ||
            !reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick))
            return false;

        state.Exists = exists != 0;
        state.Tier = tier;
        state.MinimumRate = minimumRate;
        state.MaximumRate = maximumRate;
        state.Active = active != 0;
        state.Rate = rate;
        state.Status = (SubsystemStatus)status;
        state.ConsumedEnergyThisTick = consumedEnergyThisTick;
        state.ConsumedIonsThisTick = consumedIonsThisTick;
        state.ConsumedNeutrinosThisTick = consumedNeutrinosThisTick;
        return true;
    }

    private static bool ReadFabricatorRuntime(PacketReader reader, out FabricatorRuntime runtime)
    {
        runtime = default;

        if (!reader.Read(out byte active) ||
            !reader.Read(out float rate) ||
            !reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick))
            return false;

        runtime.Active = active != 0;
        runtime.Rate = rate;
        runtime.Status = (SubsystemStatus)status;
        runtime.ConsumedEnergyThisTick = consumedEnergyThisTick;
        runtime.ConsumedIonsThisTick = consumedIonsThisTick;
        runtime.ConsumedNeutrinosThisTick = consumedNeutrinosThisTick;
        return true;
    }

    private static bool ReadRailgunState(PacketReader reader, out RailgunState state)
    {
        state = default;

        if (!reader.Read(out byte exists) ||
            !reader.Read(out byte tier) ||
            !reader.Read(out float projectileSpeed) ||
            !reader.Read(out ushort projectileLifetime) ||
            !reader.Read(out float energyCost) ||
            !reader.Read(out float metalCost) ||
            !reader.Read(out byte direction) ||
            !reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick))
            return false;

        state.Exists = exists != 0;
        state.Tier = tier;
        state.ProjectileSpeed = projectileSpeed;
        state.ProjectileLifetime = projectileLifetime;
        state.EnergyCost = energyCost;
        state.MetalCost = metalCost;
        state.Direction = (RailgunDirection)direction;
        state.Status = (SubsystemStatus)status;
        state.ConsumedEnergyThisTick = consumedEnergyThisTick;
        state.ConsumedIonsThisTick = consumedIonsThisTick;
        state.ConsumedNeutrinosThisTick = consumedNeutrinosThisTick;
        return true;
    }

    private static bool ReadRailgunRuntime(PacketReader reader, out RailgunRuntime runtime)
    {
        runtime = default;

        if (!reader.Read(out byte direction) ||
            !reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick))
            return false;

        runtime.Direction = (RailgunDirection)direction;
        runtime.Status = (SubsystemStatus)status;
        runtime.ConsumedEnergyThisTick = consumedEnergyThisTick;
        runtime.ConsumedIonsThisTick = consumedIonsThisTick;
        runtime.ConsumedNeutrinosThisTick = consumedNeutrinosThisTick;
        return true;
    }

    private static string GetSlotSuffix(SubsystemSlot slot)
    {
        switch (slot)
        {
            case SubsystemSlot.ModernEngineN:
            case SubsystemSlot.ModernScannerN:
            case SubsystemSlot.StaticShotLauncherN:
            case SubsystemSlot.StaticShotMagazineN:
            case SubsystemSlot.StaticShotFabricatorN:
            case SubsystemSlot.ModernRailgunN:
                return "N";
            case SubsystemSlot.ModernEngineNE:
            case SubsystemSlot.ModernScannerNE:
            case SubsystemSlot.StaticShotLauncherNE:
            case SubsystemSlot.StaticShotMagazineNE:
            case SubsystemSlot.StaticShotFabricatorNE:
            case SubsystemSlot.ModernRailgunNE:
                return "NE";
            case SubsystemSlot.ModernEngineE:
            case SubsystemSlot.ModernScannerE:
            case SubsystemSlot.StaticShotLauncherE:
            case SubsystemSlot.StaticShotMagazineE:
            case SubsystemSlot.StaticShotFabricatorE:
            case SubsystemSlot.StaticInterceptorLauncherE:
            case SubsystemSlot.StaticInterceptorMagazineE:
            case SubsystemSlot.StaticInterceptorFabricatorE:
            case SubsystemSlot.ModernRailgunE:
                return "E";
            case SubsystemSlot.ModernEngineSE:
            case SubsystemSlot.ModernScannerSE:
            case SubsystemSlot.StaticShotLauncherSE:
            case SubsystemSlot.StaticShotMagazineSE:
            case SubsystemSlot.StaticShotFabricatorSE:
            case SubsystemSlot.ModernRailgunSE:
                return "SE";
            case SubsystemSlot.ModernEngineS:
            case SubsystemSlot.ModernScannerS:
            case SubsystemSlot.StaticShotLauncherS:
            case SubsystemSlot.StaticShotMagazineS:
            case SubsystemSlot.StaticShotFabricatorS:
            case SubsystemSlot.ModernRailgunS:
                return "S";
            case SubsystemSlot.ModernEngineSW:
            case SubsystemSlot.ModernScannerSW:
            case SubsystemSlot.StaticShotLauncherSW:
            case SubsystemSlot.StaticShotMagazineSW:
            case SubsystemSlot.StaticShotFabricatorSW:
            case SubsystemSlot.ModernRailgunSW:
                return "SW";
            case SubsystemSlot.ModernEngineW:
            case SubsystemSlot.ModernScannerW:
            case SubsystemSlot.StaticShotLauncherW:
            case SubsystemSlot.StaticShotMagazineW:
            case SubsystemSlot.StaticShotFabricatorW:
            case SubsystemSlot.StaticInterceptorLauncherW:
            case SubsystemSlot.StaticInterceptorMagazineW:
            case SubsystemSlot.StaticInterceptorFabricatorW:
            case SubsystemSlot.ModernRailgunW:
                return "W";
            case SubsystemSlot.ModernEngineNW:
            case SubsystemSlot.ModernScannerNW:
            case SubsystemSlot.StaticShotLauncherNW:
            case SubsystemSlot.StaticShotMagazineNW:
            case SubsystemSlot.StaticShotFabricatorNW:
            case SubsystemSlot.ModernRailgunNW:
                return "NW";
            default:
                throw new InvalidDataException($"Unsupported modern ship slot {slot}.");
        }
    }

    private struct NebulaCollectorState
    {
        public bool Exists;
        public byte Tier;
        public float MinimumRate;
        public float MaximumRate;
        public float Rate;
        public SubsystemStatus Status;
        public float ConsumedEnergyThisTick;
        public float ConsumedIonsThisTick;
        public float ConsumedNeutrinosThisTick;
        public float CollectedThisTick;
        public float CollectedHueThisTick;
    }

    private struct ScannerState
    {
        public bool Exists;
        public byte Tier;
        public float MaximumWidth;
        public float MaximumLength;
        public float WidthSpeed;
        public float LengthSpeed;
        public float AngleSpeed;
        public bool Active;
        public float CurrentWidth;
        public float CurrentLength;
        public float CurrentAngle;
        public float TargetWidth;
        public float TargetLength;
        public float TargetAngle;
        public SubsystemStatus Status;
        public float ConsumedEnergyThisTick;
        public float ConsumedIonsThisTick;
        public float ConsumedNeutrinosThisTick;
    }

    private struct ScannerRuntime
    {
        public bool Active;
        public float CurrentWidth;
        public float CurrentLength;
        public float CurrentAngle;
        public float TargetWidth;
        public float TargetLength;
        public float TargetAngle;
        public SubsystemStatus Status;
        public float ConsumedEnergyThisTick;
        public float ConsumedIonsThisTick;
        public float ConsumedNeutrinosThisTick;
    }

    private struct EngineState
    {
        public bool Exists;
        public byte Tier;
        public float MaximumForwardThrust;
        public float MaximumReverseThrust;
        public float MaximumThrustChangePerTick;
        public float CurrentThrust;
        public float TargetThrust;
        public SubsystemStatus Status;
        public float ConsumedEnergyThisTick;
        public float ConsumedIonsThisTick;
        public float ConsumedNeutrinosThisTick;
    }

    private struct EngineRuntime
    {
        public float CurrentThrust;
        public float TargetThrust;
        public SubsystemStatus Status;
        public float ConsumedEnergyThisTick;
        public float ConsumedIonsThisTick;
        public float ConsumedNeutrinosThisTick;
    }

    private struct LauncherState
    {
        public bool Exists;
        public byte Tier;
        public float MinimumRelativeMovement;
        public float MaximumRelativeMovement;
        public ushort MinimumTicks;
        public ushort MaximumTicks;
        public float MinimumLoad;
        public float MaximumLoad;
        public float MinimumDamage;
        public float MaximumDamage;
        public Vector RelativeMovement;
        public ushort Ticks;
        public float Load;
        public float Damage;
        public SubsystemStatus Status;
        public float ConsumedEnergyThisTick;
        public float ConsumedIonsThisTick;
        public float ConsumedNeutrinosThisTick;
    }

    private struct LauncherRuntime
    {
        public Vector RelativeMovement;
        public ushort Ticks;
        public float Load;
        public float Damage;
        public SubsystemStatus Status;
        public float ConsumedEnergyThisTick;
        public float ConsumedIonsThisTick;
        public float ConsumedNeutrinosThisTick;
    }

    private struct MagazineState
    {
        public bool Exists;
        public byte Tier;
        public float MaximumShots;
        public float CurrentShots;
        public SubsystemStatus Status;
    }

    private struct MagazineRuntime
    {
        public float CurrentShots;
        public SubsystemStatus Status;
    }

    private struct FabricatorState
    {
        public bool Exists;
        public byte Tier;
        public float MinimumRate;
        public float MaximumRate;
        public bool Active;
        public float Rate;
        public SubsystemStatus Status;
        public float ConsumedEnergyThisTick;
        public float ConsumedIonsThisTick;
        public float ConsumedNeutrinosThisTick;
    }

    private struct FabricatorRuntime
    {
        public bool Active;
        public float Rate;
        public SubsystemStatus Status;
        public float ConsumedEnergyThisTick;
        public float ConsumedIonsThisTick;
        public float ConsumedNeutrinosThisTick;
    }

    private struct RailgunState
    {
        public bool Exists;
        public byte Tier;
        public float ProjectileSpeed;
        public ushort ProjectileLifetime;
        public float EnergyCost;
        public float MetalCost;
        public RailgunDirection Direction;
        public SubsystemStatus Status;
        public float ConsumedEnergyThisTick;
        public float ConsumedIonsThisTick;
        public float ConsumedNeutrinosThisTick;
    }

    private struct RailgunRuntime
    {
        public RailgunDirection Direction;
        public SubsystemStatus Status;
        public float ConsumedEnergyThisTick;
        public float ConsumedIonsThisTick;
        public float ConsumedNeutrinosThisTick;
    }
}
