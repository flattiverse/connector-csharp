using System.Diagnostics;
using Flattiverse.Connector.Network;

namespace Flattiverse.Connector.GalaxyHierarchy;

static partial class SubsystemTierCatalogs
{
    public static SubsystemKind GetKind(SubsystemSlot slot)
    {
        switch (slot)
        {
            case SubsystemSlot.EnergyBattery:
                return SubsystemKind.EnergyBattery;
            case SubsystemSlot.IonBattery:
                return SubsystemKind.IonBattery;
            case SubsystemSlot.NeutrinoBattery:
                return SubsystemKind.NeutrinoBattery;
            case SubsystemSlot.EnergyCell:
                return SubsystemKind.EnergyCell;
            case SubsystemSlot.IonCell:
                return SubsystemKind.IonCell;
            case SubsystemSlot.NeutrinoCell:
                return SubsystemKind.NeutrinoCell;
            case SubsystemSlot.Hull:
                return SubsystemKind.Hull;
            case SubsystemSlot.Shield:
                return SubsystemKind.Shield;
            case SubsystemSlot.Armor:
                return SubsystemKind.Armor;
            case SubsystemSlot.Repair:
                return SubsystemKind.Repair;
            case SubsystemSlot.Cargo:
                return SubsystemKind.Cargo;
            case SubsystemSlot.ResourceMiner:
                return SubsystemKind.ResourceMiner;
            case SubsystemSlot.NebulaCollector:
                return SubsystemKind.NebulaCollector;
            case SubsystemSlot.StructureOptimizer:
                return SubsystemKind.StructureOptimizer;
            case SubsystemSlot.PrimaryEngine:
            case SubsystemSlot.SecondaryEngine:
            case SubsystemSlot.TertiaryEngine:
                return SubsystemKind.ClassicShipEngine;
            case SubsystemSlot.ModernEngineN:
            case SubsystemSlot.ModernEngineNE:
            case SubsystemSlot.ModernEngineE:
            case SubsystemSlot.ModernEngineSE:
            case SubsystemSlot.ModernEngineS:
            case SubsystemSlot.ModernEngineSW:
            case SubsystemSlot.ModernEngineW:
            case SubsystemSlot.ModernEngineNW:
                return SubsystemKind.ModernShipEngine;
            case SubsystemSlot.PrimaryScanner:
            case SubsystemSlot.SecondaryScanner:
            case SubsystemSlot.TertiaryScanner:
                return SubsystemKind.DynamicScanner;
            case SubsystemSlot.ModernScannerN:
            case SubsystemSlot.ModernScannerNE:
            case SubsystemSlot.ModernScannerE:
            case SubsystemSlot.ModernScannerSE:
            case SubsystemSlot.ModernScannerS:
            case SubsystemSlot.ModernScannerSW:
            case SubsystemSlot.ModernScannerW:
            case SubsystemSlot.ModernScannerNW:
                return SubsystemKind.StaticScanner;
            case SubsystemSlot.DynamicShotLauncher:
                return SubsystemKind.DynamicShotLauncher;
            case SubsystemSlot.StaticShotLauncherN:
            case SubsystemSlot.StaticShotLauncherNE:
            case SubsystemSlot.StaticShotLauncherE:
            case SubsystemSlot.StaticShotLauncherSE:
            case SubsystemSlot.StaticShotLauncherS:
            case SubsystemSlot.StaticShotLauncherSW:
            case SubsystemSlot.StaticShotLauncherW:
            case SubsystemSlot.StaticShotLauncherNW:
                return SubsystemKind.StaticShotLauncher;
            case SubsystemSlot.DynamicShotMagazine:
                return SubsystemKind.DynamicShotMagazine;
            case SubsystemSlot.StaticShotMagazineN:
            case SubsystemSlot.StaticShotMagazineNE:
            case SubsystemSlot.StaticShotMagazineE:
            case SubsystemSlot.StaticShotMagazineSE:
            case SubsystemSlot.StaticShotMagazineS:
            case SubsystemSlot.StaticShotMagazineSW:
            case SubsystemSlot.StaticShotMagazineW:
            case SubsystemSlot.StaticShotMagazineNW:
                return SubsystemKind.StaticShotMagazine;
            case SubsystemSlot.DynamicShotFabricator:
                return SubsystemKind.DynamicShotFabricator;
            case SubsystemSlot.StaticShotFabricatorN:
            case SubsystemSlot.StaticShotFabricatorNE:
            case SubsystemSlot.StaticShotFabricatorE:
            case SubsystemSlot.StaticShotFabricatorSE:
            case SubsystemSlot.StaticShotFabricatorS:
            case SubsystemSlot.StaticShotFabricatorSW:
            case SubsystemSlot.StaticShotFabricatorW:
            case SubsystemSlot.StaticShotFabricatorNW:
                return SubsystemKind.StaticShotFabricator;
            case SubsystemSlot.DynamicInterceptorLauncher:
                return SubsystemKind.DynamicInterceptorLauncher;
            case SubsystemSlot.StaticInterceptorLauncherE:
            case SubsystemSlot.StaticInterceptorLauncherW:
                return SubsystemKind.StaticInterceptorLauncher;
            case SubsystemSlot.DynamicInterceptorMagazine:
                return SubsystemKind.DynamicInterceptorMagazine;
            case SubsystemSlot.StaticInterceptorMagazineE:
            case SubsystemSlot.StaticInterceptorMagazineW:
                return SubsystemKind.StaticInterceptorMagazine;
            case SubsystemSlot.DynamicInterceptorFabricator:
                return SubsystemKind.DynamicInterceptorFabricator;
            case SubsystemSlot.StaticInterceptorFabricatorE:
            case SubsystemSlot.StaticInterceptorFabricatorW:
                return SubsystemKind.StaticInterceptorFabricator;
            case SubsystemSlot.Railgun:
                return SubsystemKind.ClassicRailgun;
            case SubsystemSlot.ModernRailgunN:
            case SubsystemSlot.ModernRailgunNE:
            case SubsystemSlot.ModernRailgunE:
            case SubsystemSlot.ModernRailgunSE:
            case SubsystemSlot.ModernRailgunS:
            case SubsystemSlot.ModernRailgunSW:
            case SubsystemSlot.ModernRailgunW:
            case SubsystemSlot.ModernRailgunNW:
                return SubsystemKind.ModernRailgun;
            case SubsystemSlot.JumpDrive:
                return SubsystemKind.JumpDrive;
            default:
                Debug.Fail($"Unknown subsystem slot {slot}.");
                throw new InvalidDataGameException();
        }
    }

    public static IReadOnlyList<SubsystemTierInfo> GetTierInfos(SubsystemSlot slot, bool modernShip)
    {
        IReadOnlyList<SubsystemTierInfo> result;

        switch (GetKind(slot))
        {
            case SubsystemKind.EnergyBattery:
                result = EnergyBatteryTierInfos;
                break;
            case SubsystemKind.IonBattery:
                result = IonBatteryTierInfos;
                break;
            case SubsystemKind.NeutrinoBattery:
                result = NeutrinoBatteryTierInfos;
                break;
            case SubsystemKind.EnergyCell:
                result = EnergyCellTierInfos;
                break;
            case SubsystemKind.IonCell:
                result = IonCellTierInfos;
                break;
            case SubsystemKind.NeutrinoCell:
                result = NeutrinoCellTierInfos;
                break;
            case SubsystemKind.Hull:
                result = HullTierInfos;
                break;
            case SubsystemKind.Shield:
                result = ShieldTierInfos;
                break;
            case SubsystemKind.Armor:
                result = ArmorTierInfos;
                break;
            case SubsystemKind.Repair:
                result = RepairTierInfos;
                break;
            case SubsystemKind.Cargo:
                result = CargoTierInfos;
                break;
            case SubsystemKind.ResourceMiner:
                result = modernShip ? ModernResourceMinerTierInfos : ResourceMinerTierInfos;
                break;
            case SubsystemKind.NebulaCollector:
                result = modernShip ? ModernNebulaCollectorTierInfos : NebulaCollectorTierInfos;
                break;
            case SubsystemKind.StructureOptimizer:
                result = StructureOptimizerTierInfos;
                break;
            case SubsystemKind.ClassicShipEngine:
                result = ClassicEngineTierInfos;
                break;
            case SubsystemKind.ModernShipEngine:
                result = ModernEngineTierInfos;
                break;
            case SubsystemKind.DynamicScanner:
                result = DynamicScannerTierInfos;
                break;
            case SubsystemKind.StaticScanner:
                result = StaticScannerTierInfos;
                break;
            case SubsystemKind.DynamicShotLauncher:
                result = DynamicShotLauncherTierInfos;
                break;
            case SubsystemKind.StaticShotLauncher:
                result = StaticShotLauncherTierInfos;
                break;
            case SubsystemKind.DynamicShotMagazine:
                result = DynamicShotMagazineTierInfos;
                break;
            case SubsystemKind.StaticShotMagazine:
                result = StaticShotMagazineTierInfos;
                break;
            case SubsystemKind.DynamicShotFabricator:
                result = DynamicShotFabricatorTierInfos;
                break;
            case SubsystemKind.StaticShotFabricator:
                result = StaticShotFabricatorTierInfos;
                break;
            case SubsystemKind.DynamicInterceptorLauncher:
                result = DynamicInterceptorLauncherTierInfos;
                break;
            case SubsystemKind.StaticInterceptorLauncher:
                result = StaticInterceptorLauncherTierInfos;
                break;
            case SubsystemKind.DynamicInterceptorMagazine:
                result = DynamicInterceptorMagazineTierInfos;
                break;
            case SubsystemKind.StaticInterceptorMagazine:
                result = StaticInterceptorMagazineTierInfos;
                break;
            case SubsystemKind.DynamicInterceptorFabricator:
                result = DynamicInterceptorFabricatorTierInfos;
                break;
            case SubsystemKind.StaticInterceptorFabricator:
                result = StaticInterceptorFabricatorTierInfos;
                break;
            case SubsystemKind.ClassicRailgun:
                result = ClassicRailgunTierInfos;
                break;
            case SubsystemKind.ModernRailgun:
                result = ModernRailgunTierInfos;
                break;
            case SubsystemKind.JumpDrive:
                result = JumpDriveTierInfos;
                break;
            default:
                Debug.Fail($"No tier catalog available for slot {slot}.");
                result = HullTierInfos;
                break;
        }

        return NormalizeTierZero(result);
    }

    private static SubsystemTierInfo CreateTierZero(SubsystemKind kind, string description)
    {
        return new SubsystemTierInfo(kind, 0, 0f, Array.Empty<SubsystemResourceUsageFormula>(), new Costs(0, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Costs(0, 0f, 0f, 0f, 0f, 0f, 0f, 0f), Array.Empty<SubsystemPropertyInfo>(), description);
    }

    private static SubsystemTierInfo CreateTierInfo(SubsystemKind kind, SubsystemSlot slot, byte tier, float structuralLoad,
        SubsystemResourceUsageFormula[] resourceUsages, SubsystemPropertyInfo[] properties, string description)
    {
        Costs upgradeResources = ShipUpgradeBalancing.GetUpgradeCost(slot, tier);
        uint upgradeTicks = ShipUpgradeBalancing.GetUpgradeTicks(slot, tier);
        uint downgradeTicks = ShipUpgradeBalancing.GetDowngradeTicks(slot, tier);
        Costs upgradeCost = new Costs((int)upgradeTicks, upgradeResources.Energy, upgradeResources.Metal, upgradeResources.Carbon,
            upgradeResources.Hydrogen, upgradeResources.Silicon, upgradeResources.Ions, upgradeResources.Neutrinos);
        Costs downgradeCost = new Costs((int)downgradeTicks, 0f, 0f, 0f, 0f, 0f, 0f, 0f);
        return new SubsystemTierInfo(kind, tier, structuralLoad, resourceUsages, upgradeCost, downgradeCost, properties, description);
    }

    private static ResourceFormula ConstantFormula(float value)
    {
        return new ResourceFormula(0f, 0f, 0f, 0f, value);
    }

    private static ResourceFormula LinearFormula(float slope, float intercept)
    {
        return new ResourceFormula(slope, 1f, 0f, 0f, intercept);
    }

    private static ResourceFormula GetEngineLikeFormula(float fullCost)
    {
        return new ResourceFormula(0.30f * fullCost, 1f, 0.70f * fullCost, 3f, 0f);
    }

    private static ResourceFormula GetShieldFormula(byte tier, float fullCost)
    {
        switch (tier)
        {
            case 1:
                return new ResourceFormula(0.70f * fullCost, 1f, 0.30f * fullCost, 3f, 0f);
            case 2:
                return new ResourceFormula(0.55f * fullCost, 1f, 0.45f * fullCost, 3f, 0f);
            case 3:
                return new ResourceFormula(0.40f * fullCost, 1f, 0.60f * fullCost, 3f, 0f);
            case 4:
                return new ResourceFormula(0.46f * fullCost, 1f, 0.54f * fullCost, 3f, 0f);
            case 5:
                return new ResourceFormula(0.52f * fullCost, 1f, 0.48f * fullCost, 3f, 0f);
            default:
                return ConstantFormula(0f);
        }
    }

    private static ResourceFormula GetRepairFormula(byte tier)
    {
        switch (tier)
        {
            case 1:
                return new ResourceFormula(18f, 0.35f, 7f, 3f, 0f);
            case 2:
                return new ResourceFormula(24f, 0.35f, 12f, 3f, 0f);
            case 3:
                return new ResourceFormula(32f, 0.35f, 20f, 3f, 0f);
            case 4:
                return new ResourceFormula(44f, 0.35f, 32f, 3f, 0f);
            case 5:
                return new ResourceFormula(58f, 0.35f, 50f, 3f, 0f);
            default:
                return ConstantFormula(0f);
        }
    }

    private static ResourceFormula GetScannerWidthFormula()
    {
        return new ResourceFormula(0.141176f, 1f, 0f, 0f, -0.705882f);
    }

    private static ResourceFormula GetScannerRangeFormula()
    {
        return new ResourceFormula(0.3926f, 0.5f, 2.76e-10f, 4f, -0.617f);
    }

    private static IReadOnlyList<SubsystemTierInfo> NormalizeTierZero(IReadOnlyList<SubsystemTierInfo> tierInfos)
    {
        if (tierInfos is not SubsystemTierInfo[] tierInfoArray)
            return tierInfos;

        if (tierInfoArray.Length <= 1)
            return tierInfoArray;

        if (tierInfoArray[0].Properties.Count != 0)
            return tierInfoArray;

        SubsystemTierInfo tierOne = tierInfoArray[1];
        SubsystemPropertyInfo[] zeroProperties = new SubsystemPropertyInfo[tierOne.Properties.Count];

        for (int index = 0; index < zeroProperties.Length; index++)
        {
            SubsystemPropertyInfo property = tierOne.Properties[index];
            if (property.Key == "minimumTicks")
                zeroProperties[index] = new SubsystemPropertyInfo(property.Key, property.Label, property.Unit,
                    property.MinimumValue, property.MaximumValue);
            else
                zeroProperties[index] = new SubsystemPropertyInfo(property.Key, property.Label, property.Unit, 0f, 0f);
        }

        SubsystemResourceUsageFormula[] zeroResourceUsages = new SubsystemResourceUsageFormula[tierOne.ResourceUsages.Count];

        for (int index = 0; index < zeroResourceUsages.Length; index++)
        {
            SubsystemResourceUsageFormula usage = tierOne.ResourceUsages[index];
            zeroResourceUsages[index] = new SubsystemResourceUsageFormula(usage.ComponentKind, usage.Label, null, null, null, null, null,
                null, null);
        }

        tierInfoArray[0] = new SubsystemTierInfo(tierOne.SubsystemKind, 0, 0f, zeroResourceUsages, new Costs(0, 0f, 0f, 0f, 0f, 0f, 0f, 0f),
            new Costs(0, 0f, 0f, 0f, 0f, 0f, 0f, 0f), zeroProperties, tierInfoArray[0].Description);
        return tierInfoArray;
    }
}
