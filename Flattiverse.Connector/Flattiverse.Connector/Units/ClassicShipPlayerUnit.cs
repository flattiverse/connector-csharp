using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// A classic ship for noobs.
/// </summary>
public class ClassicShipPlayerUnit : PlayerUnit
{
    private readonly ClassicShipEngineSubsystemInfo _engine;
    private readonly ShotWeaponSubsystemInfo _weapon;
    private readonly ScannerSubsystemInfo _mainScanner;
    private readonly ScannerSubsystemInfo _secondaryScanner;

    internal ClassicShipPlayerUnit(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
        _engine = new ClassicShipEngineSubsystemInfo();
        _weapon = new ShotWeaponSubsystemInfo();
        _mainScanner = new ScannerSubsystemInfo();
        _secondaryScanner = new ScannerSubsystemInfo();
    }

    internal ClassicShipPlayerUnit(ClassicShipPlayerUnit unit) : base(unit)
    {
        _engine = new ClassicShipEngineSubsystemInfo();
        _engine.Update(unit._engine.Exists, unit._engine.Maximum, unit._engine.Current, unit._engine.Target, unit._engine.Status,
            unit._engine.ConsumedEnergyThisTick, unit._engine.ConsumedIonsThisTick, unit._engine.ConsumedNeutrinosThisTick);
        _weapon = new ShotWeaponSubsystemInfo();
        _weapon.Update(unit._weapon.Exists, unit._weapon.MinimumRelativeMovement, unit._weapon.MaximumRelativeMovement,
            unit._weapon.MinimumTicks, unit._weapon.MaximumTicks, unit._weapon.MinimumLoad, unit._weapon.MaximumLoad,
            unit._weapon.MinimumDamage, unit._weapon.MaximumDamage, unit._weapon.RelativeMovement, unit._weapon.Ticks,
            unit._weapon.Load, unit._weapon.Damage, unit._weapon.Status, unit._weapon.ConsumedEnergyThisTick,
            unit._weapon.ConsumedIonsThisTick, unit._weapon.ConsumedNeutrinosThisTick);
        _mainScanner = new ScannerSubsystemInfo();
        _mainScanner.Update(unit._mainScanner.Exists, unit._mainScanner.MaximumWidth, unit._mainScanner.MaximumLength, unit._mainScanner.WidthSpeed,
            unit._mainScanner.LengthSpeed, unit._mainScanner.AngleSpeed, unit._mainScanner.Active, unit._mainScanner.CurrentWidth,
            unit._mainScanner.CurrentLength, unit._mainScanner.CurrentAngle, unit._mainScanner.TargetWidth, unit._mainScanner.TargetLength,
            unit._mainScanner.TargetAngle, unit._mainScanner.Status, unit._mainScanner.ConsumedEnergyThisTick,
            unit._mainScanner.ConsumedIonsThisTick, unit._mainScanner.ConsumedNeutrinosThisTick);
        _secondaryScanner = new ScannerSubsystemInfo();
        _secondaryScanner.Update(unit._secondaryScanner.Exists, unit._secondaryScanner.MaximumWidth, unit._secondaryScanner.MaximumLength,
            unit._secondaryScanner.WidthSpeed, unit._secondaryScanner.LengthSpeed, unit._secondaryScanner.AngleSpeed, unit._secondaryScanner.Active,
            unit._secondaryScanner.CurrentWidth, unit._secondaryScanner.CurrentLength, unit._secondaryScanner.CurrentAngle,
            unit._secondaryScanner.TargetWidth, unit._secondaryScanner.TargetLength, unit._secondaryScanner.TargetAngle,
            unit._secondaryScanner.Status, unit._secondaryScanner.ConsumedEnergyThisTick, unit._secondaryScanner.ConsumedIonsThisTick,
            unit._secondaryScanner.ConsumedNeutrinosThisTick);
    }

    /// <inheritdoc/>
    public override float Gravity => 0.0012f;

    /// <inheritdoc/>
    public override float Radius => 14f;

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.ClassicShipPlayerUnit;

    /// <summary>
    /// Visible snapshot of the engine subsystem.
    /// </summary>
    public ClassicShipEngineSubsystemInfo Engine
    {
        get { return _engine; }
    }

    /// <summary>
    /// Visible snapshot of the weapon subsystem.
    /// </summary>
    public ShotWeaponSubsystemInfo Weapon
    {
        get { return _weapon; }
    }

    /// <summary>
    /// Visible snapshot of the primary scanner subsystem.
    /// </summary>
    public ScannerSubsystemInfo MainScanner
    {
        get { return _mainScanner; }
    }

    /// <summary>
    /// Visible snapshot of the secondary scanner subsystem.
    /// </summary>
    public ScannerSubsystemInfo SecondaryScanner
    {
        get { return _secondaryScanner; }
    }
    
    /// <summary>
    /// Creates a snapshot copy of this visible player unit.
    /// </summary>
    public override Unit Clone()
    {
        return new ClassicShipPlayerUnit(this);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{base.ToString()}, EngineExists={_engine.Exists}, EngineStatus={_engine.Status}, EngineCurrent={_engine.Current}, EngineTarget={_engine.Target}, " +
               $"EngineConsumed=({_engine.ConsumedEnergyThisTick:0.###},{_engine.ConsumedIonsThisTick:0.###},{_engine.ConsumedNeutrinosThisTick:0.###}), " +
               $"MainScannerExists={_mainScanner.Exists}, MainScannerStatus={_mainScanner.Status}, MainScannerActive={_mainScanner.Active}, MainScannerCurrent=({_mainScanner.CurrentWidth:0.###},{_mainScanner.CurrentLength:0.###},{_mainScanner.CurrentAngle:0.###}), " +
               $"MainScannerTarget=({_mainScanner.TargetWidth:0.###},{_mainScanner.TargetLength:0.###},{_mainScanner.TargetAngle:0.###}), MainScannerConsumed=({_mainScanner.ConsumedEnergyThisTick:0.###},{_mainScanner.ConsumedIonsThisTick:0.###},{_mainScanner.ConsumedNeutrinosThisTick:0.###}), " +
               $"SecondaryScannerExists={_secondaryScanner.Exists}, SecondaryScannerStatus={_secondaryScanner.Status}, SecondaryScannerActive={_secondaryScanner.Active}, SecondaryScannerCurrent=({_secondaryScanner.CurrentWidth:0.###},{_secondaryScanner.CurrentLength:0.###},{_secondaryScanner.CurrentAngle:0.###}), " +
               $"SecondaryScannerTarget=({_secondaryScanner.TargetWidth:0.###},{_secondaryScanner.TargetLength:0.###},{_secondaryScanner.TargetAngle:0.###}), SecondaryScannerConsumed=({_secondaryScanner.ConsumedEnergyThisTick:0.###},{_secondaryScanner.ConsumedIonsThisTick:0.###},{_secondaryScanner.ConsumedNeutrinosThisTick:0.###}), " +
               $"WeaponExists={_weapon.Exists}, WeaponStatus={_weapon.Status}, WeaponRelativeMovement={_weapon.RelativeMovement}, WeaponTicks={_weapon.Ticks}, WeaponLoad={_weapon.Load:0.###}, WeaponDamage={_weapon.Damage:0.###}, WeaponConsumed=({_weapon.ConsumedEnergyThisTick:0.###},{_weapon.ConsumedIonsThisTick:0.###},{_weapon.ConsumedNeutrinosThisTick:0.###})";
    }

    internal override void UpdateState(PacketReader reader)
    {
        base.UpdateState(reader);

        if (!reader.Read(out byte mainScannerExists) ||
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
            !Vector.FromReader(reader, out Vector engineCurrent) ||
            !Vector.FromReader(reader, out Vector engineTarget) ||
            !reader.Read(out byte engineStatus) ||
            !reader.Read(out float engineConsumedEnergyThisTick) ||
            !reader.Read(out float engineConsumedIonsThisTick) ||
            !reader.Read(out float engineConsumedNeutrinosThisTick) ||
            !reader.Read(out byte weaponExists) ||
            !reader.Read(out float weaponMinimumRelativeMovement) ||
            !reader.Read(out float weaponMaximumRelativeMovement) ||
            !reader.Read(out ushort weaponMinimumTicks) ||
            !reader.Read(out ushort weaponMaximumTicks) ||
            !reader.Read(out float weaponMinimumLoad) ||
            !reader.Read(out float weaponMaximumLoad) ||
            !reader.Read(out float weaponMinimumDamage) ||
            !reader.Read(out float weaponMaximumDamage) ||
            !Vector.FromReader(reader, out Vector weaponRelativeMovement) ||
            !reader.Read(out ushort weaponTicks) ||
            !reader.Read(out float weaponLoad) ||
            !reader.Read(out float weaponDamage) ||
            !reader.Read(out byte weaponStatus) ||
            !reader.Read(out float weaponConsumedEnergyThisTick) ||
            !reader.Read(out float weaponConsumedIonsThisTick) ||
            !reader.Read(out float weaponConsumedNeutrinosThisTick))
            throw new InvalidDataException("Couldn't read Unit.");

        _mainScanner.Update(mainScannerExists != 0, mainScannerMaximumWidth, mainScannerMaximumLength, mainScannerWidthSpeed,
            mainScannerLengthSpeed, mainScannerAngleSpeed, mainScannerActive != 0, mainScannerCurrentWidth, mainScannerCurrentLength,
            mainScannerCurrentAngle, mainScannerTargetWidth, mainScannerTargetLength, mainScannerTargetAngle, (SubsystemStatus)mainScannerStatus,
            mainScannerConsumedEnergyThisTick, mainScannerConsumedIonsThisTick, mainScannerConsumedNeutrinosThisTick);
        _secondaryScanner.Update(secondaryScannerExists != 0, secondaryScannerMaximumWidth, secondaryScannerMaximumLength,
            secondaryScannerWidthSpeed, secondaryScannerLengthSpeed, secondaryScannerAngleSpeed, secondaryScannerActive != 0,
            secondaryScannerCurrentWidth, secondaryScannerCurrentLength, secondaryScannerCurrentAngle, secondaryScannerTargetWidth,
            secondaryScannerTargetLength, secondaryScannerTargetAngle, (SubsystemStatus)secondaryScannerStatus,
            secondaryScannerConsumedEnergyThisTick, secondaryScannerConsumedIonsThisTick, secondaryScannerConsumedNeutrinosThisTick);
        _engine.Update(engineExists != 0, engineMaximum, engineCurrent, engineTarget, (SubsystemStatus)engineStatus,
            engineConsumedEnergyThisTick, engineConsumedIonsThisTick, engineConsumedNeutrinosThisTick);
        _weapon.Update(weaponExists != 0, weaponMinimumRelativeMovement, weaponMaximumRelativeMovement, weaponMinimumTicks,
            weaponMaximumTicks, weaponMinimumLoad, weaponMaximumLoad, weaponMinimumDamage, weaponMaximumDamage, weaponRelativeMovement,
            weaponTicks, weaponLoad, weaponDamage, (SubsystemStatus)weaponStatus, weaponConsumedEnergyThisTick,
            weaponConsumedIonsThisTick, weaponConsumedNeutrinosThisTick);
    }
}
