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

        _engine = new ClassicShipEngineSubsystem(this);
        _shotLauncher = new DynamicShotLauncherSubsystem(this, "ShotLauncher", true, SubsystemSlot.DynamicShotLauncher);
        _shotMagazine = new DynamicShotMagazineSubsystem(this, "ShotMagazine", true, SubsystemSlot.DynamicShotMagazine);
        _shotFabricator = new DynamicShotFabricatorSubsystem(this, "ShotFabricator", true, SubsystemSlot.DynamicShotFabricator);
        _interceptorLauncher = new DynamicInterceptorLauncherSubsystem(this, "InterceptorLauncher", true,
            SubsystemSlot.DynamicInterceptorLauncher);
        _interceptorMagazine = new DynamicInterceptorMagazineSubsystem(this, "InterceptorMagazine", true,
            SubsystemSlot.DynamicInterceptorMagazine);
        _interceptorFabricator = new DynamicInterceptorFabricatorSubsystem(this, "InterceptorFabricator", true,
            SubsystemSlot.DynamicInterceptorFabricator);
        _railgun = new ClassicRailgunSubsystem(this, "Railgun", true, SubsystemSlot.Railgun);
        _mainScanner = DynamicScannerSubsystem.CreateClassicShipPrimaryScanner(this);
        _secondaryScanner = DynamicScannerSubsystem.CreateClassicShipSecondaryScanner(this);
        _jumpDrive = new JumpDriveSubsystem(this, true);
        _equippedCrystals = new string[3];

        ReadInitialState(reader);

        if (!ReadNebulaCollectorInitialState(reader) ||
            !ReadScannerInitialState(reader, _mainScanner) ||
            !ReadScannerInitialState(reader, _secondaryScanner) ||
            !ReadEngineInitialState(reader) ||
            !ReadShotLauncherInitialState(reader) ||
            !ReadShotMagazineInitialState(reader) ||
            !ReadShotFabricatorInitialState(reader) ||
            !ReadInterceptorLauncherInitialState(reader) ||
            !ReadInterceptorMagazineInitialState(reader) ||
            !ReadInterceptorFabricatorInitialState(reader) ||
            !ReadRailgunInitialState(reader) ||
            !ReadJumpDriveInitialState(reader) ||
            !reader.Read(out _equippedCrystals[0]) ||
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

    private bool ReadNebulaCollectorInitialState(PacketReader reader)
    {
        if (!reader.Read(out byte exists))
            return false;

        _nebulaCollector.SetExists(exists != 0);

        if (exists == 0)
            return true;

        if (!reader.Read(out byte tier) ||
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

        _nebulaCollector.SetCapabilities(minimumRate, maximumRate);
        _nebulaCollector.UpdateRuntime(rate, (SubsystemStatus)status, consumedEnergyThisTick, consumedIonsThisTick,
            consumedNeutrinosThisTick, collectedThisTick, collectedHueThisTick);
        _nebulaCollector.SetReportedTier(tier);
        return true;
    }

    private static bool ReadScannerInitialState(PacketReader reader, DynamicScannerSubsystem scanner)
    {
        if (!reader.Read(out byte exists))
            return false;

        scanner.SetExists(exists != 0);

        if (exists == 0)
            return true;

        if (!reader.Read(out byte tier) ||
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

        scanner.SetCapabilities(maximumWidth, maximumLength, widthSpeed, lengthSpeed, angleSpeed);
        scanner.UpdateRuntime(active != 0, currentWidth, currentLength, currentAngle, targetWidth, targetLength, targetAngle,
            (SubsystemStatus)status, consumedEnergyThisTick, consumedIonsThisTick, consumedNeutrinosThisTick);
        scanner.SetReportedTier(tier);
        return true;
    }

    private bool ReadEngineInitialState(PacketReader reader)
    {
        if (!reader.Read(out byte exists))
            return false;

        _engine.SetExists(exists != 0);

        if (exists == 0)
            return true;

        if (!reader.Read(out byte tier) ||
            !reader.Read(out float maximum) ||
            !Vector.FromReader(reader, out Vector? current) ||
            !Vector.FromReader(reader, out Vector? target) ||
            !reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick))
            return false;

        _engine.SetMaximum(maximum);
        _engine.UpdateRuntime(current, target, (SubsystemStatus)status, consumedEnergyThisTick, consumedIonsThisTick,
            consumedNeutrinosThisTick);
        _engine.SetReportedTier(tier);
        return true;
    }

    private bool ReadShotLauncherInitialState(PacketReader reader)
    {
        if (!reader.Read(out byte exists))
            return false;

        _shotLauncher.SetExists(exists != 0);

        if (exists == 0)
            return true;

        if (!reader.Read(out byte tier) ||
            !reader.Read(out float minimumRelativeMovement) ||
            !reader.Read(out float maximumRelativeMovement) ||
            !reader.Read(out ushort minimumTicks) ||
            !reader.Read(out ushort maximumTicks) ||
            !reader.Read(out float minimumLoad) ||
            !reader.Read(out float maximumLoad) ||
            !reader.Read(out float minimumDamage) ||
            !reader.Read(out float maximumDamage) ||
            !Vector.FromReader(reader, out Vector? relativeMovement) ||
            !reader.Read(out ushort ticks) ||
            !reader.Read(out float load) ||
            !reader.Read(out float damage) ||
            !reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick))
            return false;

        _shotLauncher.SetCapabilities(minimumRelativeMovement, maximumRelativeMovement, minimumTicks, maximumTicks, minimumLoad,
            maximumLoad, minimumDamage, maximumDamage);
        _shotLauncher.UpdateRuntime(relativeMovement, ticks, load, damage, (SubsystemStatus)status, consumedEnergyThisTick,
            consumedIonsThisTick, consumedNeutrinosThisTick);
        _shotLauncher.SetReportedTier(tier);
        return true;
    }

    private bool ReadShotMagazineInitialState(PacketReader reader)
    {
        if (!reader.Read(out byte exists))
            return false;

        _shotMagazine.SetExists(exists != 0);

        if (exists == 0)
            return true;

        if (!reader.Read(out byte tier) ||
            !reader.Read(out float maximumShots) ||
            !reader.Read(out float currentShots) ||
            !reader.Read(out byte status))
            return false;

        _shotMagazine.SetMaximumShots(maximumShots);
        _shotMagazine.UpdateRuntime(currentShots, (SubsystemStatus)status);
        _shotMagazine.SetReportedTier(tier);
        return true;
    }

    private bool ReadShotFabricatorInitialState(PacketReader reader)
    {
        if (!reader.Read(out byte exists))
            return false;

        _shotFabricator.SetExists(exists != 0);

        if (exists == 0)
            return true;

        if (!reader.Read(out byte tier) ||
            !reader.Read(out float maximumRate) ||
            !reader.Read(out byte active) ||
            !reader.Read(out float rate) ||
            !reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick))
            return false;

        _shotFabricator.SetMaximumRate(maximumRate);
        _shotFabricator.UpdateRuntime(active != 0, rate, (SubsystemStatus)status, consumedEnergyThisTick, consumedIonsThisTick,
            consumedNeutrinosThisTick);
        _shotFabricator.SetReportedTier(tier);
        return true;
    }

    private bool ReadInterceptorLauncherInitialState(PacketReader reader)
    {
        if (!reader.Read(out byte exists))
            return false;

        _interceptorLauncher.SetExists(exists != 0);

        if (exists == 0)
            return true;

        if (!reader.Read(out byte tier) ||
            !reader.Read(out float minimumRelativeMovement) ||
            !reader.Read(out float maximumRelativeMovement) ||
            !reader.Read(out ushort minimumTicks) ||
            !reader.Read(out ushort maximumTicks) ||
            !reader.Read(out float minimumLoad) ||
            !reader.Read(out float maximumLoad) ||
            !reader.Read(out float minimumDamage) ||
            !reader.Read(out float maximumDamage) ||
            !Vector.FromReader(reader, out Vector? relativeMovement) ||
            !reader.Read(out ushort ticks) ||
            !reader.Read(out float load) ||
            !reader.Read(out float damage) ||
            !reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick))
            return false;

        _interceptorLauncher.SetCapabilities(minimumRelativeMovement, maximumRelativeMovement, minimumTicks, maximumTicks,
            minimumLoad, maximumLoad, minimumDamage, maximumDamage);
        _interceptorLauncher.UpdateRuntime(relativeMovement, ticks, load, damage, (SubsystemStatus)status, consumedEnergyThisTick,
            consumedIonsThisTick, consumedNeutrinosThisTick);
        _interceptorLauncher.SetReportedTier(tier);
        return true;
    }

    private bool ReadInterceptorMagazineInitialState(PacketReader reader)
    {
        if (!reader.Read(out byte exists))
            return false;

        _interceptorMagazine.SetExists(exists != 0);

        if (exists == 0)
            return true;

        if (!reader.Read(out byte tier) ||
            !reader.Read(out float maximumShots) ||
            !reader.Read(out float currentShots) ||
            !reader.Read(out byte status))
            return false;

        _interceptorMagazine.SetMaximumShots(maximumShots);
        _interceptorMagazine.UpdateRuntime(currentShots, (SubsystemStatus)status);
        _interceptorMagazine.SetReportedTier(tier);
        return true;
    }

    private bool ReadInterceptorFabricatorInitialState(PacketReader reader)
    {
        if (!reader.Read(out byte exists))
            return false;

        _interceptorFabricator.SetExists(exists != 0);

        if (exists == 0)
            return true;

        if (!reader.Read(out byte tier) ||
            !reader.Read(out float maximumRate) ||
            !reader.Read(out byte active) ||
            !reader.Read(out float rate) ||
            !reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick))
            return false;

        _interceptorFabricator.SetMaximumRate(maximumRate);
        _interceptorFabricator.UpdateRuntime(active != 0, rate, (SubsystemStatus)status, consumedEnergyThisTick,
            consumedIonsThisTick, consumedNeutrinosThisTick);
        _interceptorFabricator.SetReportedTier(tier);
        return true;
    }

    private bool ReadRailgunInitialState(PacketReader reader)
    {
        if (!reader.Read(out byte exists))
            return false;

        _railgun.SetExists(exists != 0);

        if (exists == 0)
            return true;

        if (!reader.Read(out byte tier) ||
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

        _railgun.SetCapabilities(projectileSpeed, projectileLifetime, energyCost, metalCost);
        _railgun.UpdateRuntime((RailgunDirection)direction, (SubsystemStatus)status, consumedEnergyThisTick,
            consumedIonsThisTick, consumedNeutrinosThisTick);
        _railgun.SetReportedTier(tier);
        return true;
    }

    private bool ReadJumpDriveInitialState(PacketReader reader)
    {
        if (!reader.Read(out byte exists))
            return false;

        _jumpDrive.SetExists(exists != 0);

        if (exists == 0)
            return true;

        if (!reader.Read(out byte tier) ||
            !reader.Read(out float energyCost))
            return false;

        _jumpDrive.SetEnergyCost(energyCost);
        _jumpDrive.SetReportedTier(tier);
        return true;
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
        if (!ReadNebulaCollectorRuntime(reader) ||
            !ReadScannerRuntime(reader, _mainScanner) ||
            !ReadScannerRuntime(reader, _secondaryScanner) ||
            !ReadEngineRuntime(reader) ||
            !ReadShotLauncherRuntime(reader) ||
            !ReadShotMagazineRuntime(reader) ||
            !ReadShotFabricatorRuntime(reader) ||
            !ReadInterceptorLauncherRuntime(reader) ||
            !ReadInterceptorMagazineRuntime(reader) ||
            !ReadInterceptorFabricatorRuntime(reader) ||
            !ReadRailgunRuntime(reader) ||
            !ReadJumpDriveRuntime(reader))
            throw new InvalidDataException("Couldn't read ClassicShipControllable runtime.");
    }

    private bool ReadNebulaCollectorRuntime(PacketReader reader)
    {
        if (!_nebulaCollector.Exists)
            return true;

        if (!reader.Read(out float rate) ||
            !reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick) ||
            !reader.Read(out float collectedThisTick) ||
            !reader.Read(out float collectedHueThisTick))
            return false;

        _nebulaCollector.UpdateRuntime(rate, (SubsystemStatus)status, consumedEnergyThisTick, consumedIonsThisTick,
            consumedNeutrinosThisTick, collectedThisTick, collectedHueThisTick);
        return true;
    }

    private static bool ReadScannerRuntime(PacketReader reader, DynamicScannerSubsystem scanner)
    {
        if (!scanner.Exists)
            return true;

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

        scanner.UpdateRuntime(active != 0, currentWidth, currentLength, currentAngle, targetWidth, targetLength, targetAngle,
            (SubsystemStatus)status, consumedEnergyThisTick, consumedIonsThisTick, consumedNeutrinosThisTick);
        return true;
    }

    private bool ReadEngineRuntime(PacketReader reader)
    {
        if (!_engine.Exists)
            return true;

        if (!Vector.FromReader(reader, out Vector? current) ||
            !Vector.FromReader(reader, out Vector? target) ||
            !reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick))
            return false;

        _engine.UpdateRuntime(current, target, (SubsystemStatus)status, consumedEnergyThisTick, consumedIonsThisTick,
            consumedNeutrinosThisTick);
        return true;
    }

    private bool ReadShotLauncherRuntime(PacketReader reader)
    {
        if (!_shotLauncher.Exists)
            return true;

        if (!Vector.FromReader(reader, out Vector? relativeMovement) ||
            !reader.Read(out ushort ticks) ||
            !reader.Read(out float load) ||
            !reader.Read(out float damage) ||
            !reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick))
            return false;

        _shotLauncher.UpdateRuntime(relativeMovement, ticks, load, damage, (SubsystemStatus)status, consumedEnergyThisTick,
            consumedIonsThisTick, consumedNeutrinosThisTick);
        return true;
    }

    private bool ReadShotMagazineRuntime(PacketReader reader)
    {
        if (!_shotMagazine.Exists)
            return true;

        if (!reader.Read(out float currentShots) ||
            !reader.Read(out byte status))
            return false;

        _shotMagazine.UpdateRuntime(currentShots, (SubsystemStatus)status);
        return true;
    }

    private bool ReadShotFabricatorRuntime(PacketReader reader)
    {
        if (!_shotFabricator.Exists)
            return true;

        if (!reader.Read(out byte active) ||
            !reader.Read(out float rate) ||
            !reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick))
            return false;

        _shotFabricator.UpdateRuntime(active != 0, rate, (SubsystemStatus)status, consumedEnergyThisTick, consumedIonsThisTick,
            consumedNeutrinosThisTick);
        return true;
    }

    private bool ReadInterceptorLauncherRuntime(PacketReader reader)
    {
        if (!_interceptorLauncher.Exists)
            return true;

        if (!Vector.FromReader(reader, out Vector? relativeMovement) ||
            !reader.Read(out ushort ticks) ||
            !reader.Read(out float load) ||
            !reader.Read(out float damage) ||
            !reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick))
            return false;

        _interceptorLauncher.UpdateRuntime(relativeMovement, ticks, load, damage, (SubsystemStatus)status, consumedEnergyThisTick,
            consumedIonsThisTick, consumedNeutrinosThisTick);
        return true;
    }

    private bool ReadInterceptorMagazineRuntime(PacketReader reader)
    {
        if (!_interceptorMagazine.Exists)
            return true;

        if (!reader.Read(out float currentShots) ||
            !reader.Read(out byte status))
            return false;

        _interceptorMagazine.UpdateRuntime(currentShots, (SubsystemStatus)status);
        return true;
    }

    private bool ReadInterceptorFabricatorRuntime(PacketReader reader)
    {
        if (!_interceptorFabricator.Exists)
            return true;

        if (!reader.Read(out byte active) ||
            !reader.Read(out float rate) ||
            !reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick))
            return false;

        _interceptorFabricator.UpdateRuntime(active != 0, rate, (SubsystemStatus)status, consumedEnergyThisTick,
            consumedIonsThisTick, consumedNeutrinosThisTick);
        return true;
    }

    private bool ReadRailgunRuntime(PacketReader reader)
    {
        if (!_railgun.Exists)
            return true;

        if (!reader.Read(out byte direction) ||
            !reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick))
            return false;

        _railgun.UpdateRuntime((RailgunDirection)direction, (SubsystemStatus)status, consumedEnergyThisTick,
            consumedIonsThisTick, consumedNeutrinosThisTick);
        return true;
    }

    private bool ReadJumpDriveRuntime(PacketReader reader)
    {
        if (!_jumpDrive.Exists)
            return true;

        if (!reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick))
            return false;

        _jumpDrive.UpdateRuntime((SubsystemStatus)status, consumedEnergyThisTick, consumedIonsThisTick, consumedNeutrinosThisTick);
        return true;
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
