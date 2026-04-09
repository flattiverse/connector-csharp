using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.Units;

/// <summary>
/// Visible snapshot of a modern-ship player unit in a cluster.
/// </summary>
public class ModernShipPlayerUnit : PlayerUnit
{
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
        _nebulaCollector = new NebulaCollectorSubsystemInfo();
        _nebulaCollector.Update(unit._nebulaCollector.Exists, unit._nebulaCollector.MinimumRate, unit._nebulaCollector.MaximumRate,
            unit._nebulaCollector.Rate, unit._nebulaCollector.Status, unit._nebulaCollector.ConsumedEnergyThisTick,
            unit._nebulaCollector.ConsumedIonsThisTick, unit._nebulaCollector.ConsumedNeutrinosThisTick,
            unit._nebulaCollector.CollectedThisTick, unit._nebulaCollector.CollectedHueThisTick);
        _engines = CloneEngines(unit._engines);
        _scanners = CloneScanners(unit._scanners);
        _shotLaunchers = CloneShotLaunchers(unit._shotLaunchers);
        _shotMagazines = CloneShotMagazines(unit._shotMagazines);
        _shotFabricators = CloneShotFabricators(unit._shotFabricators);
        _interceptorLaunchers = CloneInterceptorLaunchers(unit._interceptorLaunchers);
        _interceptorMagazines = CloneInterceptorMagazines(unit._interceptorMagazines);
        _interceptorFabricators = CloneInterceptorFabricators(unit._interceptorFabricators);
        _railguns = CloneRailguns(unit._railguns);
        _jumpDrive = new JumpDriveSubsystemInfo();
        _jumpDrive.Update(unit._jumpDrive.Exists, unit._jumpDrive.EnergyCost);
    }

    public override float Gravity => 0.0012f;
    public override float Radius => ModernShipGeometry.Radius;
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

        if (!ReadNebulaCollectorState(reader))
            throw new InvalidDataException("Couldn't read Unit.");

        for (int index = 0; index < _scanners.Length; index++)
            if (!ReadScannerState(reader, _scanners[index]))
                throw new InvalidDataException("Couldn't read Unit.");

        for (int index = 0; index < _engines.Length; index++)
            if (!ReadEngineState(reader, _engines[index]))
                throw new InvalidDataException("Couldn't read Unit.");

        for (int index = 0; index < _shotLaunchers.Length; index++)
            if (!ReadLauncherState(reader, _shotLaunchers[index]) ||
                !ReadMagazineState(reader, _shotMagazines[index]) ||
                !ReadFabricatorState(reader, _shotFabricators[index]))
                throw new InvalidDataException("Couldn't read Unit.");

        for (int index = 0; index < _interceptorLaunchers.Length; index++)
            if (!ReadLauncherState(reader, _interceptorLaunchers[index]) ||
                !ReadMagazineState(reader, _interceptorMagazines[index]) ||
                !ReadFabricatorState(reader, _interceptorFabricators[index]))
                throw new InvalidDataException("Couldn't read Unit.");

        for (int index = 0; index < _railguns.Length; index++)
            if (!ReadRailgunState(reader, _railguns[index]))
                throw new InvalidDataException("Couldn't read Unit.");

        if (!reader.Read(out byte jumpDriveExists))
            throw new InvalidDataException("Couldn't read Unit.");

        if (jumpDriveExists == 0)
        {
            _jumpDrive.Update(false, 0f);
            return;
        }

        if (!reader.Read(out float jumpDriveEnergyCost))
            throw new InvalidDataException("Couldn't read Unit.");

        _jumpDrive.Update(true, jumpDriveEnergyCost);
    }

    private static ModernShipEngineSubsystemInfo[] CloneEngines(ModernShipEngineSubsystemInfo[] source)
    {
        ModernShipEngineSubsystemInfo[] clone = CreateEngineInfos(source.Length);

        for (int index = 0; index < clone.Length; index++)
            clone[index].Update(source[index].Exists, source[index].MaximumThrust, source[index].MaximumThrustChangePerTick,
                source[index].CurrentThrust, source[index].TargetThrust, source[index].Status,
                source[index].ConsumedEnergyThisTick, source[index].ConsumedIonsThisTick, source[index].ConsumedNeutrinosThisTick);

        return clone;
    }

    private static DynamicScannerSubsystemInfo[] CloneScanners(DynamicScannerSubsystemInfo[] source)
    {
        DynamicScannerSubsystemInfo[] clone = CreateScannerInfos(source.Length);

        for (int index = 0; index < clone.Length; index++)
            clone[index].Update(source[index].Exists, source[index].MaximumWidth, source[index].MaximumLength, source[index].WidthSpeed,
                source[index].LengthSpeed, source[index].AngleSpeed, source[index].Active, source[index].CurrentWidth,
                source[index].CurrentLength, source[index].CurrentAngle, source[index].TargetWidth, source[index].TargetLength,
                source[index].TargetAngle, source[index].Status, source[index].ConsumedEnergyThisTick, source[index].ConsumedIonsThisTick,
                source[index].ConsumedNeutrinosThisTick);

        return clone;
    }

    private static DynamicShotLauncherSubsystemInfo[] CloneShotLaunchers(DynamicShotLauncherSubsystemInfo[] source)
    {
        DynamicShotLauncherSubsystemInfo[] clone = CreateShotLauncherInfos(source.Length);

        for (int index = 0; index < clone.Length; index++)
            clone[index].Update(source[index].Exists, source[index].MinimumRelativeMovement, source[index].MaximumRelativeMovement,
                source[index].MinimumTicks, source[index].MaximumTicks, source[index].MinimumLoad, source[index].MaximumLoad,
                source[index].MinimumDamage, source[index].MaximumDamage, source[index].RelativeMovement, source[index].Ticks,
                source[index].Load, source[index].Damage, source[index].Status, source[index].ConsumedEnergyThisTick,
                source[index].ConsumedIonsThisTick, source[index].ConsumedNeutrinosThisTick);

        return clone;
    }

    private static StaticShotMagazineSubsystemInfo[] CloneShotMagazines(StaticShotMagazineSubsystemInfo[] source)
    {
        StaticShotMagazineSubsystemInfo[] clone = CreateShotMagazineInfos(source.Length);

        for (int index = 0; index < clone.Length; index++)
            clone[index].Update(source[index].Exists, source[index].MaximumShots, source[index].CurrentShots, source[index].Status);

        return clone;
    }

    private static DynamicShotFabricatorSubsystemInfo[] CloneShotFabricators(DynamicShotFabricatorSubsystemInfo[] source)
    {
        DynamicShotFabricatorSubsystemInfo[] clone = CreateShotFabricatorInfos(source.Length);

        for (int index = 0; index < clone.Length; index++)
            clone[index].Update(source[index].Exists, source[index].MaximumRate, source[index].Active, source[index].Rate,
                source[index].Status, source[index].ConsumedEnergyThisTick, source[index].ConsumedIonsThisTick,
                source[index].ConsumedNeutrinosThisTick);

        return clone;
    }

    private static DynamicInterceptorLauncherSubsystemInfo[] CloneInterceptorLaunchers(DynamicInterceptorLauncherSubsystemInfo[] source)
    {
        DynamicInterceptorLauncherSubsystemInfo[] clone = CreateInterceptorLauncherInfos(source.Length);

        for (int index = 0; index < clone.Length; index++)
            clone[index].Update(source[index].Exists, source[index].MinimumRelativeMovement, source[index].MaximumRelativeMovement,
                source[index].MinimumTicks, source[index].MaximumTicks, source[index].MinimumLoad, source[index].MaximumLoad,
                source[index].MinimumDamage, source[index].MaximumDamage, source[index].RelativeMovement, source[index].Ticks,
                source[index].Load, source[index].Damage, source[index].Status, source[index].ConsumedEnergyThisTick,
                source[index].ConsumedIonsThisTick, source[index].ConsumedNeutrinosThisTick);

        return clone;
    }

    private static StaticInterceptorMagazineSubsystemInfo[] CloneInterceptorMagazines(StaticInterceptorMagazineSubsystemInfo[] source)
    {
        StaticInterceptorMagazineSubsystemInfo[] clone = CreateInterceptorMagazineInfos(source.Length);

        for (int index = 0; index < clone.Length; index++)
            clone[index].Update(source[index].Exists, source[index].MaximumShots, source[index].CurrentShots, source[index].Status);

        return clone;
    }

    private static DynamicInterceptorFabricatorSubsystemInfo[] CloneInterceptorFabricators(DynamicInterceptorFabricatorSubsystemInfo[] source)
    {
        DynamicInterceptorFabricatorSubsystemInfo[] clone = CreateInterceptorFabricatorInfos(source.Length);

        for (int index = 0; index < clone.Length; index++)
            clone[index].Update(source[index].Exists, source[index].MaximumRate, source[index].Active, source[index].Rate,
                source[index].Status, source[index].ConsumedEnergyThisTick, source[index].ConsumedIonsThisTick,
                source[index].ConsumedNeutrinosThisTick);

        return clone;
    }

    private static ModernRailgunSubsystemInfo[] CloneRailguns(ModernRailgunSubsystemInfo[] source)
    {
        ModernRailgunSubsystemInfo[] clone = CreateRailgunInfos(source.Length);

        for (int index = 0; index < clone.Length; index++)
            clone[index].Update(source[index].Exists, source[index].EnergyCost, source[index].MetalCost, source[index].Direction,
                source[index].Status, source[index].ConsumedEnergyThisTick, source[index].ConsumedIonsThisTick,
                source[index].ConsumedNeutrinosThisTick);

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

    private bool ReadNebulaCollectorState(PacketReader reader)
    {
        if (!reader.Read(out byte exists))
            return false;

        if (exists == 0)
        {
            _nebulaCollector.Update(false, 0f, 0f, 0f, SubsystemStatus.Off, 0f, 0f, 0f, 0f, 0f);
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

        _nebulaCollector.Update(true, minimumRate, maximumRate, rate, (SubsystemStatus)status, consumedEnergyThisTick,
            consumedIonsThisTick, consumedNeutrinosThisTick, collectedThisTick, collectedHueThisTick);
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

        scanner.Update(true, maximumWidth, maximumLength, widthSpeed, lengthSpeed, angleSpeed, active != 0, currentWidth,
            currentLength, currentAngle, targetWidth, targetLength, targetAngle, (SubsystemStatus)status, consumedEnergyThisTick,
            consumedIonsThisTick, consumedNeutrinosThisTick);
        return true;
    }

    private static bool ReadEngineState(PacketReader reader, ModernShipEngineSubsystemInfo engine)
    {
        if (!reader.Read(out byte exists))
            return false;

        if (exists == 0)
        {
            engine.Update(false, 0f, 0f, 0f, 0f, SubsystemStatus.Off, 0f, 0f, 0f);
            return true;
        }

        if (!reader.Read(out float maximumForwardThrust) ||
            !reader.Read(out float maximumReverseThrust) ||
            !reader.Read(out float maximumThrustChangePerTick) ||
            !reader.Read(out float currentThrust) ||
            !reader.Read(out float targetThrust) ||
            !reader.Read(out byte status) ||
            !reader.Read(out float consumedEnergyThisTick) ||
            !reader.Read(out float consumedIonsThisTick) ||
            !reader.Read(out float consumedNeutrinosThisTick))
            return false;

        engine.Update(true, MathF.Max(maximumForwardThrust, maximumReverseThrust), maximumThrustChangePerTick, currentThrust,
            targetThrust, (SubsystemStatus)status, consumedEnergyThisTick, consumedIonsThisTick, consumedNeutrinosThisTick);
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

        launcher.Update(true, minimumRelativeMovement, maximumRelativeMovement, minimumTicks, maximumTicks, minimumLoad,
            maximumLoad, minimumDamage, maximumDamage, relativeMovement, ticks, load, damage, (SubsystemStatus)status,
            consumedEnergyThisTick, consumedIonsThisTick, consumedNeutrinosThisTick);
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

        fabricator.Update(true, maximumRate, active != 0, rate, (SubsystemStatus)status, consumedEnergyThisTick,
            consumedIonsThisTick, consumedNeutrinosThisTick);
        return true;
    }

    private static bool ReadRailgunState(PacketReader reader, ModernRailgunSubsystemInfo railgun)
    {
        if (!reader.Read(out byte exists))
            return false;

        if (exists == 0)
        {
            railgun.Update(false, 0f, 0f, Flattiverse.Connector.GalaxyHierarchy.RailgunDirection.None,
                SubsystemStatus.Off, 0f, 0f, 0f);
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
}
