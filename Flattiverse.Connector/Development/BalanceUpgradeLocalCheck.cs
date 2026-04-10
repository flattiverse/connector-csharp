using System.Collections.Concurrent;
using System.Globalization;
using Flattiverse.Connector;
using Flattiverse.Connector.Events;
using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Development;

partial class Program
{
    private const int BalanceUpgradeTimeoutMs = 30000;
    private const int BalanceBehaviorTimeoutMs = 15000;
    private const int BalanceConditionPollMs = 50;
    private const float BalanceRefillX = -1060f;
    private const float BalanceRefillY = -487f;
    private const float BalanceUpgradeX = -1220f;
    private const float BalanceUpgradeY = -487f;
    private const float BalanceBehaviorX = -1320f;
    private const float BalanceBehaviorY = -360f;
    private const float BalanceJumpX = BalanceBehaviorX + 120f;
    private const float BalanceJumpY = BalanceBehaviorY + 120f;

    private readonly struct ResourceSnapshot
    {
        public readonly float Energy;
        public readonly float IonEnergy;
        public readonly float NeutrinoEnergy;
        public readonly float Metal;
        public readonly float Carbon;
        public readonly float Hydrogen;
        public readonly float Silicon;

        public ResourceSnapshot(float energy, float ionEnergy, float neutrinoEnergy, float metal, float carbon, float hydrogen, float silicon)
        {
            Energy = energy;
            IonEnergy = ionEnergy;
            NeutrinoEnergy = neutrinoEnergy;
            Metal = metal;
            Carbon = carbon;
            Hydrogen = hydrogen;
            Silicon = silicon;
        }
    }

    private static async Task RunBalanceUpgradeCheckLocal()
    {
        Galaxy? adminGalaxy = null;
        Galaxy? playerGalaxy = null;
        Task? playerEventPump = null;
        ConcurrentQueue<FlattiverseEvent> playerEvents = new ConcurrentQueue<FlattiverseEvent>();
        string adminAuth = ResolveNpcUnitsLocalAdminAuth();
        DatabaseAccountRow adminAccountRow = QueryAccountRow(adminAuth);
        string playerAuth = ResolveNpcUnitsLocalPlayerAuth(adminAccountRow.AccountId);

        try
        {
            adminGalaxy = await ConnectLocalPlayer(adminAuth, TeamName, "BALANCE-LOCAL:ADMIN").ConfigureAwait(false);
            playerGalaxy = await ConnectLocalPlayer(playerAuth, TeamName, "BALANCE-LOCAL:PLAYER").ConfigureAwait(false);
            playerEventPump = StartEventPump("BALANCE-LOCAL:PLAYER", playerGalaxy, playerEvents);

            await Task.Delay(500).ConfigureAwait(false);
            DrainEvents(playerEvents);

            await CleanupRuntimeBalanceUnits(adminGalaxy).ConfigureAwait(false);
            await RunClassicBalanceUpgradeCheck(adminGalaxy, playerGalaxy).ConfigureAwait(false);
            await CleanupRuntimeBalanceUnits(adminGalaxy).ConfigureAwait(false);
            await RunModernBalanceUpgradeCheck(adminGalaxy, playerGalaxy).ConfigureAwait(false);

            Console.WriteLine("BALANCE-LOCAL: OK");
        }
        finally
        {
            if (playerGalaxy is not null)
                playerGalaxy.Dispose();

            if (playerEventPump is not null)
                await Task.WhenAny(playerEventPump, Task.Delay(1000)).ConfigureAwait(false);

            if (adminGalaxy is not null)
                adminGalaxy.Dispose();
        }
    }

    private static async Task RunClassicBalanceUpgradeCheck(Galaxy adminGalaxy, Galaxy playerGalaxy)
    {
        ClassicShipControllable ship = await playerGalaxy.CreateClassicShip($"BalanceClassic{Environment.ProcessId}").ConfigureAwait(false);

        try
        {
            Console.WriteLine("BALANCE-LOCAL: classic create");
            await ship.Continue().ConfigureAwait(false);

            if (!await WaitForAliveState(ship, true, 5000).ConfigureAwait(false))
                throw new InvalidOperationException("BALANCE-LOCAL: classic ship did not become alive.");

            await PrepareUpgradeResources(adminGalaxy, ship, "Classic Spawn").ConfigureAwait(false);

            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.Cargo, "Classic Cargo T2").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.Cargo, "Classic Cargo T3").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.Cargo, "Classic Cargo T4").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.Hull, "Classic Hull").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.ResourceMiner, "Classic Miner").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.EnergyBattery, "Classic EnergyBattery").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.EnergyCell, "Classic EnergyCell").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.MainScanner, "Classic MainScanner").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.Engine, "Classic Engine").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.ShotLauncher, "Classic ShotLauncher").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.ShotMagazine, "Classic ShotMagazine").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.ShotFabricator, "Classic ShotFabricator").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.Armor, "Classic Armor install").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.Shield, "Classic Shield install").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.Repair, "Classic Repair install").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.SecondaryScanner, "Classic SecondaryScanner install").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.InterceptorLauncher, "Classic InterceptorLauncher install").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.InterceptorMagazine, "Classic InterceptorMagazine install").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.InterceptorFabricator, "Classic InterceptorFabricator install").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.Railgun, "Classic Railgun install").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.NebulaCollector, "Classic NebulaCollector install").ConfigureAwait(false);

            ship = await DowngradeAndVerify(adminGalaxy, ship, ship.MainScanner, "Classic MainScanner downgrade").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.MainScanner, "Classic MainScanner restore").ConfigureAwait(false);
            ship = await DowngradeAndVerify(adminGalaxy, ship, ship.SecondaryScanner, "Classic SecondaryScanner downgrade").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.SecondaryScanner, "Classic SecondaryScanner restore").ConfigureAwait(false);
            ship = await DowngradeAndVerify(adminGalaxy, ship, ship.Engine, "Classic Engine downgrade").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.Engine, "Classic Engine restore").ConfigureAwait(false);
            ship = await DowngradeAndVerify(adminGalaxy, ship, ship.ShotLauncher, "Classic ShotLauncher downgrade").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.ShotLauncher, "Classic ShotLauncher restore").ConfigureAwait(false);
            ship = await DowngradeAndVerify(adminGalaxy, ship, ship.ShotMagazine, "Classic ShotMagazine downgrade").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.ShotMagazine, "Classic ShotMagazine restore").ConfigureAwait(false);
            ship = await DowngradeAndVerify(adminGalaxy, ship, ship.ShotFabricator, "Classic ShotFabricator downgrade").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.ShotFabricator, "Classic ShotFabricator restore").ConfigureAwait(false);

            ship = await DebugSetTier(adminGalaxy, ship, ship.MainScanner, 5, "Classic MainScanner T5 debug").ConfigureAwait(false);
            await ship.MainScanner.Set(ship.MainScanner.MaximumWidth, ship.MainScanner.MaximumLength, 0f).ConfigureAwait(false);
            await ship.MainScanner.On().ConfigureAwait(false);

            if (!await WaitForCondition(delegate { return ship.MainScanner.Status == SubsystemStatus.Failed; }, 4000).ConfigureAwait(false))
                throw new InvalidOperationException("BALANCE-LOCAL: classic T5 scanner did not fail without neutrinos.");

            await ship.MainScanner.Off().ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.NeutrinoBattery, "Classic NeutrinoBattery install").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.NeutrinoCell, "Classic NeutrinoCell install").ConfigureAwait(false);

            if (!await WaitForCondition(delegate { return ship.NeutrinoBattery.Current >= 10f; }, 5000).ConfigureAwait(false))
                throw new InvalidOperationException("BALANCE-LOCAL: classic neutrino charge missing.");

            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.StructureOptimizer, "Classic StructureOptimizer install").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.JumpDrive, "Classic JumpDrive install").ConfigureAwait(false);
            ship = await DowngradeAndVerify(adminGalaxy, ship, ship.StructureOptimizer, "Classic StructureOptimizer downgrade").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.StructureOptimizer, "Classic StructureOptimizer restore").ConfigureAwait(false);

            await ship.MainScanner.Set(ship.MainScanner.MaximumWidth, ship.MainScanner.MaximumLength, 0f).ConfigureAwait(false);
            await ship.MainScanner.On().ConfigureAwait(false);

            if (!await WaitForCondition(delegate
                {
                    return ship.MainScanner.Status == SubsystemStatus.Worked && ship.MainScanner.ConsumedNeutrinosThisTick > 0f;
                }, 4000).ConfigureAwait(false))
                throw new InvalidOperationException("BALANCE-LOCAL: classic T5 scanner did not consume neutrinos.");

            Costs classicScannerCosts = ship.MainScanner.TierInfo.CalculateResourceUsage(
                new SubsystemComponentValue(SubsystemComponentKind.Width, ship.MainScanner.CurrentWidth),
                new SubsystemComponentValue(SubsystemComponentKind.Range, ship.MainScanner.CurrentLength));
            VerifyUsageCosts("Classic T5 scanner", classicScannerCosts, ship.MainScanner.ConsumedEnergyThisTick,
                ship.MainScanner.ConsumedIonsThisTick, ship.MainScanner.ConsumedNeutrinosThisTick);

            await ship.MainScanner.Off().ConfigureAwait(false);
            ship = await DebugSetTier(adminGalaxy, ship, ship.Shield, 5, "Classic Shield T5 debug").ConfigureAwait(false);
            await DebugSetShieldCurrent(adminGalaxy, ship, 0f, "Classic Shield current").ConfigureAwait(false);
            await ship.Shield.Set(ship.Shield.MaximumRate).ConfigureAwait(false);
            await ship.Shield.On().ConfigureAwait(false);

            if (!await WaitForCondition(delegate { return ship.Shield.Status == SubsystemStatus.Failed; }, 4000).ConfigureAwait(false))
                throw new InvalidOperationException(
                    $"BALANCE-LOCAL: classic T5 shield did not fail without ions. " +
                    $"Shield=(Exists={ship.Shield.Exists}, Active={ship.Shield.Active}, Rate={ship.Shield.Rate:0.###}, " +
                    $"Current={ship.Shield.Current:0.###}, Status={ship.Shield.Status}, Consumed=({ship.Shield.ConsumedEnergyThisTick:0.###}," +
                    $"{ship.Shield.ConsumedIonsThisTick:0.###},{ship.Shield.ConsumedNeutrinosThisTick:0.###})), " +
                    $"IonBattery=({ship.IonBattery.Current:0.###}/{ship.IonBattery.Maximum:0.###}, Exists={ship.IonBattery.Exists}, Status={ship.IonBattery.Status}).");

            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.IonBattery, "Classic IonBattery install").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.IonCell, "Classic IonCell install").ConfigureAwait(false);

            if (!await WaitForCondition(delegate { return ship.IonBattery.Current > 1f; }, 5000).ConfigureAwait(false))
                throw new InvalidOperationException("BALANCE-LOCAL: classic ion charge missing.");

            await ship.Shield.Set(ship.Shield.MaximumRate).ConfigureAwait(false);
            await ship.Shield.On().ConfigureAwait(false);

            if (!await WaitForCondition(delegate
                {
                    return ship.Shield.Status == SubsystemStatus.Worked && ship.Shield.Current > 0f && ship.Shield.ConsumedIonsThisTick > 0f;
                }, 4000).ConfigureAwait(false))
                throw new InvalidOperationException("BALANCE-LOCAL: classic T5 shield did not consume ions.");

            Costs classicShieldCosts = ship.Shield.TierInfo.CalculateResourceUsage(
                new SubsystemComponentValue(SubsystemComponentKind.NormalizedPower, ship.Shield.Rate / ship.Shield.MaximumRate));
            VerifyUsageCosts("Classic T5 shield", classicShieldCosts, ship.Shield.ConsumedEnergyThisTick, ship.Shield.ConsumedIonsThisTick,
                ship.Shield.ConsumedNeutrinosThisTick);

            await ship.Shield.Off().ConfigureAwait(false);
            await VerifyClassicBehavior(adminGalaxy, ship).ConfigureAwait(false);
        }
        finally
        {
            await CloseControllable(ship).ConfigureAwait(false);
        }
    }

    private static async Task RunModernBalanceUpgradeCheck(Galaxy adminGalaxy, Galaxy playerGalaxy)
    {
        ModernShipControllable ship = await playerGalaxy.CreateModernShip($"BalanceModern{Environment.ProcessId}").ConfigureAwait(false);

        try
        {
            Console.WriteLine("BALANCE-LOCAL: modern create");
            await ship.Continue().ConfigureAwait(false);

            if (!await WaitForAliveState(ship, true, 5000).ConfigureAwait(false))
                throw new InvalidOperationException("BALANCE-LOCAL: modern ship did not become alive.");

            await PrepareUpgradeResources(adminGalaxy, ship, "Modern Spawn").ConfigureAwait(false);

            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.Cargo, "Modern Cargo T2").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.Cargo, "Modern Cargo T3").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.Cargo, "Modern Cargo T4").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.Hull, "Modern Hull").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.ResourceMiner, "Modern Miner").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.EnergyBattery, "Modern EnergyBattery").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.EnergyCell, "Modern EnergyCell").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.Armor, "Modern Armor install").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.Shield, "Modern Shield install").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.Repair, "Modern Repair install").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.NebulaCollector, "Modern NebulaCollector install").ConfigureAwait(false);

            string[] engineLabels = new string[]
            {
                "Modern EngineN", "Modern EngineNE", "Modern EngineE", "Modern EngineSE",
                "Modern EngineS", "Modern EngineSW", "Modern EngineW", "Modern EngineNW"
            };

            for (int index = 0; index < engineLabels.Length; index++)
                ship = await UpgradeAndVerify(adminGalaxy, ship, GetModernEngineSubsystem(ship, index), engineLabels[index]).ConfigureAwait(false);

            string[] scannerLabels = new string[]
            {
                "Modern ScannerN", "Modern ScannerNE", "Modern ScannerE", "Modern ScannerSE",
                "Modern ScannerS", "Modern ScannerSW", "Modern ScannerW", "Modern ScannerNW"
            };

            for (int index = 0; index < scannerLabels.Length; index++)
                ship = await UpgradeAndVerify(adminGalaxy, ship, GetModernScannerSubsystem(ship, index), scannerLabels[index]).ConfigureAwait(false);

            string[] shotSlotLabels = new string[] { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };

            for (int index = 0; index < shotSlotLabels.Length; index++)
            {
                ship = await UpgradeAndVerify(adminGalaxy, ship, GetModernShotLauncherSubsystem(ship, index), $"Modern ShotLauncher{shotSlotLabels[index]}")
                    .ConfigureAwait(false);
                ship = await UpgradeAndVerify(adminGalaxy, ship, GetModernShotMagazineSubsystem(ship, index), $"Modern ShotMagazine{shotSlotLabels[index]}")
                    .ConfigureAwait(false);
                ship = await UpgradeAndVerify(adminGalaxy, ship, GetModernShotFabricatorSubsystem(ship, index), $"Modern ShotFabricator{shotSlotLabels[index]}")
                    .ConfigureAwait(false);
            }

            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.InterceptorLauncherE, "Modern InterceptorLauncherE install").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.InterceptorLauncherW, "Modern InterceptorLauncherW install").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.InterceptorMagazineE, "Modern InterceptorMagazineE install").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.InterceptorMagazineW, "Modern InterceptorMagazineW install").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.InterceptorFabricatorE, "Modern InterceptorFabricatorE install").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.InterceptorFabricatorW, "Modern InterceptorFabricatorW install").ConfigureAwait(false);

            for (int index = 0; index < shotSlotLabels.Length; index++)
                ship = await UpgradeAndVerify(adminGalaxy, ship, GetModernRailgunSubsystem(ship, index), $"Modern Railgun{shotSlotLabels[index]} install")
                    .ConfigureAwait(false);

            ship = await DowngradeAndVerify(adminGalaxy, ship, ship.EngineNE, "Modern EngineNE downgrade").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.EngineNE, "Modern EngineNE restore").ConfigureAwait(false);
            ship = await DowngradeAndVerify(adminGalaxy, ship, ship.ScannerNE, "Modern ScannerNE downgrade").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.ScannerNE, "Modern ScannerNE restore").ConfigureAwait(false);
            ship = await DowngradeAndVerify(adminGalaxy, ship, ship.ShotLauncherNE, "Modern ShotLauncherNE downgrade").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.ShotLauncherNE, "Modern ShotLauncherNE restore").ConfigureAwait(false);
            ship = await DowngradeAndVerify(adminGalaxy, ship, ship.ShotMagazineNE, "Modern ShotMagazineNE downgrade").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.ShotMagazineNE, "Modern ShotMagazineNE restore").ConfigureAwait(false);
            ship = await DowngradeAndVerify(adminGalaxy, ship, ship.ShotFabricatorNE, "Modern ShotFabricatorNE downgrade").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.ShotFabricatorNE, "Modern ShotFabricatorNE restore").ConfigureAwait(false);
            ship = await DowngradeAndVerify(adminGalaxy, ship, ship.InterceptorLauncherE, "Modern InterceptorLauncherE downgrade").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.InterceptorLauncherE, "Modern InterceptorLauncherE restore").ConfigureAwait(false);
            ship = await DowngradeAndVerify(adminGalaxy, ship, ship.InterceptorMagazineE, "Modern InterceptorMagazineE downgrade").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.InterceptorMagazineE, "Modern InterceptorMagazineE restore").ConfigureAwait(false);
            ship = await DowngradeAndVerify(adminGalaxy, ship, ship.InterceptorFabricatorE, "Modern InterceptorFabricatorE downgrade").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.InterceptorFabricatorE, "Modern InterceptorFabricatorE restore").ConfigureAwait(false);
            ship = await DowngradeAndVerify(adminGalaxy, ship, ship.RailgunN, "Modern RailgunN downgrade").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.RailgunN, "Modern RailgunN restore").ConfigureAwait(false);

            ship = await DebugSetTier(adminGalaxy, ship, ship.ScannerN, 5, "Modern ScannerN T5 debug").ConfigureAwait(false);
            await ship.ScannerN.Set(ship.ScannerN.MaximumWidth, ship.ScannerN.MaximumLength, 0f).ConfigureAwait(false);
            await ship.ScannerN.On().ConfigureAwait(false);

            if (!await WaitForCondition(delegate { return ship.ScannerN.Status == SubsystemStatus.Failed; }, 4000).ConfigureAwait(false))
                throw new InvalidOperationException("BALANCE-LOCAL: modern T5 scanner did not fail without neutrinos.");

            await ship.ScannerN.Off().ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.NeutrinoBattery, "Modern NeutrinoBattery install").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.NeutrinoCell, "Modern NeutrinoCell install").ConfigureAwait(false);

            if (!await WaitForCondition(delegate { return ship.NeutrinoBattery.Current >= 10f; }, 5000).ConfigureAwait(false))
                throw new InvalidOperationException("BALANCE-LOCAL: modern neutrino charge missing.");

            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.StructureOptimizer, "Modern StructureOptimizer install").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.JumpDrive, "Modern JumpDrive install").ConfigureAwait(false);
            ship = await DowngradeAndVerify(adminGalaxy, ship, ship.StructureOptimizer, "Modern StructureOptimizer downgrade").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.StructureOptimizer, "Modern StructureOptimizer restore").ConfigureAwait(false);

            await ship.ScannerN.Set(ship.ScannerN.MaximumWidth, ship.ScannerN.MaximumLength, 0f).ConfigureAwait(false);
            await ship.ScannerN.On().ConfigureAwait(false);

            if (!await WaitForCondition(delegate
                {
                    return ship.ScannerN.Status == SubsystemStatus.Worked && ship.ScannerN.ConsumedNeutrinosThisTick > 0f;
                }, 4000).ConfigureAwait(false))
                throw new InvalidOperationException("BALANCE-LOCAL: modern T5 scanner did not consume neutrinos.");

            Costs modernScannerCosts = ship.ScannerN.TierInfo.CalculateResourceUsage(
                new SubsystemComponentValue(SubsystemComponentKind.Width, ship.ScannerN.CurrentWidth),
                new SubsystemComponentValue(SubsystemComponentKind.Range, ship.ScannerN.CurrentLength));
            VerifyUsageCosts("Modern T5 scanner", modernScannerCosts, ship.ScannerN.ConsumedEnergyThisTick,
                ship.ScannerN.ConsumedIonsThisTick, ship.ScannerN.ConsumedNeutrinosThisTick);

            await ship.ScannerN.Off().ConfigureAwait(false);
            ship = await DebugSetTier(adminGalaxy, ship, ship.Shield, 5, "Modern Shield T5 debug").ConfigureAwait(false);
            await DebugSetShieldCurrent(adminGalaxy, ship, 0f, "Modern Shield current").ConfigureAwait(false);
            await ship.Shield.Set(ship.Shield.MaximumRate).ConfigureAwait(false);
            await ship.Shield.On().ConfigureAwait(false);

            if (!await WaitForCondition(delegate { return ship.Shield.Status == SubsystemStatus.Failed; }, 4000).ConfigureAwait(false))
                throw new InvalidOperationException(
                    $"BALANCE-LOCAL: modern T5 shield did not fail without ions. " +
                    $"Shield=(Exists={ship.Shield.Exists}, Active={ship.Shield.Active}, Rate={ship.Shield.Rate:0.###}, " +
                    $"Current={ship.Shield.Current:0.###}, Status={ship.Shield.Status}, Consumed=({ship.Shield.ConsumedEnergyThisTick:0.###}," +
                    $"{ship.Shield.ConsumedIonsThisTick:0.###},{ship.Shield.ConsumedNeutrinosThisTick:0.###})), " +
                    $"IonBattery=({ship.IonBattery.Current:0.###}/{ship.IonBattery.Maximum:0.###}, Exists={ship.IonBattery.Exists}, Status={ship.IonBattery.Status}).");

            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.IonBattery, "Modern IonBattery install").ConfigureAwait(false);
            ship = await UpgradeAndVerify(adminGalaxy, ship, ship.IonCell, "Modern IonCell install").ConfigureAwait(false);

            if (!await WaitForCondition(delegate { return ship.IonBattery.Current > 1f; }, 5000).ConfigureAwait(false))
                throw new InvalidOperationException("BALANCE-LOCAL: modern ion charge missing.");

            await ship.Shield.Set(ship.Shield.MaximumRate).ConfigureAwait(false);
            await ship.Shield.On().ConfigureAwait(false);

            if (!await WaitForCondition(delegate
                {
                    return ship.Shield.Status == SubsystemStatus.Worked && ship.Shield.Current > 0f && ship.Shield.ConsumedIonsThisTick > 0f;
                }, 4000).ConfigureAwait(false))
                throw new InvalidOperationException("BALANCE-LOCAL: modern T5 shield did not consume ions.");

            Costs modernShieldCosts = ship.Shield.TierInfo.CalculateResourceUsage(
                new SubsystemComponentValue(SubsystemComponentKind.NormalizedPower, ship.Shield.Rate / ship.Shield.MaximumRate));
            VerifyUsageCosts("Modern T5 shield", modernShieldCosts, ship.Shield.ConsumedEnergyThisTick, ship.Shield.ConsumedIonsThisTick,
                ship.Shield.ConsumedNeutrinosThisTick);

            await ship.Shield.Off().ConfigureAwait(false);
            await VerifyModernBehavior(adminGalaxy, ship).ConfigureAwait(false);
        }
        finally
        {
            await CloseControllable(ship).ConfigureAwait(false);
        }
    }

    private static async Task VerifyClassicBehavior(Galaxy adminGalaxy, ClassicShipControllable ship)
    {
        Console.WriteLine("BALANCE-LOCAL: classic behavior");
        await ship.Shield.Set(ship.Shield.MaximumRate).ConfigureAwait(false);
        await ship.Shield.On().ConfigureAwait(false);

        if (!await WaitForCondition(delegate
            {
                return ship.Shield.Status == SubsystemStatus.Worked && ship.Shield.Current > 0f;
            }, BalanceBehaviorTimeoutMs).ConfigureAwait(false))
            throw new InvalidOperationException("BALANCE-LOCAL: classic shield did not work.");

        Costs classicShieldCosts = ship.Shield.TierInfo.CalculateResourceUsage(
            new SubsystemComponentValue(SubsystemComponentKind.NormalizedPower, ship.Shield.Rate / ship.Shield.MaximumRate));
        VerifyUsageCosts("Classic behavior shield", classicShieldCosts, ship.Shield.ConsumedEnergyThisTick, ship.Shield.ConsumedIonsThisTick,
            ship.Shield.ConsumedNeutrinosThisTick);

        await ship.Shield.Off().ConfigureAwait(false);

        if (!ship.Alive)
            throw new InvalidOperationException("BALANCE-LOCAL: classic ship died before repair setup.");

        float damagedHull = await DebugSetHullCurrent(adminGalaxy, ship, ship.Hull.Maximum - 10f, "Classic Hull current").ConfigureAwait(false);

        if (!(damagedHull < ship.Hull.Maximum))
            throw new InvalidOperationException("BALANCE-LOCAL: classic hull was not damaged for repair test.");

        if (!ship.Alive)
            throw new InvalidOperationException("BALANCE-LOCAL: classic ship died during hull debug-set.");

        await ship.Repair.Set(ship.Repair.MaximumRate).ConfigureAwait(false);

        if (!await WaitForCondition(delegate
            {
                return ship.Repair.Status == SubsystemStatus.Worked && ship.Repair.RepairedHullThisTick > 0f;
            }, BalanceBehaviorTimeoutMs).ConfigureAwait(false))
            throw new InvalidOperationException("BALANCE-LOCAL: classic repair did not work.");

        Costs classicRepairCosts = ship.Repair.TierInfo.CalculateResourceUsage(
            new SubsystemComponentValue(SubsystemComponentKind.NormalizedPower, ship.Repair.Rate / ship.Repair.MaximumRate));
        VerifyUsageCosts("Classic behavior repair", classicRepairCosts, ship.Repair.ConsumedEnergyThisTick, ship.Repair.ConsumedIonsThisTick,
            ship.Repair.ConsumedNeutrinosThisTick);

        await ship.Repair.Set(0f).ConfigureAwait(false);

        if (!ship.Alive)
            throw new InvalidOperationException("BALANCE-LOCAL: classic ship died after repair.");

        Vector classicBehaviorPosition = new Vector(BalanceBehaviorX, BalanceBehaviorY);
        await DebugSetPosition(adminGalaxy, ship, classicBehaviorPosition, "Classic Behavior position").ConfigureAwait(false);
        await StabilizeShipForStationarySystems(adminGalaxy, ship, "Classic Behavior stabilize").ConfigureAwait(false);
        await DebugClearNonMetalCargo(adminGalaxy, ship, "Classic Cargo clear").ConfigureAwait(false);
        await RefillBehaviorSupplies(adminGalaxy, ship, classicBehaviorPosition, "Classic Behavior").ConfigureAwait(false);
        await DebugClearNonMetalCargo(adminGalaxy, ship, "Classic Cargo reclear").ConfigureAwait(false);
        await DebugSetBatteryCurrent(adminGalaxy, ship, ship.EnergyBattery, ship.EnergyBattery.Maximum, "Classic EnergyBattery current")
            .ConfigureAwait(false);
        await DebugSetBatteryCurrent(adminGalaxy, ship, ship.IonBattery, ship.IonBattery.Maximum, "Classic IonBattery current")
            .ConfigureAwait(false);
        await DebugSetBatteryCurrent(adminGalaxy, ship, ship.NeutrinoBattery, ship.NeutrinoBattery.Maximum,
            "Classic NeutrinoBattery current").ConfigureAwait(false);
        await PrepareRuntimeBalanceResources(adminGalaxy, ship).ConfigureAwait(false);
        await ship.ResourceMiner.Set(ship.ResourceMiner.MaximumRate).ConfigureAwait(false);

        if (!await WaitForCondition(delegate
            {
                return ship.ResourceMiner.Status == SubsystemStatus.Worked &&
                    (ship.ResourceMiner.MinedMetalThisTick > 0f || ship.ResourceMiner.MinedCarbonThisTick > 0f ||
                     ship.ResourceMiner.MinedHydrogenThisTick > 0f || ship.ResourceMiner.MinedSiliconThisTick > 0f);
            }, BalanceBehaviorTimeoutMs).ConfigureAwait(false))
            throw new InvalidOperationException(
                $"BALANCE-LOCAL: classic miner did not work. Position={ship.Position}, Movement={ship.Movement}, Size={ship.Size:0.###}, " +
                $"Status={ship.ResourceMiner.Status}, Rate={ship.ResourceMiner.Rate:0.###}, " +
                $"Mined=({ship.ResourceMiner.MinedMetalThisTick:0.###},{ship.ResourceMiner.MinedCarbonThisTick:0.###}," +
                $"{ship.ResourceMiner.MinedHydrogenThisTick:0.###},{ship.ResourceMiner.MinedSiliconThisTick:0.###}).");

        Costs classicMinerCosts = ship.ResourceMiner.TierInfo.CalculateResourceUsage(
            new SubsystemComponentValue(SubsystemComponentKind.NormalizedPower, ship.ResourceMiner.Rate / ship.ResourceMiner.MaximumRate));
        VerifyUsageCosts("Classic behavior miner", classicMinerCosts, ship.ResourceMiner.ConsumedEnergyThisTick,
            ship.ResourceMiner.ConsumedIonsThisTick, ship.ResourceMiner.ConsumedNeutrinosThisTick);

        await ship.ResourceMiner.Off().ConfigureAwait(false);
        await ship.NebulaCollector.Set(ship.NebulaCollector.MaximumRate).ConfigureAwait(false);

        if (!await WaitForCondition(delegate
            {
                return ship.NebulaCollector.Status == SubsystemStatus.Worked && ship.NebulaCollector.CollectedThisTick > 0f;
            }, BalanceBehaviorTimeoutMs).ConfigureAwait(false))
            throw new InvalidOperationException("BALANCE-LOCAL: classic nebula collector did not work.");

        Costs classicNebulaCosts = ship.NebulaCollector.TierInfo.CalculateResourceUsage(
            new SubsystemComponentValue(SubsystemComponentKind.NormalizedPower, ship.NebulaCollector.Rate / ship.NebulaCollector.MaximumRate));
        VerifyUsageCosts("Classic behavior nebula collector", classicNebulaCosts, ship.NebulaCollector.ConsumedEnergyThisTick,
            ship.NebulaCollector.ConsumedIonsThisTick, ship.NebulaCollector.ConsumedNeutrinosThisTick);

        await ship.NebulaCollector.Off().ConfigureAwait(false);
        await CleanupRuntimeBalanceResources(adminGalaxy, ship).ConfigureAwait(false);

        if (ship.ShotMagazine.CurrentShots < 1f)
        {
            await ship.ShotFabricator.Set(ship.ShotFabricator.MaximumRate).ConfigureAwait(false);
            await ship.ShotFabricator.On().ConfigureAwait(false);

            if (!await WaitForCondition(delegate
                {
                    return ship.ShotFabricator.Status == SubsystemStatus.Worked && ship.ShotMagazine.CurrentShots >= 1f;
                }, BalanceBehaviorTimeoutMs).ConfigureAwait(false))
                throw new InvalidOperationException(
                    $"BALANCE-LOCAL: classic shot launcher prefill failed. " +
                    $"Magazine={ship.ShotMagazine.CurrentShots:0.###}, " +
                    $"Fabricator=(Status={ship.ShotFabricator.Status}, Active={ship.ShotFabricator.Active}, Rate={ship.ShotFabricator.Rate:0.###})).");

            await ship.ShotFabricator.Off().ConfigureAwait(false);
        }

        ResourceSnapshot classicShotResourcesBefore = CaptureResourceSnapshot(ship);
        float shotsBefore = ship.ShotMagazine.CurrentShots;
        float classicShotSpeed = ship.ShotLauncher.MaximumRelativeMovement;
        ushort classicShotTicks = ship.ShotLauncher.MinimumTicks;
        float classicShotLoad = ship.ShotLauncher.MinimumLoad;
        float classicShotDamage = ship.ShotLauncher.MinimumDamage;
        await ship.ShotLauncher.Shoot(Vector.FromAngleLength(0f, classicShotSpeed), classicShotTicks, classicShotLoad, classicShotDamage)
            .ConfigureAwait(false);

        if (!await WaitForCondition(delegate
            {
                return ship.ShotMagazine.CurrentShots < shotsBefore;
            }, BalanceBehaviorTimeoutMs).ConfigureAwait(false))
            throw new InvalidOperationException(
                $"BALANCE-LOCAL: classic shot launcher did not fire. " +
                $"Magazine={ship.ShotMagazine.CurrentShots:0.###}/{shotsBefore:0.###}, " +
                $"Launcher=(Status={ship.ShotLauncher.Status}, Speed={ship.ShotLauncher.RelativeMovement.Length:0.###}, " +
                $"Ticks={ship.ShotLauncher.Ticks}, Load={ship.ShotLauncher.Load:0.###}, Damage={ship.ShotLauncher.Damage:0.###}, " +
                $"Consumed=({ship.ShotLauncher.ConsumedEnergyThisTick:0.###},{ship.ShotLauncher.ConsumedIonsThisTick:0.###}," +
                $"{ship.ShotLauncher.ConsumedNeutrinosThisTick:0.###}))).");

        Costs classicShotLaunchCosts = ship.ShotLauncher.TierInfo.CalculateResourceUsage(
            new SubsystemComponentValue(SubsystemComponentKind.Base, 1f),
            new SubsystemComponentValue(SubsystemComponentKind.RelativeSpeed, classicShotSpeed),
            new SubsystemComponentValue(SubsystemComponentKind.Ticks, classicShotTicks),
            new SubsystemComponentValue(SubsystemComponentKind.ExplosionLoad, classicShotLoad),
            new SubsystemComponentValue(SubsystemComponentKind.Damage, classicShotDamage));
        ResourceSnapshot classicShotResourcesAfter = CaptureResourceSnapshot(ship);
        Console.WriteLine(
            $"BALANCE-LOCAL: classic shot request speed={classicShotSpeed:0.###} ticks={classicShotTicks} load={classicShotLoad:0.###} damage={classicShotDamage:0.###} " +
            $"runtimeSpeed={ship.ShotLauncher.RelativeMovement.Length:0.###} runtimeTicks={ship.ShotLauncher.Ticks} runtimeLoad={ship.ShotLauncher.Load:0.###} " +
            $"runtimeDamage={ship.ShotLauncher.Damage:0.###} consumed=({ship.ShotLauncher.ConsumedEnergyThisTick:0.###}," +
            $"{ship.ShotLauncher.ConsumedIonsThisTick:0.###},{ship.ShotLauncher.ConsumedNeutrinosThisTick:0.###})");
        VerifyTransientUsageCosts("Classic behavior shot launcher", classicShotLaunchCosts, classicShotResourcesBefore,
            classicShotResourcesAfter, ship.ShotLauncher.ConsumedEnergyThisTick, ship.ShotLauncher.ConsumedIonsThisTick,
            ship.ShotLauncher.ConsumedNeutrinosThisTick);

        float shotsAfterFire = ship.ShotMagazine.CurrentShots;
        await ship.ShotFabricator.Set(ship.ShotFabricator.MaximumRate).ConfigureAwait(false);
        await ship.ShotFabricator.On().ConfigureAwait(false);

        if (!await WaitForCondition(delegate
            {
                return ship.ShotFabricator.Status == SubsystemStatus.Worked && ship.ShotMagazine.CurrentShots > shotsAfterFire;
            }, BalanceBehaviorTimeoutMs).ConfigureAwait(false))
            throw new InvalidOperationException(
                $"BALANCE-LOCAL: classic shot fabricator did not refill. " +
                $"Magazine={ship.ShotMagazine.CurrentShots:0.###}/{shotsAfterFire:0.###}, " +
                $"Fabricator=(Status={ship.ShotFabricator.Status}, Active={ship.ShotFabricator.Active}, Rate={ship.ShotFabricator.Rate:0.###}, " +
                $"Consumed=({ship.ShotFabricator.ConsumedEnergyThisTick:0.###},{ship.ShotFabricator.ConsumedIonsThisTick:0.###}," +
                $"{ship.ShotFabricator.ConsumedNeutrinosThisTick:0.###})), TimeoutMs={BalanceBehaviorTimeoutMs}.");

        Costs classicShotFabricatorCosts = ship.ShotFabricator.TierInfo.CalculateResourceUsage(
            new SubsystemComponentValue(SubsystemComponentKind.NormalizedPower, ship.ShotFabricator.Rate / ship.ShotFabricator.MaximumRate));
        VerifyUsageCosts("Classic behavior shot fabricator", classicShotFabricatorCosts, ship.ShotFabricator.ConsumedEnergyThisTick,
            ship.ShotFabricator.ConsumedIonsThisTick, ship.ShotFabricator.ConsumedNeutrinosThisTick);

        await ship.ShotFabricator.Off().ConfigureAwait(false);

        if (ship.InterceptorMagazine.CurrentShots < 1f)
        {
            await ship.InterceptorFabricator.Set(ship.InterceptorFabricator.MaximumRate).ConfigureAwait(false);
            await ship.InterceptorFabricator.On().ConfigureAwait(false);

            if (!await WaitForCondition(delegate
                {
                    return ship.InterceptorFabricator.Status == SubsystemStatus.Worked && ship.InterceptorMagazine.CurrentShots >= 1f;
                }, BalanceBehaviorTimeoutMs).ConfigureAwait(false))
                throw new InvalidOperationException(
                    $"BALANCE-LOCAL: classic interceptor launcher prefill failed. " +
                    $"Magazine={ship.InterceptorMagazine.CurrentShots:0.###}, " +
                    $"Fabricator=(Status={ship.InterceptorFabricator.Status}, Active={ship.InterceptorFabricator.Active}, Rate={ship.InterceptorFabricator.Rate:0.###})).");

            await ship.InterceptorFabricator.Off().ConfigureAwait(false);
        }

        ResourceSnapshot classicInterceptorResourcesBefore = CaptureResourceSnapshot(ship);
        float interceptorsBefore = ship.InterceptorMagazine.CurrentShots;
        await ship.InterceptorLauncher.Shoot(Vector.FromAngleLength(0f, ship.InterceptorLauncher.MaximumRelativeMovement),
            ship.InterceptorLauncher.MinimumTicks, ship.InterceptorLauncher.MinimumLoad, ship.InterceptorLauncher.MinimumDamage).ConfigureAwait(false);

        if (!await WaitForCondition(delegate
            {
                return ship.InterceptorMagazine.CurrentShots < interceptorsBefore;
            }, BalanceBehaviorTimeoutMs).ConfigureAwait(false))
            throw new InvalidOperationException(
                $"BALANCE-LOCAL: classic interceptor launcher did not fire. " +
                $"Magazine={ship.InterceptorMagazine.CurrentShots:0.###}/{interceptorsBefore:0.###}, " +
                $"Launcher=(Status={ship.InterceptorLauncher.Status}, Ticks={ship.InterceptorLauncher.Ticks}, Load={ship.InterceptorLauncher.Load:0.###}, " +
                $"Damage={ship.InterceptorLauncher.Damage:0.###}, Consumed=({ship.InterceptorLauncher.ConsumedEnergyThisTick:0.###}," +
                $"{ship.InterceptorLauncher.ConsumedIonsThisTick:0.###},{ship.InterceptorLauncher.ConsumedNeutrinosThisTick:0.###}))).");

        Costs classicInterceptorLaunchCosts = ship.InterceptorLauncher.TierInfo.CalculateResourceUsage(
            new SubsystemComponentValue(SubsystemComponentKind.Base, 1f));
        ResourceSnapshot classicInterceptorResourcesAfter = CaptureResourceSnapshot(ship);
        VerifyTransientUsageCosts("Classic behavior interceptor launcher", classicInterceptorLaunchCosts, classicInterceptorResourcesBefore,
            classicInterceptorResourcesAfter, ship.InterceptorLauncher.ConsumedEnergyThisTick, ship.InterceptorLauncher.ConsumedIonsThisTick,
            ship.InterceptorLauncher.ConsumedNeutrinosThisTick);

        float interceptorsAfterFire = ship.InterceptorMagazine.CurrentShots;
        await ship.InterceptorFabricator.Set(ship.InterceptorFabricator.MaximumRate).ConfigureAwait(false);
        await ship.InterceptorFabricator.On().ConfigureAwait(false);

        if (!await WaitForCondition(delegate
            {
                return ship.InterceptorFabricator.Status == SubsystemStatus.Worked &&
                    ship.InterceptorMagazine.CurrentShots > interceptorsAfterFire;
            }, BalanceBehaviorTimeoutMs).ConfigureAwait(false))
            throw new InvalidOperationException(
                $"BALANCE-LOCAL: classic interceptor fabricator did not refill. " +
                $"Magazine={ship.InterceptorMagazine.CurrentShots:0.###}/{interceptorsAfterFire:0.###}, " +
                $"Fabricator=(Status={ship.InterceptorFabricator.Status}, Active={ship.InterceptorFabricator.Active}, Rate={ship.InterceptorFabricator.Rate:0.###}, " +
                $"Consumed=({ship.InterceptorFabricator.ConsumedEnergyThisTick:0.###},{ship.InterceptorFabricator.ConsumedIonsThisTick:0.###}," +
                $"{ship.InterceptorFabricator.ConsumedNeutrinosThisTick:0.###})), TimeoutMs={BalanceBehaviorTimeoutMs}.");

        Costs classicInterceptorFabricatorCosts = ship.InterceptorFabricator.TierInfo.CalculateResourceUsage(
            new SubsystemComponentValue(SubsystemComponentKind.NormalizedPower,
                ship.InterceptorFabricator.Rate / ship.InterceptorFabricator.MaximumRate));
        VerifyUsageCosts("Classic behavior interceptor fabricator", classicInterceptorFabricatorCosts,
            ship.InterceptorFabricator.ConsumedEnergyThisTick, ship.InterceptorFabricator.ConsumedIonsThisTick,
            ship.InterceptorFabricator.ConsumedNeutrinosThisTick);

        await ship.InterceptorFabricator.Off().ConfigureAwait(false);
        float metalBefore = ship.Cargo.CurrentMetal;
        float railgunMetalCost = ship.Railgun.MetalCost;
        await ship.Railgun.FireFront().ConfigureAwait(false);

        if (!await WaitForCondition(delegate
            {
                return ship.Cargo.CurrentMetal <= metalBefore - railgunMetalCost;
            }, BalanceBehaviorTimeoutMs).ConfigureAwait(false))
            throw new InvalidOperationException(
                $"BALANCE-LOCAL: classic railgun did not fire. MetalBefore={metalBefore:0.###}, " +
                $"MetalAfter={ship.Cargo.CurrentMetal:0.###}, MetalCost={railgunMetalCost:0.###}, " +
                $"Status={ship.Railgun.Status}, Direction={ship.Railgun.Direction}.");

        Costs classicRailgunCosts = ship.Railgun.TierInfo.CalculateResourceUsage(new SubsystemComponentValue(SubsystemComponentKind.Base, 1f));

        if (MathF.Abs(classicRailgunCosts.Metal - (metalBefore - ship.Cargo.CurrentMetal)) > 0.001f)
            throw new InvalidOperationException(
                $"BALANCE-LOCAL: classic railgun metal mismatch. Expected={classicRailgunCosts.Metal:0.###}, " +
                $"Actual={metalBefore - ship.Cargo.CurrentMetal:0.###}.");

        Vector positionBefore = ship.Position;
        await ship.Engine.Set(Vector.FromAngleLength(0f, ship.Engine.Maximum)).ConfigureAwait(false);

        if (!await WaitForCondition(delegate { return Vector.Distance(positionBefore, ship.Position) > 5f; }, BalanceBehaviorTimeoutMs).ConfigureAwait(false))
            throw new InvalidOperationException("BALANCE-LOCAL: classic ship did not move.");

        Costs classicEngineCosts = ship.Engine.TierInfo.CalculateResourceUsage(
            new SubsystemComponentValue(SubsystemComponentKind.NormalizedPower, ship.Engine.Current.Length / ship.Engine.Maximum));
        VerifyUsageCosts("Classic behavior engine", classicEngineCosts, ship.Engine.ConsumedEnergyThisTick, ship.Engine.ConsumedIonsThisTick,
            ship.Engine.ConsumedNeutrinosThisTick);

        await ship.Engine.Off().ConfigureAwait(false);
        Vector classicJumpPosition = new Vector(BalanceJumpX, BalanceJumpY);
        await DebugSetPosition(adminGalaxy, ship, classicJumpPosition, "Classic Jump position").ConfigureAwait(false);

        await PrepareRuntimeBalanceWormhole(adminGalaxy, ship).ConfigureAwait(false);
        await ship.JumpDrive.Jump().ConfigureAwait(false);

        if (!await WaitForCondition(delegate { return ship.Cluster.Id == 1; }, BalanceBehaviorTimeoutMs).ConfigureAwait(false))
            throw new InvalidOperationException("BALANCE-LOCAL: classic jump drive did not work.");

        Costs classicJumpCosts = ship.JumpDrive.TierInfo.CalculateResourceUsage(new SubsystemComponentValue(SubsystemComponentKind.Base, 1f));
        VerifyUsageCosts("Classic behavior jump drive", classicJumpCosts, ship.JumpDrive.ConsumedEnergyThisTick,
            ship.JumpDrive.ConsumedIonsThisTick, ship.JumpDrive.ConsumedNeutrinosThisTick);
    }

    private static async Task VerifyModernBehavior(Galaxy adminGalaxy, ModernShipControllable ship)
    {
        Console.WriteLine("BALANCE-LOCAL: modern behavior");

        if (!ship.Alive)
            throw new InvalidOperationException("BALANCE-LOCAL: modern ship died before repair setup.");

        float damagedHull = await DebugSetHullCurrent(adminGalaxy, ship, ship.Hull.Maximum - 10f, "Modern Hull current").ConfigureAwait(false);

        if (!(damagedHull < ship.Hull.Maximum))
            throw new InvalidOperationException("BALANCE-LOCAL: modern hull was not damaged for repair test.");

        if (!ship.Alive)
            throw new InvalidOperationException("BALANCE-LOCAL: modern ship died during hull debug-set.");

        await ship.Repair.Set(ship.Repair.MaximumRate).ConfigureAwait(false);

        if (!await WaitForCondition(delegate
            {
                return ship.Repair.Status == SubsystemStatus.Worked && ship.Repair.RepairedHullThisTick > 0f;
            }, BalanceBehaviorTimeoutMs).ConfigureAwait(false))
            throw new InvalidOperationException("BALANCE-LOCAL: modern repair did not work.");

        Costs modernRepairCosts = ship.Repair.TierInfo.CalculateResourceUsage(
            new SubsystemComponentValue(SubsystemComponentKind.NormalizedPower, ship.Repair.Rate / ship.Repair.MaximumRate));
        VerifyUsageCosts("Modern behavior repair", modernRepairCosts, ship.Repair.ConsumedEnergyThisTick, ship.Repair.ConsumedIonsThisTick,
            ship.Repair.ConsumedNeutrinosThisTick);

        await ship.Repair.Set(0f).ConfigureAwait(false);

        if (!ship.Alive)
            throw new InvalidOperationException("BALANCE-LOCAL: modern ship died after repair.");

        Vector modernBehaviorPosition = new Vector(BalanceBehaviorX, BalanceBehaviorY);
        await DebugSetPosition(adminGalaxy, ship, modernBehaviorPosition, "Modern Behavior position").ConfigureAwait(false);
        await StabilizeShipForStationarySystems(adminGalaxy, ship, "Modern Behavior stabilize").ConfigureAwait(false);
        await DebugClearNonMetalCargo(adminGalaxy, ship, "Modern Cargo clear").ConfigureAwait(false);
        await RefillBehaviorSupplies(adminGalaxy, ship, modernBehaviorPosition, "Modern Behavior").ConfigureAwait(false);
        await DebugClearNonMetalCargo(adminGalaxy, ship, "Modern Cargo reclear").ConfigureAwait(false);
        await DebugSetBatteryCurrent(adminGalaxy, ship, ship.EnergyBattery, ship.EnergyBattery.Maximum, "Modern EnergyBattery current")
            .ConfigureAwait(false);
        await DebugSetBatteryCurrent(adminGalaxy, ship, ship.IonBattery, ship.IonBattery.Maximum, "Modern IonBattery current")
            .ConfigureAwait(false);
        await DebugSetBatteryCurrent(adminGalaxy, ship, ship.NeutrinoBattery, ship.NeutrinoBattery.Maximum,
            "Modern NeutrinoBattery current").ConfigureAwait(false);
        await PrepareRuntimeBalanceResources(adminGalaxy, ship).ConfigureAwait(false);
        await ship.ResourceMiner.Set(ship.ResourceMiner.MaximumRate).ConfigureAwait(false);

        if (!await WaitForCondition(delegate
            {
                return ship.ResourceMiner.Status == SubsystemStatus.Worked &&
                    (ship.ResourceMiner.MinedMetalThisTick > 0f || ship.ResourceMiner.MinedCarbonThisTick > 0f ||
                     ship.ResourceMiner.MinedHydrogenThisTick > 0f || ship.ResourceMiner.MinedSiliconThisTick > 0f);
            }, BalanceBehaviorTimeoutMs).ConfigureAwait(false))
            throw new InvalidOperationException(
                $"BALANCE-LOCAL: modern miner did not work. Position={ship.Position}, Movement={ship.Movement}, Size={ship.Size:0.###}, " +
                $"Status={ship.ResourceMiner.Status}, Rate={ship.ResourceMiner.Rate:0.###}, " +
                $"Mined=({ship.ResourceMiner.MinedMetalThisTick:0.###},{ship.ResourceMiner.MinedCarbonThisTick:0.###}," +
                $"{ship.ResourceMiner.MinedHydrogenThisTick:0.###},{ship.ResourceMiner.MinedSiliconThisTick:0.###}).");

        Costs modernMinerCosts = ship.ResourceMiner.TierInfo.CalculateResourceUsage(
            new SubsystemComponentValue(SubsystemComponentKind.NormalizedPower, ship.ResourceMiner.Rate / ship.ResourceMiner.MaximumRate));
        VerifyUsageCosts("Modern behavior miner", modernMinerCosts, ship.ResourceMiner.ConsumedEnergyThisTick,
            ship.ResourceMiner.ConsumedIonsThisTick, ship.ResourceMiner.ConsumedNeutrinosThisTick);

        await ship.ResourceMiner.Off().ConfigureAwait(false);
        await ship.NebulaCollector.Set(ship.NebulaCollector.MaximumRate).ConfigureAwait(false);

        if (!await WaitForCondition(delegate
            {
                return ship.NebulaCollector.Status == SubsystemStatus.Worked && ship.NebulaCollector.CollectedThisTick > 0f;
            }, BalanceBehaviorTimeoutMs).ConfigureAwait(false))
            throw new InvalidOperationException("BALANCE-LOCAL: modern nebula collector did not work.");

        Costs modernNebulaCosts = ship.NebulaCollector.TierInfo.CalculateResourceUsage(
            new SubsystemComponentValue(SubsystemComponentKind.NormalizedPower, ship.NebulaCollector.Rate / ship.NebulaCollector.MaximumRate));
        VerifyUsageCosts("Modern behavior nebula collector", modernNebulaCosts, ship.NebulaCollector.ConsumedEnergyThisTick,
            ship.NebulaCollector.ConsumedIonsThisTick, ship.NebulaCollector.ConsumedNeutrinosThisTick);

        await ship.NebulaCollector.Off().ConfigureAwait(false);
        await CleanupRuntimeBalanceResources(adminGalaxy, ship).ConfigureAwait(false);

        if (ship.ShotMagazineN.CurrentShots < 1f)
        {
            await ship.ShotFabricatorN.Set(ship.ShotFabricatorN.MaximumRate).ConfigureAwait(false);
            await ship.ShotFabricatorN.On().ConfigureAwait(false);

            if (!await WaitForCondition(delegate
                {
                    return ship.ShotFabricatorN.Status == SubsystemStatus.Worked && ship.ShotMagazineN.CurrentShots >= 1f;
                }, BalanceBehaviorTimeoutMs).ConfigureAwait(false))
                throw new InvalidOperationException(
                    $"BALANCE-LOCAL: modern shot launcher prefill failed. " +
                    $"Magazine={ship.ShotMagazineN.CurrentShots:0.###}, " +
                    $"Fabricator=(Status={ship.ShotFabricatorN.Status}, Active={ship.ShotFabricatorN.Active}, Rate={ship.ShotFabricatorN.Rate:0.###})).");

            await ship.ShotFabricatorN.Off().ConfigureAwait(false);
        }

        ResourceSnapshot modernShotResourcesBefore = CaptureResourceSnapshot(ship);
        float shotsBefore = ship.ShotMagazineN.CurrentShots;
        float modernShotSpeed = ship.ShotLauncherN.MaximumRelativeMovement;
        ushort modernShotTicks = ship.ShotLauncherN.MinimumTicks;
        float modernShotLoad = ship.ShotLauncherN.MinimumLoad;
        float modernShotDamage = ship.ShotLauncherN.MinimumDamage;
        await ship.ShotLauncherN.Shoot(modernShotSpeed, modernShotTicks, modernShotLoad, modernShotDamage).ConfigureAwait(false);

        if (!await WaitForCondition(delegate
            {
                return ship.ShotMagazineN.CurrentShots < shotsBefore;
            }, BalanceBehaviorTimeoutMs).ConfigureAwait(false))
            throw new InvalidOperationException(
                $"BALANCE-LOCAL: modern shot launcher did not fire. " +
                $"Magazine={ship.ShotMagazineN.CurrentShots:0.###}/{shotsBefore:0.###}, " +
                $"Launcher=(Status={ship.ShotLauncherN.Status}, Speed={ship.ShotLauncherN.RelativeMovement:0.###}, " +
                $"Ticks={ship.ShotLauncherN.Ticks}, Load={ship.ShotLauncherN.Load:0.###}, Damage={ship.ShotLauncherN.Damage:0.###}, " +
                $"Consumed=({ship.ShotLauncherN.ConsumedEnergyThisTick:0.###},{ship.ShotLauncherN.ConsumedIonsThisTick:0.###}," +
                $"{ship.ShotLauncherN.ConsumedNeutrinosThisTick:0.###}))).");

        Costs modernShotLaunchCosts = ship.ShotLauncherN.TierInfo.CalculateResourceUsage(
            new SubsystemComponentValue(SubsystemComponentKind.Base, 1f),
            new SubsystemComponentValue(SubsystemComponentKind.RelativeSpeed, modernShotSpeed),
            new SubsystemComponentValue(SubsystemComponentKind.Ticks, modernShotTicks),
            new SubsystemComponentValue(SubsystemComponentKind.ExplosionLoad, modernShotLoad),
            new SubsystemComponentValue(SubsystemComponentKind.Damage, modernShotDamage));
        ResourceSnapshot modernShotResourcesAfter = CaptureResourceSnapshot(ship);
        VerifyTransientUsageCosts("Modern behavior shot launcher", modernShotLaunchCosts, modernShotResourcesBefore,
            modernShotResourcesAfter, ship.ShotLauncherN.ConsumedEnergyThisTick, ship.ShotLauncherN.ConsumedIonsThisTick,
            ship.ShotLauncherN.ConsumedNeutrinosThisTick);

        float shotsAfterFire = ship.ShotMagazineN.CurrentShots;
        await ship.ShotFabricatorN.Set(ship.ShotFabricatorN.MaximumRate).ConfigureAwait(false);
        await ship.ShotFabricatorN.On().ConfigureAwait(false);

        if (!await WaitForCondition(delegate
            {
                return ship.ShotFabricatorN.Status == SubsystemStatus.Worked && ship.ShotMagazineN.CurrentShots > shotsAfterFire;
            }, BalanceBehaviorTimeoutMs).ConfigureAwait(false))
            throw new InvalidOperationException(
                $"BALANCE-LOCAL: modern shot fabricator did not refill. " +
                $"Magazine={ship.ShotMagazineN.CurrentShots:0.###}/{shotsAfterFire:0.###}, " +
                $"Fabricator=(Status={ship.ShotFabricatorN.Status}, Active={ship.ShotFabricatorN.Active}, Rate={ship.ShotFabricatorN.Rate:0.###}, " +
                $"Consumed=({ship.ShotFabricatorN.ConsumedEnergyThisTick:0.###},{ship.ShotFabricatorN.ConsumedIonsThisTick:0.###}," +
                $"{ship.ShotFabricatorN.ConsumedNeutrinosThisTick:0.###})), TimeoutMs={BalanceBehaviorTimeoutMs}.");

        Costs modernShotFabricatorCosts = ship.ShotFabricatorN.TierInfo.CalculateResourceUsage(
            new SubsystemComponentValue(SubsystemComponentKind.NormalizedPower, ship.ShotFabricatorN.Rate / ship.ShotFabricatorN.MaximumRate));
        VerifyUsageCosts("Modern behavior shot fabricator", modernShotFabricatorCosts, ship.ShotFabricatorN.ConsumedEnergyThisTick,
            ship.ShotFabricatorN.ConsumedIonsThisTick, ship.ShotFabricatorN.ConsumedNeutrinosThisTick);

        await ship.ShotFabricatorN.Off().ConfigureAwait(false);

        if (ship.InterceptorMagazineE.CurrentShots < 1f)
        {
            await ship.InterceptorFabricatorE.Set(ship.InterceptorFabricatorE.MaximumRate).ConfigureAwait(false);
            await ship.InterceptorFabricatorE.On().ConfigureAwait(false);

            if (!await WaitForCondition(delegate
                {
                    return ship.InterceptorFabricatorE.Status == SubsystemStatus.Worked && ship.InterceptorMagazineE.CurrentShots >= 1f;
                }, BalanceBehaviorTimeoutMs).ConfigureAwait(false))
                throw new InvalidOperationException(
                    $"BALANCE-LOCAL: modern interceptor launcher prefill failed. " +
                    $"Magazine={ship.InterceptorMagazineE.CurrentShots:0.###}, " +
                    $"Fabricator=(Status={ship.InterceptorFabricatorE.Status}, Active={ship.InterceptorFabricatorE.Active}, Rate={ship.InterceptorFabricatorE.Rate:0.###})).");

            await ship.InterceptorFabricatorE.Off().ConfigureAwait(false);
        }

        ResourceSnapshot modernInterceptorResourcesBefore = CaptureResourceSnapshot(ship);
        float interceptorsBefore = ship.InterceptorMagazineE.CurrentShots;
        await ship.InterceptorLauncherE.Shoot(ship.InterceptorLauncherE.MaximumRelativeMovement, 0f, ship.InterceptorLauncherE.MinimumTicks,
            ship.InterceptorLauncherE.MinimumLoad, ship.InterceptorLauncherE.MinimumDamage).ConfigureAwait(false);

        if (!await WaitForCondition(delegate
            {
                return ship.InterceptorMagazineE.CurrentShots < interceptorsBefore;
            }, BalanceBehaviorTimeoutMs).ConfigureAwait(false))
            throw new InvalidOperationException(
                $"BALANCE-LOCAL: modern interceptor launcher did not fire. " +
                $"Magazine={ship.InterceptorMagazineE.CurrentShots:0.###}/{interceptorsBefore:0.###}, " +
                $"Launcher=(Status={ship.InterceptorLauncherE.Status}, Ticks={ship.InterceptorLauncherE.Ticks}, Load={ship.InterceptorLauncherE.Load:0.###}, " +
                $"Damage={ship.InterceptorLauncherE.Damage:0.###}, Consumed=({ship.InterceptorLauncherE.ConsumedEnergyThisTick:0.###}," +
                $"{ship.InterceptorLauncherE.ConsumedIonsThisTick:0.###},{ship.InterceptorLauncherE.ConsumedNeutrinosThisTick:0.###}))).");

        Costs modernInterceptorLaunchCosts = ship.InterceptorLauncherE.TierInfo.CalculateResourceUsage(
            new SubsystemComponentValue(SubsystemComponentKind.Base, 1f));
        ResourceSnapshot modernInterceptorResourcesAfter = CaptureResourceSnapshot(ship);
        VerifyTransientUsageCosts("Modern behavior interceptor launcher", modernInterceptorLaunchCosts, modernInterceptorResourcesBefore,
            modernInterceptorResourcesAfter, ship.InterceptorLauncherE.ConsumedEnergyThisTick, ship.InterceptorLauncherE.ConsumedIonsThisTick,
            ship.InterceptorLauncherE.ConsumedNeutrinosThisTick);

        float interceptorsAfterFire = ship.InterceptorMagazineE.CurrentShots;
        await ship.InterceptorFabricatorE.Set(ship.InterceptorFabricatorE.MaximumRate).ConfigureAwait(false);
        await ship.InterceptorFabricatorE.On().ConfigureAwait(false);

        if (!await WaitForCondition(delegate
            {
                return ship.InterceptorFabricatorE.Status == SubsystemStatus.Worked &&
                    ship.InterceptorMagazineE.CurrentShots > interceptorsAfterFire;
            }, BalanceBehaviorTimeoutMs).ConfigureAwait(false))
            throw new InvalidOperationException(
                $"BALANCE-LOCAL: modern interceptor fabricator did not refill. " +
                $"Magazine={ship.InterceptorMagazineE.CurrentShots:0.###}/{interceptorsAfterFire:0.###}, " +
                $"Fabricator=(Status={ship.InterceptorFabricatorE.Status}, Active={ship.InterceptorFabricatorE.Active}, Rate={ship.InterceptorFabricatorE.Rate:0.###}, " +
                $"Consumed=({ship.InterceptorFabricatorE.ConsumedEnergyThisTick:0.###},{ship.InterceptorFabricatorE.ConsumedIonsThisTick:0.###}," +
                $"{ship.InterceptorFabricatorE.ConsumedNeutrinosThisTick:0.###})), TimeoutMs={BalanceBehaviorTimeoutMs}.");

        Costs modernInterceptorFabricatorCosts = ship.InterceptorFabricatorE.TierInfo.CalculateResourceUsage(
            new SubsystemComponentValue(SubsystemComponentKind.NormalizedPower,
                ship.InterceptorFabricatorE.Rate / ship.InterceptorFabricatorE.MaximumRate));
        VerifyUsageCosts("Modern behavior interceptor fabricator", modernInterceptorFabricatorCosts,
            ship.InterceptorFabricatorE.ConsumedEnergyThisTick, ship.InterceptorFabricatorE.ConsumedIonsThisTick,
            ship.InterceptorFabricatorE.ConsumedNeutrinosThisTick);

        await ship.InterceptorFabricatorE.Off().ConfigureAwait(false);
        float metalBefore = ship.Cargo.CurrentMetal;
        float railgunMetalCost = ship.RailgunN.MetalCost;
        await ship.RailgunN.Fire().ConfigureAwait(false);

        if (!await WaitForCondition(delegate
            {
                return ship.Cargo.CurrentMetal <= metalBefore - railgunMetalCost;
            }, BalanceBehaviorTimeoutMs).ConfigureAwait(false))
            throw new InvalidOperationException(
                $"BALANCE-LOCAL: modern railgun did not fire. MetalBefore={metalBefore:0.###}, " +
                $"MetalAfter={ship.Cargo.CurrentMetal:0.###}, MetalCost={railgunMetalCost:0.###}, " +
                $"Status={ship.RailgunN.Status}, Direction={ship.RailgunN.Direction}.");

        Costs modernRailgunCosts = ship.RailgunN.TierInfo.CalculateResourceUsage(new SubsystemComponentValue(SubsystemComponentKind.Base, 1f));

        if (MathF.Abs(modernRailgunCosts.Metal - (metalBefore - ship.Cargo.CurrentMetal)) > 0.001f)
            throw new InvalidOperationException(
                $"BALANCE-LOCAL: modern railgun metal mismatch. Expected={modernRailgunCosts.Metal:0.###}, " +
                $"Actual={metalBefore - ship.Cargo.CurrentMetal:0.###}.");

        Vector positionBefore = ship.Position;
        await ship.EngineE.SetThrust(ship.EngineE.MaximumThrust).ConfigureAwait(false);

        if (!await WaitForCondition(delegate { return Vector.Distance(positionBefore, ship.Position) > 5f; }, BalanceBehaviorTimeoutMs).ConfigureAwait(false))
            throw new InvalidOperationException("BALANCE-LOCAL: modern ship did not move.");

        Costs modernEngineCosts = ship.EngineE.TierInfo.CalculateResourceUsage(
            new SubsystemComponentValue(SubsystemComponentKind.NormalizedPower,
                MathF.Abs(ship.EngineE.CurrentThrust) / ship.EngineE.MaximumThrust));
        VerifyUsageCosts("Modern behavior engine", modernEngineCosts, ship.EngineE.ConsumedEnergyThisTick, ship.EngineE.ConsumedIonsThisTick,
            ship.EngineE.ConsumedNeutrinosThisTick);

        await ship.EngineE.Off().ConfigureAwait(false);
        await ship.EngineW.Off().ConfigureAwait(false);
        Vector modernJumpPosition = new Vector(BalanceJumpX, BalanceJumpY);
        await DebugSetPosition(adminGalaxy, ship, modernJumpPosition, "Modern Jump position").ConfigureAwait(false);

        await PrepareRuntimeBalanceWormhole(adminGalaxy, ship).ConfigureAwait(false);
        await ship.JumpDrive.Jump().ConfigureAwait(false);

        if (!await WaitForCondition(delegate { return ship.Cluster.Id == 1; }, BalanceBehaviorTimeoutMs).ConfigureAwait(false))
            throw new InvalidOperationException("BALANCE-LOCAL: modern jump drive did not work.");

        Costs modernJumpCosts = ship.JumpDrive.TierInfo.CalculateResourceUsage(new SubsystemComponentValue(SubsystemComponentKind.Base, 1f));
        VerifyUsageCosts("Modern behavior jump drive", modernJumpCosts, ship.JumpDrive.ConsumedEnergyThisTick,
            ship.JumpDrive.ConsumedIonsThisTick, ship.JumpDrive.ConsumedNeutrinosThisTick);
    }

    private static async Task PrepareRuntimeBalanceResources(Galaxy adminGalaxy, Controllable ship)
    {
        if (!adminGalaxy.Clusters.TryGet(ship.Cluster.Id, out Cluster? adminCluster) || adminCluster is null)
            throw new InvalidOperationException($"BALANCE-LOCAL: admin cluster {ship.Cluster.Id} not found for runtime balance resources.");

        string prefix = ship.Name + "_Runtime";
        float x = ship.Position.X;
        float y = ship.Position.Y;

        await adminCluster.SetUnit(
            $"<Planet Name=\"{prefix}Planet\" X=\"{FormatFloat(x)}\" Y=\"{FormatFloat(y + 52f)}\" Radius=\"13\" Gravity=\"0\" Type=\"RockyFrontier\" Metal=\"0.90\" Carbon=\"0.90\" Hydrogen=\"0.90\" Silicon=\"0.90\" />")
            .ConfigureAwait(false);
        await adminCluster.SetUnit(
            $"<Moon Name=\"{prefix}Moon\" X=\"{FormatFloat(x + 10f)}\" Y=\"{FormatFloat(y + 48f)}\" Radius=\"10\" Gravity=\"0\" Type=\"IceMoon\" Metal=\"0.75\" Carbon=\"0.75\" Hydrogen=\"1.10\" Silicon=\"0.75\" />")
            .ConfigureAwait(false);
        await adminCluster.SetUnit(
            $"<Meteoroid Name=\"{prefix}Meteoroid\" X=\"{FormatFloat(x - 44f)}\" Y=\"{FormatFloat(y + 12f)}\" Radius=\"8\" Gravity=\"0\" Type=\"MetallicSlug\" Metal=\"1.40\" Carbon=\"0.45\" Hydrogen=\"0.45\" Silicon=\"1.40\" />")
            .ConfigureAwait(false);
        await adminCluster.SetUnit(
            $"<Nebula Name=\"{prefix}Nebula\" X=\"{FormatFloat(x)}\" Y=\"{FormatFloat(y)}\" Radius=\"28\" Gravity=\"0\" Hue=\"120\" />")
            .ConfigureAwait(false);
        await Task.Delay(250).ConfigureAwait(false);
    }

    private static async Task PrepareRuntimeBalanceWormhole(Galaxy adminGalaxy, Controllable ship)
    {
        if (!adminGalaxy.Clusters.TryGet(ship.Cluster.Id, out Cluster? adminCluster) || adminCluster is null)
            throw new InvalidOperationException($"BALANCE-LOCAL: admin cluster {ship.Cluster.Id} not found for runtime balance wormhole.");

        float x = ship.Position.X;
        float y = ship.Position.Y;
        await adminCluster.SetUnit(
            $"<WormHole Name=\"{ship.Name}_RuntimeWormHole\" X=\"{FormatFloat(x)}\" Y=\"{FormatFloat(y)}\" Radius=\"40\" Gravity=\"0\" TargetCluster=\"1\" TargetLeft=\"-60\" TargetTop=\"-40\" TargetRight=\"60\" TargetBottom=\"40\" />")
            .ConfigureAwait(false);
    }

    private static async Task CleanupRuntimeBalanceResources(Galaxy adminGalaxy, Controllable ship)
    {
        if (!adminGalaxy.Clusters.TryGet(ship.Cluster.Id, out Cluster? adminCluster) || adminCluster is null)
            throw new InvalidOperationException($"BALANCE-LOCAL: admin cluster {ship.Cluster.Id} not found for runtime balance cleanup.");

        string prefix = ship.Name + "_Runtime";
        string[] names = new string[]
        {
            prefix + "Planet",
            prefix + "Moon",
            prefix + "Meteoroid",
            prefix + "Nebula"
        };

        for (int index = 0; index < names.Length; index++)
            try
            {
                await adminCluster.RemoveUnit(names[index]).ConfigureAwait(false);
            }
            catch (Exception)
            {
            }
    }

    private static string FormatFloat(float value)
    {
        return value.ToString("R", CultureInfo.InvariantCulture);
    }

    private static async Task CleanupRuntimeBalanceUnits(Galaxy adminGalaxy)
    {
        for (byte clusterId = 0; clusterId <= 1; clusterId++)
        {
            if (!adminGalaxy.Clusters.TryGet(clusterId, out Cluster? cluster) || cluster is null)
                continue;

            EditableUnitSummary[] editableUnits = await cluster.QueryEditableUnits().ConfigureAwait(false);

            for (int index = 0; index < editableUnits.Length; index++)
            {
                string name = editableUnits[index].Name;

                if (!name.StartsWith("BalanceClassic", StringComparison.Ordinal) &&
                    !name.StartsWith("BalanceModern", StringComparison.Ordinal))
                    continue;

                try
                {
                    await cluster.RemoveUnit(name).ConfigureAwait(false);
                }
                catch (Exception)
                {
                }
            }
        }
    }

    private static async Task PrepareUpgradeResources(Galaxy adminGalaxy, Controllable ship, string label)
    {
        Vector refillPosition = new Vector(BalanceRefillX, BalanceRefillY);
        Vector upgradePosition = new Vector(BalanceUpgradeX, BalanceUpgradeY);

        await DebugSetPosition(adminGalaxy, ship, refillPosition, label + " refill").ConfigureAwait(false);
        await WaitForSpawnSupplies(ship, 15000f, 40f, 8f, 8f, 8f).ConfigureAwait(false);
        await DebugSetPosition(adminGalaxy, ship, upgradePosition, label + " upgrade-position").ConfigureAwait(false);
    }

    private static async Task RefillBehaviorSupplies(Galaxy adminGalaxy, Controllable ship, Vector behaviorPosition, string label)
    {
        Vector refillPosition = new Vector(BalanceRefillX, BalanceRefillY);

        await DebugSetPosition(adminGalaxy, ship, refillPosition, label + " refill").ConfigureAwait(false);
        await WaitForSpawnSupplies(ship, 0f, 0f, 0f, 0f, 0f).ConfigureAwait(false);
        await DebugSetPosition(adminGalaxy, ship, behaviorPosition, label + " return").ConfigureAwait(false);
        await StabilizeShipForStationarySystems(adminGalaxy, ship, label + " restabilize").ConfigureAwait(false);
    }

    private static async Task<T> UpgradeAndVerify<T>(Galaxy adminGalaxy, T ship, Subsystem subsystem, string label) where T : Controllable
    {
        byte currentTier = subsystem.Tier;

        await PrepareUpgradeResources(adminGalaxy, ship, label).ConfigureAwait(false);
        ResourceSnapshot resourceSnapshotBefore = CaptureResourceSnapshot(ship);
        VerifyTierMetadata(subsystem, label + " before");

        if (currentTier + 1 >= subsystem.TierInfos.Count)
            throw new InvalidOperationException($"BALANCE-LOCAL: {label} has no upgrade target from tier {currentTier}.");

        Costs expectedUpgradeCost = subsystem.TierInfos[currentTier + 1].UpgradeCost;
        Console.WriteLine($"BALANCE-LOCAL: upgrade {label}");
        await subsystem.Upgrade().ConfigureAwait(false);

        await WaitForTierChangeStart(subsystem, (byte)(currentTier + 1), expectedUpgradeCost.Ticks, label).ConfigureAwait(false);

        if (!await WaitForCondition(delegate
            {
                Subsystem refreshedSubsystem = GetSubsystem(ship, subsystem.Slot);
                return ship.Active &&
                    ship.Alive &&
                    refreshedSubsystem.Tier == currentTier + 1 &&
                    refreshedSubsystem.TargetTier == refreshedSubsystem.Tier &&
                    refreshedSubsystem.RemainingTierChangeTicks == 0;
            }, BalanceUpgradeTimeoutMs).ConfigureAwait(false))
            throw new InvalidOperationException($"BALANCE-LOCAL: upgrade timeout for {label}.");

        Subsystem refreshedSubsystem = GetSubsystem(ship, subsystem.Slot);
        VerifyResourceConsumption(label, expectedUpgradeCost, resourceSnapshotBefore, CaptureResourceSnapshot(ship));
        VerifyTierMetadata(refreshedSubsystem, label + " after");
        return ship;
    }

    private static int CalculateRefillTimeoutMs(float currentShots, float targetShots, float maximumRate)
    {
        if (maximumRate <= 0f)
            return 10000;

        float missingShots = MathF.Max(0f, targetShots - currentShots);
        float requiredTicks = missingShots / maximumRate;
        int timeoutMs = (int)MathF.Ceiling(requiredTicks * 150f) + 5000;
        return Math.Max(timeoutMs, 10000);
    }

    private static async Task<T> DowngradeAndVerify<T>(Galaxy adminGalaxy, T ship, Subsystem subsystem, string label) where T : Controllable
    {
        byte currentTier = subsystem.Tier;

        await PrepareUpgradeResources(adminGalaxy, ship, label).ConfigureAwait(false);
        ResourceSnapshot resourceSnapshotBefore = CaptureResourceSnapshot(ship);
        VerifyTierMetadata(subsystem, label + " before");

        if (currentTier == 0)
            throw new InvalidOperationException($"BALANCE-LOCAL: {label} cannot downgrade tier 0.");

        Costs expectedDowngradeCost = subsystem.TierInfo.DowngradeCost;
        Console.WriteLine($"BALANCE-LOCAL: downgrade {label}");
        await subsystem.Downgrade().ConfigureAwait(false);

        await WaitForTierChangeStart(subsystem, (byte)(currentTier - 1), expectedDowngradeCost.Ticks, label).ConfigureAwait(false);

        if (!await WaitForCondition(delegate
            {
                Subsystem refreshedSubsystem = GetSubsystem(ship, subsystem.Slot);
                return ship.Active &&
                    ship.Alive &&
                    refreshedSubsystem.Tier == currentTier - 1 &&
                    refreshedSubsystem.TargetTier == refreshedSubsystem.Tier &&
                    refreshedSubsystem.RemainingTierChangeTicks == 0;
            }, BalanceUpgradeTimeoutMs).ConfigureAwait(false))
            throw new InvalidOperationException($"BALANCE-LOCAL: downgrade timeout for {label}.");

        Subsystem refreshedSubsystem = GetSubsystem(ship, subsystem.Slot);
        VerifyResourceConsumption(label, expectedDowngradeCost, resourceSnapshotBefore, CaptureResourceSnapshot(ship));
        VerifyTierMetadata(refreshedSubsystem, label + " after");
        return ship;
    }

    private static async Task<T> DebugSetTier<T>(Galaxy adminGalaxy, T ship, Subsystem subsystem, byte tier, string label) where T : Controllable
    {
        if (!adminGalaxy.Clusters.TryGet(ship.Cluster.Id, out Cluster? adminCluster) || adminCluster is null)
            throw new InvalidOperationException($"BALANCE-LOCAL: admin cluster {ship.Cluster.Id} not found for {label}.");

        Console.WriteLine($"BALANCE-LOCAL: debug-set {label} -> T{tier}");
        await adminCluster.DebugSetPlayerUnitSubsystemTier(ship.Name, subsystem.Slot, tier).ConfigureAwait(false);

        if (!await WaitForCondition(delegate
            {
                Subsystem refreshedSubsystem = GetSubsystem(ship, subsystem.Slot);
                return ship.Active && ship.Alive && refreshedSubsystem.Tier == tier;
            }, BalanceUpgradeTimeoutMs).ConfigureAwait(false))
            throw new InvalidOperationException($"BALANCE-LOCAL: debug tier set timeout for {label}.");

        Subsystem refreshedSubsystem = GetSubsystem(ship, subsystem.Slot);
        VerifyTierMetadata(refreshedSubsystem, label + " after");
        return ship;
    }

    private static async Task<Vector> DebugSetPosition(Galaxy adminGalaxy, Controllable ship, Vector position, string label)
    {
        if (!adminGalaxy.Clusters.TryGet(ship.Cluster.Id, out Cluster? adminCluster) || adminCluster is null)
            throw new InvalidOperationException($"BALANCE-LOCAL: admin cluster {ship.Cluster.Id} not found for {label}.");

        Console.WriteLine($"BALANCE-LOCAL: debug-set {label}");
        await adminCluster.DebugSetPlayerUnitPosition(ship.Name, position).ConfigureAwait(false);

        if (!await WaitForCondition(delegate
            {
                return Vector.Distance(ship.Position, position) < 0.5f;
            }, 4000).ConfigureAwait(false))
            throw new InvalidOperationException($"BALANCE-LOCAL: position debug-set failed for {label}.");

        return ship.Position;
    }

    private static async Task DebugClearNonMetalCargo(Galaxy adminGalaxy, Controllable ship, string label)
    {
        if (!adminGalaxy.Clusters.TryGet(ship.Cluster.Id, out Cluster? adminCluster) || adminCluster is null)
            throw new InvalidOperationException($"BALANCE-LOCAL: admin cluster {ship.Cluster.Id} not found for {label}.");

        Console.WriteLine($"BALANCE-LOCAL: debug-clear {label}");
        await adminCluster.DebugClearPlayerUnitNonMetalCargo(ship.Name).ConfigureAwait(false);

        if (!await WaitForCondition(delegate
            {
                return ship.Cargo.CurrentCarbon == 0f &&
                    ship.Cargo.CurrentHydrogen == 0f &&
                    ship.Cargo.CurrentSilicon == 0f &&
                    ship.Cargo.CurrentNebula == 0f;
            }, 4000).ConfigureAwait(false))
            throw new InvalidOperationException($"BALANCE-LOCAL: cargo clear failed for {label}.");
    }

    private static async Task DebugSetShieldCurrent(Galaxy adminGalaxy, Controllable ship, float current, string label)
    {
        if (!adminGalaxy.Clusters.TryGet(ship.Cluster.Id, out Cluster? adminCluster) || adminCluster is null)
            throw new InvalidOperationException($"BALANCE-LOCAL: admin cluster {ship.Cluster.Id} not found for {label}.");

        Console.WriteLine($"BALANCE-LOCAL: debug-set {label} -> {current:0.###}");
        await adminCluster.DebugSetPlayerUnitShieldCurrent(ship.Name, current).ConfigureAwait(false);

        ShieldSubsystem shield = GetShieldSubsystem(ship);

        if (await WaitForCondition(delegate { return MathF.Abs(shield.Current - current) < 0.01f; }, 5000).ConfigureAwait(false))
            return;

        throw new InvalidOperationException(
            $"BALANCE-LOCAL: shield current debug-set timeout for {label}. Current={shield.Current:0.###}, Target={current:0.###}.");
    }

    private static async Task<float> DebugSetHullCurrent(Galaxy adminGalaxy, Controllable ship, float current, string label)
    {
        if (!adminGalaxy.Clusters.TryGet(ship.Cluster.Id, out Cluster? adminCluster) || adminCluster is null)
            throw new InvalidOperationException($"BALANCE-LOCAL: admin cluster {ship.Cluster.Id} not found for {label}.");

        Console.WriteLine($"BALANCE-LOCAL: debug-set {label} -> {current:0.###}");
        await adminCluster.DebugSetPlayerUnitHullCurrent(ship.Name, current).ConfigureAwait(false);

        if (await WaitForCondition(delegate { return MathF.Abs(ship.Hull.Current - current) < 0.01f; }, 5000).ConfigureAwait(false))
            return ship.Hull.Current;

        throw new InvalidOperationException(
            $"BALANCE-LOCAL: hull current debug-set timeout for {label}. Current={ship.Hull.Current:0.###}, Target={current:0.###}.");
    }

    private static async Task StabilizeShipForStationarySystems(Galaxy adminGalaxy, Controllable ship, string label)
    {
        if (ship is ClassicShipControllable classicShip)
            await classicShip.Engine.Off().ConfigureAwait(false);
        else if (ship is ModernShipControllable modernShip)
            for (int index = 0; index < modernShip.Engines.Count; index++)
                await modernShip.Engines[index].Off().ConfigureAwait(false);

        Vector position = ship.Position;
        await DebugSetPosition(adminGalaxy, ship, position, label).ConfigureAwait(false);

        if (await WaitForCondition(delegate { return ship.Movement < 0.02f; }, 5000).ConfigureAwait(false))
            return;

        throw new InvalidOperationException(
            $"BALANCE-LOCAL: stabilization failed for {label}. Movement={ship.Movement}, Position={ship.Position}.");
    }

    private static async Task<float> DebugSetBatteryCurrent(Galaxy adminGalaxy, Controllable ship, BatterySubsystem battery, float current,
        string label)
    {
        if (!adminGalaxy.Clusters.TryGet(ship.Cluster.Id, out Cluster? adminCluster) || adminCluster is null)
            throw new InvalidOperationException($"BALANCE-LOCAL: admin cluster {ship.Cluster.Id} not found for {label}.");

        Console.WriteLine($"BALANCE-LOCAL: debug-set {label} -> {current:0.###}");
        await adminCluster.DebugSetPlayerUnitBatteryCurrent(ship.Name, battery.Slot, current).ConfigureAwait(false);

        if (await WaitForCondition(delegate { return MathF.Abs(battery.Current - current) < 0.01f; }, 5000).ConfigureAwait(false))
            return battery.Current;

        throw new InvalidOperationException(
            $"BALANCE-LOCAL: battery current debug-set timeout for {label}. Current={battery.Current:0.###}, Target={current:0.###}.");
    }

    private static ShieldSubsystem GetShieldSubsystem(Controllable ship)
    {
        if (ship is ClassicShipControllable classicShip)
            return classicShip.Shield;

        if (ship is ModernShipControllable modernShip)
            return modernShip.Shield;

        throw new ArgumentOutOfRangeException(nameof(ship));
    }

    private static ResourceSnapshot CaptureResourceSnapshot(Controllable ship)
    {
        return new ResourceSnapshot(ship.EnergyBattery.Current, ship.IonBattery.Current, ship.NeutrinoBattery.Current, ship.Cargo.CurrentMetal,
            ship.Cargo.CurrentCarbon, ship.Cargo.CurrentHydrogen, ship.Cargo.CurrentSilicon);
    }

    private static async Task WaitForTierChangeStart(Subsystem subsystem, byte expectedTargetTier, int maximumTicks, string label)
    {
        if (maximumTicks <= 0)
            throw new InvalidOperationException($"BALANCE-LOCAL: missing tier-change ticks for {label}.");

        if (await WaitForCondition(delegate
            {
                return subsystem.TargetTier == expectedTargetTier &&
                    subsystem.TargetTierInfo.Tier == expectedTargetTier &&
                    subsystem.RemainingTierChangeTicks > 0 &&
                    subsystem.RemainingTierChangeTicks <= maximumTicks;
            }, 5000).ConfigureAwait(false))
            return;

        throw new InvalidOperationException(
            $"BALANCE-LOCAL: pending tier-change metadata mismatch for {label}. " +
            $"Tier={subsystem.Tier}, TargetTier={subsystem.TargetTier}, Remaining={subsystem.RemainingTierChangeTicks}, MaxTicks={maximumTicks}.");
    }

    private static void VerifyResourceConsumption(string label, Costs expected, ResourceSnapshot before, ResourceSnapshot after)
    {
        try
        {
            VerifyApprox(before.Energy - after.Energy, expected.Energy, 0.05f, label, "upgrade energy");
            VerifyApprox(before.IonEnergy - after.IonEnergy, expected.Ions, 0.05f, label, "upgrade ions");
            VerifyApprox(before.NeutrinoEnergy - after.NeutrinoEnergy, expected.Neutrinos, 0.05f, label, "upgrade neutrinos");
            VerifyApprox(before.Metal - after.Metal, expected.Metal, 0.05f, label, "upgrade metal");
            VerifyApprox(before.Carbon - after.Carbon, expected.Carbon, 0.05f, label, "upgrade carbon");
            VerifyApprox(before.Hydrogen - after.Hydrogen, expected.Hydrogen, 0.05f, label, "upgrade hydrogen");
            VerifyApprox(before.Silicon - after.Silicon, expected.Silicon, 0.05f, label, "upgrade silicon");
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException(
                $"{exception.Message} ExpectedCosts=[{expected}], Before=[{Describe(before)}], After=[{Describe(after)}].", exception);
        }
    }

    private static void VerifyUsageCosts(string label, Costs expected, float energy, float ions, float neutrinos)
    {
        VerifyApprox(energy, expected.Energy, 0.05f, label, "energy");
        VerifyApprox(ions, expected.Ions, 0.05f, label, "ions");
        VerifyApprox(neutrinos, expected.Neutrinos, 0.05f, label, "neutrinos");
        VerifyApprox(0f, expected.Metal, 0.05f, label, "metal");
        VerifyApprox(0f, expected.Carbon, 0.05f, label, "carbon");
        VerifyApprox(0f, expected.Hydrogen, 0.05f, label, "hydrogen");
        VerifyApprox(0f, expected.Silicon, 0.05f, label, "silicon");
    }

    private static void VerifyUsageCosts(string label, Costs expected, float energy, float ions, float neutrinos, float metal)
    {
        VerifyApprox(energy, expected.Energy, 0.05f, label, "energy");
        VerifyApprox(ions, expected.Ions, 0.05f, label, "ions");
        VerifyApprox(neutrinos, expected.Neutrinos, 0.05f, label, "neutrinos");
        VerifyApprox(metal, expected.Metal, 0.05f, label, "metal");
        VerifyApprox(0f, expected.Carbon, 0.05f, label, "carbon");
        VerifyApprox(0f, expected.Hydrogen, 0.05f, label, "hydrogen");
        VerifyApprox(0f, expected.Silicon, 0.05f, label, "silicon");
    }

    private static void VerifyTransientUsageCosts(string label, Costs expected, ResourceSnapshot before, ResourceSnapshot after, float energy,
        float ions, float neutrinos)
    {
        if (energy > 0f || ions > 0f || neutrinos > 0f)
        {
            VerifyUsageCosts(label, expected, energy, ions, neutrinos);
            return;
        }

        VerifyApprox(before.Energy - after.Energy, expected.Energy, 10f, label, "energy");
        VerifyApprox(before.IonEnergy - after.IonEnergy, expected.Ions, 1f, label, "ions");
        VerifyApprox(before.NeutrinoEnergy - after.NeutrinoEnergy, expected.Neutrinos, 1f, label, "neutrinos");
        VerifyApprox(0f, expected.Metal, 0.05f, label, "metal");
        VerifyApprox(0f, expected.Carbon, 0.05f, label, "carbon");
        VerifyApprox(0f, expected.Hydrogen, 0.05f, label, "hydrogen");
        VerifyApprox(0f, expected.Silicon, 0.05f, label, "silicon");
    }

    private static void VerifyApprox(float actual, float expected, float tolerance, string label, string component)
    {
        if (MathF.Abs(actual - expected) <= tolerance)
            return;

        throw new InvalidOperationException(
            $"BALANCE-LOCAL: {label} {component} mismatch. Expected={expected:0.###}, Actual={actual:0.###}, Tolerance={tolerance:0.###}.");
    }

    private static string Describe(ResourceSnapshot snapshot)
    {
        return $"Energy={snapshot.Energy:0.###}, Ions={snapshot.IonEnergy:0.###}, Neutrinos={snapshot.NeutrinoEnergy:0.###}, " +
            $"Metal={snapshot.Metal:0.###}, Carbon={snapshot.Carbon:0.###}, Hydrogen={snapshot.Hydrogen:0.###}, Silicon={snapshot.Silicon:0.###}";
    }

    private static SubsystemPropertyInfo GetProperty(Subsystem subsystem, string key, string label)
    {
        if (subsystem.TierInfo.TryGetProperty(key, out SubsystemPropertyInfo? property) && property is not null)
            return property;

        List<string> availableKeysList = new List<string>();

        for (int index = 0; index < subsystem.TierInfo.Properties.Count; index++)
            availableKeysList.Add(subsystem.TierInfo.Properties[index].Key);

        string availableKeys = string.Join(", ", availableKeysList);

        throw new InvalidOperationException(
            $"BALANCE-LOCAL: {label} missing tier property '{key}'. " +
            $"Tier={subsystem.Tier}, Kind={subsystem.Kind}, Exists={subsystem.Exists}, Available=[{availableKeys}].");
    }

    private static void VerifyPropertyValue(Subsystem subsystem, string key, float actual, string label)
    {
        SubsystemPropertyInfo property = GetProperty(subsystem, key, label);
        VerifyApprox(property.MinimumValue, actual, 0.001f, label, $"{key}.min");
        VerifyApprox(property.MaximumValue, actual, 0.001f, label, $"{key}.max");
    }

    private static void VerifyTierMetadata(Subsystem subsystem, string label)
    {
        IReadOnlyList<SubsystemTierInfo> tierInfos = subsystem.TierInfos;

        if (tierInfos.Count == 0)
            throw new InvalidOperationException($"BALANCE-LOCAL: {label} has no tier infos.");

        if (tierInfos[0].Tier != 0)
            throw new InvalidOperationException($"BALANCE-LOCAL: {label} tier catalog does not start at tier 0.");

        for (int index = 0; index < tierInfos.Count; index++)
        {
            if (tierInfos[index].Tier != index)
                throw new InvalidOperationException($"BALANCE-LOCAL: {label} tier catalog mismatch at index {index}.");

            if (tierInfos[index].SubsystemKind != subsystem.Kind)
                throw new InvalidOperationException(
                    $"BALANCE-LOCAL: {label} subsystem kind mismatch at tier {index}. " +
                    $"Expected={subsystem.Kind}, Actual={tierInfos[index].SubsystemKind}.");

            if (tierInfos[index].Description.Length == 0)
                throw new InvalidOperationException($"BALANCE-LOCAL: {label} tier {index} has no description.");
        }

        if (subsystem.TierInfo.Tier != subsystem.Tier)
            throw new InvalidOperationException($"BALANCE-LOCAL: {label} tier info mismatch.");

        if (subsystem.TargetTierInfo.Tier != subsystem.TargetTier)
            throw new InvalidOperationException($"BALANCE-LOCAL: {label} target tier info mismatch.");

        if (!subsystem.Exists)
            return;

        switch (subsystem)
        {
            case BatterySubsystem battery:
                VerifyPropertyValue(subsystem, "maximum", battery.Maximum, label);
                break;
            case EnergyCellSubsystem energyCell:
                VerifyPropertyValue(subsystem, "efficiency", energyCell.Efficiency, label);
                break;
            case HullSubsystem hull:
                VerifyPropertyValue(subsystem, "maximum", hull.Maximum, label);
                break;
            case ShieldSubsystem shield:
                VerifyPropertyValue(subsystem, "maximum", shield.Maximum, label);
                VerifyPropertyValue(subsystem, "minimumRate", shield.MinimumRate, label);
                VerifyPropertyValue(subsystem, "maximumRate", shield.MaximumRate, label);
                break;
            case ArmorSubsystem armor:
                VerifyPropertyValue(subsystem, "reduction", armor.Reduction, label);
                break;
            case RepairSubsystem repair:
                VerifyPropertyValue(subsystem, "minimumRate", repair.MinimumRate, label);
                VerifyPropertyValue(subsystem, "maximumRate", repair.MaximumRate, label);
                break;
            case CargoSubsystem cargo:
                VerifyPropertyValue(subsystem, "maximumMetal", cargo.MaximumMetal, label);
                VerifyPropertyValue(subsystem, "maximumCarbon", cargo.MaximumCarbon, label);
                VerifyPropertyValue(subsystem, "maximumHydrogen", cargo.MaximumHydrogen, label);
                VerifyPropertyValue(subsystem, "maximumSilicon", cargo.MaximumSilicon, label);
                VerifyPropertyValue(subsystem, "maximumNebula", cargo.MaximumNebula, label);
                break;
            case ResourceMinerSubsystem resourceMiner:
                VerifyPropertyValue(subsystem, "minimumRate", resourceMiner.MinimumRate, label);
                VerifyPropertyValue(subsystem, "maximumRate", resourceMiner.MaximumRate, label);
                break;
            case NebulaCollectorSubsystem nebulaCollector:
                VerifyPropertyValue(subsystem, "minimumRate", nebulaCollector.MinimumRate, label);
                VerifyPropertyValue(subsystem, "maximumRate", nebulaCollector.MaximumRate, label);
                break;
            case StructureOptimizerSubsystem structureOptimizer:
                VerifyPropertyValue(subsystem, "reductionPercent", structureOptimizer.ReductionPercent, label);
                break;
            case ClassicShipEngineSubsystem classicEngine:
                VerifyPropertyValue(subsystem, "maximumThrust", classicEngine.Maximum, label);
                break;
            case ModernShipEngineSubsystem modernEngine:
                VerifyPropertyValue(subsystem, "maximumThrust", modernEngine.MaximumThrust, label);
                VerifyPropertyValue(subsystem, "maximumThrustChangePerTick", modernEngine.MaximumThrustChangePerTick, label);
                break;
            case DynamicScannerSubsystem scanner:
                VerifyPropertyValue(subsystem, "maximumWidth", scanner.MaximumWidth, label);
                VerifyPropertyValue(subsystem, "maximumRange", scanner.MaximumLength, label);
                VerifyPropertyValue(subsystem, "widthSpeed", scanner.WidthSpeed, label);
                VerifyPropertyValue(subsystem, "rangeSpeed", scanner.LengthSpeed, label);
                VerifyPropertyValue(subsystem, "angleSpeed", scanner.AngleSpeed, label);
                break;
            case DynamicShotLauncherSubsystem launcher when subsystem.Kind is SubsystemKind.DynamicShotLauncher or
                SubsystemKind.StaticShotLauncher:
                VerifyPropertyValue(subsystem, "minimumRelativeSpeed", launcher.MinimumRelativeMovement, label);
                VerifyPropertyValue(subsystem, "maximumRelativeSpeed", launcher.MaximumRelativeMovement, label);
                VerifyPropertyValue(subsystem, "minimumTicks", launcher.MinimumTicks, label);
                VerifyPropertyValue(subsystem, "maximumTicks", launcher.MaximumTicks, label);
                VerifyPropertyValue(subsystem, "minimumExplosionLoad", launcher.MinimumLoad, label);
                VerifyPropertyValue(subsystem, "maximumExplosionLoad", launcher.MaximumLoad, label);
                VerifyPropertyValue(subsystem, "minimumDamage", launcher.MinimumDamage, label);
                VerifyPropertyValue(subsystem, "maximumDamage", launcher.MaximumDamage, label);
                break;
            case DynamicShotMagazineSubsystem magazine when subsystem.Kind is SubsystemKind.DynamicShotMagazine or
                SubsystemKind.StaticShotMagazine or SubsystemKind.DynamicInterceptorMagazine or SubsystemKind.StaticInterceptorMagazine:
                VerifyPropertyValue(subsystem, "maximumShots", magazine.MaximumShots, label);
                break;
            case DynamicShotFabricatorSubsystem fabricator when subsystem.Kind is SubsystemKind.DynamicShotFabricator or
                SubsystemKind.StaticShotFabricator or SubsystemKind.DynamicInterceptorFabricator or
                SubsystemKind.StaticInterceptorFabricator:
                VerifyPropertyValue(subsystem, "minimumRate", fabricator.MinimumRate, label);
                VerifyPropertyValue(subsystem, "maximumRate", fabricator.MaximumRate, label);
                break;
            case DynamicInterceptorLauncherSubsystem interceptorLauncher:
                VerifyPropertyValue(subsystem, "maximumRelativeSpeed", interceptorLauncher.MaximumRelativeMovement, label);
                VerifyPropertyValue(subsystem, "ticks", interceptorLauncher.MaximumTicks, label);
                VerifyPropertyValue(subsystem, "fixedExplosionLoad", interceptorLauncher.MaximumLoad, label);
                VerifyPropertyValue(subsystem, "fixedDamage", interceptorLauncher.MaximumDamage, label);
                break;
            case ClassicRailgunSubsystem railgun:
                VerifyPropertyValue(subsystem, "projectileSpeed", railgun.ProjectileSpeed, label);
                VerifyPropertyValue(subsystem, "projectileLifetime", railgun.ProjectileLifetime, label);
                VerifyPropertyValue(subsystem, "energyCost", railgun.EnergyCost, label);
                VerifyPropertyValue(subsystem, "metalCost", railgun.MetalCost, label);
                break;
            case JumpDriveSubsystem jumpDrive:
                VerifyPropertyValue(subsystem, "energyCost", jumpDrive.EnergyCost, label);
                break;
        }
    }

    private static Subsystem GetSubsystem(Controllable ship, SubsystemSlot slot)
    {
        switch (slot)
        {
            case SubsystemSlot.EnergyBattery:
                return ship.EnergyBattery;
            case SubsystemSlot.IonBattery:
                return ship.IonBattery;
            case SubsystemSlot.NeutrinoBattery:
                return ship.NeutrinoBattery;
            case SubsystemSlot.EnergyCell:
                return ship.EnergyCell;
            case SubsystemSlot.IonCell:
                return ship.IonCell;
            case SubsystemSlot.NeutrinoCell:
                return ship.NeutrinoCell;
            case SubsystemSlot.Hull:
                return ship.Hull;
            case SubsystemSlot.Shield:
                return ship.Shield;
            case SubsystemSlot.Armor:
                return ship.Armor;
            case SubsystemSlot.Repair:
                return ship.Repair;
            case SubsystemSlot.Cargo:
                return ship.Cargo;
            case SubsystemSlot.ResourceMiner:
                return ship.ResourceMiner;
            case SubsystemSlot.StructureOptimizer:
                return ship.StructureOptimizer;
            case SubsystemSlot.NebulaCollector:
                if (ship is ClassicShipControllable classicNebulaShip)
                    return classicNebulaShip.NebulaCollector;

                if (ship is ModernShipControllable modernNebulaShip)
                    return modernNebulaShip.NebulaCollector;
                break;
            case SubsystemSlot.PrimaryEngine:
            case SubsystemSlot.SecondaryEngine:
            case SubsystemSlot.TertiaryEngine:
                if (ship is ClassicShipControllable classicEngineShip)
                    return classicEngineShip.Engine;
                break;
            case SubsystemSlot.PrimaryScanner:
                if (ship is ClassicShipControllable classicPrimaryScannerShip)
                    return classicPrimaryScannerShip.MainScanner;
                break;
            case SubsystemSlot.SecondaryScanner:
                if (ship is ClassicShipControllable classicSecondaryScannerShip)
                    return classicSecondaryScannerShip.SecondaryScanner;
                break;
            case SubsystemSlot.DynamicShotLauncher:
                if (ship is ClassicShipControllable classicShotShip)
                    return classicShotShip.ShotLauncher;
                break;
            case SubsystemSlot.DynamicShotMagazine:
                if (ship is ClassicShipControllable classicShotMagazineShip)
                    return classicShotMagazineShip.ShotMagazine;
                break;
            case SubsystemSlot.DynamicShotFabricator:
                if (ship is ClassicShipControllable classicShotFabricatorShip)
                    return classicShotFabricatorShip.ShotFabricator;
                break;
            case SubsystemSlot.DynamicInterceptorLauncher:
                if (ship is ClassicShipControllable classicInterceptorShip)
                    return classicInterceptorShip.InterceptorLauncher;
                break;
            case SubsystemSlot.DynamicInterceptorMagazine:
                if (ship is ClassicShipControllable classicInterceptorMagazineShip)
                    return classicInterceptorMagazineShip.InterceptorMagazine;
                break;
            case SubsystemSlot.DynamicInterceptorFabricator:
                if (ship is ClassicShipControllable classicInterceptorFabricatorShip)
                    return classicInterceptorFabricatorShip.InterceptorFabricator;
                break;
            case SubsystemSlot.Railgun:
                if (ship is ClassicShipControllable classicRailgunShip)
                    return classicRailgunShip.Railgun;
                break;
            case SubsystemSlot.JumpDrive:
                if (ship is ClassicShipControllable classicJumpShip)
                    return classicJumpShip.JumpDrive;

                if (ship is ModernShipControllable modernJumpShip)
                    return modernJumpShip.JumpDrive;
                break;
            case SubsystemSlot.ModernEngineN:
                if (ship is ModernShipControllable modernEngineNShip)
                    return modernEngineNShip.EngineN;
                break;
            case SubsystemSlot.ModernEngineNE:
                if (ship is ModernShipControllable modernEngineNEShip)
                    return modernEngineNEShip.EngineNE;
                break;
            case SubsystemSlot.ModernEngineE:
                if (ship is ModernShipControllable modernEngineEShip)
                    return modernEngineEShip.EngineE;
                break;
            case SubsystemSlot.ModernEngineSE:
                if (ship is ModernShipControllable modernEngineSEShip)
                    return modernEngineSEShip.EngineSE;
                break;
            case SubsystemSlot.ModernEngineS:
                if (ship is ModernShipControllable modernEngineSShip)
                    return modernEngineSShip.EngineS;
                break;
            case SubsystemSlot.ModernEngineSW:
                if (ship is ModernShipControllable modernEngineSWShip)
                    return modernEngineSWShip.EngineSW;
                break;
            case SubsystemSlot.ModernEngineW:
                if (ship is ModernShipControllable modernEngineWShip)
                    return modernEngineWShip.EngineW;
                break;
            case SubsystemSlot.ModernEngineNW:
                if (ship is ModernShipControllable modernEngineNWShip)
                    return modernEngineNWShip.EngineNW;
                break;
            case SubsystemSlot.ModernScannerN:
                if (ship is ModernShipControllable modernScannerNShip)
                    return modernScannerNShip.ScannerN;
                break;
            case SubsystemSlot.ModernScannerNE:
                if (ship is ModernShipControllable modernScannerNEShip)
                    return modernScannerNEShip.ScannerNE;
                break;
            case SubsystemSlot.ModernScannerE:
                if (ship is ModernShipControllable modernScannerEShip)
                    return modernScannerEShip.ScannerE;
                break;
            case SubsystemSlot.ModernScannerSE:
                if (ship is ModernShipControllable modernScannerSEShip)
                    return modernScannerSEShip.ScannerSE;
                break;
            case SubsystemSlot.ModernScannerS:
                if (ship is ModernShipControllable modernScannerSShip)
                    return modernScannerSShip.ScannerS;
                break;
            case SubsystemSlot.ModernScannerSW:
                if (ship is ModernShipControllable modernScannerSWShip)
                    return modernScannerSWShip.ScannerSW;
                break;
            case SubsystemSlot.ModernScannerW:
                if (ship is ModernShipControllable modernScannerWShip)
                    return modernScannerWShip.ScannerW;
                break;
            case SubsystemSlot.ModernScannerNW:
                if (ship is ModernShipControllable modernScannerNWShip)
                    return modernScannerNWShip.ScannerNW;
                break;
            case SubsystemSlot.StaticShotLauncherN:
                if (ship is ModernShipControllable modernShotLauncherNShip)
                    return modernShotLauncherNShip.ShotLauncherN;
                break;
            case SubsystemSlot.StaticShotLauncherNE:
                if (ship is ModernShipControllable modernShotLauncherNEShip)
                    return modernShotLauncherNEShip.ShotLauncherNE;
                break;
            case SubsystemSlot.StaticShotLauncherE:
                if (ship is ModernShipControllable modernShotLauncherEShip)
                    return modernShotLauncherEShip.ShotLauncherE;
                break;
            case SubsystemSlot.StaticShotLauncherSE:
                if (ship is ModernShipControllable modernShotLauncherSEShip)
                    return modernShotLauncherSEShip.ShotLauncherSE;
                break;
            case SubsystemSlot.StaticShotLauncherS:
                if (ship is ModernShipControllable modernShotLauncherSShip)
                    return modernShotLauncherSShip.ShotLauncherS;
                break;
            case SubsystemSlot.StaticShotLauncherSW:
                if (ship is ModernShipControllable modernShotLauncherSWShip)
                    return modernShotLauncherSWShip.ShotLauncherSW;
                break;
            case SubsystemSlot.StaticShotLauncherW:
                if (ship is ModernShipControllable modernShotLauncherWShip)
                    return modernShotLauncherWShip.ShotLauncherW;
                break;
            case SubsystemSlot.StaticShotLauncherNW:
                if (ship is ModernShipControllable modernShotLauncherNWShip)
                    return modernShotLauncherNWShip.ShotLauncherNW;
                break;
            case SubsystemSlot.StaticShotMagazineN:
                if (ship is ModernShipControllable modernShotMagazineNShip)
                    return modernShotMagazineNShip.ShotMagazineN;
                break;
            case SubsystemSlot.StaticShotMagazineNE:
                if (ship is ModernShipControllable modernShotMagazineNEShip)
                    return modernShotMagazineNEShip.ShotMagazineNE;
                break;
            case SubsystemSlot.StaticShotMagazineE:
                if (ship is ModernShipControllable modernShotMagazineEShip)
                    return modernShotMagazineEShip.ShotMagazineE;
                break;
            case SubsystemSlot.StaticShotMagazineSE:
                if (ship is ModernShipControllable modernShotMagazineSEShip)
                    return modernShotMagazineSEShip.ShotMagazineSE;
                break;
            case SubsystemSlot.StaticShotMagazineS:
                if (ship is ModernShipControllable modernShotMagazineSShip)
                    return modernShotMagazineSShip.ShotMagazineS;
                break;
            case SubsystemSlot.StaticShotMagazineSW:
                if (ship is ModernShipControllable modernShotMagazineSWShip)
                    return modernShotMagazineSWShip.ShotMagazineSW;
                break;
            case SubsystemSlot.StaticShotMagazineW:
                if (ship is ModernShipControllable modernShotMagazineWShip)
                    return modernShotMagazineWShip.ShotMagazineW;
                break;
            case SubsystemSlot.StaticShotMagazineNW:
                if (ship is ModernShipControllable modernShotMagazineNWShip)
                    return modernShotMagazineNWShip.ShotMagazineNW;
                break;
            case SubsystemSlot.StaticShotFabricatorN:
                if (ship is ModernShipControllable modernShotFabricatorNShip)
                    return modernShotFabricatorNShip.ShotFabricatorN;
                break;
            case SubsystemSlot.StaticShotFabricatorNE:
                if (ship is ModernShipControllable modernShotFabricatorNEShip)
                    return modernShotFabricatorNEShip.ShotFabricatorNE;
                break;
            case SubsystemSlot.StaticShotFabricatorE:
                if (ship is ModernShipControllable modernShotFabricatorEShip)
                    return modernShotFabricatorEShip.ShotFabricatorE;
                break;
            case SubsystemSlot.StaticShotFabricatorSE:
                if (ship is ModernShipControllable modernShotFabricatorSEShip)
                    return modernShotFabricatorSEShip.ShotFabricatorSE;
                break;
            case SubsystemSlot.StaticShotFabricatorS:
                if (ship is ModernShipControllable modernShotFabricatorSShip)
                    return modernShotFabricatorSShip.ShotFabricatorS;
                break;
            case SubsystemSlot.StaticShotFabricatorSW:
                if (ship is ModernShipControllable modernShotFabricatorSWShip)
                    return modernShotFabricatorSWShip.ShotFabricatorSW;
                break;
            case SubsystemSlot.StaticShotFabricatorW:
                if (ship is ModernShipControllable modernShotFabricatorWShip)
                    return modernShotFabricatorWShip.ShotFabricatorW;
                break;
            case SubsystemSlot.StaticShotFabricatorNW:
                if (ship is ModernShipControllable modernShotFabricatorNWShip)
                    return modernShotFabricatorNWShip.ShotFabricatorNW;
                break;
            case SubsystemSlot.StaticInterceptorLauncherE:
                if (ship is ModernShipControllable modernInterceptorLauncherEShip)
                    return modernInterceptorLauncherEShip.InterceptorLauncherE;
                break;
            case SubsystemSlot.StaticInterceptorLauncherW:
                if (ship is ModernShipControllable modernInterceptorLauncherWShip)
                    return modernInterceptorLauncherWShip.InterceptorLauncherW;
                break;
            case SubsystemSlot.StaticInterceptorMagazineE:
                if (ship is ModernShipControllable modernInterceptorMagazineEShip)
                    return modernInterceptorMagazineEShip.InterceptorMagazineE;
                break;
            case SubsystemSlot.StaticInterceptorMagazineW:
                if (ship is ModernShipControllable modernInterceptorMagazineWShip)
                    return modernInterceptorMagazineWShip.InterceptorMagazineW;
                break;
            case SubsystemSlot.StaticInterceptorFabricatorE:
                if (ship is ModernShipControllable modernInterceptorFabricatorEShip)
                    return modernInterceptorFabricatorEShip.InterceptorFabricatorE;
                break;
            case SubsystemSlot.StaticInterceptorFabricatorW:
                if (ship is ModernShipControllable modernInterceptorFabricatorWShip)
                    return modernInterceptorFabricatorWShip.InterceptorFabricatorW;
                break;
            case SubsystemSlot.ModernRailgunN:
                if (ship is ModernShipControllable modernRailgunNShip)
                    return modernRailgunNShip.RailgunN;
                break;
            case SubsystemSlot.ModernRailgunNE:
                if (ship is ModernShipControllable modernRailgunNEShip)
                    return modernRailgunNEShip.RailgunNE;
                break;
            case SubsystemSlot.ModernRailgunE:
                if (ship is ModernShipControllable modernRailgunEShip)
                    return modernRailgunEShip.RailgunE;
                break;
            case SubsystemSlot.ModernRailgunSE:
                if (ship is ModernShipControllable modernRailgunSEShip)
                    return modernRailgunSEShip.RailgunSE;
                break;
            case SubsystemSlot.ModernRailgunS:
                if (ship is ModernShipControllable modernRailgunSShip)
                    return modernRailgunSShip.RailgunS;
                break;
            case SubsystemSlot.ModernRailgunSW:
                if (ship is ModernShipControllable modernRailgunSWShip)
                    return modernRailgunSWShip.RailgunSW;
                break;
            case SubsystemSlot.ModernRailgunW:
                if (ship is ModernShipControllable modernRailgunWShip)
                    return modernRailgunWShip.RailgunW;
                break;
            case SubsystemSlot.ModernRailgunNW:
                if (ship is ModernShipControllable modernRailgunNWShip)
                    return modernRailgunNWShip.RailgunNW;
                break;
        }

        throw new ArgumentOutOfRangeException(nameof(slot), $"Unsupported subsystem slot {slot} for {ship.GetType().Name}.");
    }

    private static Subsystem GetModernEngineSubsystem(ModernShipControllable ship, int index)
    {
        return index switch
        {
            0 => ship.EngineN,
            1 => ship.EngineNE,
            2 => ship.EngineE,
            3 => ship.EngineSE,
            4 => ship.EngineS,
            5 => ship.EngineSW,
            6 => ship.EngineW,
            7 => ship.EngineNW,
            _ => throw new ArgumentOutOfRangeException(nameof(index))
        };
    }

    private static Subsystem GetModernScannerSubsystem(ModernShipControllable ship, int index)
    {
        return index switch
        {
            0 => ship.ScannerN,
            1 => ship.ScannerNE,
            2 => ship.ScannerE,
            3 => ship.ScannerSE,
            4 => ship.ScannerS,
            5 => ship.ScannerSW,
            6 => ship.ScannerW,
            7 => ship.ScannerNW,
            _ => throw new ArgumentOutOfRangeException(nameof(index))
        };
    }

    private static Subsystem GetModernShotLauncherSubsystem(ModernShipControllable ship, int index)
    {
        return index switch
        {
            0 => ship.ShotLauncherN,
            1 => ship.ShotLauncherNE,
            2 => ship.ShotLauncherE,
            3 => ship.ShotLauncherSE,
            4 => ship.ShotLauncherS,
            5 => ship.ShotLauncherSW,
            6 => ship.ShotLauncherW,
            7 => ship.ShotLauncherNW,
            _ => throw new ArgumentOutOfRangeException(nameof(index))
        };
    }

    private static Subsystem GetModernShotMagazineSubsystem(ModernShipControllable ship, int index)
    {
        return index switch
        {
            0 => ship.ShotMagazineN,
            1 => ship.ShotMagazineNE,
            2 => ship.ShotMagazineE,
            3 => ship.ShotMagazineSE,
            4 => ship.ShotMagazineS,
            5 => ship.ShotMagazineSW,
            6 => ship.ShotMagazineW,
            7 => ship.ShotMagazineNW,
            _ => throw new ArgumentOutOfRangeException(nameof(index))
        };
    }

    private static Subsystem GetModernShotFabricatorSubsystem(ModernShipControllable ship, int index)
    {
        return index switch
        {
            0 => ship.ShotFabricatorN,
            1 => ship.ShotFabricatorNE,
            2 => ship.ShotFabricatorE,
            3 => ship.ShotFabricatorSE,
            4 => ship.ShotFabricatorS,
            5 => ship.ShotFabricatorSW,
            6 => ship.ShotFabricatorW,
            7 => ship.ShotFabricatorNW,
            _ => throw new ArgumentOutOfRangeException(nameof(index))
        };
    }

    private static Subsystem GetModernRailgunSubsystem(ModernShipControllable ship, int index)
    {
        return index switch
        {
            0 => ship.RailgunN,
            1 => ship.RailgunNE,
            2 => ship.RailgunE,
            3 => ship.RailgunSE,
            4 => ship.RailgunS,
            5 => ship.RailgunSW,
            6 => ship.RailgunW,
            7 => ship.RailgunNW,
            _ => throw new ArgumentOutOfRangeException(nameof(index))
        };
    }

    private static async Task WaitForSpawnSupplies(Controllable ship, float minimumEnergy, float minimumMetal, float minimumCarbon,
        float minimumHydrogen, float minimumSilicon)
    {
        if (await WaitForCondition(delegate
            {
                return ship.EnergyBattery.Current >= ship.EnergyBattery.Maximum - 0.5f &&
                    ship.Cargo.CurrentMetal >= ship.Cargo.MaximumMetal - 0.5f &&
                    ship.Cargo.CurrentCarbon >= ship.Cargo.MaximumCarbon - 0.5f &&
                    ship.Cargo.CurrentHydrogen >= ship.Cargo.MaximumHydrogen - 0.5f &&
                    ship.Cargo.CurrentSilicon >= ship.Cargo.MaximumSilicon - 0.5f;
            }, BalanceUpgradeTimeoutMs).ConfigureAwait(false))
            return;

        throw new InvalidOperationException(
            $"BALANCE-LOCAL: spawn supplies missing. Energy={ship.EnergyBattery.Current:0.###}/{ship.EnergyBattery.Maximum:0.###}, " +
            $"Metal={ship.Cargo.CurrentMetal:0.###}/{ship.Cargo.MaximumMetal:0.###}, " +
            $"Carbon={ship.Cargo.CurrentCarbon:0.###}/{ship.Cargo.MaximumCarbon:0.###}, " +
            $"Hydrogen={ship.Cargo.CurrentHydrogen:0.###}/{ship.Cargo.MaximumHydrogen:0.###}, " +
            $"Silicon={ship.Cargo.CurrentSilicon:0.###}/{ship.Cargo.MaximumSilicon:0.###}.");
    }

    private static async Task CloseControllable(Controllable ship)
    {
        if (!ship.Active)
            return;

        ship.RequestClose();

        if (await WaitForCondition(delegate { return !ship.Active; }, 15000).ConfigureAwait(false))
            return;

        throw new InvalidOperationException($"BALANCE-LOCAL: controllable {ship.Name} did not close.");
    }

    private static async Task<bool> WaitForCondition(Func<bool> condition, int timeoutMs)
    {
        DateTime deadline = DateTime.UtcNow.AddMilliseconds(timeoutMs);

        while (DateTime.UtcNow < deadline)
        {
            if (condition())
                return true;

            await Task.Delay(BalanceConditionPollMs).ConfigureAwait(false);
        }

        return condition();
    }

    private static string GetSubsystemSignature(Subsystem subsystem)
    {
        if (subsystem is ModernShipEngineSubsystem modernEngine)
            return FormattableString.Invariant(
                $"ME:{modernEngine.Exists}:{modernEngine.MaximumThrust:0.###}:{modernEngine.MaximumThrustChangePerTick:0.###}");

        if (subsystem is ClassicShipEngineSubsystem classicEngine)
            return FormattableString.Invariant($"CE:{classicEngine.Exists}:{classicEngine.Maximum:0.###}");

        if (subsystem is StaticScannerSubsystem staticScanner)
            return FormattableString.Invariant(
                $"SS:{staticScanner.Exists}:{staticScanner.MaximumWidth:0.###}:{staticScanner.MaximumLength:0.###}:{staticScanner.MaximumAngleOffset:0.###}");

        if (subsystem is DynamicScannerSubsystem dynamicScanner)
            return FormattableString.Invariant(
                $"DS:{dynamicScanner.Exists}:{dynamicScanner.MaximumWidth:0.###}:{dynamicScanner.MaximumLength:0.###}");

        if (subsystem is StaticShotLauncherSubsystem staticShotLauncher)
            return FormattableString.Invariant(
                $"SSL:{staticShotLauncher.Exists}:{staticShotLauncher.MaximumRelativeMovement:0.###}:{staticShotLauncher.MaximumTicks}:{staticShotLauncher.MaximumLoad:0.###}:{staticShotLauncher.MaximumDamage:0.###}");

        if (subsystem is DynamicShotLauncherSubsystem dynamicShotLauncher)
            return FormattableString.Invariant(
                $"DSL:{dynamicShotLauncher.Exists}:{dynamicShotLauncher.MaximumRelativeMovement:0.###}:{dynamicShotLauncher.MaximumTicks}:{dynamicShotLauncher.MaximumLoad:0.###}:{dynamicShotLauncher.MaximumDamage:0.###}");

        if (subsystem is StaticShotMagazineSubsystem staticShotMagazine)
            return FormattableString.Invariant($"SSM:{staticShotMagazine.Exists}:{staticShotMagazine.MaximumShots:0.###}");

        if (subsystem is DynamicShotMagazineSubsystem dynamicShotMagazine)
            return FormattableString.Invariant($"DSM:{dynamicShotMagazine.Exists}:{dynamicShotMagazine.MaximumShots:0.###}");

        if (subsystem is StaticShotFabricatorSubsystem staticShotFabricator)
            return FormattableString.Invariant($"SSF:{staticShotFabricator.Exists}:{staticShotFabricator.MaximumRate:0.###}");

        if (subsystem is DynamicShotFabricatorSubsystem dynamicShotFabricator)
            return FormattableString.Invariant($"DSF:{dynamicShotFabricator.Exists}:{dynamicShotFabricator.MaximumRate:0.###}");

        if (subsystem is StaticInterceptorLauncherSubsystem staticInterceptorLauncher)
            return FormattableString.Invariant(
                $"SIL:{staticInterceptorLauncher.Exists}:{staticInterceptorLauncher.MaximumRelativeMovement:0.###}:{staticInterceptorLauncher.MaximumTicks}:{staticInterceptorLauncher.MaximumLoad:0.###}:{staticInterceptorLauncher.MaximumDamage:0.###}");

        if (subsystem is DynamicInterceptorLauncherSubsystem dynamicInterceptorLauncher)
            return FormattableString.Invariant(
                $"DIL:{dynamicInterceptorLauncher.Exists}:{dynamicInterceptorLauncher.MaximumRelativeMovement:0.###}:{dynamicInterceptorLauncher.MaximumTicks}:{dynamicInterceptorLauncher.MaximumLoad:0.###}:{dynamicInterceptorLauncher.MaximumDamage:0.###}");

        if (subsystem is StaticInterceptorMagazineSubsystem staticInterceptorMagazine)
            return FormattableString.Invariant($"SIM:{staticInterceptorMagazine.Exists}:{staticInterceptorMagazine.MaximumShots:0.###}");

        if (subsystem is DynamicInterceptorMagazineSubsystem dynamicInterceptorMagazine)
            return FormattableString.Invariant($"DIM:{dynamicInterceptorMagazine.Exists}:{dynamicInterceptorMagazine.MaximumShots:0.###}");

        if (subsystem is StaticInterceptorFabricatorSubsystem staticInterceptorFabricator)
            return FormattableString.Invariant($"SIF:{staticInterceptorFabricator.Exists}:{staticInterceptorFabricator.MaximumRate:0.###}");

        if (subsystem is DynamicInterceptorFabricatorSubsystem dynamicInterceptorFabricator)
            return FormattableString.Invariant($"DIF:{dynamicInterceptorFabricator.Exists}:{dynamicInterceptorFabricator.MaximumRate:0.###}");

        if (subsystem is ClassicRailgunSubsystem railgun)
            return FormattableString.Invariant(
                $"RG:{railgun.Exists}:{railgun.ProjectileSpeed:0.###}:{railgun.ProjectileLifetime}:{railgun.EnergyCost:0.###}:{railgun.MetalCost:0.###}");

        if (subsystem is ShieldSubsystem shield)
            return FormattableString.Invariant($"SH:{shield.Exists}:{shield.Maximum:0.###}:{shield.MaximumRate:0.###}");

        if (subsystem is RepairSubsystem repair)
            return FormattableString.Invariant($"RP:{repair.Exists}:{repair.MaximumRate:0.###}");

        if (subsystem is ResourceMinerSubsystem resourceMiner)
            return FormattableString.Invariant($"RM:{resourceMiner.Exists}:{resourceMiner.MaximumRate:0.###}");

        if (subsystem is NebulaCollectorSubsystem nebulaCollector)
            return FormattableString.Invariant($"NC:{nebulaCollector.Exists}:{nebulaCollector.MaximumRate:0.###}");

        if (subsystem is CargoSubsystem cargo)
            return FormattableString.Invariant(
                $"CG:{cargo.Exists}:{cargo.MaximumMetal:0.###}:{cargo.MaximumCarbon:0.###}:{cargo.MaximumHydrogen:0.###}:{cargo.MaximumSilicon:0.###}:{cargo.MaximumNebula:0.###}");

        if (subsystem is BatterySubsystem battery)
            return FormattableString.Invariant($"BT:{battery.Exists}:{battery.Maximum:0.###}");

        if (subsystem is EnergyCellSubsystem energyCell)
            return FormattableString.Invariant($"EC:{energyCell.Exists}:{energyCell.Efficiency:0.###}");

        if (subsystem is HullSubsystem hull)
            return FormattableString.Invariant($"HL:{hull.Exists}:{hull.Maximum:0.###}");

        if (subsystem is ArmorSubsystem armor)
            return FormattableString.Invariant($"AR:{armor.Exists}:{armor.Reduction:0.###}");

        if (subsystem is StructureOptimizerSubsystem structureOptimizer)
            return FormattableString.Invariant($"SO:{structureOptimizer.Exists}:{structureOptimizer.ReductionPercent:0.###}");

        if (subsystem is JumpDriveSubsystem jumpDrive)
            return FormattableString.Invariant($"JD:{jumpDrive.Exists}:{jumpDrive.EnergyCost:0.###}");

        return $"{subsystem.GetType().Name}:{subsystem.Exists}:{subsystem.Status}";
    }
}

