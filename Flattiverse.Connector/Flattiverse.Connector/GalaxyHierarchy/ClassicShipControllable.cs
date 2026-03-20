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
    private readonly ShotWeaponSubsystem _weapon;
    private readonly ScannerSubsystem _mainScanner;
    private readonly ScannerSubsystem _secondaryScanner;

    internal ClassicShipControllable(Cluster cluster, byte id, string name, PacketReader reader) : base(id, name, cluster, reader)
    {
        _hull = HullSubsystem.CreateClassicShipHull(this);
        _energyBattery = BatterySubsystem.CreateClassicShipEnergyBattery(this);
        _ionBattery = BatterySubsystem.CreateMissingBattery(this, "IonBattery", SubsystemSlot.IonBattery);
        _neutrinoBattery = BatterySubsystem.CreateMissingBattery(this, "NeutrinoBattery", SubsystemSlot.NeutrinoBattery);
        _energyCell = EnergyCellSubsystem.CreateClassicShipEnergyCell(this);
        _ionCell = EnergyCellSubsystem.CreateMissingCell(this, "IonCell", SubsystemSlot.IonCell);
        _neutrinoCell = EnergyCellSubsystem.CreateMissingCell(this, "NeutrinoCell", SubsystemSlot.NeutrinoCell);

        _engine = new ClassicShipEngineSubsystem(this);
        _weapon = new ShotWeaponSubsystem(this, "Weapon", true, SubsystemSlot.FrontShotLauncher);
        _mainScanner = ScannerSubsystem.CreateClassicShipPrimaryScanner(this);
        _secondaryScanner = ScannerSubsystem.CreateClassicShipSecondaryScanner(this);
    }

    /// <summary>
    /// The engine subsystem of the classic ship.
    /// </summary>
    public ClassicShipEngineSubsystem Engine
    {
        get { return _engine; }
    }

    /// <summary>
    /// The weapon subsystem of the classic ship.
    /// </summary>
    public ShotWeaponSubsystem Weapon
    {
        get { return _weapon; }
    }

    /// <summary>
    /// The primary scanner subsystem of the classic ship.
    /// </summary>
    public ScannerSubsystem MainScanner
    {
        get { return _mainScanner; }
    }

    /// <summary>
    /// The secondary scanner subsystem of the classic ship.
    /// </summary>
    public ScannerSubsystem SecondaryScanner
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
        _weapon.ResetRuntime();
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
        Vector? weaponRelativeMovement;
        ushort weaponTicks;
        float weaponLoad;
        float weaponDamage;
        byte weaponStatus;
        float weaponConsumedEnergyThisTick;
        float weaponConsumedIonsThisTick;
        float weaponConsumedNeutrinosThisTick;

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
            !Vector.FromReader(reader, out weaponRelativeMovement) || !reader.Read(out weaponTicks) || !reader.Read(out weaponLoad) ||
            !reader.Read(out weaponDamage) || !reader.Read(out weaponStatus) || !reader.Read(out weaponConsumedEnergyThisTick) ||
            !reader.Read(out weaponConsumedIonsThisTick) || !reader.Read(out weaponConsumedNeutrinosThisTick))
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
        _weapon.UpdateRuntime(weaponRelativeMovement, weaponTicks, weaponLoad, weaponDamage, (SubsystemStatus)weaponStatus,
            weaponConsumedEnergyThisTick, weaponConsumedIonsThisTick, weaponConsumedNeutrinosThisTick);
    }

    private protected override void EmitRuntimeEvents()
    {
        base.EmitRuntimeEvents();
        PushRuntimeEvent(_mainScanner.CreateRuntimeEvent());
        PushRuntimeEvent(_secondaryScanner.CreateRuntimeEvent());
        PushRuntimeEvent(_engine.CreateRuntimeEvent());
        PushRuntimeEvent(_weapon.CreateRuntimeEvent());
    }

    private void PushRuntimeEvent(FlattiverseEvent? @event)
    {
        if (@event is not null)
            _cluster.Galaxy.PushEvent(@event);
    }
}
