using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using Flattiverse.Connector;
using Flattiverse.Connector.Events;
using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Units;

namespace Development;

partial class Program
{
    private const int ScannerVisibilityLocalTimeoutMs = 8000;
    private const float ScannerVisibilityStepAngle = 5f;
    private const float ScannerVisibilityWidth = 5f;
    private const float ScannerVisibilityLength = 220f;
    private const float ScannerVisibilityBitTolerance = 1f;

    private readonly struct VisibilityExpectation
    {
        public readonly string Name;
        public readonly UnitKind Kind;
        public readonly Vector Position;
        public readonly float Radius;

        public VisibilityExpectation(string name, UnitKind kind, Vector position, float radius)
        {
            Name = name;
            Kind = kind;
            Position = position;
            Radius = radius;
        }
    }

    private static async Task RunScannerVisibilityCheckLocal()
    {
        CultureInfo originalCulture = CultureInfo.CurrentCulture;
        CultureInfo originalUiCulture = CultureInfo.CurrentUICulture;
        string adminAuth = ResolveNpcUnitsLocalAdminAuth();
        DatabaseAccountRow adminAccountRow = QueryAccountRow(adminAuth);
        string playerAuth = ResolveNpcUnitsLocalPlayerAuth(adminAccountRow.AccountId);
        Process? galaxyProcess = null;
        Galaxy? adminGalaxy = null;
        Galaxy? playerGalaxy = null;
        Task? adminEventPump = null;
        Task? playerEventPump = null;
        ConcurrentQueue<FlattiverseEvent> adminEvents = new ConcurrentQueue<FlattiverseEvent>();
        ConcurrentQueue<FlattiverseEvent> playerEvents = new ConcurrentQueue<FlattiverseEvent>();
        string? restoreConfigurationXml = null;
        Dictionary<byte, string>? restoreRegionsByCluster = null;
        GameMode restoreGameMode = GameMode.Mission;
        byte testClusterId = 0;
        string clusterName = $"ScannerVisibilityLocal{Environment.ProcessId}";
        string regionName = $"ScannerVisibilityLocalRegion{Environment.ProcessId}";
        string shipName = $"ScannerVisibilityLocalShip{Environment.ProcessId}";
        string planetName = $"ScannerVisibilityPlanet{Environment.ProcessId}";
        string turretName = $"ScannerVisibilityTurret{Environment.ProcessId}";
        string buoyName = $"ScannerVisibilityBuoy{Environment.ProcessId}";
        List<VisibilityExpectation> expectations = new List<VisibilityExpectation>(3);
        HashSet<string> seenWithinExpectedWindow = new HashSet<string>(StringComparer.Ordinal);
        List<string> failures = new List<string>();

        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;

        ClearLocalAccountSession(adminAuth, "SCAN-VIS-LOCAL:ADMIN");
        ClearLocalAccountSession(playerAuth, "SCAN-VIS-LOCAL:PLAYER");

        try
        {
            Console.WriteLine("SCAN-VIS-LOCAL: starting local galaxy 666...");
            galaxyProcess = StartLocalGalaxyProcess();
            (adminGalaxy, _) = await ConnectLocalAdminAfterInitialRebuild(galaxyProcess, adminAuth).ConfigureAwait(false);
            adminEventPump = StartEventPump("SCAN-VIS-LOCAL:ADMIN", adminGalaxy, adminEvents);
            DrainEvents(adminEvents);

            restoreConfigurationXml = BuildConfigurationXml(adminGalaxy, (ClusterSpec[]?)null);
            restoreRegionsByCluster = await CaptureRegionsByCluster(adminGalaxy).ConfigureAwait(false);
            restoreGameMode = adminGalaxy.GameMode;

            if (adminGalaxy.Tournament is not null)
            {
                Console.WriteLine("SCAN-VIS-LOCAL: cancelling pre-existing tournament...");
                await adminGalaxy.CancelTournament().ConfigureAwait(false);
                await Task.Delay(250).ConfigureAwait(false);
                DrainEvents(adminEvents);
            }

            if (adminGalaxy.GameMode != GameMode.Mission)
            {
                Console.WriteLine("SCAN-VIS-LOCAL: switching galaxy 666 to Mission...");
                await adminGalaxy.Configure(BuildGameModeConfigurationXml(adminGalaxy, GameMode.Mission)).ConfigureAwait(false);

                if (!await WaitForCondition(delegate { return adminGalaxy.GameMode == GameMode.Mission; }, 5000).ConfigureAwait(false))
                    throw new InvalidOperationException("SCAN-VIS-LOCAL: galaxy did not switch to Mission.");
            }

            if (!TryGetUnusedClusterId(adminGalaxy, 255, out testClusterId))
                throw new InvalidOperationException("SCAN-VIS-LOCAL: no free cluster id available.");

            Console.WriteLine($"SCAN-VIS-LOCAL: configuring dedicated cluster #{testClusterId}...");
            await adminGalaxy.Configure(BuildStaticMapTestConfigurationXml(adminGalaxy, testClusterId, clusterName)).ConfigureAwait(false);

            if (!await WaitForCondition(delegate
                {
                    return adminGalaxy.Clusters.TryGet(testClusterId, out Cluster? _);
                }, 5000).ConfigureAwait(false))
                throw new InvalidOperationException("SCAN-VIS-LOCAL: dedicated cluster did not appear after configure.");

            await RemoveAllRegions(adminGalaxy).ConfigureAwait(false);

            Cluster adminCluster = adminGalaxy.Clusters[testClusterId];

            if (!TryGetTeamByName(adminGalaxy, TeamName, out Team? pinkTeam) || pinkTeam is null)
                throw new InvalidOperationException($"SCAN-VIS-LOCAL: team {TeamName} not found.");

            if (!adminGalaxy.Teams.TryGet(SpectatorsTeamId, out Team? spectatorsTeam) || spectatorsTeam is null)
                throw new InvalidOperationException($"SCAN-VIS-LOCAL: team {SpectatorsTeamId} not found.");

            byte regionId = await FindUnusedRegionId(adminCluster).ConfigureAwait(false);
            string regionXml = FormattableString.Invariant(
                $"<Region Id=\"{regionId}\" Name=\"{regionName}\" Left=\"-80\" Top=\"-80\" Right=\"80\" Bottom=\"80\"><Team Id=\"{pinkTeam.Id}\" /></Region>");

            Console.WriteLine("SCAN-VIS-LOCAL: creating start region...");
            await adminCluster.SetRegion(regionXml).ConfigureAwait(false);

            Console.WriteLine("SCAN-VIS-LOCAL: connecting local player...");
            playerGalaxy = await ConnectLocalPlayer(playerAuth, TeamName, "SCAN-VIS-LOCAL:PLAYER").ConfigureAwait(false);
            playerEventPump = StartEventPump("SCAN-VIS-LOCAL:PLAYER", playerGalaxy, playerEvents);
            DrainEvents(playerEvents);

            ClassicShipControllable ship = await playerGalaxy.CreateClassicShip(shipName).ConfigureAwait(false);
            await ship.Continue().ConfigureAwait(false);

            if (!await WaitForAliveState(ship, true, 10000).ConfigureAwait(false))
                throw new InvalidOperationException("SCAN-VIS-LOCAL: ship did not become alive after Continue().");

            if (!await WaitForCondition(delegate { return ship.Cluster.Id == testClusterId; }, 10000).ConfigureAwait(false))
                throw new InvalidOperationException("SCAN-VIS-LOCAL: ship did not spawn in the dedicated cluster.");

            if (!await WaitForCondition(delegate { return ship.Movement < 0.01f; }, 3000).ConfigureAwait(false))
                throw new InvalidOperationException("SCAN-VIS-LOCAL: ship did not settle after spawning.");

            Vector shipPosition = new Vector(ship.Position);
            Vector planetPosition = shipPosition + Vector.FromAngleLength(90f, 220f);
            Vector turretPosition = shipPosition + Vector.FromAngleLength(180f, 220f);
            Vector buoyPosition = shipPosition + Vector.FromAngleLength(270f, 220f);
            string planetXml = FormattableString.Invariant(
                $"<Planet Name=\"{planetName}\" X=\"{planetPosition.X:R}\" Y=\"{planetPosition.Y:R}\" Radius=\"24\" Gravity=\"0\" Type=\"OceanWorld\" Metal=\"10\" Carbon=\"10\" Hydrogen=\"10\" Silicon=\"10\" />");
            string turretXml = FormattableString.Invariant(
                $"<AiTurret Name=\"{turretName}\" Team=\"{spectatorsTeam.Id}\" X=\"{turretPosition.X:R}\" Y=\"{turretPosition.Y:R}\" Radius=\"16\" Gravity=\"0\" Hull=\"30\" RepairPerTick=\"0\" RespawnTicks=\"0\" RespawnPlayerDistance=\"180\" ShotSpeed=\"5.0\" ShotDamage=\"0.1\" />");
            string buoyXml = FormattableString.Invariant(
                $"<Buoy Name=\"{buoyName}\" X=\"{buoyPosition.X:R}\" Y=\"{buoyPosition.Y:R}\" Radius=\"10\" Gravity=\"0\" Message=\"ScannerVisibilityControl\" />");

            Console.WriteLine("SCAN-VIS-LOCAL: creating probe units...");
            await adminCluster.SetUnit(planetXml).ConfigureAwait(false);
            await adminCluster.SetUnit(turretXml).ConfigureAwait(false);
            await adminCluster.SetUnit(buoyXml).ConfigureAwait(false);

            expectations.Add(new VisibilityExpectation(planetName, UnitKind.Planet, planetPosition, 24f));
            expectations.Add(new VisibilityExpectation(turretName, UnitKind.AiTurret, turretPosition, 16f));
            expectations.Add(new VisibilityExpectation(buoyName, UnitKind.Buoy, buoyPosition, 10f));

            DrainEvents(playerEvents);
            await Task.Delay(250).ConfigureAwait(false);

            for (int expectationIndex = 0; expectationIndex < expectations.Count; expectationIndex++)
                if (TryIsUnitVisible(playerGalaxy, testClusterId, expectations[expectationIndex]))
                    failures.Add($"unit {expectations[expectationIndex].Name} was visible before the scanner was activated");

            Console.WriteLine("SCAN-VIS-LOCAL: activating narrow scanner sweep...");
            await ship.MainScanner.Set(ScannerVisibilityWidth, ScannerVisibilityLength, 0f).ConfigureAwait(false);
            await ship.MainScanner.On().ConfigureAwait(false);

            if (!await WaitForCondition(delegate
                {
                    return ship.MainScanner.Active &&
                           MathF.Abs(ship.MainScanner.CurrentWidth - ScannerVisibilityWidth) < 0.1f &&
                           MathF.Abs(ship.MainScanner.CurrentLength - ScannerVisibilityLength) < 0.1f &&
                           CalculateWrappedAngleDistance(ship.MainScanner.CurrentAngle, 0f) < 0.1f;
                }, ScannerVisibilityLocalTimeoutMs).ConfigureAwait(false))
                throw new InvalidOperationException("SCAN-VIS-LOCAL: scanner did not reach the requested initial state.");

            List<FlattiverseEvent> initialSweepEvents = DrainEvents(playerEvents);
            GalaxyTickEvent? initialSweepTick = await WaitForQueuedEvent(playerEvents, 4000, initialSweepEvents,
                delegate(GalaxyTickEvent @event)
                {
                    return true;
                }).ConfigureAwait(false);

            if (initialSweepTick is null)
                throw new InvalidOperationException("SCAN-VIS-LOCAL: no galaxy tick arrived for the initial scanner state.");

            for (int angleIndex = 0; angleIndex < 72; angleIndex++)
            {
                float targetAngle = angleIndex * ScannerVisibilityStepAngle;
                List<FlattiverseEvent> angleEvents = DrainEvents(playerEvents);

                await ship.MainScanner.Set(ScannerVisibilityWidth, ScannerVisibilityLength, targetAngle).ConfigureAwait(false);

                if (!await WaitForCondition(delegate
                    {
                        return ship.MainScanner.Active &&
                               MathF.Abs(ship.MainScanner.CurrentWidth - ScannerVisibilityWidth) < 0.1f &&
                               MathF.Abs(ship.MainScanner.CurrentLength - ScannerVisibilityLength) < 0.1f &&
                               CalculateWrappedAngleDistance(ship.MainScanner.CurrentAngle, targetAngle) < 0.1f;
                    }, ScannerVisibilityLocalTimeoutMs).ConfigureAwait(false))
                    throw new InvalidOperationException($"SCAN-VIS-LOCAL: scanner did not settle at angle {targetAngle:0.###}.");

                GalaxyTickEvent? settledTick = await WaitForQueuedEvent(playerEvents, 4000, angleEvents,
                    delegate(GalaxyTickEvent @event)
                    {
                        return true;
                    }).ConfigureAwait(false);

                if (settledTick is null)
                    throw new InvalidOperationException($"SCAN-VIS-LOCAL: no galaxy tick arrived after settling at angle {targetAngle:0.###}.");

                float currentAngle = ship.MainScanner.CurrentAngle;
                float currentWidth = ship.MainScanner.CurrentWidth;
                float currentLength = ship.MainScanner.CurrentLength;
                Vector currentShipPosition = new Vector(ship.Position);

                for (int expectationIndex = 0; expectationIndex < expectations.Count; expectationIndex++)
                {
                    VisibilityExpectation expectation = expectations[expectationIndex];
                    float distance = Vector.Distance(currentShipPosition, expectation.Position);
                    float surfaceDistance = distance - expectation.Radius;
                    float unitHalfAngle = MathF.Asin(expectation.Radius / distance) * 180f / MathF.PI + 0.5f;
                    float allowedAngleDistance = currentWidth / 2f + unitHalfAngle + ScannerVisibilityBitTolerance;
                    float actualAngle = (expectation.Position - currentShipPosition).Angle;
                    bool shouldBeVisible = surfaceDistance <= currentLength + 0.5f &&
                                           CalculateWrappedAngleDistance(currentAngle, actualAngle) <= allowedAngleDistance;
                    bool isVisible = TryIsUnitVisible(playerGalaxy, testClusterId, expectation);

                    if (isVisible && shouldBeVisible)
                        seenWithinExpectedWindow.Add(expectation.Name);

                    if (!isVisible || shouldBeVisible)
                        continue;

                    string angleTrace = string.Join(" || ", angleEvents.Select(DescribeSpectatorEvent));
                    failures.Add(
                        $"false positive for {expectation.Name} at scannerAngle={currentAngle:0.###} expectedAngle={actualAngle:0.###} allowedDiff={allowedAngleDistance:0.###} actualDiff={CalculateWrappedAngleDistance(currentAngle, actualAngle):0.###} events=[{angleTrace}]");
                }
            }

            for (int expectationIndex = 0; expectationIndex < expectations.Count; expectationIndex++)
                if (!seenWithinExpectedWindow.Contains(expectations[expectationIndex].Name))
                    failures.Add($"unit {expectations[expectationIndex].Name} was never visible inside its expected scan window");

            if (failures.Count != 0)
            {
                for (int failureIndex = 0; failureIndex < failures.Count; failureIndex++)
                    Console.WriteLine($"SCAN-VIS-LOCAL: FAILURE {failureIndex + 1}: {failures[failureIndex]}");

                throw new InvalidOperationException($"SCAN-VIS-LOCAL: detected {failures.Count} scanner visibility mismatches.");
            }

            Console.WriteLine("SCAN-VIS-LOCAL: SUCCESS");
            Console.WriteLine("SCAN-VIS-LOCAL: verified that masking units stay hidden outside the narrow scanner cone.");
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUiCulture;

            if (playerGalaxy is not null)
                playerGalaxy.Dispose();

            await WaitForSessionGalaxy(playerAuth, null, 7000).ConfigureAwait(false);

            if (adminGalaxy is not null)
            {
                if (adminGalaxy.Tournament is not null)
                    try
                    {
                        await adminGalaxy.CancelTournament().ConfigureAwait(false);
                    }
                    catch (GameException exception)
                    {
                        Console.WriteLine($"SCAN-VIS-LOCAL: cleanup CancelTournament failed: {exception.Message}");
                    }

                if (restoreGameMode != adminGalaxy.GameMode && adminGalaxy.Tournament is null)
                    try
                    {
                        await adminGalaxy.Configure(BuildGameModeConfigurationXml(adminGalaxy, restoreGameMode)).ConfigureAwait(false);
                    }
                    catch (GameException exception)
                    {
                        Console.WriteLine($"SCAN-VIS-LOCAL: cleanup game mode restore failed: {exception.Message}");
                    }

                if (restoreConfigurationXml is not null)
                    try
                    {
                        await adminGalaxy.Configure(restoreConfigurationXml).ConfigureAwait(false);
                    }
                    catch (GameException exception)
                    {
                        Console.WriteLine($"SCAN-VIS-LOCAL: cleanup configuration restore failed: {exception.Message}");
                    }

                if (restoreRegionsByCluster is not null)
                    try
                    {
                        await RestoreRegionsByCluster(adminGalaxy, restoreRegionsByCluster).ConfigureAwait(false);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine($"SCAN-VIS-LOCAL: cleanup region restore failed: {exception.Message}");
                    }

                adminGalaxy.Dispose();
            }

            await WaitForSessionGalaxy(adminAuth, null, 7000).ConfigureAwait(false);

            if (playerEventPump is not null)
                await Task.WhenAny(playerEventPump, Task.Delay(1000)).ConfigureAwait(false);

            if (adminEventPump is not null)
                await Task.WhenAny(adminEventPump, Task.Delay(1000)).ConfigureAwait(false);

            if (galaxyProcess is not null)
                StopProcess(galaxyProcess);
        }
    }

    private static bool TryIsUnitVisible(Galaxy galaxy, byte clusterId, VisibilityExpectation expectation)
    {
        if (expectation.Kind == UnitKind.Planet)
            return TryFindUnit<Planet>(galaxy, clusterId, expectation.Name, out Planet? _);

        if (expectation.Kind == UnitKind.AiTurret)
            return TryFindUnit<AiTurret>(galaxy, clusterId, expectation.Name, out AiTurret? _);

        if (expectation.Kind == UnitKind.Buoy)
            return TryFindUnit<Buoy>(galaxy, clusterId, expectation.Name, out Buoy? _);

        throw new InvalidOperationException($"SCAN-VIS-LOCAL: unsupported expectation kind {expectation.Kind}.");
    }

    private static float CalculateWrappedAngleDistance(float left, float right)
    {
        float distance = MathF.Abs(left - right) % 360f;

        if (distance > 180f)
            distance = 360f - distance;

        return distance;
    }
}
