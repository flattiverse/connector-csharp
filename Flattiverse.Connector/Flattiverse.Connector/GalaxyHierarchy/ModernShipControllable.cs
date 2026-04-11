using Flattiverse.Connector.Events;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Flattiverse.Connector.GalaxyHierarchy;

/// <summary>
/// Owner-side handle for one registered modern-ship controllable.
/// </summary>
public class ModernShipControllable : Controllable
{
    private readonly NebulaCollectorSubsystem _nebulaCollector;
    private readonly ModernShipEngineSubsystem[] _engines;
    private readonly StaticScannerSubsystem[] _scanners;
    private readonly StaticShotLauncherSubsystem[] _shotLaunchers;
    private readonly StaticShotMagazineSubsystem[] _shotMagazines;
    private readonly StaticShotFabricatorSubsystem[] _shotFabricators;
    private readonly StaticInterceptorLauncherSubsystem[] _interceptorLaunchers;
    private readonly StaticInterceptorMagazineSubsystem[] _interceptorMagazines;
    private readonly StaticInterceptorFabricatorSubsystem[] _interceptorFabricators;
    private readonly ModernRailgunSubsystem[] _railguns;
    private readonly JumpDriveSubsystem _jumpDrive;
    private readonly string[] _equippedCrystals;

    internal ModernShipControllable(Cluster cluster, byte id, string name, PacketReader reader) : base(id, name, cluster, reader)
    {
        _energyBattery = new BatterySubsystem(this, "EnergyBattery", reader, SubsystemSlot.EnergyBattery);
        _ionBattery = new BatterySubsystem(this, "IonBattery", reader, SubsystemSlot.IonBattery);
        _neutrinoBattery = new BatterySubsystem(this, "NeutrinoBattery", reader, SubsystemSlot.NeutrinoBattery);
        _energyCell = new EnergyCellSubsystem(this, "EnergyCell", reader, SubsystemSlot.EnergyCell);
        _ionCell = new EnergyCellSubsystem(this, "IonCell", reader, SubsystemSlot.IonCell);
        _neutrinoCell = new EnergyCellSubsystem(this, "NeutrinoCell", reader, SubsystemSlot.NeutrinoCell);
        _hull = new HullSubsystem(this, "Hull", reader, SubsystemSlot.Hull);
        _shield = new ShieldSubsystem(this, "Shield", reader, SubsystemSlot.Shield);
        _armor = new ArmorSubsystem(this, "Armor", reader, SubsystemSlot.Armor);
        _repair = new RepairSubsystem(this, "Repair", reader, SubsystemSlot.Repair);
        _cargo = new CargoSubsystem(this, reader, SubsystemSlot.Cargo);
        _resourceMiner = new ResourceMinerSubsystem(this, reader, SubsystemSlot.ResourceMiner);
        _structureOptimizer = new StructureOptimizerSubsystem(this, reader);
        _nebulaCollector = new NebulaCollectorSubsystem(this, reader, SubsystemSlot.NebulaCollector);

        _engines = new ModernShipEngineSubsystem[ModernShipGeometry.EngineSlots.Length];
        _scanners = new StaticScannerSubsystem[ModernShipGeometry.ScannerSlots.Length];
        _shotLaunchers = new StaticShotLauncherSubsystem[ModernShipGeometry.ShotLauncherSlots.Length];
        _shotMagazines = new StaticShotMagazineSubsystem[ModernShipGeometry.ShotMagazineSlots.Length];
        _shotFabricators = new StaticShotFabricatorSubsystem[ModernShipGeometry.ShotFabricatorSlots.Length];
        _interceptorLaunchers = new StaticInterceptorLauncherSubsystem[2];
        _interceptorMagazines = new StaticInterceptorMagazineSubsystem[2];
        _interceptorFabricators = new StaticInterceptorFabricatorSubsystem[2];
        _railguns = new ModernRailgunSubsystem[ModernShipGeometry.RailgunSlots.Length];
        _equippedCrystals = new string[3];

        for (int index = 0; index < _scanners.Length; index++)
        {
            string suffix = GetSlotSuffix(ModernShipGeometry.ScannerSlots[index]);
            _scanners[index] = new StaticScannerSubsystem(this, $"Scanner{suffix}", reader, ModernShipGeometry.ScannerSlots[index]);
        }

        for (int index = 0; index < _engines.Length; index++)
        {
            string suffix = GetSlotSuffix(ModernShipGeometry.EngineSlots[index]);
            _engines[index] = new ModernShipEngineSubsystem(this, $"Engine{suffix}", reader, ModernShipGeometry.EngineSlots[index]);
        }

        for (int index = 0; index < _shotLaunchers.Length; index++)
        {
            string suffix = GetSlotSuffix(ModernShipGeometry.ShotLauncherSlots[index]);
            _shotLaunchers[index] = new StaticShotLauncherSubsystem(this, $"ShotLauncher{suffix}", reader,
                ModernShipGeometry.ShotLauncherSlots[index]);
            _shotMagazines[index] = new StaticShotMagazineSubsystem(this, $"ShotMagazine{suffix}", reader,
                ModernShipGeometry.ShotMagazineSlots[index]);
            _shotFabricators[index] = new StaticShotFabricatorSubsystem(this, $"ShotFabricator{suffix}", reader,
                ModernShipGeometry.ShotFabricatorSlots[index]);
        }

        SubsystemSlot[] interceptorLauncherSlots = { SubsystemSlot.StaticInterceptorLauncherE, SubsystemSlot.StaticInterceptorLauncherW };
        SubsystemSlot[] interceptorMagazineSlots = { SubsystemSlot.StaticInterceptorMagazineE, SubsystemSlot.StaticInterceptorMagazineW };
        SubsystemSlot[] interceptorFabricatorSlots = { SubsystemSlot.StaticInterceptorFabricatorE, SubsystemSlot.StaticInterceptorFabricatorW };

        for (int index = 0; index < _interceptorLaunchers.Length; index++)
        {
            string suffix = GetSlotSuffix(interceptorLauncherSlots[index]);
            _interceptorLaunchers[index] = new StaticInterceptorLauncherSubsystem(this, $"InterceptorLauncher{suffix}", reader,
                interceptorLauncherSlots[index]);
            _interceptorMagazines[index] = new StaticInterceptorMagazineSubsystem(this, $"InterceptorMagazine{suffix}", reader,
                interceptorMagazineSlots[index]);
            _interceptorFabricators[index] = new StaticInterceptorFabricatorSubsystem(this, $"InterceptorFabricator{suffix}", reader,
                interceptorFabricatorSlots[index]);
        }

        for (int index = 0; index < _railguns.Length; index++)
        {
            string suffix = GetSlotSuffix(ModernShipGeometry.RailgunSlots[index]);
            _railguns[index] = new ModernRailgunSubsystem(this, $"Railgun{suffix}", reader, ModernShipGeometry.RailgunSlots[index]);
        }

        _jumpDrive = new JumpDriveSubsystem(this, reader);

        if (!reader.Read(out _equippedCrystals[0]) ||
            !reader.Read(out _equippedCrystals[1]) ||
            !reader.Read(out _equippedCrystals[2]))
            throw new InvalidDataException("Couldn't read ModernShipControllable create state.");
    }

    public override UnitKind Kind => UnitKind.ModernShipPlayerUnit;
    public NebulaCollectorSubsystem NebulaCollector => _nebulaCollector;
    public JumpDriveSubsystem JumpDrive => _jumpDrive;
    public IReadOnlyList<string> EquippedCrystals => _equippedCrystals;
    public IReadOnlyList<ModernShipEngineSubsystem> Engines => _engines;
    public IReadOnlyList<StaticScannerSubsystem> Scanners => _scanners;
    public IReadOnlyList<StaticShotLauncherSubsystem> ShotLaunchers => _shotLaunchers;
    public IReadOnlyList<StaticShotMagazineSubsystem> ShotMagazines => _shotMagazines;
    public IReadOnlyList<StaticShotFabricatorSubsystem> ShotFabricators => _shotFabricators;
    public IReadOnlyList<ModernRailgunSubsystem> Railguns => _railguns;

    public ModernShipEngineSubsystem EngineN => _engines[0];
    public ModernShipEngineSubsystem EngineNE => _engines[1];
    public ModernShipEngineSubsystem EngineE => _engines[2];
    public ModernShipEngineSubsystem EngineSE => _engines[3];
    public ModernShipEngineSubsystem EngineS => _engines[4];
    public ModernShipEngineSubsystem EngineSW => _engines[5];
    public ModernShipEngineSubsystem EngineW => _engines[6];
    public ModernShipEngineSubsystem EngineNW => _engines[7];

    public StaticScannerSubsystem ScannerN => _scanners[0];
    public StaticScannerSubsystem ScannerNE => _scanners[1];
    public StaticScannerSubsystem ScannerE => _scanners[2];
    public StaticScannerSubsystem ScannerSE => _scanners[3];
    public StaticScannerSubsystem ScannerS => _scanners[4];
    public StaticScannerSubsystem ScannerSW => _scanners[5];
    public StaticScannerSubsystem ScannerW => _scanners[6];
    public StaticScannerSubsystem ScannerNW => _scanners[7];

    public StaticShotLauncherSubsystem ShotLauncherN => _shotLaunchers[0];
    public StaticShotLauncherSubsystem ShotLauncherNE => _shotLaunchers[1];
    public StaticShotLauncherSubsystem ShotLauncherE => _shotLaunchers[2];
    public StaticShotLauncherSubsystem ShotLauncherSE => _shotLaunchers[3];
    public StaticShotLauncherSubsystem ShotLauncherS => _shotLaunchers[4];
    public StaticShotLauncherSubsystem ShotLauncherSW => _shotLaunchers[5];
    public StaticShotLauncherSubsystem ShotLauncherW => _shotLaunchers[6];
    public StaticShotLauncherSubsystem ShotLauncherNW => _shotLaunchers[7];

    public StaticShotMagazineSubsystem ShotMagazineN => _shotMagazines[0];
    public StaticShotMagazineSubsystem ShotMagazineNE => _shotMagazines[1];
    public StaticShotMagazineSubsystem ShotMagazineE => _shotMagazines[2];
    public StaticShotMagazineSubsystem ShotMagazineSE => _shotMagazines[3];
    public StaticShotMagazineSubsystem ShotMagazineS => _shotMagazines[4];
    public StaticShotMagazineSubsystem ShotMagazineSW => _shotMagazines[5];
    public StaticShotMagazineSubsystem ShotMagazineW => _shotMagazines[6];
    public StaticShotMagazineSubsystem ShotMagazineNW => _shotMagazines[7];

    public StaticShotFabricatorSubsystem ShotFabricatorN => _shotFabricators[0];
    public StaticShotFabricatorSubsystem ShotFabricatorNE => _shotFabricators[1];
    public StaticShotFabricatorSubsystem ShotFabricatorE => _shotFabricators[2];
    public StaticShotFabricatorSubsystem ShotFabricatorSE => _shotFabricators[3];
    public StaticShotFabricatorSubsystem ShotFabricatorS => _shotFabricators[4];
    public StaticShotFabricatorSubsystem ShotFabricatorSW => _shotFabricators[5];
    public StaticShotFabricatorSubsystem ShotFabricatorW => _shotFabricators[6];
    public StaticShotFabricatorSubsystem ShotFabricatorNW => _shotFabricators[7];

    public StaticInterceptorLauncherSubsystem InterceptorLauncherE => _interceptorLaunchers[0];
    public StaticInterceptorLauncherSubsystem InterceptorLauncherW => _interceptorLaunchers[1];
    public StaticInterceptorMagazineSubsystem InterceptorMagazineE => _interceptorMagazines[0];
    public StaticInterceptorMagazineSubsystem InterceptorMagazineW => _interceptorMagazines[1];
    public StaticInterceptorFabricatorSubsystem InterceptorFabricatorE => _interceptorFabricators[0];
    public StaticInterceptorFabricatorSubsystem InterceptorFabricatorW => _interceptorFabricators[1];

    public ModernRailgunSubsystem RailgunN => _railguns[0];
    public ModernRailgunSubsystem RailgunNE => _railguns[1];
    public ModernRailgunSubsystem RailgunE => _railguns[2];
    public ModernRailgunSubsystem RailgunSE => _railguns[3];
    public ModernRailgunSubsystem RailgunS => _railguns[4];
    public ModernRailgunSubsystem RailgunSW => _railguns[5];
    public ModernRailgunSubsystem RailgunW => _railguns[6];
    public ModernRailgunSubsystem RailgunNW => _railguns[7];

    /// <inheritdoc/>
    public override float SpeedLimit
    {
        get { return SubsystemTierInfo.CalculateModernSpeedLimit(EffectiveStructureLoad); }
    }

    internal override float GetProjectedRawStructuralLoad(SubsystemSlot slot, float projectedStructuralLoad)
    {
        float result = GetCommonProjectedStructuralLoad(slot, projectedStructuralLoad) +
            StructuralLoadFor(_nebulaCollector, slot, projectedStructuralLoad) +
            StructuralLoadFor(_jumpDrive, slot, projectedStructuralLoad);

        for (int index = 0; index < _engines.Length; index++)
            result += StructuralLoadFor(_engines[index], slot, projectedStructuralLoad);

        for (int index = 0; index < _scanners.Length; index++)
            result += StructuralLoadFor(_scanners[index], slot, projectedStructuralLoad);

        for (int index = 0; index < _shotLaunchers.Length; index++)
        {
            result += StructuralLoadFor(_shotLaunchers[index], slot, projectedStructuralLoad);
            result += StructuralLoadFor(_shotMagazines[index], slot, projectedStructuralLoad);
            result += StructuralLoadFor(_shotFabricators[index], slot, projectedStructuralLoad);
        }

        for (int index = 0; index < _interceptorLaunchers.Length; index++)
        {
            result += StructuralLoadFor(_interceptorLaunchers[index], slot, projectedStructuralLoad);
            result += StructuralLoadFor(_interceptorMagazines[index], slot, projectedStructuralLoad);
            result += StructuralLoadFor(_interceptorFabricators[index], slot, projectedStructuralLoad);
        }

        for (int index = 0; index < _railguns.Length; index++)
            result += StructuralLoadFor(_railguns[index], slot, projectedStructuralLoad);

        return result;
    }

    internal override void ApplyCreateRefresh(Controllable refreshed)
    {
        base.ApplyCreateRefresh(refreshed);

        ModernShipControllable modernRefreshed = (ModernShipControllable)refreshed;
        _nebulaCollector.CopyFrom(modernRefreshed._nebulaCollector);
        _jumpDrive.CopyFrom(modernRefreshed._jumpDrive);

        for (int index = 0; index < _engines.Length; index++)
            _engines[index].CopyFrom(modernRefreshed._engines[index]);

        for (int index = 0; index < _scanners.Length; index++)
            _scanners[index].CopyFrom(modernRefreshed._scanners[index]);

        for (int index = 0; index < _shotLaunchers.Length; index++)
        {
            _shotLaunchers[index].CopyFrom(modernRefreshed._shotLaunchers[index]);
            _shotMagazines[index].CopyFrom(modernRefreshed._shotMagazines[index]);
            _shotFabricators[index].CopyFrom(modernRefreshed._shotFabricators[index]);
        }

        for (int index = 0; index < _interceptorLaunchers.Length; index++)
        {
            _interceptorLaunchers[index].CopyFrom(modernRefreshed._interceptorLaunchers[index]);
            _interceptorMagazines[index].CopyFrom(modernRefreshed._interceptorMagazines[index]);
            _interceptorFabricators[index].CopyFrom(modernRefreshed._interceptorFabricators[index]);
        }

        for (int index = 0; index < _railguns.Length; index++)
            _railguns[index].CopyFrom(modernRefreshed._railguns[index]);

        _equippedCrystals[0] = modernRefreshed._equippedCrystals[0];
        _equippedCrystals[1] = modernRefreshed._equippedCrystals[1];
        _equippedCrystals[2] = modernRefreshed._equippedCrystals[2];
    }

    private protected override void ResetRuntime()
    {
        base.ResetRuntime();
        _nebulaCollector.ResetRuntime();

        for (int index = 0; index < _engines.Length; index++)
            _engines[index].ResetRuntime();

        for (int index = 0; index < _scanners.Length; index++)
            _scanners[index].ResetRuntime();

        for (int index = 0; index < _shotLaunchers.Length; index++)
        {
            _shotLaunchers[index].ResetRuntime();
            _shotMagazines[index].ResetRuntime();
            _shotFabricators[index].ResetRuntime();
        }

        for (int index = 0; index < _interceptorLaunchers.Length; index++)
        {
            _interceptorLaunchers[index].ResetRuntime();
            _interceptorMagazines[index].ResetRuntime();
            _interceptorFabricators[index].ResetRuntime();
        }

        for (int index = 0; index < _railguns.Length; index++)
            _railguns[index].ResetRuntime();

        _jumpDrive.ResetRuntime();
    }

    private protected override void ReadRuntime(PacketReader reader)
    {
        if (!_nebulaCollector.Update(reader))
            throw new InvalidDataException("Couldn't read ModernShipControllable runtime.");

        for (int index = 0; index < _scanners.Length; index++)
            if (!_scanners[index].Update(reader))
                throw new InvalidDataException("Couldn't read ModernShipControllable runtime.");

        for (int index = 0; index < _engines.Length; index++)
            if (!_engines[index].Update(reader))
                throw new InvalidDataException("Couldn't read ModernShipControllable runtime.");

        for (int index = 0; index < _shotLaunchers.Length; index++)
            if (!_shotLaunchers[index].Update(reader) ||
                !_shotMagazines[index].Update(reader) ||
                !_shotFabricators[index].Update(reader))
                throw new InvalidDataException("Couldn't read ModernShipControllable runtime.");

        for (int index = 0; index < _interceptorLaunchers.Length; index++)
            if (!_interceptorLaunchers[index].Update(reader) ||
                !_interceptorMagazines[index].Update(reader) ||
                !_interceptorFabricators[index].Update(reader))
                throw new InvalidDataException("Couldn't read ModernShipControllable runtime.");

        for (int index = 0; index < _railguns.Length; index++)
            if (!_railguns[index].Update(reader))
                throw new InvalidDataException("Couldn't read ModernShipControllable runtime.");

        if (!_jumpDrive.Update(reader))
            throw new InvalidDataException("Couldn't read ModernShipControllable runtime.");
    }

    private protected override void EmitRuntimeEvents()
    {
        base.EmitRuntimeEvents();
        PushRuntimeEvent(_nebulaCollector.CreateRuntimeEvent());

        for (int index = 0; index < _scanners.Length; index++)
            PushRuntimeEvent(_scanners[index].CreateRuntimeEvent());

        for (int index = 0; index < _engines.Length; index++)
            PushRuntimeEvent(_engines[index].CreateRuntimeEvent());

        for (int index = 0; index < _shotLaunchers.Length; index++)
        {
            PushRuntimeEvent(_shotLaunchers[index].CreateRuntimeEvent());
            PushRuntimeEvent(_shotMagazines[index].CreateRuntimeEvent());
            PushRuntimeEvent(_shotFabricators[index].CreateRuntimeEvent());
        }

        for (int index = 0; index < _interceptorLaunchers.Length; index++)
        {
            PushRuntimeEvent(_interceptorLaunchers[index].CreateRuntimeEvent());
            PushRuntimeEvent(_interceptorMagazines[index].CreateRuntimeEvent());
            PushRuntimeEvent(_interceptorFabricators[index].CreateRuntimeEvent());
        }

        for (int index = 0; index < _railguns.Length; index++)
            PushRuntimeEvent(_railguns[index].CreateRuntimeEvent());
    }

    private void PushRuntimeEvent(FlattiverseEvent? @event)
    {
        if (@event is not null)
            Cluster.Galaxy.PushEvent(@event);
    }

    private static string GetSlotSuffix(SubsystemSlot slot)
    {
        switch (slot)
        {
            case SubsystemSlot.ModernEngineN:
            case SubsystemSlot.ModernScannerN:
            case SubsystemSlot.StaticShotLauncherN:
            case SubsystemSlot.StaticShotMagazineN:
            case SubsystemSlot.StaticShotFabricatorN:
            case SubsystemSlot.ModernRailgunN:
                return "N";
            case SubsystemSlot.ModernEngineNE:
            case SubsystemSlot.ModernScannerNE:
            case SubsystemSlot.StaticShotLauncherNE:
            case SubsystemSlot.StaticShotMagazineNE:
            case SubsystemSlot.StaticShotFabricatorNE:
            case SubsystemSlot.ModernRailgunNE:
                return "NE";
            case SubsystemSlot.ModernEngineE:
            case SubsystemSlot.ModernScannerE:
            case SubsystemSlot.StaticShotLauncherE:
            case SubsystemSlot.StaticShotMagazineE:
            case SubsystemSlot.StaticShotFabricatorE:
            case SubsystemSlot.StaticInterceptorLauncherE:
            case SubsystemSlot.StaticInterceptorMagazineE:
            case SubsystemSlot.StaticInterceptorFabricatorE:
            case SubsystemSlot.ModernRailgunE:
                return "E";
            case SubsystemSlot.ModernEngineSE:
            case SubsystemSlot.ModernScannerSE:
            case SubsystemSlot.StaticShotLauncherSE:
            case SubsystemSlot.StaticShotMagazineSE:
            case SubsystemSlot.StaticShotFabricatorSE:
            case SubsystemSlot.ModernRailgunSE:
                return "SE";
            case SubsystemSlot.ModernEngineS:
            case SubsystemSlot.ModernScannerS:
            case SubsystemSlot.StaticShotLauncherS:
            case SubsystemSlot.StaticShotMagazineS:
            case SubsystemSlot.StaticShotFabricatorS:
            case SubsystemSlot.ModernRailgunS:
                return "S";
            case SubsystemSlot.ModernEngineSW:
            case SubsystemSlot.ModernScannerSW:
            case SubsystemSlot.StaticShotLauncherSW:
            case SubsystemSlot.StaticShotMagazineSW:
            case SubsystemSlot.StaticShotFabricatorSW:
            case SubsystemSlot.ModernRailgunSW:
                return "SW";
            case SubsystemSlot.ModernEngineW:
            case SubsystemSlot.ModernScannerW:
            case SubsystemSlot.StaticShotLauncherW:
            case SubsystemSlot.StaticShotMagazineW:
            case SubsystemSlot.StaticShotFabricatorW:
            case SubsystemSlot.StaticInterceptorLauncherW:
            case SubsystemSlot.StaticInterceptorMagazineW:
            case SubsystemSlot.StaticInterceptorFabricatorW:
            case SubsystemSlot.ModernRailgunW:
                return "W";
            case SubsystemSlot.ModernEngineNW:
            case SubsystemSlot.ModernScannerNW:
            case SubsystemSlot.StaticShotLauncherNW:
            case SubsystemSlot.StaticShotMagazineNW:
            case SubsystemSlot.StaticShotFabricatorNW:
            case SubsystemSlot.ModernRailgunNW:
                return "NW";
            default:
                throw new InvalidDataException($"Unsupported modern ship slot {slot}.");
        }
    }
}
