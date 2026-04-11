using System.Diagnostics;
using Flattiverse.Connector;
using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Units;

namespace Development;

partial class Program
{
    private const int EffectiveStructureLoadLocalTimeoutMs = 10000;
    private const float EffectiveStructureLoadTolerance = 0.0001f;

    private static async Task RunEffectiveStructureLoadCheckLocal()
    {
        Process? galaxyProcess = null;
        Galaxy? adminGalaxy = null;
        Galaxy? playerGalaxy = null;
        string? restoreConfigurationXml = null;
        Dictionary<byte, string>? restoreRegionsByCluster = null;
        byte testClusterId = 0;
        string clusterName = $"EffectiveLoadLocal{Environment.ProcessId}";
        string regionName = $"EffectiveLoadRegion{Environment.ProcessId}";
        string shipName = $"EffectiveLoadShip{Environment.ProcessId}";
        string adminAuth = ResolveNpcUnitsLocalAdminAuth();
        DatabaseAccountRow adminAccountRow = QueryAccountRow(adminAuth);
        string playerAuth = ResolveNpcUnitsLocalPlayerAuth(adminAccountRow.AccountId);
        ClassicShipControllable? ship = null;

        ClearLocalAccountSession(adminAuth, "EFFECTIVE-LOAD-LOCAL:ADMIN");
        ClearLocalAccountSession(playerAuth, "EFFECTIVE-LOAD-LOCAL:PLAYER");

        try
        {
            Console.WriteLine("EFFECTIVE-LOAD-LOCAL: starting local galaxy 666...");
            galaxyProcess = StartLocalGalaxyProcess();
            (adminGalaxy, _) = await ConnectLocalAdminAfterInitialRebuild(galaxyProcess, adminAuth).ConfigureAwait(false);
            restoreConfigurationXml = BuildConfigurationXml(adminGalaxy, (ClusterSpec[]?)null);
            restoreRegionsByCluster = await CaptureRegionsByCluster(adminGalaxy).ConfigureAwait(false);

            if (!TryGetUnusedClusterId(adminGalaxy, 255, out testClusterId))
                throw new InvalidOperationException("EFFECTIVE-LOAD-LOCAL: no free cluster id available.");

            Console.WriteLine($"EFFECTIVE-LOAD-LOCAL: configuring dedicated cluster #{testClusterId}...");
            await adminGalaxy.Configure(BuildStaticMapTestConfigurationXml(adminGalaxy, testClusterId, clusterName)).ConfigureAwait(false);

            if (!await WaitForCondition(delegate
                {
                    return adminGalaxy.Clusters.TryGet(testClusterId, out Cluster? _);
                }, 5000).ConfigureAwait(false))
                throw new InvalidOperationException("EFFECTIVE-LOAD-LOCAL: dedicated cluster did not appear after configure.");

            await RemoveAllRegions(adminGalaxy).ConfigureAwait(false);

            Cluster adminCluster = adminGalaxy.Clusters[testClusterId];

            if (!TryGetTeamByName(adminGalaxy, TeamName, out Team? pinkTeam) || pinkTeam is null)
                throw new InvalidOperationException($"EFFECTIVE-LOAD-LOCAL: team {TeamName} not found.");

            byte regionId = await FindUnusedRegionId(adminCluster).ConfigureAwait(false);
            string regionXml = FormattableString.Invariant(
                $"<Region Id=\"{regionId}\" Name=\"{regionName}\" Left=\"-200\" Top=\"-200\" Right=\"200\" Bottom=\"200\"><Team Id=\"{pinkTeam.Id}\" /></Region>");

            Console.WriteLine("EFFECTIVE-LOAD-LOCAL: creating start region...");
            await adminCluster.SetRegion(regionXml).ConfigureAwait(false);

            Console.WriteLine("EFFECTIVE-LOAD-LOCAL: connecting local player...");
            playerGalaxy = await ConnectLocalPlayer(playerAuth, TeamName, "EFFECTIVE-LOAD-LOCAL:PLAYER").ConfigureAwait(false);

            ship = await playerGalaxy.CreateClassicShip(shipName).ConfigureAwait(false);
            await ship.Continue().ConfigureAwait(false);

            if (!await WaitForAliveState(ship, true, EffectiveStructureLoadLocalTimeoutMs).ConfigureAwait(false))
                throw new InvalidOperationException("EFFECTIVE-LOAD-LOCAL: ship did not become alive after Continue().");

            if (!await WaitForCondition(delegate { return ship.Cluster.Id == testClusterId; }, EffectiveStructureLoadLocalTimeoutMs).ConfigureAwait(false))
                throw new InvalidOperationException("EFFECTIVE-LOAD-LOCAL: ship did not spawn in the dedicated cluster.");

            if (!await WaitForCondition(delegate
                {
                    if (!adminGalaxy.Clusters.TryGet(ship.Cluster.Id, out Cluster? ownerAdminCluster) || ownerAdminCluster is null)
                        return false;

                    foreach (Unit unit in ownerAdminCluster.Units)
                        if (unit.Name == ship.Name)
                            return true;

                    return false;
                }, EffectiveStructureLoadLocalTimeoutMs).ConfigureAwait(false))
                throw new InvalidOperationException("EFFECTIVE-LOAD-LOCAL: admin session did not observe the spawned ship in time.");

            BatterySubsystem ionBattery = ship.IonBattery;

            if (ionBattery.Tier != 0)
                throw new InvalidOperationException($"EFFECTIVE-LOAD-LOCAL: expected IonBattery tier 0, received {ionBattery.Tier}.");

            byte targetTier = 1;
            SubsystemTierInfo targetTierInfo = ionBattery.TierInfos[targetTier];
            float initialEffectiveStructureLoad = ship.EffectiveStructureLoad;
            float expectedEffectiveStructureLoad = ship.CalculateProjectedEffectiveStructureLoad(SubsystemSlot.IonBattery, targetTierInfo.StructuralLoad);

            Console.WriteLine(
                $"EFFECTIVE-LOAD-LOCAL: debug-refreshing IonBattery T{ionBattery.Tier} -> T{targetTier}, expectedLoad={expectedEffectiveStructureLoad:0.###}...");
            await adminCluster.DebugSetPlayerUnitSubsystemTier(ship.Name, SubsystemSlot.IonBattery, targetTier).ConfigureAwait(false);

            if (!await WaitForCondition(delegate { return ship.IonBattery.Tier == targetTier; }, EffectiveStructureLoadLocalTimeoutMs).ConfigureAwait(false))
                throw new InvalidOperationException("EFFECTIVE-LOAD-LOCAL: tier refresh did not complete in time.");

            if (!ReferenceEquals(playerGalaxy.Controllables[ship.Id], ship))
                throw new InvalidOperationException("EFFECTIVE-LOAD-LOCAL: controllable was replaced instead of refreshed in place.");

            if (MathF.Abs(ship.EffectiveStructureLoad - expectedEffectiveStructureLoad) > EffectiveStructureLoadTolerance)
                throw new InvalidOperationException(
                    $"EFFECTIVE-LOAD-LOCAL: effective load mismatch. expected={expectedEffectiveStructureLoad:0.###}, actual={ship.EffectiveStructureLoad:0.###}.");

            if (!(ship.EffectiveStructureLoad > initialEffectiveStructureLoad))
                throw new InvalidOperationException(
                    $"EFFECTIVE-LOAD-LOCAL: effective load did not increase. before={initialEffectiveStructureLoad:0.###}, after={ship.EffectiveStructureLoad:0.###}.");

            float expectedGravity = SubsystemTierInfo.CalculateGravity(expectedEffectiveStructureLoad);
            float expectedSize = SubsystemTierInfo.CalculateRadius(expectedEffectiveStructureLoad);
            float expectedSpeedLimit = SubsystemTierInfo.CalculateClassicSpeedLimit(expectedEffectiveStructureLoad);
            float expectedEngineEfficiency = SubsystemTierInfo.CalculateEngineEfficiency(expectedEffectiveStructureLoad);

            if (MathF.Abs(ship.Gravity - expectedGravity) > EffectiveStructureLoadTolerance)
                throw new InvalidOperationException(
                    $"EFFECTIVE-LOAD-LOCAL: gravity mismatch. expected={expectedGravity:0.###}, actual={ship.Gravity:0.###}.");

            if (MathF.Abs(ship.Size - expectedSize) > EffectiveStructureLoadTolerance)
                throw new InvalidOperationException(
                    $"EFFECTIVE-LOAD-LOCAL: size mismatch. expected={expectedSize:0.###}, actual={ship.Size:0.###}.");

            if (MathF.Abs(ship.SpeedLimit - expectedSpeedLimit) > EffectiveStructureLoadTolerance)
                throw new InvalidOperationException(
                    $"EFFECTIVE-LOAD-LOCAL: speed-limit mismatch. expected={expectedSpeedLimit:0.###}, actual={ship.SpeedLimit:0.###}.");

            if (MathF.Abs(ship.EngineEfficiency - expectedEngineEfficiency) > EffectiveStructureLoadTolerance)
                throw new InvalidOperationException(
                    $"EFFECTIVE-LOAD-LOCAL: engine-efficiency mismatch. expected={expectedEngineEfficiency:0.###}, actual={ship.EngineEfficiency:0.###}.");

            Console.WriteLine(
                $"EFFECTIVE-LOAD-LOCAL: OK load={ship.EffectiveStructureLoad:0.###}, gravity={ship.Gravity:0.###}, size={ship.Size:0.###}, speedLimit={ship.SpeedLimit:0.###}, engineEfficiency={ship.EngineEfficiency:0.###}");
        }
        finally
        {
            if (ship is not null && ship.Active)
                try
                {
                    await CloseControllable(ship).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"EFFECTIVE-LOAD-LOCAL: cleanup close failed: {exception.Message}");
                }

            if (playerGalaxy is not null)
                playerGalaxy.Dispose();

            await WaitForSessionGalaxy(playerAuth, null, 7000).ConfigureAwait(false);

            if (adminGalaxy is not null)
            {
                if (restoreConfigurationXml is not null)
                    try
                    {
                        await adminGalaxy.Configure(restoreConfigurationXml).ConfigureAwait(false);
                    }
                    catch (GameException exception)
                    {
                        Console.WriteLine($"EFFECTIVE-LOAD-LOCAL: cleanup configuration restore failed: {exception.Message}");
                    }

                if (restoreRegionsByCluster is not null)
                    try
                    {
                        await RestoreRegionsByCluster(adminGalaxy, restoreRegionsByCluster).ConfigureAwait(false);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine($"EFFECTIVE-LOAD-LOCAL: cleanup region restore failed: {exception.Message}");
                    }

                adminGalaxy.Dispose();
            }

            await WaitForSessionGalaxy(adminAuth, null, 7000).ConfigureAwait(false);

            if (galaxyProcess is not null)
                StopProcess(galaxyProcess);
        }
    }
}
