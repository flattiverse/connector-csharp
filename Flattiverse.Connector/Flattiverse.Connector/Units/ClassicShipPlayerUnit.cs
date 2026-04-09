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
        _shotFabricator.Update(unit._shotFabricator.Exists, unit._shotFabricator.MaximumRate, unit._shotFabricator.Active,
            unit._shotFabricator.Rate, unit._shotFabricator.Status, unit._shotFabricator.ConsumedEnergyThisTick,
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
        _interceptorFabricator.Update(unit._interceptorFabricator.Exists, unit._interceptorFabricator.MaximumRate,
            unit._interceptorFabricator.Active, unit._interceptorFabricator.Rate,
            unit._interceptorFabricator.Status, unit._interceptorFabricator.ConsumedEnergyThisTick,
            unit._interceptorFabricator.ConsumedIonsThisTick, unit._interceptorFabricator.ConsumedNeutrinosThisTick);
        _railgun = new ClassicRailgunSubsystemInfo();
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

        if (!ReadNebulaCollectorState(reader, _nebulaCollector) ||
            !ReadScannerState(reader, _mainScanner) ||
            !ReadScannerState(reader, _secondaryScanner) ||
            !ReadEngineState(reader, _engine) ||
            !ReadLauncherState(reader, _shotLauncher) ||
            !ReadMagazineState(reader, _shotMagazine) ||
            !ReadFabricatorState(reader, _shotFabricator) ||
            !ReadLauncherState(reader, _interceptorLauncher) ||
            !ReadMagazineState(reader, _interceptorMagazine) ||
            !ReadFabricatorState(reader, _interceptorFabricator) ||
            !ReadRailgunState(reader, _railgun) ||
            !ReadJumpDriveState(reader, _jumpDrive))
            throw new InvalidDataException("Couldn't read Unit.");
    }

    private static bool ReadNebulaCollectorState(PacketReader reader, NebulaCollectorSubsystemInfo nebulaCollector)
    {
        if (!reader.Read(out byte exists))
            return false;

        if (exists == 0)
        {
            nebulaCollector.Update(false, 0f, 0f, 0f, SubsystemStatus.Off, 0f, 0f, 0f, 0f, 0f);
            return true;
        }

        if (!reader.Read(out float minimumRate) ||
            !reader.Read(out float maximumRate) ||
            !reader.Read(out float rate) ||
            !reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick) ||
            !reader.Read(out float collectedThisTick) ||
            !reader.Read(out float collectedHueThisTick))
            return false;

        nebulaCollector.Update(true, minimumRate, maximumRate, rate, (SubsystemStatus)status, consumedEnergyThisTick, consumedIonsThisTick,
            consumedNeutrinosThisTick, collectedThisTick, collectedHueThisTick);
        return true;
    }

    private static bool ReadScannerState(PacketReader reader, DynamicScannerSubsystemInfo scanner)
    {
        if (!reader.Read(out byte exists))
            return false;

        if (exists == 0)
        {
            scanner.Update(false, 0f, 0f, 0f, 0f, 0f, false, 0f, 0f, 0f, 0f, 0f, 0f, SubsystemStatus.Off, 0f, 0f, 0f);
            return true;
        }

        if (!reader.Read(out float maximumWidth) ||
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

        scanner.Update(true, maximumWidth, maximumLength, widthSpeed, lengthSpeed, angleSpeed, active != 0, currentWidth, currentLength,
            currentAngle, targetWidth, targetLength, targetAngle, (SubsystemStatus)status, consumedEnergyThisTick, consumedIonsThisTick,
            consumedNeutrinosThisTick);
        return true;
    }

    private static bool ReadEngineState(PacketReader reader, ClassicShipEngineSubsystemInfo engine)
    {
        if (!reader.Read(out byte exists))
            return false;

        if (exists == 0)
        {
            engine.Update(false, 0f, new Vector(), new Vector(), SubsystemStatus.Off, 0f, 0f, 0f);
            return true;
        }

        if (!reader.Read(out float maximum) ||
            !Vector.FromReader(reader, out Vector current) ||
            !Vector.FromReader(reader, out Vector target) ||
            !reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick))
            return false;

        engine.Update(true, maximum, current, target, (SubsystemStatus)status, consumedEnergyThisTick, consumedIonsThisTick,
            consumedNeutrinosThisTick);
        return true;
    }

    private static bool ReadLauncherState(PacketReader reader, DynamicShotLauncherSubsystemInfo launcher)
    {
        if (!reader.Read(out byte exists))
            return false;

        if (exists == 0)
        {
            launcher.Update(false, 0f, 0f, 0, 0, 0f, 0f, 0f, 0f, new Vector(), 0, 0f, 0f, SubsystemStatus.Off, 0f, 0f, 0f);
            return true;
        }

        if (!reader.Read(out float minimumRelativeMovement) ||
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

        launcher.Update(true, minimumRelativeMovement, maximumRelativeMovement, minimumTicks, maximumTicks, minimumLoad, maximumLoad,
            minimumDamage, maximumDamage, relativeMovement, ticks, load, damage, (SubsystemStatus)status, consumedEnergyThisTick,
            consumedIonsThisTick, consumedNeutrinosThisTick);
        return true;
    }

    private static bool ReadMagazineState(PacketReader reader, DynamicShotMagazineSubsystemInfo magazine)
    {
        if (!reader.Read(out byte exists))
            return false;

        if (exists == 0)
        {
            magazine.Update(false, 0f, 0f, SubsystemStatus.Off);
            return true;
        }

        if (!reader.Read(out float maximumShots) ||
            !reader.Read(out float currentShots) ||
            !reader.Read(out byte status))
            return false;

        magazine.Update(true, maximumShots, currentShots, (SubsystemStatus)status);
        return true;
    }

    private static bool ReadFabricatorState(PacketReader reader, DynamicShotFabricatorSubsystemInfo fabricator)
    {
        if (!reader.Read(out byte exists))
            return false;

        if (exists == 0)
        {
            fabricator.Update(false, 0f, false, 0f, SubsystemStatus.Off, 0f, 0f, 0f);
            return true;
        }

        if (!reader.Read(out float maximumRate) ||
            !reader.Read(out byte active) ||
            !reader.Read(out float rate) ||
            !reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick))
            return false;

        fabricator.Update(true, maximumRate, active != 0, rate, (SubsystemStatus)status, consumedEnergyThisTick, consumedIonsThisTick,
            consumedNeutrinosThisTick);
        return true;
    }

    private static bool ReadRailgunState(PacketReader reader, ClassicRailgunSubsystemInfo railgun)
    {
        if (!reader.Read(out byte exists))
            return false;

        if (exists == 0)
        {
            railgun.Update(false, 0f, 0f, Flattiverse.Connector.GalaxyHierarchy.RailgunDirection.None, SubsystemStatus.Off, 0f, 0f, 0f);
            return true;
        }

        if (!reader.Read(out float energyCost) ||
            !reader.Read(out float metalCost) ||
            !reader.Read(out byte direction) ||
            !reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick))
            return false;

        railgun.Update(true, energyCost, metalCost, (Flattiverse.Connector.GalaxyHierarchy.RailgunDirection)direction,
            (SubsystemStatus)status, consumedEnergyThisTick, consumedIonsThisTick, consumedNeutrinosThisTick);
        return true;
    }

    private static bool ReadJumpDriveState(PacketReader reader, JumpDriveSubsystemInfo jumpDrive)
    {
        if (!reader.Read(out byte exists))
            return false;

        if (exists == 0)
        {
            jumpDrive.Update(false, 0f);
            return true;
        }

        if (!reader.Read(out float energyCost))
            return false;

        jumpDrive.Update(true, energyCost);
        return true;
    }
}
