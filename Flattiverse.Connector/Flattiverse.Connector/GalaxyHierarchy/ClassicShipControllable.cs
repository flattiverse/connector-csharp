using Flattiverse.Connector.Events;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// The controllable of a classic ship.
/// </summary>
public class ClassicShipControllable : Controllable
{
    private readonly ClassicShipEngineSubsystem _engine;
    private readonly DynamicShotLauncherSubsystem _shotLauncher;
    private readonly DynamicShotMagazineSubsystem _shotMagazine;
    private readonly DynamicShotFabricatorSubsystem _shotFabricator;
    private readonly DynamicScannerSubsystem _mainScanner;
    private readonly DynamicScannerSubsystem _secondaryScanner;

    internal ClassicShipControllable(Cluster cluster, byte id, string name, PacketReader reader) : base(id, name, cluster, reader)
    {
        _hull = HullSubsystem.CreateClassicShipHull(this);
        _shield = ShieldSubsystem.CreateClassicShipShield(this);
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
        _mainScanner = DynamicScannerSubsystem.CreateClassicShipPrimaryScanner(this);
        _secondaryScanner = DynamicScannerSubsystem.CreateClassicShipSecondaryScanner(this);
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
    
    /// <inheritdoc/>
    public override float Gravity => 0.0012f;
    
    /// <inheritdoc/>
    public override float Size => 14f;

    private protected override void ResetRuntime()
    {
        base.ResetRuntime();
        _engine.ResetRuntime();
        _shotLauncher.ResetRuntime();
        _shotMagazine.ResetRuntime();
        _shotFabricator.ResetRuntime();
        _mainScanner.ResetRuntime();
        _secondaryScanner.ResetRuntime();
    }

    private protected override void ReadRuntime(PacketReader reader)
    {
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

        if (!reader.Read(out mainScannerActive) ||
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
            !reader.Read(out shotFabricatorConsumedNeutrinosThisTick))
            throw new InvalidDataException("Couldan't read ClassicShipControllable runtime.");

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
    }

    private protected override void EmitRuntimeEvents()
    {
        base.EmitRuntimeEvents();
        PushRuntimeEvent(_mainScanner.CreateRuntimeEvent());
        PushRuntimeEvent(_secondaryScanner.CreateRuntimeEvent());
        PushRuntimeEvent(_engine.CreateRuntimeEvent());
        PushRuntimeEvent(_shotLauncher.CreateRuntimeEvent());
        PushRuntimeEvent(_shotMagazine.CreateRuntimeEvent());
        PushRuntimeEvent(_shotFabricator.CreateRuntimeEvent());
    }

    private void PushRuntimeEvent(FlattiverseEvent? @event)
    {
        if (@event is not null)
            _cluster.Galaxy.PushEvent(@event);
    }
}
