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
    private readonly RailgunSubsystem _railgun;
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
        _railgun = new RailgunSubsystem(this, "Railgun", true, SubsystemSlot.Railgun);
        _mainScanner = DynamicScannerSubsystem.CreateClassicShipPrimaryScanner(this);
        _secondaryScanner = DynamicScannerSubsystem.CreateClassicShipSecondaryScanner(this);
        _jumpDrive = new JumpDriveSubsystem(this, true);
        _equippedCrystals = new string[3];

        ReadInitialState(reader);

        if (!reader.Read(out byte nebulaCollectorExists) ||
            !reader.Read(out float nebulaCollectorMinimumRate) ||
            !reader.Read(out float nebulaCollectorMaximumRate) ||
            !reader.Read(out float nebulaCollectorRate) ||
            !reader.Read(out byte nebulaCollectorStatus) ||
            !reader.Read(out float nebulaCollectorConsumedEnergyThisTick) ||
            !reader.Read(out float nebulaCollectorConsumedIonsThisTick) ||
            !reader.Read(out float nebulaCollectorConsumedNeutrinosThisTick) ||
            !reader.Read(out float nebulaCollectorCollectedThisTick) ||
            !reader.Read(out float nebulaCollectorCollectedHueThisTick) ||
            !reader.Read(out byte mainScannerExists) ||
            !reader.Read(out float mainScannerMaximumWidth) ||
            !reader.Read(out float mainScannerMaximumLength) ||
            !reader.Read(out float mainScannerWidthSpeed) ||
            !reader.Read(out float mainScannerLengthSpeed) ||
            !reader.Read(out float mainScannerAngleSpeed) ||
            !reader.Read(out byte mainScannerActive) ||
            !reader.Read(out float mainScannerCurrentWidth) ||
            !reader.Read(out float mainScannerCurrentLength) ||
            !reader.Read(out float mainScannerCurrentAngle) ||
            !reader.Read(out float mainScannerTargetWidth) ||
            !reader.Read(out float mainScannerTargetLength) ||
            !reader.Read(out float mainScannerTargetAngle) ||
            !reader.Read(out byte mainScannerStatus) ||
            !reader.Read(out float mainScannerConsumedEnergyThisTick) ||
            !reader.Read(out float mainScannerConsumedIonsThisTick) ||
            !reader.Read(out float mainScannerConsumedNeutrinosThisTick) ||
            !reader.Read(out byte secondaryScannerExists) ||
            !reader.Read(out float secondaryScannerMaximumWidth) ||
            !reader.Read(out float secondaryScannerMaximumLength) ||
            !reader.Read(out float secondaryScannerWidthSpeed) ||
            !reader.Read(out float secondaryScannerLengthSpeed) ||
            !reader.Read(out float secondaryScannerAngleSpeed) ||
            !reader.Read(out byte secondaryScannerActive) ||
            !reader.Read(out float secondaryScannerCurrentWidth) ||
            !reader.Read(out float secondaryScannerCurrentLength) ||
            !reader.Read(out float secondaryScannerCurrentAngle) ||
            !reader.Read(out float secondaryScannerTargetWidth) ||
            !reader.Read(out float secondaryScannerTargetLength) ||
            !reader.Read(out float secondaryScannerTargetAngle) ||
            !reader.Read(out byte secondaryScannerStatus) ||
            !reader.Read(out float secondaryScannerConsumedEnergyThisTick) ||
            !reader.Read(out float secondaryScannerConsumedIonsThisTick) ||
            !reader.Read(out float secondaryScannerConsumedNeutrinosThisTick) ||
            !reader.Read(out byte engineExists) ||
            !reader.Read(out float engineMaximum) ||
            !Vector.FromReader(reader, out Vector? engineCurrent) ||
            !Vector.FromReader(reader, out Vector? engineTarget) ||
            !reader.Read(out byte engineStatus) ||
            !reader.Read(out float engineConsumedEnergyThisTick) ||
            !reader.Read(out float engineConsumedIonsThisTick) ||
            !reader.Read(out float engineConsumedNeutrinosThisTick) ||
            !reader.Read(out byte shotLauncherExists) ||
            !reader.Read(out float shotLauncherMinimumRelativeMovement) ||
            !reader.Read(out float shotLauncherMaximumRelativeMovement) ||
            !reader.Read(out ushort shotLauncherMinimumTicks) ||
            !reader.Read(out ushort shotLauncherMaximumTicks) ||
            !reader.Read(out float shotLauncherMinimumLoad) ||
            !reader.Read(out float shotLauncherMaximumLoad) ||
            !reader.Read(out float shotLauncherMinimumDamage) ||
            !reader.Read(out float shotLauncherMaximumDamage) ||
            !Vector.FromReader(reader, out Vector? shotLauncherRelativeMovement) ||
            !reader.Read(out ushort shotLauncherTicks) ||
            !reader.Read(out float shotLauncherLoad) ||
            !reader.Read(out float shotLauncherDamage) ||
            !reader.Read(out byte shotLauncherStatus) ||
            !reader.Read(out float shotLauncherConsumedEnergyThisTick) ||
            !reader.Read(out float shotLauncherConsumedIonsThisTick) ||
            !reader.Read(out float shotLauncherConsumedNeutrinosThisTick) ||
            !reader.Read(out byte shotMagazineExists) ||
            !reader.Read(out float shotMagazineMaximumShots) ||
            !reader.Read(out float shotMagazineCurrentShots) ||
            !reader.Read(out byte shotMagazineStatus) ||
            !reader.Read(out byte shotFabricatorExists) ||
            !reader.Read(out float shotFabricatorMinimumRate) ||
            !reader.Read(out float shotFabricatorMaximumRate) ||
            !reader.Read(out byte shotFabricatorActive) ||
            !reader.Read(out float shotFabricatorRate) ||
            !reader.Read(out byte shotFabricatorStatus) ||
            !reader.Read(out float shotFabricatorConsumedEnergyThisTick) ||
            !reader.Read(out float shotFabricatorConsumedIonsThisTick) ||
            !reader.Read(out float shotFabricatorConsumedNeutrinosThisTick) ||
            !reader.Read(out byte interceptorLauncherExists) ||
            !reader.Read(out float interceptorLauncherMinimumRelativeMovement) ||
            !reader.Read(out float interceptorLauncherMaximumRelativeMovement) ||
            !reader.Read(out ushort interceptorLauncherMinimumTicks) ||
            !reader.Read(out ushort interceptorLauncherMaximumTicks) ||
            !reader.Read(out float interceptorLauncherMinimumLoad) ||
            !reader.Read(out float interceptorLauncherMaximumLoad) ||
            !reader.Read(out float interceptorLauncherMinimumDamage) ||
            !reader.Read(out float interceptorLauncherMaximumDamage) ||
            !Vector.FromReader(reader, out Vector? interceptorLauncherRelativeMovement) ||
            !reader.Read(out ushort interceptorLauncherTicks) ||
            !reader.Read(out float interceptorLauncherLoad) ||
            !reader.Read(out float interceptorLauncherDamage) ||
            !reader.Read(out byte interceptorLauncherStatus) ||
            !reader.Read(out float interceptorLauncherConsumedEnergyThisTick) ||
            !reader.Read(out float interceptorLauncherConsumedIonsThisTick) ||
            !reader.Read(out float interceptorLauncherConsumedNeutrinosThisTick) ||
            !reader.Read(out byte interceptorMagazineExists) ||
            !reader.Read(out float interceptorMagazineMaximumShots) ||
            !reader.Read(out float interceptorMagazineCurrentShots) ||
            !reader.Read(out byte interceptorMagazineStatus) ||
            !reader.Read(out byte interceptorFabricatorExists) ||
            !reader.Read(out float interceptorFabricatorMinimumRate) ||
            !reader.Read(out float interceptorFabricatorMaximumRate) ||
            !reader.Read(out byte interceptorFabricatorActive) ||
            !reader.Read(out float interceptorFabricatorRate) ||
            !reader.Read(out byte interceptorFabricatorStatus) ||
            !reader.Read(out float interceptorFabricatorConsumedEnergyThisTick) ||
            !reader.Read(out float interceptorFabricatorConsumedIonsThisTick) ||
            !reader.Read(out float interceptorFabricatorConsumedNeutrinosThisTick) ||
            !reader.Read(out byte railgunExists) ||
            !reader.Read(out float railgunEnergyCost) ||
            !reader.Read(out float railgunMetalCost) ||
            !reader.Read(out byte railgunDirection) ||
            !reader.Read(out byte railgunStatus) ||
            !reader.Read(out float railgunConsumedEnergyThisTick) ||
            !reader.Read(out float railgunConsumedIonsThisTick) ||
            !reader.Read(out float railgunConsumedNeutrinosThisTick) ||
            !reader.Read(out byte jumpDriveExists) ||
            !reader.Read(out float jumpDriveEnergyCost) ||
            !reader.Read(out _equippedCrystals[0]) ||
            !reader.Read(out _equippedCrystals[1]) ||
            !reader.Read(out _equippedCrystals[2]))
            throw new InvalidDataException("Couldn't read ClassicShipControllable create state.");

        _nebulaCollector.UpdateRuntime(nebulaCollectorRate, (SubsystemStatus)nebulaCollectorStatus, nebulaCollectorConsumedEnergyThisTick,
            nebulaCollectorConsumedIonsThisTick, nebulaCollectorConsumedNeutrinosThisTick, nebulaCollectorCollectedThisTick,
            nebulaCollectorCollectedHueThisTick);
        _mainScanner.UpdateRuntime(mainScannerActive != 0, mainScannerCurrentWidth, mainScannerCurrentLength, mainScannerCurrentAngle,
            mainScannerTargetWidth, mainScannerTargetLength, mainScannerTargetAngle, (SubsystemStatus)mainScannerStatus,
            mainScannerConsumedEnergyThisTick, mainScannerConsumedIonsThisTick, mainScannerConsumedNeutrinosThisTick);
        _secondaryScanner.UpdateRuntime(secondaryScannerActive != 0, secondaryScannerCurrentWidth, secondaryScannerCurrentLength,
            secondaryScannerCurrentAngle, secondaryScannerTargetWidth, secondaryScannerTargetLength,
            secondaryScannerTargetAngle, (SubsystemStatus)secondaryScannerStatus, secondaryScannerConsumedEnergyThisTick,
            secondaryScannerConsumedIonsThisTick, secondaryScannerConsumedNeutrinosThisTick);
        _engine.UpdateRuntime(engineCurrent, engineTarget, (SubsystemStatus)engineStatus, engineConsumedEnergyThisTick,
            engineConsumedIonsThisTick, engineConsumedNeutrinosThisTick);
        _shotLauncher.UpdateRuntime(shotLauncherRelativeMovement, shotLauncherTicks, shotLauncherLoad, shotLauncherDamage,
            (SubsystemStatus)shotLauncherStatus, shotLauncherConsumedEnergyThisTick, shotLauncherConsumedIonsThisTick,
            shotLauncherConsumedNeutrinosThisTick);
        _shotMagazine.UpdateRuntime(shotMagazineCurrentShots, (SubsystemStatus)shotMagazineStatus);
        _shotFabricator.SetMaximumRate(shotFabricatorMaximumRate);
        _shotFabricator.UpdateRuntime(shotFabricatorActive != 0, shotFabricatorRate, (SubsystemStatus)shotFabricatorStatus,
            shotFabricatorConsumedEnergyThisTick, shotFabricatorConsumedIonsThisTick, shotFabricatorConsumedNeutrinosThisTick);
        _interceptorLauncher.UpdateRuntime(interceptorLauncherRelativeMovement, interceptorLauncherTicks, interceptorLauncherLoad,
            interceptorLauncherDamage, (SubsystemStatus)interceptorLauncherStatus, interceptorLauncherConsumedEnergyThisTick,
            interceptorLauncherConsumedIonsThisTick, interceptorLauncherConsumedNeutrinosThisTick);
        _interceptorMagazine.UpdateRuntime(interceptorMagazineCurrentShots, (SubsystemStatus)interceptorMagazineStatus);
        _interceptorFabricator.SetMaximumRate(interceptorFabricatorMaximumRate);
        _interceptorFabricator.UpdateRuntime(interceptorFabricatorActive != 0, interceptorFabricatorRate,
            (SubsystemStatus)interceptorFabricatorStatus, interceptorFabricatorConsumedEnergyThisTick,
            interceptorFabricatorConsumedIonsThisTick, interceptorFabricatorConsumedNeutrinosThisTick);
        _railgun.UpdateRuntime((RailgunDirection)railgunDirection, (SubsystemStatus)railgunStatus, railgunConsumedEnergyThisTick,
            railgunConsumedIonsThisTick, railgunConsumedNeutrinosThisTick);
        _jumpDrive.SetEnergyCost(jumpDriveEnergyCost);
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
    public RailgunSubsystem Railgun
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
        float nebulaCollectorRate;
        byte nebulaCollectorStatus;
        float nebulaCollectorConsumedEnergyThisTick;
        float nebulaCollectorConsumedIonsThisTick;
        float nebulaCollectorConsumedNeutrinosThisTick;
        float nebulaCollectorCollectedThisTick;
        float nebulaCollectorCollectedHueThisTick;
        byte mainScannerActive;
        float mainScannerCurrentWidth;
        float mainScannerCurrentLength;
        float mainScannerCurrentAngle;
        float mainScannerTargetWidth;
        float mainScannerTargetLength;
        float mainScannerTargetAngle;
        byte mainScannerStatus;
        float mainScannerConsumedEnergyThisTick;
        float mainScannerConsumedIonsThisTick;
        float mainScannerConsumedNeutrinosThisTick;
        byte secondaryScannerActive;
        float secondaryScannerCurrentWidth;
        float secondaryScannerCurrentLength;
        float secondaryScannerCurrentAngle;
        float secondaryScannerTargetWidth;
        float secondaryScannerTargetLength;
        float secondaryScannerTargetAngle;
        byte secondaryScannerStatus;
        float secondaryScannerConsumedEnergyThisTick;
        float secondaryScannerConsumedIonsThisTick;
        float secondaryScannerConsumedNeutrinosThisTick;
        Vector? engineCurrent;
        Vector? engineTarget;
        byte engineStatus;
        float engineConsumedEnergyThisTick;
        float engineConsumedIonsThisTick;
        float engineConsumedNeutrinosThisTick;
        Vector? shotLauncherRelativeMovement;
        ushort shotLauncherTicks;
        float shotLauncherLoad;
        float shotLauncherDamage;
        byte shotLauncherStatus;
        float shotLauncherConsumedEnergyThisTick;
        float shotLauncherConsumedIonsThisTick;
        float shotLauncherConsumedNeutrinosThisTick;
        float shotMagazineCurrentShots;
        byte shotMagazineStatus;
        byte shotFabricatorActive;
        float shotFabricatorRate;
        byte shotFabricatorStatus;
        float shotFabricatorConsumedEnergyThisTick;
        float shotFabricatorConsumedIonsThisTick;
        float shotFabricatorConsumedNeutrinosThisTick;
        Vector? interceptorLauncherRelativeMovement;
        ushort interceptorLauncherTicks;
        float interceptorLauncherLoad;
        float interceptorLauncherDamage;
        byte interceptorLauncherStatus;
        float interceptorLauncherConsumedEnergyThisTick;
        float interceptorLauncherConsumedIonsThisTick;
        float interceptorLauncherConsumedNeutrinosThisTick;
        float interceptorMagazineCurrentShots;
        byte interceptorMagazineStatus;
        byte interceptorFabricatorActive;
        float interceptorFabricatorRate;
        byte interceptorFabricatorStatus;
        float interceptorFabricatorConsumedEnergyThisTick;
        float interceptorFabricatorConsumedIonsThisTick;
        float interceptorFabricatorConsumedNeutrinosThisTick;
        byte railgunDirection;
        byte railgunStatus;
        float railgunConsumedEnergyThisTick;
        float railgunConsumedIonsThisTick;
        float railgunConsumedNeutrinosThisTick;

        if (!reader.Read(out nebulaCollectorRate) ||
            !reader.Read(out nebulaCollectorStatus) ||
            !reader.Read(out nebulaCollectorConsumedEnergyThisTick) ||
            !reader.Read(out nebulaCollectorConsumedIonsThisTick) ||
            !reader.Read(out nebulaCollectorConsumedNeutrinosThisTick) ||
            !reader.Read(out nebulaCollectorCollectedThisTick) ||
            !reader.Read(out nebulaCollectorCollectedHueThisTick) ||
            !reader.Read(out mainScannerActive) ||
            !reader.Read(out mainScannerCurrentWidth) || !reader.Read(out mainScannerCurrentLength) || !reader.Read(out mainScannerCurrentAngle) ||
            !reader.Read(out mainScannerTargetWidth) || !reader.Read(out mainScannerTargetLength) || !reader.Read(out mainScannerTargetAngle) ||
            !reader.Read(out mainScannerStatus) || !reader.Read(out mainScannerConsumedEnergyThisTick) ||
            !reader.Read(out mainScannerConsumedIonsThisTick) || !reader.Read(out mainScannerConsumedNeutrinosThisTick) ||
            !reader.Read(out secondaryScannerActive) ||
            !reader.Read(out secondaryScannerCurrentWidth) || !reader.Read(out secondaryScannerCurrentLength) ||
            !reader.Read(out secondaryScannerCurrentAngle) || !reader.Read(out secondaryScannerTargetWidth) ||
            !reader.Read(out secondaryScannerTargetLength) || !reader.Read(out secondaryScannerTargetAngle) ||
            !reader.Read(out secondaryScannerStatus) || !reader.Read(out secondaryScannerConsumedEnergyThisTick) ||
            !reader.Read(out secondaryScannerConsumedIonsThisTick) || !reader.Read(out secondaryScannerConsumedNeutrinosThisTick) ||
            !Vector.FromReader(reader, out engineCurrent) || !Vector.FromReader(reader, out engineTarget) ||
            !reader.Read(out engineStatus) || !reader.Read(out engineConsumedEnergyThisTick) ||
            !reader.Read(out engineConsumedIonsThisTick) || !reader.Read(out engineConsumedNeutrinosThisTick) ||
            !Vector.FromReader(reader, out shotLauncherRelativeMovement) || !reader.Read(out shotLauncherTicks) || !reader.Read(out shotLauncherLoad) ||
            !reader.Read(out shotLauncherDamage) || !reader.Read(out shotLauncherStatus) || !reader.Read(out shotLauncherConsumedEnergyThisTick) ||
            !reader.Read(out shotLauncherConsumedIonsThisTick) || !reader.Read(out shotLauncherConsumedNeutrinosThisTick) ||
            !reader.Read(out shotMagazineCurrentShots) || !reader.Read(out shotMagazineStatus) ||
            !reader.Read(out shotFabricatorActive) || !reader.Read(out shotFabricatorRate) || !reader.Read(out shotFabricatorStatus) ||
            !reader.Read(out shotFabricatorConsumedEnergyThisTick) || !reader.Read(out shotFabricatorConsumedIonsThisTick) ||
            !reader.Read(out shotFabricatorConsumedNeutrinosThisTick) ||
            !Vector.FromReader(reader, out interceptorLauncherRelativeMovement) || !reader.Read(out interceptorLauncherTicks) ||
            !reader.Read(out interceptorLauncherLoad) || !reader.Read(out interceptorLauncherDamage) || !reader.Read(out interceptorLauncherStatus) ||
            !reader.Read(out interceptorLauncherConsumedEnergyThisTick) || !reader.Read(out interceptorLauncherConsumedIonsThisTick) ||
            !reader.Read(out interceptorLauncherConsumedNeutrinosThisTick) || !reader.Read(out interceptorMagazineCurrentShots) ||
            !reader.Read(out interceptorMagazineStatus) || !reader.Read(out interceptorFabricatorActive) ||
            !reader.Read(out interceptorFabricatorRate) || !reader.Read(out interceptorFabricatorStatus) ||
            !reader.Read(out interceptorFabricatorConsumedEnergyThisTick) || !reader.Read(out interceptorFabricatorConsumedIonsThisTick) ||
            !reader.Read(out interceptorFabricatorConsumedNeutrinosThisTick) || !reader.Read(out railgunDirection) ||
            !reader.Read(out railgunStatus) || !reader.Read(out railgunConsumedEnergyThisTick) ||
            !reader.Read(out railgunConsumedIonsThisTick) || !reader.Read(out railgunConsumedNeutrinosThisTick))
            throw new InvalidDataException("Couldn't read ClassicShipControllable runtime.");

        _nebulaCollector.UpdateRuntime(nebulaCollectorRate, (SubsystemStatus)nebulaCollectorStatus, nebulaCollectorConsumedEnergyThisTick,
            nebulaCollectorConsumedIonsThisTick, nebulaCollectorConsumedNeutrinosThisTick, nebulaCollectorCollectedThisTick,
            nebulaCollectorCollectedHueThisTick);
        _mainScanner.UpdateRuntime(mainScannerActive != 0, mainScannerCurrentWidth, mainScannerCurrentLength, mainScannerCurrentAngle,
            mainScannerTargetWidth, mainScannerTargetLength, mainScannerTargetAngle, (SubsystemStatus)mainScannerStatus,
            mainScannerConsumedEnergyThisTick, mainScannerConsumedIonsThisTick, mainScannerConsumedNeutrinosThisTick);
        _secondaryScanner.UpdateRuntime(secondaryScannerActive != 0, secondaryScannerCurrentWidth, secondaryScannerCurrentLength,
            secondaryScannerCurrentAngle, secondaryScannerTargetWidth, secondaryScannerTargetLength,
            secondaryScannerTargetAngle, (SubsystemStatus)secondaryScannerStatus, secondaryScannerConsumedEnergyThisTick,
            secondaryScannerConsumedIonsThisTick, secondaryScannerConsumedNeutrinosThisTick);
        _engine.UpdateRuntime(engineCurrent, engineTarget, (SubsystemStatus)engineStatus, engineConsumedEnergyThisTick,
            engineConsumedIonsThisTick, engineConsumedNeutrinosThisTick);
        _shotLauncher.UpdateRuntime(shotLauncherRelativeMovement, shotLauncherTicks, shotLauncherLoad, shotLauncherDamage,
            (SubsystemStatus)shotLauncherStatus, shotLauncherConsumedEnergyThisTick, shotLauncherConsumedIonsThisTick,
            shotLauncherConsumedNeutrinosThisTick);
        _shotMagazine.UpdateRuntime(shotMagazineCurrentShots, (SubsystemStatus)shotMagazineStatus);
        _shotFabricator.UpdateRuntime(shotFabricatorActive != 0, shotFabricatorRate, (SubsystemStatus)shotFabricatorStatus,
            shotFabricatorConsumedEnergyThisTick, shotFabricatorConsumedIonsThisTick, shotFabricatorConsumedNeutrinosThisTick);
        _interceptorLauncher.UpdateRuntime(interceptorLauncherRelativeMovement, interceptorLauncherTicks, interceptorLauncherLoad,
            interceptorLauncherDamage, (SubsystemStatus)interceptorLauncherStatus, interceptorLauncherConsumedEnergyThisTick,
            interceptorLauncherConsumedIonsThisTick, interceptorLauncherConsumedNeutrinosThisTick);
        _interceptorMagazine.UpdateRuntime(interceptorMagazineCurrentShots, (SubsystemStatus)interceptorMagazineStatus);
        _interceptorFabricator.UpdateRuntime(interceptorFabricatorActive != 0, interceptorFabricatorRate,
            (SubsystemStatus)interceptorFabricatorStatus, interceptorFabricatorConsumedEnergyThisTick,
            interceptorFabricatorConsumedIonsThisTick, interceptorFabricatorConsumedNeutrinosThisTick);
        _railgun.UpdateRuntime((RailgunDirection)railgunDirection, (SubsystemStatus)railgunStatus, railgunConsumedEnergyThisTick,
            railgunConsumedIonsThisTick, railgunConsumedNeutrinosThisTick);
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
