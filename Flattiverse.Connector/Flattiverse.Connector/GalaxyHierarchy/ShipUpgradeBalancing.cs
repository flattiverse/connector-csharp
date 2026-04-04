namespace Flattiverse.Connector.GalaxyHierarchy;

static class ShipUpgradeBalancing
{
    private static readonly Costs ZeroCosts = new Costs(0, 0f, 0f, 0f, 0f, 0f, 0f, 0f);

    public static byte GetMaximumTier(SubsystemSlot slot)
    {
        switch (slot)
        {
            case SubsystemSlot.EnergyBattery:
                return 9;
            case SubsystemSlot.IonBattery:
            case SubsystemSlot.NeutrinoBattery:
            case SubsystemSlot.EnergyCell:
            case SubsystemSlot.IonCell:
            case SubsystemSlot.NeutrinoCell:
            case SubsystemSlot.Hull:
            case SubsystemSlot.Shield:
            case SubsystemSlot.Armor:
            case SubsystemSlot.Repair:
            case SubsystemSlot.PrimaryScanner:
            case SubsystemSlot.SecondaryScanner:
            case SubsystemSlot.TertiaryScanner:
            case SubsystemSlot.ModernScannerN:
            case SubsystemSlot.ModernScannerNE:
            case SubsystemSlot.ModernScannerE:
            case SubsystemSlot.ModernScannerSE:
            case SubsystemSlot.ModernScannerS:
            case SubsystemSlot.ModernScannerSW:
            case SubsystemSlot.ModernScannerW:
            case SubsystemSlot.ModernScannerNW:
            case SubsystemSlot.PrimaryEngine:
            case SubsystemSlot.SecondaryEngine:
            case SubsystemSlot.TertiaryEngine:
            case SubsystemSlot.DynamicShotLauncher:
            case SubsystemSlot.DynamicShotMagazine:
            case SubsystemSlot.DynamicShotFabricator:
            case SubsystemSlot.DynamicInterceptorMagazine:
            case SubsystemSlot.DynamicInterceptorFabricator:
            case SubsystemSlot.Cargo:
            case SubsystemSlot.ResourceMiner:
            case SubsystemSlot.NebulaCollector:
            case SubsystemSlot.StructureOptimizer:
            case SubsystemSlot.StaticShotLauncherN:
            case SubsystemSlot.StaticShotLauncherNE:
            case SubsystemSlot.StaticShotLauncherE:
            case SubsystemSlot.StaticShotLauncherSE:
            case SubsystemSlot.StaticShotLauncherS:
            case SubsystemSlot.StaticShotLauncherSW:
            case SubsystemSlot.StaticShotLauncherW:
            case SubsystemSlot.StaticShotLauncherNW:
            case SubsystemSlot.StaticShotMagazineN:
            case SubsystemSlot.StaticShotMagazineNE:
            case SubsystemSlot.StaticShotMagazineE:
            case SubsystemSlot.StaticShotMagazineSE:
            case SubsystemSlot.StaticShotMagazineS:
            case SubsystemSlot.StaticShotMagazineSW:
            case SubsystemSlot.StaticShotMagazineW:
            case SubsystemSlot.StaticShotMagazineNW:
            case SubsystemSlot.StaticShotFabricatorN:
            case SubsystemSlot.StaticShotFabricatorNE:
            case SubsystemSlot.StaticShotFabricatorE:
            case SubsystemSlot.StaticShotFabricatorSE:
            case SubsystemSlot.StaticShotFabricatorS:
            case SubsystemSlot.StaticShotFabricatorSW:
            case SubsystemSlot.StaticShotFabricatorW:
            case SubsystemSlot.StaticShotFabricatorNW:
            case SubsystemSlot.StaticInterceptorMagazineE:
            case SubsystemSlot.StaticInterceptorMagazineW:
            case SubsystemSlot.StaticInterceptorFabricatorE:
            case SubsystemSlot.StaticInterceptorFabricatorW:
                return 5;
            case SubsystemSlot.ModernEngineN:
            case SubsystemSlot.ModernEngineNE:
            case SubsystemSlot.ModernEngineE:
            case SubsystemSlot.ModernEngineSE:
            case SubsystemSlot.ModernEngineS:
            case SubsystemSlot.ModernEngineSW:
            case SubsystemSlot.ModernEngineW:
            case SubsystemSlot.ModernEngineNW:
                return 4;
            case SubsystemSlot.DynamicInterceptorLauncher:
            case SubsystemSlot.StaticInterceptorLauncherE:
            case SubsystemSlot.StaticInterceptorLauncherW:
                return 2;
            case SubsystemSlot.JumpDrive:
                return 1;
            case SubsystemSlot.Railgun:
            case SubsystemSlot.ModernRailgunN:
            case SubsystemSlot.ModernRailgunNE:
            case SubsystemSlot.ModernRailgunE:
            case SubsystemSlot.ModernRailgunSE:
            case SubsystemSlot.ModernRailgunS:
            case SubsystemSlot.ModernRailgunSW:
            case SubsystemSlot.ModernRailgunW:
            case SubsystemSlot.ModernRailgunNW:
                return 3;
            default:
                return 0;
        }
    }

    public static uint GetUpgradeTicks(SubsystemSlot slot, byte targetTier)
    {
        switch (slot)
        {
            case SubsystemSlot.EnergyBattery:
                return GetBatteryTicks(targetTier);
            case SubsystemSlot.IonBattery:
            case SubsystemSlot.NeutrinoBattery:
            case SubsystemSlot.EnergyCell:
            case SubsystemSlot.IonCell:
            case SubsystemSlot.NeutrinoCell:
            case SubsystemSlot.PrimaryScanner:
            case SubsystemSlot.SecondaryScanner:
            case SubsystemSlot.TertiaryScanner:
            case SubsystemSlot.DynamicShotFabricator:
            case SubsystemSlot.DynamicInterceptorFabricator:
            case SubsystemSlot.StaticInterceptorFabricatorE:
            case SubsystemSlot.StaticInterceptorFabricatorW:
            case SubsystemSlot.Armor:
            case SubsystemSlot.Repair:
            case SubsystemSlot.Cargo:
            case SubsystemSlot.ResourceMiner:
            case SubsystemSlot.NebulaCollector:
            case SubsystemSlot.PrimaryEngine:
            case SubsystemSlot.SecondaryEngine:
            case SubsystemSlot.TertiaryEngine:
                return GetStandardTicks(targetTier);
            case SubsystemSlot.Hull:
            case SubsystemSlot.StructureOptimizer:
                return GetHeavyTicks(targetTier);
            case SubsystemSlot.Shield:
                return targetTier switch
                {
                    1 => 35,
                    2 => 45,
                    3 => 65,
                    4 => 95,
                    5 => 135,
                    _ => 0
                };
            case SubsystemSlot.ModernScannerN:
            case SubsystemSlot.ModernScannerNE:
            case SubsystemSlot.ModernScannerE:
            case SubsystemSlot.ModernScannerSE:
            case SubsystemSlot.ModernScannerS:
            case SubsystemSlot.ModernScannerSW:
            case SubsystemSlot.ModernScannerW:
            case SubsystemSlot.ModernScannerNW:
            case SubsystemSlot.StaticShotFabricatorN:
            case SubsystemSlot.StaticShotFabricatorNE:
            case SubsystemSlot.StaticShotFabricatorE:
            case SubsystemSlot.StaticShotFabricatorSE:
            case SubsystemSlot.StaticShotFabricatorS:
            case SubsystemSlot.StaticShotFabricatorSW:
            case SubsystemSlot.StaticShotFabricatorW:
            case SubsystemSlot.StaticShotFabricatorNW:
            case SubsystemSlot.ModernEngineN:
            case SubsystemSlot.ModernEngineNE:
            case SubsystemSlot.ModernEngineE:
            case SubsystemSlot.ModernEngineSE:
            case SubsystemSlot.ModernEngineS:
            case SubsystemSlot.ModernEngineSW:
            case SubsystemSlot.ModernEngineW:
            case SubsystemSlot.ModernEngineNW:
                return GetCompactTicks(targetTier);
            case SubsystemSlot.StaticShotMagazineN:
            case SubsystemSlot.StaticShotMagazineNE:
            case SubsystemSlot.StaticShotMagazineE:
            case SubsystemSlot.StaticShotMagazineSE:
            case SubsystemSlot.StaticShotMagazineS:
            case SubsystemSlot.StaticShotMagazineSW:
            case SubsystemSlot.StaticShotMagazineW:
            case SubsystemSlot.StaticShotMagazineNW:
                return targetTier switch
                {
                    1 => 15,
                    2 => 25,
                    3 => 40,
                    4 => 60,
                    5 => 90,
                    _ => 0
                };
            case SubsystemSlot.DynamicShotLauncher:
                return targetTier switch
                {
                    1 => 25,
                    2 => 35,
                    3 => 50,
                    4 => 75,
                    5 => 110,
                    _ => 0
                };
            case SubsystemSlot.StaticShotLauncherN:
            case SubsystemSlot.StaticShotLauncherNE:
            case SubsystemSlot.StaticShotLauncherE:
            case SubsystemSlot.StaticShotLauncherSE:
            case SubsystemSlot.StaticShotLauncherS:
            case SubsystemSlot.StaticShotLauncherSW:
            case SubsystemSlot.StaticShotLauncherW:
            case SubsystemSlot.StaticShotLauncherNW:
                return targetTier switch
                {
                    1 => 18,
                    2 => 25,
                    3 => 40,
                    4 => 60,
                    5 => 90,
                    _ => 0
                };
            case SubsystemSlot.DynamicShotMagazine:
            case SubsystemSlot.DynamicInterceptorMagazine:
            case SubsystemSlot.StaticInterceptorMagazineE:
            case SubsystemSlot.StaticInterceptorMagazineW:
                return GetWeaponTicks(targetTier);
            case SubsystemSlot.DynamicInterceptorLauncher:
                return targetTier switch
                {
                    1 => 30,
                    2 => 55,
                    _ => 0
                };
            case SubsystemSlot.StaticInterceptorLauncherE:
            case SubsystemSlot.StaticInterceptorLauncherW:
                return targetTier switch
                {
                    1 => 25,
                    2 => 45,
                    _ => 0
                };
            case SubsystemSlot.Railgun:
            case SubsystemSlot.ModernRailgunN:
            case SubsystemSlot.ModernRailgunNE:
            case SubsystemSlot.ModernRailgunE:
            case SubsystemSlot.ModernRailgunSE:
            case SubsystemSlot.ModernRailgunS:
            case SubsystemSlot.ModernRailgunSW:
            case SubsystemSlot.ModernRailgunW:
            case SubsystemSlot.ModernRailgunNW:
                return targetTier switch
                {
                    1 => 40,
                    2 => 70,
                    3 => 110,
                    _ => 0
                };
            case SubsystemSlot.JumpDrive:
                return targetTier == 1 ? 70u : 0u;
            default:
                return 0;
        }
    }

    public static Costs GetUpgradeCost(SubsystemSlot slot, byte targetTier)
    {
        switch (slot)
        {
            case SubsystemSlot.EnergyBattery:
                return targetTier switch
                {
                    1 => new Costs(0, 0f, 6f, 0f, 0f, 4f, 0f, 0f),
                    2 => new Costs(0, 0f, 8f, 0f, 0f, 6f, 0f, 0f),
                    3 => new Costs(0, 0f, 10f, 0f, 0f, 8f, 0f, 0f),
                    4 => new Costs(0, 0f, 14f, 0f, 0f, 12f, 0f, 0f),
                    5 => new Costs(0, 0f, 20f, 0f, 0f, 18f, 0f, 0f),
                    6 => new Costs(0, 0f, 28f, 0f, 0f, 24f, 0f, 0f),
                    7 => new Costs(0, 0f, 36f, 0f, 0f, 32f, 0f, 0f),
                    8 => new Costs(0, 0f, 0f, 0f, 0f, 48f, 2f, 0f),
                    9 => new Costs(0, 0f, 0f, 0f, 0f, 56f, 4f, 0f),
                    _ => ZeroCosts
                };
            case SubsystemSlot.IonBattery:
                return targetTier switch
                {
                    1 => new Costs(0, 0f, 4f, 0f, 0f, 0f, 0f, 0f),
                    2 => new Costs(0, 0f, 6f, 0f, 0f, 0f, 2f, 0f),
                    3 => new Costs(0, 0f, 8f, 0f, 0f, 0f, 4f, 0f),
                    4 => new Costs(0, 0f, 12f, 0f, 0f, 0f, 7f, 0f),
                    5 => new Costs(0, 0f, 16f, 0f, 0f, 0f, 12f, 0f),
                    _ => ZeroCosts
                };
            case SubsystemSlot.NeutrinoBattery:
                return targetTier switch
                {
                    1 => new Costs(0, 0f, 2f, 0f, 0f, 0f, 0f, 0f),
                    2 => new Costs(0, 0f, 4f, 0f, 0f, 0f, 0f, 2f),
                    3 => new Costs(0, 0f, 6f, 0f, 0f, 0f, 0f, 4f),
                    4 => new Costs(0, 0f, 8f, 0f, 0f, 0f, 0f, 7f),
                    5 => new Costs(0, 0f, 10f, 0f, 0f, 0f, 0f, 12f),
                    _ => ZeroCosts
                };
            case SubsystemSlot.EnergyCell:
                return targetTier switch
                {
                    1 => new Costs(0, 0f, 0f, 4f, 0f, 8f, 0f, 0f),
                    2 => new Costs(0, 0f, 0f, 8f, 0f, 16f, 0f, 0f),
                    3 => new Costs(0, 0f, 0f, 12f, 0f, 24f, 0f, 0f),
                    4 => new Costs(0, 0f, 0f, 18f, 0f, 36f, 0f, 0f),
                    5 => new Costs(0, 0f, 0f, 24f, 0f, 48f, 0f, 0f),
                    _ => ZeroCosts
                };
            case SubsystemSlot.IonCell:
                return targetTier switch
                {
                    1 => new Costs(0, 0f, 0f, 0f, 0f, 8f, 0f, 0f),
                    2 => new Costs(0, 0f, 0f, 0f, 0f, 16f, 2f, 0f),
                    3 => new Costs(0, 0f, 0f, 0f, 0f, 24f, 4f, 0f),
                    4 => new Costs(0, 0f, 0f, 0f, 0f, 36f, 6f, 0f),
                    5 => new Costs(0, 0f, 0f, 0f, 0f, 48f, 10f, 0f),
                    _ => ZeroCosts
                };
            case SubsystemSlot.NeutrinoCell:
                return targetTier switch
                {
                    1 => new Costs(0, 0f, 0f, 0f, 0f, 10f, 0f, 0f),
                    2 => new Costs(0, 0f, 0f, 0f, 0f, 18f, 0f, 4f),
                    3 => new Costs(0, 0f, 0f, 0f, 0f, 26f, 0f, 7f),
                    4 => new Costs(0, 0f, 0f, 0f, 0f, 38f, 0f, 12f),
                    5 => new Costs(0, 0f, 0f, 0f, 0f, 50f, 0f, 20f),
                    _ => ZeroCosts
                };
            case SubsystemSlot.Hull:
                return targetTier switch
                {
                    1 => new Costs(0, 0f, 8f, 8f, 0f, 0f, 0f, 0f),
                    2 => new Costs(0, 0f, 16f, 16f, 0f, 0f, 0f, 0f),
                    3 => new Costs(0, 0f, 24f, 24f, 0f, 0f, 0f, 0f),
                    4 => new Costs(0, 0f, 36f, 36f, 0f, 0f, 0f, 0f),
                    5 => new Costs(0, 0f, 48f, 48f, 0f, 0f, 0f, 0f),
                    _ => ZeroCosts
                };
            case SubsystemSlot.Armor:
                return targetTier switch
                {
                    1 => new Costs(0, 0f, 6f, 6f, 0f, 0f, 0f, 0f),
                    2 => new Costs(0, 0f, 12f, 12f, 0f, 0f, 0f, 0f),
                    3 => new Costs(0, 0f, 18f, 20f, 0f, 0f, 0f, 0f),
                    4 => new Costs(0, 0f, 24f, 32f, 0f, 0f, 0f, 0f),
                    5 => new Costs(0, 0f, 30f, 48f, 0f, 0f, 0f, 0f),
                    _ => ZeroCosts
                };
            case SubsystemSlot.Shield:
                return targetTier switch
                {
                    1 => new Costs(0, 0f, 0f, 0f, 8f, 8f, 0f, 0f),
                    2 => new Costs(0, 0f, 0f, 0f, 16f, 16f, 0f, 0f),
                    3 => new Costs(0, 0f, 0f, 0f, 24f, 24f, 0f, 0f),
                    4 => new Costs(0, 0f, 0f, 0f, 36f, 36f, 0f, 0f),
                    5 => new Costs(0, 0f, 0f, 0f, 48f, 48f, 6f, 0f),
                    _ => ZeroCosts
                };
            case SubsystemSlot.Repair:
                return targetTier switch
                {
                    1 => new Costs(0, 0f, 0f, 0f, 8f, 6f, 0f, 0f),
                    2 => new Costs(0, 0f, 0f, 0f, 16f, 10f, 0f, 0f),
                    3 => new Costs(0, 0f, 0f, 0f, 24f, 16f, 0f, 0f),
                    4 => new Costs(0, 0f, 0f, 0f, 36f, 24f, 0f, 0f),
                    5 => new Costs(0, 0f, 0f, 0f, 48f, 32f, 0f, 0f),
                    _ => ZeroCosts
                };
            case SubsystemSlot.Cargo:
                return targetTier switch
                {
                    1 => new Costs(0, 4000f, 0f, 0f, 0f, 0f, 0f, 0f),
                    2 => new Costs(0, 7000f, 0f, 0f, 0f, 0f, 0f, 0f),
                    3 => new Costs(0, 11000f, 0f, 0f, 0f, 0f, 0f, 0f),
                    4 => new Costs(0, 17000f, 0f, 0f, 0f, 0f, 0f, 0f),
                    5 => new Costs(0, 26000f, 0f, 0f, 0f, 0f, 0f, 0f),
                    _ => ZeroCosts
                };
            case SubsystemSlot.ResourceMiner:
                return targetTier switch
                {
                    1 => new Costs(0, 0f, 0f, 0f, 8f, 4f, 0f, 0f),
                    2 => new Costs(0, 0f, 0f, 0f, 16f, 8f, 0f, 0f),
                    3 => new Costs(0, 0f, 0f, 0f, 24f, 12f, 0f, 0f),
                    4 => new Costs(0, 0f, 0f, 0f, 36f, 18f, 0f, 0f),
                    5 => new Costs(0, 0f, 0f, 0f, 48f, 24f, 0f, 0f),
                    _ => ZeroCosts
                };
            case SubsystemSlot.NebulaCollector:
                return targetTier switch
                {
                    1 => new Costs(0, 0f, 0f, 0f, 6f, 8f, 0f, 0f),
                    2 => new Costs(0, 0f, 0f, 0f, 12f, 16f, 0f, 0f),
                    3 => new Costs(0, 0f, 0f, 0f, 18f, 24f, 0f, 0f),
                    4 => new Costs(0, 0f, 0f, 0f, 30f, 36f, 0f, 0f),
                    5 => new Costs(0, 0f, 0f, 0f, 42f, 48f, 0f, 0f),
                    _ => ZeroCosts
                };
            case SubsystemSlot.PrimaryEngine:
            case SubsystemSlot.SecondaryEngine:
            case SubsystemSlot.TertiaryEngine:
                return targetTier switch
                {
                    1 => new Costs(0, 0f, 6f, 0f, 8f, 0f, 0f, 0f),
                    2 => new Costs(0, 0f, 10f, 0f, 16f, 0f, 0f, 0f),
                    3 => new Costs(0, 0f, 16f, 0f, 24f, 0f, 0f, 0f),
                    4 => new Costs(0, 0f, 24f, 0f, 36f, 0f, 0f, 0f),
                    5 => new Costs(0, 0f, 32f, 0f, 48f, 0f, 0f, 0f),
                    _ => ZeroCosts
                };
            case SubsystemSlot.ModernEngineN:
            case SubsystemSlot.ModernEngineNE:
            case SubsystemSlot.ModernEngineE:
            case SubsystemSlot.ModernEngineSE:
            case SubsystemSlot.ModernEngineS:
            case SubsystemSlot.ModernEngineSW:
            case SubsystemSlot.ModernEngineW:
            case SubsystemSlot.ModernEngineNW:
                return targetTier switch
                {
                    1 => new Costs(0, 0f, 2f, 0f, 6f, 0f, 0f, 0f),
                    2 => new Costs(0, 0f, 4f, 0f, 16f, 0f, 0f, 0f),
                    3 => new Costs(0, 0f, 8f, 0f, 36f, 0f, 0f, 0f),
                    4 => new Costs(0, 0f, 12f, 0f, 48f, 0f, 0f, 0f),
                    _ => ZeroCosts
                };
            case SubsystemSlot.PrimaryScanner:
            case SubsystemSlot.SecondaryScanner:
            case SubsystemSlot.TertiaryScanner:
                return targetTier switch
                {
                    1 => new Costs(0, 0f, 0f, 2f, 0f, 8f, 0f, 0f),
                    2 => new Costs(0, 0f, 0f, 4f, 0f, 16f, 0f, 0f),
                    3 => new Costs(0, 0f, 0f, 8f, 0f, 24f, 0f, 0f),
                    4 => new Costs(0, 0f, 0f, 12f, 0f, 36f, 0f, 0f),
                    5 => new Costs(0, 0f, 0f, 0f, 0f, 48f, 0f, 4f),
                    _ => ZeroCosts
                };
            case SubsystemSlot.ModernScannerN:
            case SubsystemSlot.ModernScannerNE:
            case SubsystemSlot.ModernScannerE:
            case SubsystemSlot.ModernScannerSE:
            case SubsystemSlot.ModernScannerS:
            case SubsystemSlot.ModernScannerSW:
            case SubsystemSlot.ModernScannerW:
            case SubsystemSlot.ModernScannerNW:
                return targetTier switch
                {
                    1 => new Costs(0, 0f, 0f, 1f, 0f, 6f, 0f, 0f),
                    2 => new Costs(0, 0f, 0f, 2f, 0f, 12f, 0f, 0f),
                    3 => new Costs(0, 0f, 0f, 4f, 0f, 20f, 0f, 0f),
                    4 => new Costs(0, 0f, 0f, 6f, 0f, 32f, 0f, 0f),
                    5 => new Costs(0, 0f, 0f, 0f, 0f, 48f, 0f, 3f),
                    _ => ZeroCosts
                };
            case SubsystemSlot.JumpDrive:
                return targetTier == 1 ? new Costs(0, 0f, 0f, 0f, 0f, 24f, 0f, 4f) : ZeroCosts;
            case SubsystemSlot.StructureOptimizer:
                return targetTier switch
                {
                    1 => new Costs(0, 0f, 0f, 0f, 0f, 8f, 0f, 1f),
                    2 => new Costs(0, 0f, 0f, 0f, 0f, 16f, 0f, 2f),
                    3 => new Costs(0, 0f, 0f, 0f, 0f, 24f, 0f, 4f),
                    4 => new Costs(0, 0f, 0f, 0f, 0f, 36f, 0f, 8f),
                    5 => new Costs(0, 0f, 0f, 0f, 0f, 48f, 0f, 14f),
                    _ => ZeroCosts
                };
            case SubsystemSlot.DynamicShotLauncher:
                return targetTier switch
                {
                    1 => new Costs(0, 0f, 4f, 4f, 0f, 0f, 0f, 0f),
                    2 => new Costs(0, 0f, 8f, 8f, 0f, 0f, 0f, 0f),
                    3 => new Costs(0, 0f, 12f, 16f, 0f, 0f, 0f, 0f),
                    4 => new Costs(0, 0f, 18f, 32f, 0f, 0f, 0f, 0f),
                    5 => new Costs(0, 0f, 24f, 48f, 0f, 0f, 0f, 0f),
                    _ => ZeroCosts
                };
            case SubsystemSlot.StaticShotLauncherN:
            case SubsystemSlot.StaticShotLauncherNE:
            case SubsystemSlot.StaticShotLauncherE:
            case SubsystemSlot.StaticShotLauncherSE:
            case SubsystemSlot.StaticShotLauncherS:
            case SubsystemSlot.StaticShotLauncherSW:
            case SubsystemSlot.StaticShotLauncherW:
            case SubsystemSlot.StaticShotLauncherNW:
                return targetTier switch
                {
                    1 => new Costs(0, 0f, 2f, 2f, 0f, 0f, 0f, 0f),
                    2 => new Costs(0, 0f, 4f, 8f, 0f, 0f, 0f, 0f),
                    3 => new Costs(0, 0f, 6f, 16f, 0f, 0f, 0f, 0f),
                    4 => new Costs(0, 0f, 8f, 32f, 0f, 0f, 0f, 0f),
                    5 => new Costs(0, 0f, 10f, 48f, 0f, 0f, 0f, 0f),
                    _ => ZeroCosts
                };
            case SubsystemSlot.DynamicShotMagazine:
                return targetTier switch
                {
                    1 => new Costs(0, 0f, 6f, 2f, 0f, 0f, 0f, 0f),
                    2 => new Costs(0, 0f, 12f, 4f, 0f, 0f, 0f, 0f),
                    3 => new Costs(0, 0f, 20f, 12f, 0f, 0f, 0f, 0f),
                    4 => new Costs(0, 0f, 32f, 32f, 0f, 0f, 0f, 0f),
                    5 => new Costs(0, 0f, 48f, 48f, 0f, 0f, 0f, 0f),
                    _ => ZeroCosts
                };
            case SubsystemSlot.StaticShotMagazineN:
            case SubsystemSlot.StaticShotMagazineNE:
            case SubsystemSlot.StaticShotMagazineE:
            case SubsystemSlot.StaticShotMagazineSE:
            case SubsystemSlot.StaticShotMagazineS:
            case SubsystemSlot.StaticShotMagazineSW:
            case SubsystemSlot.StaticShotMagazineW:
            case SubsystemSlot.StaticShotMagazineNW:
                return targetTier switch
                {
                    1 => new Costs(0, 0f, 4f, 1f, 0f, 0f, 0f, 0f),
                    2 => new Costs(0, 0f, 9f, 2f, 0f, 0f, 0f, 0f),
                    3 => new Costs(0, 0f, 16f, 8f, 0f, 0f, 0f, 0f),
                    4 => new Costs(0, 0f, 24f, 32f, 0f, 0f, 0f, 0f),
                    5 => new Costs(0, 0f, 32f, 48f, 0f, 0f, 0f, 0f),
                    _ => ZeroCosts
                };
            case SubsystemSlot.DynamicShotFabricator:
                return targetTier switch
                {
                    1 => new Costs(0, 0f, 0f, 0f, 8f, 4f, 0f, 0f),
                    2 => new Costs(0, 0f, 0f, 0f, 16f, 8f, 0f, 0f),
                    3 => new Costs(0, 0f, 0f, 0f, 24f, 12f, 0f, 0f),
                    4 => new Costs(0, 0f, 0f, 0f, 36f, 18f, 0f, 0f),
                    5 => new Costs(0, 0f, 0f, 0f, 48f, 24f, 0f, 0f),
                    _ => ZeroCosts
                };
            case SubsystemSlot.StaticShotFabricatorN:
            case SubsystemSlot.StaticShotFabricatorNE:
            case SubsystemSlot.StaticShotFabricatorE:
            case SubsystemSlot.StaticShotFabricatorSE:
            case SubsystemSlot.StaticShotFabricatorS:
            case SubsystemSlot.StaticShotFabricatorSW:
            case SubsystemSlot.StaticShotFabricatorW:
            case SubsystemSlot.StaticShotFabricatorNW:
                return targetTier switch
                {
                    1 => new Costs(0, 0f, 0f, 0f, 6f, 3f, 0f, 0f),
                    2 => new Costs(0, 0f, 0f, 0f, 12f, 6f, 0f, 0f),
                    3 => new Costs(0, 0f, 0f, 0f, 20f, 10f, 0f, 0f),
                    4 => new Costs(0, 0f, 0f, 0f, 32f, 16f, 0f, 0f),
                    5 => new Costs(0, 0f, 0f, 0f, 48f, 20f, 0f, 0f),
                    _ => ZeroCosts
                };
            case SubsystemSlot.DynamicInterceptorLauncher:
                return targetTier switch
                {
                    1 => new Costs(0, 0f, 0f, 6f, 0f, 12f, 0f, 0f),
                    2 => new Costs(0, 0f, 0f, 12f, 0f, 24f, 0f, 0f),
                    _ => ZeroCosts
                };
            case SubsystemSlot.StaticInterceptorLauncherE:
            case SubsystemSlot.StaticInterceptorLauncherW:
                return targetTier switch
                {
                    1 => new Costs(0, 0f, 0f, 4f, 0f, 8f, 0f, 0f),
                    2 => new Costs(0, 0f, 0f, 8f, 0f, 16f, 0f, 0f),
                    _ => ZeroCosts
                };
            case SubsystemSlot.DynamicInterceptorMagazine:
            case SubsystemSlot.StaticInterceptorMagazineE:
            case SubsystemSlot.StaticInterceptorMagazineW:
                return targetTier switch
                {
                    1 => new Costs(0, 0f, 4f, 2f, 0f, 0f, 0f, 0f),
                    2 => new Costs(0, 0f, 8f, 4f, 0f, 0f, 0f, 0f),
                    3 => new Costs(0, 0f, 16f, 12f, 0f, 0f, 0f, 0f),
                    4 => new Costs(0, 0f, 24f, 32f, 0f, 0f, 0f, 0f),
                    5 => new Costs(0, 0f, 32f, 48f, 0f, 0f, 0f, 0f),
                    _ => ZeroCosts
                };
            case SubsystemSlot.DynamicInterceptorFabricator:
            case SubsystemSlot.StaticInterceptorFabricatorE:
            case SubsystemSlot.StaticInterceptorFabricatorW:
                return targetTier switch
                {
                    1 => new Costs(0, 0f, 0f, 0f, 6f, 6f, 0f, 0f),
                    2 => new Costs(0, 0f, 0f, 0f, 12f, 12f, 0f, 0f),
                    3 => new Costs(0, 0f, 0f, 0f, 20f, 20f, 0f, 0f),
                    4 => new Costs(0, 0f, 0f, 0f, 32f, 32f, 0f, 0f),
                    5 => new Costs(0, 0f, 0f, 0f, 48f, 48f, 0f, 0f),
                    _ => ZeroCosts
                };
            case SubsystemSlot.Railgun:
            case SubsystemSlot.ModernRailgunN:
            case SubsystemSlot.ModernRailgunNE:
            case SubsystemSlot.ModernRailgunE:
            case SubsystemSlot.ModernRailgunSE:
            case SubsystemSlot.ModernRailgunS:
            case SubsystemSlot.ModernRailgunSW:
            case SubsystemSlot.ModernRailgunW:
            case SubsystemSlot.ModernRailgunNW:
                return targetTier switch
                {
                    1 => new Costs(0, 0f, 12f, 0f, 0f, 12f, 0f, 0f),
                    2 => new Costs(0, 0f, 20f, 0f, 0f, 30f, 0f, 0f),
                    3 => new Costs(0, 0f, 32f, 0f, 0f, 48f, 0f, 0f),
                    _ => ZeroCosts
                };
            default:
                return ZeroCosts;
        }
    }

    public static uint GetDowngradeTicks(SubsystemSlot slot, byte currentTier)
    {
        uint upgradeTicks = GetUpgradeTicks(slot, currentTier);
        return (upgradeTicks * 7u + 9u) / 10u;
    }

    private static uint GetBatteryTicks(byte targetTier)
    {
        return targetTier switch
        {
            1 => 25,
            2 => 30,
            3 => 35,
            4 => 45,
            5 => 60,
            6 => 75,
            7 => 90,
            8 => 110,
            9 => 130,
            _ => 0
        };
    }

    private static uint GetStandardTicks(byte targetTier)
    {
        return targetTier switch
        {
            1 => 30,
            2 => 40,
            3 => 55,
            4 => 80,
            5 => 120,
            _ => 0
        };
    }

    private static uint GetHeavyTicks(byte targetTier)
    {
        return targetTier switch
        {
            1 => 40,
            2 => 55,
            3 => 75,
            4 => 105,
            5 => 145,
            _ => 0
        };
    }

    private static uint GetCompactTicks(byte targetTier)
    {
        return targetTier switch
        {
            1 => 20,
            2 => 30,
            3 => 45,
            4 => 70,
            5 => 105,
            _ => 0
        };
    }

    private static uint GetWeaponTicks(byte targetTier)
    {
        return targetTier switch
        {
            1 => 20,
            2 => 30,
            3 => 45,
            4 => 70,
            5 => 105,
            _ => 0
        };
    }
}


