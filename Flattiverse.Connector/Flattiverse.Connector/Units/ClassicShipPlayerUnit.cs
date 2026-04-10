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
    private const float StartingEffectiveStructuralLoad = 22f;

    private readonly NebulaCollectorSubsystemInfo _nebulaCollector;
    private readonly ClassicShipEngineSubsystemInfo _engine;
    private readonly DynamicShotLauncherSubsystemInfo _shotLauncher;
    private readonly DynamicShotMagazineSubsystemInfo _shotMagazine;
    private readonly DynamicShotFabricatorSubsystemInfo _shotFabricator;
    private readonly DynamicInterceptorLauncherSubsystemInfo _interceptorLauncher;
    private readonly DynamicInterceptorMagazineSubsystemInfo _interceptorMagazine;
    private readonly DynamicInterceptorFabricatorSubsystemInfo _interceptorFabricator;
    private readonly ClassicRailgunSubsystemInfo _railgun;
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
        _railgun = new ClassicRailgunSubsystemInfo();
        _mainScanner = new DynamicScannerSubsystemInfo();
        _secondaryScanner = new DynamicScannerSubsystemInfo();
        _jumpDrive = new JumpDriveSubsystemInfo();
    }

    internal ClassicShipPlayerUnit(ClassicShipPlayerUnit unit) : base(unit)
    {
        _nebulaCollector = new NebulaCollectorSubsystemInfo(unit._nebulaCollector);
        _engine = new ClassicShipEngineSubsystemInfo(unit._engine);
        _shotLauncher = new DynamicShotLauncherSubsystemInfo(unit._shotLauncher);
        _shotMagazine = new DynamicShotMagazineSubsystemInfo(unit._shotMagazine);
        _shotFabricator = new DynamicShotFabricatorSubsystemInfo(unit._shotFabricator);
        _interceptorLauncher = new DynamicInterceptorLauncherSubsystemInfo(unit._interceptorLauncher);
        _interceptorMagazine = new DynamicInterceptorMagazineSubsystemInfo(unit._interceptorMagazine);
        _interceptorFabricator = new DynamicInterceptorFabricatorSubsystemInfo(unit._interceptorFabricator);
        _railgun = new ClassicRailgunSubsystemInfo(unit._railgun);
        _mainScanner = new DynamicScannerSubsystemInfo(unit._mainScanner);
        _secondaryScanner = new DynamicScannerSubsystemInfo(unit._secondaryScanner);
        _jumpDrive = new JumpDriveSubsystemInfo(unit._jumpDrive);
    }

    /// <inheritdoc/>
    public override float Gravity
    {
        get
        {
            if (TryGetOwnControllable(out Controllable? controllable) && controllable is ClassicShipControllable classicControllable)
                return classicControllable.Gravity;

            return ShipBalancing.CalculateGravity(StartingEffectiveStructuralLoad);
        }
    }

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
    public ClassicRailgunSubsystemInfo Railgun
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
            throw new InvalidDataException("Couldn't read Unit.");
    }
}
