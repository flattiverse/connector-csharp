using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// A classic ship for noobs.
/// </summary>
public class ClassicShipPlayerUnit : PlayerUnit
{
    private readonly ClassicShipEngineSubsystemInfo _engine;
    private readonly DynamicShotLauncherSubsystemInfo _shotLauncher;
    private readonly DynamicShotMagazineSubsystemInfo _shotMagazine;
    private readonly DynamicShotFabricatorSubsystemInfo _shotFabricator;
    private readonly DynamicScannerSubsystemInfo _mainScanner;
    private readonly DynamicScannerSubsystemInfo _secondaryScanner;

    internal ClassicShipPlayerUnit(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
        _engine = new ClassicShipEngineSubsystemInfo();
        _shotLauncher = new DynamicShotLauncherSubsystemInfo();
        _shotMagazine = new DynamicShotMagazineSubsystemInfo();
        _shotFabricator = new DynamicShotFabricatorSubsystemInfo();
        _mainScanner = new DynamicScannerSubsystemInfo();
        _secondaryScanner = new DynamicScannerSubsystemInfo();
    }

    internal ClassicShipPlayerUnit(ClassicShipPlayerUnit unit) : base(unit)
    {
        _engine = new ClassicShipEngineSubsystemInfo();
        _engine.Update(unit._engine.Exists, unit._engine.Maximum, unit._engine.Current, unit._engine.Target, unit._engine.Status,
            unit._engine.ConsumedEnergyThisTick, unit._engine.ConsumedIonsThisTick, unit._engine.ConsumedNeutrinosThisTick);
        _shotLauncher = new DynamicShotLauncherSubsystemInfo();
        _shotLauncher.Update(unit._shotLauncher.Exists, unit._shotLauncher.MinimumRelativeMovement, unit._shotLauncher.MaximumRelativeMovement,
            unit._shotLauncher.MinimumTicks, unit._shotLauncher.MaximumTicks, unit._shotLauncher.MinimumLoad, unit._shotLauncher.MaximumLoad,
            unit._shotLauncher.MinimumDamage, unit._shotLauncher.MaximumDamage, unit._shotLauncher.RelativeMovement, unit._shotLauncher.Ticks,
            unit._shotLauncher.Load, unit._shotLauncher.Damage, unit._shotLauncher.Status, unit._shotLauncher.ConsumedEnergyThisTick,
            unit._shotLauncher.ConsumedIonsThisTick, unit._shotLauncher.ConsumedNeutrinosThisTick);
        _shotMagazine = new DynamicShotMagazineSubsystemInfo();
        _shotMagazine.Update(unit._shotMagazine.Exists, unit._shotMagazine.MaximumShots, unit._shotMagazine.CurrentShots, unit._shotMagazine.Status);
        _shotFabricator = new DynamicShotFabricatorSubsystemInfo();
        _shotFabricator.Update(unit._shotFabricator.Exists, unit._shotFabricator.MinimumRate, unit._shotFabricator.MaximumRate,
            unit._shotFabricator.Active, unit._shotFabricator.Rate, unit._shotFabricator.Status, unit._shotFabricator.ConsumedEnergyThisTick,
            unit._shotFabricator.ConsumedIonsThisTick, unit._shotFabricator.ConsumedNeutrinosThisTick);
        _mainScanner = new DynamicScannerSubsystemInfo();
        _mainScanner.Update(unit._mainScanner.Exists, unit._mainScanner.MaximumWidth, unit._mainScanner.MaximumLength, unit._mainScanner.WidthSpeed,
            unit._mainScanner.LengthSpeed, unit._mainScanner.AngleSpeed, unit._mainScanner.Active, unit._mainScanner.CurrentWidth,
            unit._mainScanner.CurrentLength, unit._mainScanner.CurrentAngle, unit._mainScanner.TargetWidth, unit._mainScanner.TargetLength,
            unit._mainScanner.TargetAngle, unit._mainScanner.Status, unit._mainScanner.ConsumedEnergyThisTick,
            unit._mainScanner.ConsumedIonsThisTick, unit._mainScanner.ConsumedNeutrinosThisTick);
        _secondaryScanner = new DynamicScannerSubsystemInfo();
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
    /// Visible snapshot of the shot launcher subsystem.
    /// </summary>
    public DynamicShotLauncherSubsystemInfo ShotLauncher
    {
        get { return _shotLauncher; }
    }

    /// <summary>
    /// Visible snapshot of the shot magazine subsystem.
    /// </summary>
    public DynamicShotMagazineSubsystemInfo ShotMagazine
    {
        get { return _shotMagazine; }
    }

    /// <summary>
    /// Visible snapshot of the shot fabricator subsystem.
    /// </summary>
    public DynamicShotFabricatorSubsystemInfo ShotFabricator
    {
        get { return _shotFabricator; }
    }

    /// <summary>
    /// Visible snapshot of the primary scanner subsystem.
    /// </summary>
    public DynamicScannerSubsystemInfo MainScanner
    {
        get { return _mainScanner; }
    }

    /// <summary>
    /// Visible snapshot of the secondary scanner subsystem.
    /// </summary>
    public DynamicScannerSubsystemInfo SecondaryScanner
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
               $"ShotLauncherExists={_shotLauncher.Exists}, ShotLauncherStatus={_shotLauncher.Status}, ShotLauncherRelativeMovement={_shotLauncher.RelativeMovement}, ShotLauncherTicks={_shotLauncher.Ticks}, ShotLauncherLoad={_shotLauncher.Load:0.###}, ShotLauncherDamage={_shotLauncher.Damage:0.###}, ShotLauncherConsumed=({_shotLauncher.ConsumedEnergyThisTick:0.###},{_shotLauncher.ConsumedIonsThisTick:0.###},{_shotLauncher.ConsumedNeutrinosThisTick:0.###}), " +
               $"ShotMagazineExists={_shotMagazine.Exists}, ShotMagazineStatus={_shotMagazine.Status}, ShotMagazine={_shotMagazine.CurrentShots:0.###}/{_shotMagazine.MaximumShots:0.###}, " +
               $"ShotFabricatorExists={_shotFabricator.Exists}, ShotFabricatorStatus={_shotFabricator.Status}, ShotFabricatorActive={_shotFabricator.Active}, ShotFabricatorRate={_shotFabricator.Rate:0.###}, ShotFabricatorConsumed=({_shotFabricator.ConsumedEnergyThisTick:0.###},{_shotFabricator.ConsumedIonsThisTick:0.###},{_shotFabricator.ConsumedNeutrinosThisTick:0.###})";
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
            !reader.Read(out byte shotLauncherExists) ||
            !reader.Read(out float shotLauncherMinimumRelativeMovement) ||
            !reader.Read(out float shotLauncherMaximumRelativeMovement) ||
            !reader.Read(out ushort shotLauncherMinimumTicks) ||
            !reader.Read(out ushort shotLauncherMaximumTicks) ||
            !reader.Read(out float shotLauncherMinimumLoad) ||
            !reader.Read(out float shotLauncherMaximumLoad) ||
            !reader.Read(out float shotLauncherMinimumDamage) ||
            !reader.Read(out float shotLauncherMaximumDamage) ||
            !Vector.FromReader(reader, out Vector shotLauncherRelativeMovement) ||
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
            !reader.Read(out float shotFabricatorConsumedNeutrinosThisTick))
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
        _shotLauncher.Update(shotLauncherExists != 0, shotLauncherMinimumRelativeMovement, shotLauncherMaximumRelativeMovement,
            shotLauncherMinimumTicks, shotLauncherMaximumTicks, shotLauncherMinimumLoad, shotLauncherMaximumLoad, shotLauncherMinimumDamage,
            shotLauncherMaximumDamage, shotLauncherRelativeMovement, shotLauncherTicks, shotLauncherLoad, shotLauncherDamage,
            (SubsystemStatus)shotLauncherStatus, shotLauncherConsumedEnergyThisTick, shotLauncherConsumedIonsThisTick,
            shotLauncherConsumedNeutrinosThisTick);
        _shotMagazine.Update(shotMagazineExists != 0, shotMagazineMaximumShots, shotMagazineCurrentShots, (SubsystemStatus)shotMagazineStatus);
        _shotFabricator.Update(shotFabricatorExists != 0, shotFabricatorMinimumRate, shotFabricatorMaximumRate, shotFabricatorActive != 0,
            shotFabricatorRate, (SubsystemStatus)shotFabricatorStatus, shotFabricatorConsumedEnergyThisTick,
            shotFabricatorConsumedIonsThisTick, shotFabricatorConsumedNeutrinosThisTick);
    }
}
