using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// Visible snapshot of a classic-ship player unit in a cluster.
/// This mirrors what the local player can currently see about the ship and must not be confused with the owner-side
/// <see cref="ClassicShipControllable" /> used to command the local player's own ship.
/// </summary>
public class ClassicShipPlayerUnit : PlayerUnit
{
    private readonly NebulaCollectorSubsystemInfo _nebulaCollector;
    private readonly ClassicShipEngineSubsystemInfo _engine;
    private readonly DynamicShotLauncherSubsystemInfo _shotLauncher;
    private readonly DynamicShotMagazineSubsystemInfo _shotMagazine;
    private readonly DynamicShotFabricatorSubsystemInfo _shotFabricator;
    private readonly DynamicInterceptorLauncherSubsystemInfo _interceptorLauncher;
    private readonly DynamicInterceptorMagazineSubsystemInfo _interceptorMagazine;
    private readonly DynamicInterceptorFabricatorSubsystemInfo _interceptorFabricator;
    private readonly RailgunSubsystemInfo _railgun;
    private readonly DynamicScannerSubsystemInfo _mainScanner;
    private readonly DynamicScannerSubsystemInfo _secondaryScanner;
    private readonly JumpDriveSubsystemInfo _jumpDrive;

    internal ClassicShipPlayerUnit(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
        _nebulaCollector = new NebulaCollectorSubsystemInfo();
        _engine = new ClassicShipEngineSubsystemInfo();
        _shotLauncher = new DynamicShotLauncherSubsystemInfo();
        _shotMagazine = new DynamicShotMagazineSubsystemInfo();
        _shotFabricator = new DynamicShotFabricatorSubsystemInfo();
        _interceptorLauncher = new DynamicInterceptorLauncherSubsystemInfo();
        _interceptorMagazine = new DynamicInterceptorMagazineSubsystemInfo();
        _interceptorFabricator = new DynamicInterceptorFabricatorSubsystemInfo();
        _railgun = new RailgunSubsystemInfo();
        _mainScanner = new DynamicScannerSubsystemInfo();
        _secondaryScanner = new DynamicScannerSubsystemInfo();
        _jumpDrive = new JumpDriveSubsystemInfo();
    }

    internal ClassicShipPlayerUnit(ClassicShipPlayerUnit unit) : base(unit)
    {
        _nebulaCollector = new NebulaCollectorSubsystemInfo();
        _nebulaCollector.Update(unit._nebulaCollector.Exists, unit._nebulaCollector.MinimumRate, unit._nebulaCollector.MaximumRate,
            unit._nebulaCollector.Rate, unit._nebulaCollector.Status, unit._nebulaCollector.ConsumedEnergyThisTick,
            unit._nebulaCollector.ConsumedIonsThisTick, unit._nebulaCollector.ConsumedNeutrinosThisTick,
            unit._nebulaCollector.CollectedThisTick, unit._nebulaCollector.CollectedHueThisTick);
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
        _interceptorLauncher = new DynamicInterceptorLauncherSubsystemInfo();
        _interceptorLauncher.Update(unit._interceptorLauncher.Exists, unit._interceptorLauncher.MinimumRelativeMovement,
            unit._interceptorLauncher.MaximumRelativeMovement, unit._interceptorLauncher.MinimumTicks,
            unit._interceptorLauncher.MaximumTicks, unit._interceptorLauncher.MinimumLoad, unit._interceptorLauncher.MaximumLoad,
            unit._interceptorLauncher.MinimumDamage, unit._interceptorLauncher.MaximumDamage, unit._interceptorLauncher.RelativeMovement,
            unit._interceptorLauncher.Ticks, unit._interceptorLauncher.Load, unit._interceptorLauncher.Damage,
            unit._interceptorLauncher.Status, unit._interceptorLauncher.ConsumedEnergyThisTick,
            unit._interceptorLauncher.ConsumedIonsThisTick, unit._interceptorLauncher.ConsumedNeutrinosThisTick);
        _interceptorMagazine = new DynamicInterceptorMagazineSubsystemInfo();
        _interceptorMagazine.Update(unit._interceptorMagazine.Exists, unit._interceptorMagazine.MaximumShots,
            unit._interceptorMagazine.CurrentShots, unit._interceptorMagazine.Status);
        _interceptorFabricator = new DynamicInterceptorFabricatorSubsystemInfo();
        _interceptorFabricator.Update(unit._interceptorFabricator.Exists, unit._interceptorFabricator.MinimumRate,
            unit._interceptorFabricator.MaximumRate, unit._interceptorFabricator.Active, unit._interceptorFabricator.Rate,
            unit._interceptorFabricator.Status, unit._interceptorFabricator.ConsumedEnergyThisTick,
            unit._interceptorFabricator.ConsumedIonsThisTick, unit._interceptorFabricator.ConsumedNeutrinosThisTick);
        _railgun = new RailgunSubsystemInfo();
        _railgun.Update(unit._railgun.Exists, unit._railgun.EnergyCost, unit._railgun.MetalCost, unit._railgun.Direction,
            unit._railgun.Status, unit._railgun.ConsumedEnergyThisTick, unit._railgun.ConsumedIonsThisTick,
            unit._railgun.ConsumedNeutrinosThisTick);
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
        _jumpDrive = new JumpDriveSubsystemInfo();
        _jumpDrive.Update(unit._jumpDrive.Exists, unit._jumpDrive.EnergyCost);
    }

    /// <inheritdoc/>
    public override float Gravity => 0.0012f;

    /// <inheritdoc/>
    public override float Radius => 14f;

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.ClassicShipPlayerUnit;

    /// <summary>
    /// Visible snapshot of the classic-ship engine configuration and tick runtime.
    /// </summary>
    public ClassicShipEngineSubsystemInfo Engine
    {
        get { return _engine; }
    }

    /// <summary>
    /// Visible snapshot of the nebula collector subsystem.
    /// </summary>
    public NebulaCollectorSubsystemInfo NebulaCollector
    {
        get { return _nebulaCollector; }
    }

    /// <summary>
    /// Visible snapshot of the shot launcher subsystem and its configured shot profile.
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
    /// Visible snapshot of the interceptor launcher subsystem and its configured interceptor profile.
    /// </summary>
    public DynamicInterceptorLauncherSubsystemInfo InterceptorLauncher
    {
        get { return _interceptorLauncher; }
    }

    /// <summary>
    /// Visible snapshot of the interceptor magazine subsystem.
    /// </summary>
    public DynamicInterceptorMagazineSubsystemInfo InterceptorMagazine
    {
        get { return _interceptorMagazine; }
    }

    /// <summary>
    /// Visible snapshot of the interceptor fabricator subsystem.
    /// </summary>
    public DynamicInterceptorFabricatorSubsystemInfo InterceptorFabricator
    {
        get { return _interceptorFabricator; }
    }

    /// <summary>
    /// Visible snapshot of the railgun subsystem.
    /// </summary>
    public RailgunSubsystemInfo Railgun
    {
        get { return _railgun; }
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
    /// On the current reference classic ship loadout this is usually not installed, so
    /// <see cref="DynamicScannerSubsystemInfo.Exists" /> is often <see langword="false" />.
    /// </summary>
    public DynamicScannerSubsystemInfo SecondaryScanner
    {
        get { return _secondaryScanner; }
    }

    /// <summary>
    /// Visible snapshot of the jump-drive subsystem.
    /// </summary>
    public JumpDriveSubsystemInfo JumpDrive
    {
        get { return _jumpDrive; }
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
        return $"{base.ToString()}, NebulaCollectorExists={_nebulaCollector.Exists}, NebulaCollectorStatus={_nebulaCollector.Status}, NebulaCollectorRate={_nebulaCollector.Rate:0.###}, NebulaCollected={_nebulaCollector.CollectedThisTick:0.###}, NebulaHue={_nebulaCollector.CollectedHueThisTick:0.###}, EngineExists={_engine.Exists}, EngineStatus={_engine.Status}, EngineCurrent={_engine.Current}, EngineTarget={_engine.Target}, " +
               $"EngineConsumed=({_engine.ConsumedEnergyThisTick:0.###},{_engine.ConsumedIonsThisTick:0.###},{_engine.ConsumedNeutrinosThisTick:0.###}), " +
               $"MainScannerExists={_mainScanner.Exists}, MainScannerStatus={_mainScanner.Status}, MainScannerActive={_mainScanner.Active}, MainScannerCurrent=({_mainScanner.CurrentWidth:0.###},{_mainScanner.CurrentLength:0.###},{_mainScanner.CurrentAngle:0.###}), " +
               $"MainScannerTarget=({_mainScanner.TargetWidth:0.###},{_mainScanner.TargetLength:0.###},{_mainScanner.TargetAngle:0.###}), MainScannerConsumed=({_mainScanner.ConsumedEnergyThisTick:0.###},{_mainScanner.ConsumedIonsThisTick:0.###},{_mainScanner.ConsumedNeutrinosThisTick:0.###}), " +
               $"SecondaryScannerExists={_secondaryScanner.Exists}, SecondaryScannerStatus={_secondaryScanner.Status}, SecondaryScannerActive={_secondaryScanner.Active}, SecondaryScannerCurrent=({_secondaryScanner.CurrentWidth:0.###},{_secondaryScanner.CurrentLength:0.###},{_secondaryScanner.CurrentAngle:0.###}), " +
               $"SecondaryScannerTarget=({_secondaryScanner.TargetWidth:0.###},{_secondaryScanner.TargetLength:0.###},{_secondaryScanner.TargetAngle:0.###}), SecondaryScannerConsumed=({_secondaryScanner.ConsumedEnergyThisTick:0.###},{_secondaryScanner.ConsumedIonsThisTick:0.###},{_secondaryScanner.ConsumedNeutrinosThisTick:0.###}), " +
               $"ShotLauncherExists={_shotLauncher.Exists}, ShotLauncherStatus={_shotLauncher.Status}, ShotLauncherRelativeMovement={_shotLauncher.RelativeMovement}, ShotLauncherTicks={_shotLauncher.Ticks}, ShotLauncherLoad={_shotLauncher.Load:0.###}, ShotLauncherDamage={_shotLauncher.Damage:0.###}, ShotLauncherConsumed=({_shotLauncher.ConsumedEnergyThisTick:0.###},{_shotLauncher.ConsumedIonsThisTick:0.###},{_shotLauncher.ConsumedNeutrinosThisTick:0.###}), " +
               $"ShotMagazineExists={_shotMagazine.Exists}, ShotMagazineStatus={_shotMagazine.Status}, ShotMagazine={_shotMagazine.CurrentShots:0.###}/{_shotMagazine.MaximumShots:0.###}, " +
               $"ShotFabricatorExists={_shotFabricator.Exists}, ShotFabricatorStatus={_shotFabricator.Status}, ShotFabricatorActive={_shotFabricator.Active}, ShotFabricatorRate={_shotFabricator.Rate:0.###}, ShotFabricatorConsumed=({_shotFabricator.ConsumedEnergyThisTick:0.###},{_shotFabricator.ConsumedIonsThisTick:0.###},{_shotFabricator.ConsumedNeutrinosThisTick:0.###}), " +
               $"InterceptorLauncherExists={_interceptorLauncher.Exists}, InterceptorLauncherStatus={_interceptorLauncher.Status}, InterceptorLauncherRelativeMovement={_interceptorLauncher.RelativeMovement}, InterceptorLauncherTicks={_interceptorLauncher.Ticks}, InterceptorLauncherLoad={_interceptorLauncher.Load:0.###}, InterceptorLauncherDamage={_interceptorLauncher.Damage:0.###}, InterceptorLauncherConsumed=({_interceptorLauncher.ConsumedEnergyThisTick:0.###},{_interceptorLauncher.ConsumedIonsThisTick:0.###},{_interceptorLauncher.ConsumedNeutrinosThisTick:0.###}), " +
               $"InterceptorMagazineExists={_interceptorMagazine.Exists}, InterceptorMagazineStatus={_interceptorMagazine.Status}, InterceptorMagazine={_interceptorMagazine.CurrentShots:0.###}/{_interceptorMagazine.MaximumShots:0.###}, " +
               $"InterceptorFabricatorExists={_interceptorFabricator.Exists}, InterceptorFabricatorStatus={_interceptorFabricator.Status}, InterceptorFabricatorActive={_interceptorFabricator.Active}, InterceptorFabricatorRate={_interceptorFabricator.Rate:0.###}, InterceptorFabricatorConsumed=({_interceptorFabricator.ConsumedEnergyThisTick:0.###},{_interceptorFabricator.ConsumedIonsThisTick:0.###},{_interceptorFabricator.ConsumedNeutrinosThisTick:0.###}), " +
               $"RailgunExists={_railgun.Exists}, RailgunStatus={_railgun.Status}, RailgunDirection={_railgun.Direction}, RailgunEnergyCost={_railgun.EnergyCost:0.###}, RailgunMetalCost={_railgun.MetalCost:0.###}, RailgunConsumed=({_railgun.ConsumedEnergyThisTick:0.###},{_railgun.ConsumedIonsThisTick:0.###},{_railgun.ConsumedNeutrinosThisTick:0.###}), " +
               $"JumpDriveExists={_jumpDrive.Exists}, JumpDriveEnergyCost={_jumpDrive.EnergyCost:0.###}";
    }

    internal override void UpdateState(PacketReader reader)
    {
        base.UpdateState(reader);

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
            !Vector.FromReader(reader, out Vector interceptorLauncherRelativeMovement) ||
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
            !reader.Read(out float jumpDriveEnergyCost))
            throw new InvalidDataException("Couldn't read Unit.");

        _nebulaCollector.Update(nebulaCollectorExists != 0, nebulaCollectorMinimumRate, nebulaCollectorMaximumRate, nebulaCollectorRate,
            (SubsystemStatus)nebulaCollectorStatus, nebulaCollectorConsumedEnergyThisTick, nebulaCollectorConsumedIonsThisTick,
            nebulaCollectorConsumedNeutrinosThisTick, nebulaCollectorCollectedThisTick, nebulaCollectorCollectedHueThisTick);
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
        _interceptorLauncher.Update(interceptorLauncherExists != 0, interceptorLauncherMinimumRelativeMovement,
            interceptorLauncherMaximumRelativeMovement, interceptorLauncherMinimumTicks, interceptorLauncherMaximumTicks,
            interceptorLauncherMinimumLoad, interceptorLauncherMaximumLoad, interceptorLauncherMinimumDamage,
            interceptorLauncherMaximumDamage, interceptorLauncherRelativeMovement, interceptorLauncherTicks, interceptorLauncherLoad,
            interceptorLauncherDamage, (SubsystemStatus)interceptorLauncherStatus, interceptorLauncherConsumedEnergyThisTick,
            interceptorLauncherConsumedIonsThisTick, interceptorLauncherConsumedNeutrinosThisTick);
        _interceptorMagazine.Update(interceptorMagazineExists != 0, interceptorMagazineMaximumShots, interceptorMagazineCurrentShots,
            (SubsystemStatus)interceptorMagazineStatus);
        _interceptorFabricator.Update(interceptorFabricatorExists != 0, interceptorFabricatorMinimumRate, interceptorFabricatorMaximumRate,
            interceptorFabricatorActive != 0, interceptorFabricatorRate, (SubsystemStatus)interceptorFabricatorStatus,
            interceptorFabricatorConsumedEnergyThisTick, interceptorFabricatorConsumedIonsThisTick,
            interceptorFabricatorConsumedNeutrinosThisTick);
        _railgun.Update(railgunExists != 0, railgunEnergyCost, railgunMetalCost,
            (Flattiverse.Connector.GalaxyHierarchy.RailgunDirection)railgunDirection, (SubsystemStatus)railgunStatus,
            railgunConsumedEnergyThisTick, railgunConsumedIonsThisTick, railgunConsumedNeutrinosThisTick);
        _jumpDrive.Update(jumpDriveExists != 0, jumpDriveEnergyCost);
    }
}
