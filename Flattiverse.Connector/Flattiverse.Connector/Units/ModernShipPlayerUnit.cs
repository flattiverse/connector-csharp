using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// Visible snapshot of a modern-ship player unit in a cluster.
/// </summary>
public class ModernShipPlayerUnit : PlayerUnit
{
    private const float StartingEffectiveStructuralLoad = 17.4f;
    private const float StartingRadius = 14f;

    private readonly NebulaCollectorSubsystemInfo _nebulaCollector;
    private readonly ModernShipEngineSubsystemInfo[] _engines;
    private readonly DynamicScannerSubsystemInfo[] _scanners;
    private readonly DynamicShotLauncherSubsystemInfo[] _shotLaunchers;
    private readonly StaticShotMagazineSubsystemInfo[] _shotMagazines;
    private readonly DynamicShotFabricatorSubsystemInfo[] _shotFabricators;
    private readonly DynamicInterceptorLauncherSubsystemInfo[] _interceptorLaunchers;
    private readonly StaticInterceptorMagazineSubsystemInfo[] _interceptorMagazines;
    private readonly DynamicInterceptorFabricatorSubsystemInfo[] _interceptorFabricators;
    private readonly ModernRailgunSubsystemInfo[] _railguns;
    private readonly JumpDriveSubsystemInfo _jumpDrive;

    internal ModernShipPlayerUnit(Cluster cluster, string name, PacketReader reader) : base(cluster, name, reader)
    {
        _nebulaCollector = new NebulaCollectorSubsystemInfo();
        _engines = CreateEngineInfos(ModernShipGeometry.EngineSlots.Length);
        _scanners = CreateScannerInfos(ModernShipGeometry.ScannerSlots.Length);
        _shotLaunchers = CreateShotLauncherInfos(ModernShipGeometry.ShotLauncherSlots.Length);
        _shotMagazines = CreateShotMagazineInfos(ModernShipGeometry.ShotMagazineSlots.Length);
        _shotFabricators = CreateShotFabricatorInfos(ModernShipGeometry.ShotFabricatorSlots.Length);
        _interceptorLaunchers = CreateInterceptorLauncherInfos(2);
        _interceptorMagazines = CreateInterceptorMagazineInfos(2);
        _interceptorFabricators = CreateInterceptorFabricatorInfos(2);
        _railguns = CreateRailgunInfos(ModernShipGeometry.RailgunSlots.Length);
        _jumpDrive = new JumpDriveSubsystemInfo();
    }

    internal ModernShipPlayerUnit(ModernShipPlayerUnit unit) : base(unit)
    {
        _nebulaCollector = new NebulaCollectorSubsystemInfo(unit._nebulaCollector);
        _engines = CloneEngines(unit._engines);
        _scanners = CloneScanners(unit._scanners);
        _shotLaunchers = CloneShotLaunchers(unit._shotLaunchers);
        _shotMagazines = CloneShotMagazines(unit._shotMagazines);
        _shotFabricators = CloneShotFabricators(unit._shotFabricators);
        _interceptorLaunchers = CloneInterceptorLaunchers(unit._interceptorLaunchers);
        _interceptorMagazines = CloneInterceptorMagazines(unit._interceptorMagazines);
        _interceptorFabricators = CloneInterceptorFabricators(unit._interceptorFabricators);
        _railguns = CloneRailguns(unit._railguns);
        _jumpDrive = new JumpDriveSubsystemInfo(unit._jumpDrive);
    }

    /// <inheritdoc/>
    public override float Gravity
    {
        get
        {
            if (TryGetOwnControllable(out Controllable? controllable) && controllable is ModernShipControllable modernControllable)
                return modernControllable.Gravity;

            float effectiveStructureLoad = FullStateKnown ? EffectiveStructureLoad : StartingEffectiveStructuralLoad;
            return ShipBalancing.CalculateGravity(effectiveStructureLoad);
        }
    }

    /// <inheritdoc/>
    public override float Radius
    {
        get
        {
            if (!FullStateKnown)
                return StartingRadius;

            return ShipBalancing.CalculateRadius(EffectiveStructureLoad);
        }
    }

    /// <inheritdoc/>
    public override float SpeedLimit
    {
        get
        {
            float effectiveStructureLoad = FullStateKnown ? EffectiveStructureLoad : StartingEffectiveStructuralLoad;
            return ShipBalancing.CalculateModernSpeedLimit(effectiveStructureLoad);
        }
    }

    /// <inheritdoc/>
    public override UnitKind Kind => UnitKind.ModernShipPlayerUnit;
    public NebulaCollectorSubsystemInfo NebulaCollector => _nebulaCollector;
    public IReadOnlyList<ModernShipEngineSubsystemInfo> Engines => _engines;
    public IReadOnlyList<DynamicScannerSubsystemInfo> Scanners => _scanners;
    public IReadOnlyList<DynamicShotLauncherSubsystemInfo> ShotLaunchers => _shotLaunchers;
    public IReadOnlyList<StaticShotMagazineSubsystemInfo> ShotMagazines => _shotMagazines;
    public IReadOnlyList<DynamicShotFabricatorSubsystemInfo> ShotFabricators => _shotFabricators;
    public IReadOnlyList<DynamicInterceptorLauncherSubsystemInfo> InterceptorLaunchers => _interceptorLaunchers;
    public IReadOnlyList<StaticInterceptorMagazineSubsystemInfo> InterceptorMagazines => _interceptorMagazines;
    public IReadOnlyList<DynamicInterceptorFabricatorSubsystemInfo> InterceptorFabricators => _interceptorFabricators;
    public IReadOnlyList<ModernRailgunSubsystemInfo> Railguns => _railguns;
    public JumpDriveSubsystemInfo JumpDrive => _jumpDrive;

    public override Unit Clone()
    {
        return new ModernShipPlayerUnit(this);
    }

    internal override void UpdateState(PacketReader reader)
    {
        base.UpdateState(reader);

        if (!_nebulaCollector.Update(reader))
            throw new InvalidDataException("Couldn't read Unit.");

        for (int index = 0; index < _scanners.Length; index++)
            if (!_scanners[index].Update(reader))
                throw new InvalidDataException("Couldn't read Unit.");

        for (int index = 0; index < _engines.Length; index++)
            if (!_engines[index].Update(reader))
                throw new InvalidDataException("Couldn't read Unit.");

        for (int index = 0; index < _shotLaunchers.Length; index++)
            if (!_shotLaunchers[index].Update(reader) ||
                !_shotMagazines[index].Update(reader) ||
                !_shotFabricators[index].Update(reader))
                throw new InvalidDataException("Couldn't read Unit.");

        for (int index = 0; index < _interceptorLaunchers.Length; index++)
            if (!_interceptorLaunchers[index].Update(reader) ||
                !_interceptorMagazines[index].Update(reader) ||
                !_interceptorFabricators[index].Update(reader))
                throw new InvalidDataException("Couldn't read Unit.");

        for (int index = 0; index < _railguns.Length; index++)
            if (!_railguns[index].Update(reader))
                throw new InvalidDataException("Couldn't read Unit.");

        if (!_jumpDrive.Update(reader))
            throw new InvalidDataException("Couldn't read Unit.");
    }

    private static ModernShipEngineSubsystemInfo[] CloneEngines(ModernShipEngineSubsystemInfo[] source)
    {
        ModernShipEngineSubsystemInfo[] clone = CreateEngineInfos(source.Length);

        for (int index = 0; index < clone.Length; index++)
            clone[index] = new ModernShipEngineSubsystemInfo(source[index]);

        return clone;
    }

    private static DynamicScannerSubsystemInfo[] CloneScanners(DynamicScannerSubsystemInfo[] source)
    {
        DynamicScannerSubsystemInfo[] clone = CreateScannerInfos(source.Length);

        for (int index = 0; index < clone.Length; index++)
            clone[index] = new DynamicScannerSubsystemInfo(source[index]);

        return clone;
    }

    private static DynamicShotLauncherSubsystemInfo[] CloneShotLaunchers(DynamicShotLauncherSubsystemInfo[] source)
    {
        DynamicShotLauncherSubsystemInfo[] clone = CreateShotLauncherInfos(source.Length);

        for (int index = 0; index < clone.Length; index++)
            clone[index] = new DynamicShotLauncherSubsystemInfo(source[index]);

        return clone;
    }

    private static StaticShotMagazineSubsystemInfo[] CloneShotMagazines(StaticShotMagazineSubsystemInfo[] source)
    {
        StaticShotMagazineSubsystemInfo[] clone = CreateShotMagazineInfos(source.Length);

        for (int index = 0; index < clone.Length; index++)
            clone[index] = new StaticShotMagazineSubsystemInfo(source[index]);

        return clone;
    }

    private static DynamicShotFabricatorSubsystemInfo[] CloneShotFabricators(DynamicShotFabricatorSubsystemInfo[] source)
    {
        DynamicShotFabricatorSubsystemInfo[] clone = CreateShotFabricatorInfos(source.Length);

        for (int index = 0; index < clone.Length; index++)
            clone[index] = new DynamicShotFabricatorSubsystemInfo(source[index]);

        return clone;
    }

    private static DynamicInterceptorLauncherSubsystemInfo[] CloneInterceptorLaunchers(DynamicInterceptorLauncherSubsystemInfo[] source)
    {
        DynamicInterceptorLauncherSubsystemInfo[] clone = CreateInterceptorLauncherInfos(source.Length);

        for (int index = 0; index < clone.Length; index++)
            clone[index] = new DynamicInterceptorLauncherSubsystemInfo(source[index]);

        return clone;
    }

    private static StaticInterceptorMagazineSubsystemInfo[] CloneInterceptorMagazines(StaticInterceptorMagazineSubsystemInfo[] source)
    {
        StaticInterceptorMagazineSubsystemInfo[] clone = CreateInterceptorMagazineInfos(source.Length);

        for (int index = 0; index < clone.Length; index++)
            clone[index] = new StaticInterceptorMagazineSubsystemInfo(source[index]);

        return clone;
    }

    private static DynamicInterceptorFabricatorSubsystemInfo[] CloneInterceptorFabricators(DynamicInterceptorFabricatorSubsystemInfo[] source)
    {
        DynamicInterceptorFabricatorSubsystemInfo[] clone = CreateInterceptorFabricatorInfos(source.Length);

        for (int index = 0; index < clone.Length; index++)
            clone[index] = new DynamicInterceptorFabricatorSubsystemInfo(source[index]);

        return clone;
    }

    private static ModernRailgunSubsystemInfo[] CloneRailguns(ModernRailgunSubsystemInfo[] source)
    {
        ModernRailgunSubsystemInfo[] clone = CreateRailgunInfos(source.Length);

        for (int index = 0; index < clone.Length; index++)
            clone[index] = new ModernRailgunSubsystemInfo(source[index]);

        return clone;
    }

    private static ModernShipEngineSubsystemInfo[] CreateEngineInfos(int length)
    {
        ModernShipEngineSubsystemInfo[] infos = new ModernShipEngineSubsystemInfo[length];

        for (int index = 0; index < infos.Length; index++)
            infos[index] = new ModernShipEngineSubsystemInfo();

        return infos;
    }

    private static DynamicScannerSubsystemInfo[] CreateScannerInfos(int length)
    {
        DynamicScannerSubsystemInfo[] infos = new DynamicScannerSubsystemInfo[length];

        for (int index = 0; index < infos.Length; index++)
            infos[index] = new DynamicScannerSubsystemInfo();

        return infos;
    }

    private static DynamicShotLauncherSubsystemInfo[] CreateShotLauncherInfos(int length)
    {
        DynamicShotLauncherSubsystemInfo[] infos = new DynamicShotLauncherSubsystemInfo[length];

        for (int index = 0; index < infos.Length; index++)
            infos[index] = new DynamicShotLauncherSubsystemInfo();

        return infos;
    }

    private static StaticShotMagazineSubsystemInfo[] CreateShotMagazineInfos(int length)
    {
        StaticShotMagazineSubsystemInfo[] infos = new StaticShotMagazineSubsystemInfo[length];

        for (int index = 0; index < infos.Length; index++)
            infos[index] = new StaticShotMagazineSubsystemInfo();

        return infos;
    }

    private static DynamicShotFabricatorSubsystemInfo[] CreateShotFabricatorInfos(int length)
    {
        DynamicShotFabricatorSubsystemInfo[] infos = new DynamicShotFabricatorSubsystemInfo[length];

        for (int index = 0; index < infos.Length; index++)
            infos[index] = new DynamicShotFabricatorSubsystemInfo();

        return infos;
    }

    private static DynamicInterceptorLauncherSubsystemInfo[] CreateInterceptorLauncherInfos(int length)
    {
        DynamicInterceptorLauncherSubsystemInfo[] infos = new DynamicInterceptorLauncherSubsystemInfo[length];

        for (int index = 0; index < infos.Length; index++)
            infos[index] = new DynamicInterceptorLauncherSubsystemInfo();

        return infos;
    }

    private static StaticInterceptorMagazineSubsystemInfo[] CreateInterceptorMagazineInfos(int length)
    {
        StaticInterceptorMagazineSubsystemInfo[] infos = new StaticInterceptorMagazineSubsystemInfo[length];

        for (int index = 0; index < infos.Length; index++)
            infos[index] = new StaticInterceptorMagazineSubsystemInfo();

        return infos;
    }

    private static DynamicInterceptorFabricatorSubsystemInfo[] CreateInterceptorFabricatorInfos(int length)
    {
        DynamicInterceptorFabricatorSubsystemInfo[] infos = new DynamicInterceptorFabricatorSubsystemInfo[length];

        for (int index = 0; index < infos.Length; index++)
            infos[index] = new DynamicInterceptorFabricatorSubsystemInfo();

        return infos;
    }

    private static ModernRailgunSubsystemInfo[] CreateRailgunInfos(int length)
    {
        ModernRailgunSubsystemInfo[] infos = new ModernRailgunSubsystemInfo[length];

        for (int index = 0; index < infos.Length; index++)
            infos[index] = new ModernRailgunSubsystemInfo();

        return infos;
    }

}
