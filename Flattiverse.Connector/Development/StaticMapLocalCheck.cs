using System.Collections.Concurrent;
using System.Diagnostics;
using Flattiverse.Connector;
using Flattiverse.Connector.Events;
using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Development;

partial class Program
{
    private const ushort StaticMapLocalGalaxyId = 666;
    private const string LocalGalaxyProjectPath = "D:\\Projects\\fv\\fv-server\\Galaxy\\Galaxy\\Galaxy.csproj";
    private const string LocalGalaxyWorkingDirectory = "D:\\Projects\\fv";
    private const string LocalGalaxyExecutablePath = "D:\\Projects\\fv\\fv-server\\Galaxy\\Galaxy\\bin\\Debug\\net10.0\\Galaxy.exe";
    private const int InitialStaticMapTimeoutMs = 240000;
    private const int StaticMapRebuildTimeoutMs = 240000;
    private const int StaticMapDeathTimeoutMs = 30000;

    private static async Task RunStaticMapCheckLocal()
    {
        Process? galaxyProcess = null;
        Galaxy? adminGalaxy = null;
        Galaxy? playerGalaxy = null;
        Task? adminEventPump = null;
        Task? playerEventPump = null;
        string? restoreConfigurationXml = null;
        Dictionary<byte, string>? restoreRegionsByCluster = null;
        GameMode restoreGameMode = GameMode.Mission;
        ConcurrentQueue<FlattiverseEvent> adminEvents = new ConcurrentQueue<FlattiverseEvent>();
        ConcurrentQueue<FlattiverseEvent> playerEvents = new ConcurrentQueue<FlattiverseEvent>();
        byte testClusterId = 0;
        string testClusterName = $"StaticMapLocal{Environment.ProcessId}";
        string testRegionName = $"StaticMapLocalRegion{Environment.ProcessId}";
        string passiveSunName = $"StaticMapLocalSun{Environment.ProcessId}";
        string scannerBuoyName = $"StaticMapLocalBuoy{Environment.ProcessId}";
        string shipName = $"StaticMapLocalShip{Environment.ProcessId}";
        bool initialRebuildDeniedOnConnect = false;
        string adminAuth = ResolveNpcUnitsLocalAdminAuth();
        DatabaseAccountRow adminAccountRow = QueryAccountRow(adminAuth);
        string playerAuth = ResolveNpcUnitsLocalPlayerAuth(adminAccountRow.AccountId);

        ClearLocalAccountSession(adminAuth, "STATIC-MAP-LOCAL:ADMIN");
        ClearLocalAccountSession(playerAuth, "STATIC-MAP-LOCAL:PLAYER");

        try
        {
            Console.WriteLine("STATIC-MAP-LOCAL: starting local galaxy 666...");
            galaxyProcess = StartLocalGalaxyProcess();
            (adminGalaxy, initialRebuildDeniedOnConnect) = await ConnectLocalAdminAfterInitialRebuild(galaxyProcess, adminAuth)
                .ConfigureAwait(false);
            adminEventPump = StartEventPump("STATIC-MAP-LOCAL:ADMIN", adminGalaxy, adminEvents);
            DrainEvents(adminEvents);

            restoreConfigurationXml = BuildConfigurationXml(adminGalaxy, (ClusterSpec[]?)null);
            restoreRegionsByCluster = await CaptureRegionsByCluster(adminGalaxy).ConfigureAwait(false);
            restoreGameMode = adminGalaxy.GameMode;

            if (!initialRebuildDeniedOnConnect)
            {
                Console.WriteLine("STATIC-MAP-LOCAL: initial rebuild finished too early, persisting a heavier startup scenario and restarting once...");
                await PrepareHeavyInitialRebuildScenario(adminGalaxy).ConfigureAwait(false);
                adminGalaxy.Dispose();
                adminGalaxy = null;

                if (adminEventPump is not null)
                {
                    await Task.WhenAny(adminEventPump, Task.Delay(1000)).ConfigureAwait(false);
                    adminEventPump = null;
                }

                await WaitForSessionGalaxy(adminAuth, null, 7000).ConfigureAwait(false);
                StopProcess(galaxyProcess);
                galaxyProcess = StartLocalGalaxyProcess();
                (adminGalaxy, initialRebuildDeniedOnConnect) = await ConnectLocalAdminAfterInitialRebuild(galaxyProcess, adminAuth)
                    .ConfigureAwait(false);
                adminEventPump = StartEventPump("STATIC-MAP-LOCAL:ADMIN", adminGalaxy, adminEvents);
                DrainEvents(adminEvents);

                if (!initialRebuildDeniedOnConnect)
                    throw new InvalidOperationException("STATIC-MAP-LOCAL: initial static-map rebuild still never blocked login after preparing the heavy startup scenario.");

                if (restoreConfigurationXml is not null)
                    await adminGalaxy.Configure(restoreConfigurationXml).ConfigureAwait(false);

                if (restoreRegionsByCluster is not null)
                    await RestoreRegionsByCluster(adminGalaxy, restoreRegionsByCluster).ConfigureAwait(false);
            }

            if (adminGalaxy.Tournament is not null)
            {
                Console.WriteLine("STATIC-MAP-LOCAL: cancelling pre-existing tournament...");
                await adminGalaxy.CancelTournament().ConfigureAwait(false);
                await Task.Delay(250).ConfigureAwait(false);
                DrainEvents(adminEvents);
            }

            if (!TryGetUnusedClusterId(adminGalaxy, 255, out testClusterId))
                throw new InvalidOperationException("STATIC-MAP-LOCAL: no free cluster id available for the local static-map test.");

            Console.WriteLine($"STATIC-MAP-LOCAL: configuring dedicated cluster #{testClusterId}...");
            await adminGalaxy.Configure(BuildStaticMapTestConfigurationXml(adminGalaxy, testClusterId, testClusterName)).ConfigureAwait(false);

            if (!await WaitForCondition(delegate
                {
                    return adminGalaxy.Clusters.TryGet(testClusterId, out Cluster? _);
                }, 5000).ConfigureAwait(false))
                throw new InvalidOperationException("STATIC-MAP-LOCAL: dedicated cluster did not appear after configure.");

            await RemoveAllRegions(adminGalaxy).ConfigureAwait(false);

            Cluster testCluster = adminGalaxy.Clusters[testClusterId];
            
            if (!TryGetTeamByName(adminGalaxy, TeamName, out Team? pinkSpawnTeam) || pinkSpawnTeam is null)
                throw new InvalidOperationException($"STATIC-MAP-LOCAL: team {TeamName} not found.");

            byte testRegionId = await FindUnusedRegionId(testCluster).ConfigureAwait(false);
            string regionXml =
                $"<Region Id=\"{testRegionId}\" Name=\"{testRegionName}\" Left=\"2080\" Top=\"-120\" Right=\"2140\" Bottom=\"120\"><Team Id=\"{pinkSpawnTeam.Id}\" /></Region>";

            Console.WriteLine("STATIC-MAP-LOCAL: creating start region...");
            await testCluster.SetRegion(regionXml).ConfigureAwait(false);

            string sunXml =
                $"<Sun Name=\"{passiveSunName}\" X=\"1890\" Y=\"0\" Radius=\"40\" Gravity=\"0\" Energy=\"1.8\" Ions=\"1.25\" Neutrinos=\"0.6\" Heat=\"0.35\" Drain=\"0.18\" />";

            Console.WriteLine("STATIC-MAP-LOCAL: creating passive scan marker sun...");
            await testCluster.SetUnit(sunXml).ConfigureAwait(false);

            string buoyXml =
                $"<Buoy Name=\"{scannerBuoyName}\" X=\"1890\" Y=\"120\" Radius=\"10\" Gravity=\"0\" Message=\"Scanner marker\" />";

            Console.WriteLine("STATIC-MAP-LOCAL: creating scanner-visible buoy...");
            await testCluster.SetUnit(buoyXml).ConfigureAwait(false);

            (float X, float Y)[] rebuildProbePositions = new (float X, float Y)[]
            {
                (-8000f, 8000f),
                (-6000f, 6000f),
                (-4000f, 4000f),
                (-2000f, 2000f),
                (-8000f, -8000f),
                (-6000f, -6000f),
                (-4000f, -4000f),
                (-2000f, -2000f)
            };

            for (int index = 0; index < rebuildProbePositions.Length; index++)
            {
                (float X, float Y) rebuildProbePosition = rebuildProbePositions[index];
                string rebuildProbeXml =
                    $"<Sun Name=\"StaticMapProbe{Environment.ProcessId}_{index}\" X=\"{rebuildProbePosition.X:0}\" Y=\"{rebuildProbePosition.Y:0}\" Radius=\"40\" Gravity=\"0.03\" Energy=\"1.8\" Ions=\"1.25\" Neutrinos=\"0.6\" Heat=\"0.35\" Drain=\"0.18\" />";

                await testCluster.SetUnit(rebuildProbeXml).ConfigureAwait(false);
            }

            await ExpectTooLargeRadiusRejected(testCluster).ConfigureAwait(false);
            await ExpectOuterSegmentCenterRejected(testCluster).ConfigureAwait(false);

            Console.WriteLine("STATIC-MAP-LOCAL: connecting local player...");
            playerGalaxy = await ConnectLocalPlayer(playerAuth, TeamName, "STATIC-MAP-LOCAL:PLAYER").ConfigureAwait(false);
            playerEventPump = StartEventPump("STATIC-MAP-LOCAL:PLAYER", playerGalaxy, playerEvents);
            DrainEvents(playerEvents);

            await VerifyRebuildRestartAndPlayerProgress(adminGalaxy, playerEvents).ConfigureAwait(false);

            if (!TryGetTwoNonSpectatorTeams(adminGalaxy, out Team? pinkTeam, out Team? greenTeam) || pinkTeam is null || greenTeam is null)
                throw new InvalidOperationException("STATIC-MAP-LOCAL: galaxy 666 does not expose two non-spectator teams for tournament checks.");

            if (adminGalaxy.GameMode != GameMode.Domination)
            {
                Console.WriteLine("STATIC-MAP-LOCAL: switching galaxy 666 to Domination for tournament lock checks...");
                await adminGalaxy.Configure(BuildGameModeConfigurationXml(adminGalaxy, GameMode.Domination)).ConfigureAwait(false);

                if (!await WaitForCondition(delegate { return adminGalaxy.GameMode == GameMode.Domination; }, 5000).ConfigureAwait(false))
                    throw new InvalidOperationException("STATIC-MAP-LOCAL: galaxy did not switch to Domination.");
            }

            DatabaseAccountRow pinkAccountRow = QueryAccountRow(playerAuth);
            string secondPlayerAuth = LocalSwitchGatePinkPlayerAuth;

            if (secondPlayerAuth.StartsWith("<INSERT", StringComparison.Ordinal))
            {
                string[] candidateAuths = QueryCandidatePlayerAuths(new int[] { pinkAccountRow.AccountId }, 8);

                if (candidateAuths.Length == 0)
                    throw new InvalidOperationException("STATIC-MAP-LOCAL: no second player account found for the tournament lock check.");

                secondPlayerAuth = candidateAuths[0];
            }

            DatabaseAccountRow greenAccountRow = QueryAccountRow(secondPlayerAuth);
            TournamentConfiguration tournamentConfiguration =
                BuildTournamentConfiguration(pinkTeam, greenTeam, pinkAccountRow.AccountId, greenAccountRow.AccountId, 1400);

            await VerifyTournamentCommandsRejectedDuringRebuild(adminGalaxy, playerEvents, tournamentConfiguration).ConfigureAwait(false);

            Console.WriteLine("STATIC-MAP-LOCAL: configuring tournament to verify rebuild lock...");
            await adminGalaxy.ConfigureTournament(tournamentConfiguration).ConfigureAwait(false);

            if (adminGalaxy.Tournament is null)
                throw new InvalidOperationException("STATIC-MAP-LOCAL: tournament was not configured.");

            bool rebuildLocked = false;

            try
            {
                await adminGalaxy.RebuildStaticMap().ConfigureAwait(false);
            }
            catch (StaticMapRebuildLockedGameException)
            {
                rebuildLocked = true;
            }

            if (!rebuildLocked)
                throw new InvalidOperationException("STATIC-MAP-LOCAL: rebuilding during an active tournament unexpectedly succeeded.");

            Console.WriteLine("STATIC-MAP-LOCAL: cancelling tournament and restoring Mission mode...");
            await adminGalaxy.CancelTournament().ConfigureAwait(false);

            if (adminGalaxy.GameMode != GameMode.Mission)
            {
                await adminGalaxy.Configure(BuildGameModeConfigurationXml(adminGalaxy, GameMode.Mission)).ConfigureAwait(false);

                if (!await WaitForCondition(delegate { return adminGalaxy.GameMode == GameMode.Mission; }, 5000).ConfigureAwait(false))
                    throw new InvalidOperationException("STATIC-MAP-LOCAL: galaxy did not switch back to Mission.");
            }

            ClassicShipControllable ship = await playerGalaxy.CreateClassicShip(shipName).ConfigureAwait(false);
            await ship.Continue().ConfigureAwait(false);

            if (!await WaitForAliveState(ship, true, 10000).ConfigureAwait(false))
                throw new InvalidOperationException("STATIC-MAP-LOCAL: ship did not become alive after Continue().");

            if (!await WaitForCondition(delegate
                {
                    return ship.Cluster.Id == testClusterId;
                }, 10000).ConfigureAwait(false))
                throw new InvalidOperationException("STATIC-MAP-LOCAL: ship did not spawn in the dedicated static-map test cluster.");

            Vector scannerDirection = new Vector(1890f - ship.Position.X, 120f - ship.Position.Y);
            await ship.MainScanner.Set(ship.MainScanner.MaximumWidth, ship.MainScanner.MaximumLength, scannerDirection.Angle).ConfigureAwait(false);
            await ship.MainScanner.On().ConfigureAwait(false);

            if (!await WaitForCondition(delegate
                {
                    return TryFindUnit<Buoy>(playerGalaxy, testClusterId, scannerBuoyName, out Buoy? _);
                }, 12000).ConfigureAwait(false))
                throw new InvalidOperationException("STATIC-MAP-LOCAL: player never received the scanner-visible buoy via scan.");

            Console.WriteLine("STATIC-MAP-LOCAL: accelerating ship out of the activated segment ring...");
            await ship.Engine.Set(new Vector(ship.Engine.Maximum, 0f)).ConfigureAwait(false);

            if (!await WaitForAliveState(ship, false, StaticMapDeathTimeoutMs).ConfigureAwait(false))
                throw new InvalidOperationException("STATIC-MAP-LOCAL: ship did not die after leaving the activated segment ring.");

            List<FlattiverseEvent> destroyedEvents = new List<FlattiverseEvent>();
            DestroyedControllableInfoEvent? destroyedEvent = null;

            for (int index = destroyedEvents.Count - 1; index >= 0; index--)
                if (destroyedEvents[index] is DestroyedControllableInfoEvent queuedDestroyedEvent &&
                    queuedDestroyedEvent.ControllableInfo.Name == ship.Name &&
                    queuedDestroyedEvent.Reason == PlayerUnitDestroyedReason.LostInDeepSpace)
                {
                    destroyedEvent = queuedDestroyedEvent;
                    break;
                }

            if (destroyedEvent is null)
                destroyedEvent = await WaitForQueuedEvent(playerEvents, 4000, destroyedEvents,
                    delegate(DestroyedControllableInfoEvent @event)
                    {
                        return @event.ControllableInfo.Name == ship.Name && @event.Reason == PlayerUnitDestroyedReason.LostInDeepSpace;
                    }).ConfigureAwait(false);

            if (destroyedEvent is null)
                throw new InvalidOperationException("STATIC-MAP-LOCAL: player controllable did not report LostInDeepSpace after leaving the activated segments.");

            DrainEvents(adminEvents);

            List<FlattiverseEvent> postDeathTickEvents = new List<FlattiverseEvent>();
            GalaxyTickEvent? adminPostDeathTick = await WaitForQueuedEvent(adminEvents, 10000, postDeathTickEvents,
                delegate(GalaxyTickEvent @event)
                {
                    return @event.RemainingStaticSegments == 0;
                }).ConfigureAwait(false);

            if (adminPostDeathTick is null)
                throw new InvalidOperationException("STATIC-MAP-LOCAL: admin did not receive a post-death tick event.");

            ValidateTick(adminPostDeathTick, "STATIC-MAP-LOCAL:admin-post-death");

            if (galaxyProcess.HasExited)
                throw new InvalidOperationException($"STATIC-MAP-LOCAL: local galaxy process exited unexpectedly with code {galaxyProcess.ExitCode}.");

            Console.WriteLine("STATIC-MAP-LOCAL: SUCCESS");
            Console.WriteLine($"STATIC-MAP-LOCAL: verified initial rebuild login block, rebuild restart/progress, tournament locks, XML guards, scanner visibility and boundary destruction for cluster #{testClusterId}.");
        }
        finally
        {
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
                        Console.WriteLine($"STATIC-MAP-LOCAL: cleanup CancelTournament failed: {exception.Message}");
                    }

                if (restoreGameMode != adminGalaxy.GameMode && adminGalaxy.Tournament is null)
                    try
                    {
                        await adminGalaxy.Configure(BuildGameModeConfigurationXml(adminGalaxy, restoreGameMode)).ConfigureAwait(false);
                    }
                    catch (GameException exception)
                    {
                        Console.WriteLine($"STATIC-MAP-LOCAL: cleanup game mode restore failed: {exception.Message}");
                    }

                if (restoreConfigurationXml is not null)
                    try
                    {
                        await adminGalaxy.Configure(restoreConfigurationXml).ConfigureAwait(false);
                    }
                    catch (GameException exception)
                    {
                        Console.WriteLine($"STATIC-MAP-LOCAL: cleanup configuration restore failed: {exception.Message}");
                    }

                if (restoreRegionsByCluster is not null)
                    try
                    {
                        await RestoreRegionsByCluster(adminGalaxy, restoreRegionsByCluster).ConfigureAwait(false);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine($"STATIC-MAP-LOCAL: cleanup region restore failed: {exception.Message}");
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

    private static Process StartLocalGalaxyProcess()
    {
        ProcessStartInfo startInfo;

        if (File.Exists(LocalGalaxyExecutablePath))
            startInfo = new ProcessStartInfo(LocalGalaxyExecutablePath, $"{StaticMapLocalGalaxyId}");
        else
            startInfo = new ProcessStartInfo("dotnet", $"run --project \"{LocalGalaxyProjectPath}\" -- {StaticMapLocalGalaxyId}");

        startInfo.WorkingDirectory = LocalGalaxyWorkingDirectory;
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;

        Process? process = Process.Start(startInfo);

        if (process is null)
            throw new InvalidOperationException("STATIC-MAP-LOCAL: failed to start the local galaxy process.");

        return process;
    }

    private static async Task<(Galaxy Galaxy, bool SawRebuildDenial)> ConnectLocalAdminAfterInitialRebuild(Process galaxyProcess, string auth)
    {
        DateTime deadline = DateTime.UtcNow.AddMilliseconds(InitialStaticMapTimeoutMs);
        bool sawInitialRebuildDenial = false;

        while (DateTime.UtcNow < deadline)
        {
            if (galaxyProcess.HasExited)
                throw new InvalidOperationException($"STATIC-MAP-LOCAL: local galaxy process exited early with code {galaxyProcess.ExitCode}.");

            try
            {
                Galaxy galaxy = await Galaxy.Connect(LocalSwitchGateUri, auth, null).ConfigureAwait(false);
                Console.WriteLine(sawInitialRebuildDenial
                    ? "STATIC-MAP-LOCAL: initial rebuild denial observed and galaxy is now ready."
                    : "STATIC-MAP-LOCAL: galaxy became ready before the initial rebuild login block could be observed.");
                return (galaxy, sawInitialRebuildDenial);
            }
            catch (StaticMapRebuildInProgressGameException)
            {
                sawInitialRebuildDenial = true;
            }
            catch (AccountAlreadyLoggedInGameException)
            {
                ClearLocalAccountSession(auth, "STATIC-MAP-LOCAL:ADMIN");
            }
            catch (CantConnectGameException)
            {
            }

            await Task.Delay(100).ConfigureAwait(false);
        }

        throw new TimeoutException("STATIC-MAP-LOCAL: local galaxy 666 did not become ready in time.");
    }

    private static async Task PrepareHeavyInitialRebuildScenario(Galaxy adminGalaxy)
    {
        byte heavyClusterId;

        if (!TryGetUnusedClusterId(adminGalaxy, 255, out heavyClusterId))
            throw new InvalidOperationException("STATIC-MAP-LOCAL: no free cluster id available for the heavy startup scenario.");

        string heavyClusterName = $"StaticMapInitialHeavy{Environment.ProcessId}";
        await adminGalaxy.Configure(BuildStaticMapTestConfigurationXml(adminGalaxy, heavyClusterId, heavyClusterName)).ConfigureAwait(false);

        if (!await WaitForCondition(delegate
            {
                return adminGalaxy.Clusters.TryGet(heavyClusterId, out Cluster? _);
            }, 5000).ConfigureAwait(false))
            throw new InvalidOperationException("STATIC-MAP-LOCAL: heavy startup cluster did not appear after configure.");

        Cluster heavyCluster = adminGalaxy.Clusters[heavyClusterId];

        for (int yIndex = 0; yIndex < 5; yIndex++)
            for (int xIndex = 0; xIndex < 5; xIndex++)
            {
                float x = -4000f + xIndex * 2000f;
                float y = -4000f + yIndex * 2000f;
                string unitName = $"StaticMapInitialHeavy{Environment.ProcessId}_{xIndex}_{yIndex}";
                string sunXml =
                    $"<Sun Name=\"{unitName}\" X=\"{x:0}\" Y=\"{y:0}\" Radius=\"40\" Gravity=\"0.03\" Energy=\"1.8\" Ions=\"1.25\" Neutrinos=\"0.6\" Heat=\"0.35\" Drain=\"0.18\" />";

                await heavyCluster.SetUnit(sunXml).ConfigureAwait(false);
            }
    }

    private static string BuildStaticMapTestConfigurationXml(Galaxy galaxy, byte testClusterId, string testClusterName)
    {
        List<ClusterSpec> clusters = new List<ClusterSpec>();

        foreach (Cluster cluster in galaxy.Clusters)
            clusters.Add(new ClusterSpec(cluster.Id, cluster.Name, false, false));

        clusters.Add(new ClusterSpec(testClusterId, testClusterName, true, true));
        return BuildConfigurationXml(galaxy, clusters.ToArray());
    }

    private static async Task ExpectTooLargeRadiusRejected(Cluster cluster)
    {
        string xml = $"<Buoy Name=\"StaticMapTooLarge{Environment.ProcessId}\" X=\"50\" Y=\"50\" Radius=\"1001\" Gravity=\"0\" Message=\"TooLarge\" />";
        bool rejected = false;

        try
        {
            await cluster.SetUnit(xml).ConfigureAwait(false);
        }
        catch (InvalidXmlNodeValueGameException exception)
        {
            rejected = exception.NodePath == "Buoy.Radius";
        }

        if (!rejected)
            throw new InvalidOperationException("STATIC-MAP-LOCAL: Buoy radius > 1000 was not rejected with Buoy.Radius.");
    }

    private static async Task ExpectOuterSegmentCenterRejected(Cluster cluster)
    {
        string xml = $"<Buoy Name=\"StaticMapOuter{Environment.ProcessId}\" X=\"-124500\" Y=\"0\" Radius=\"10\" Gravity=\"0\" Message=\"Outer\" />";
        bool rejected = false;

        try
        {
            await cluster.SetUnit(xml).ConfigureAwait(false);
        }
        catch (InvalidXmlNodeValueGameException exception)
        {
            rejected = exception.NodePath == "Buoy.X";
        }

        if (!rejected)
            throw new InvalidOperationException("STATIC-MAP-LOCAL: outermost segment center was not rejected with Buoy.X.");
    }

    private static async Task VerifyRebuildRestartAndPlayerProgress(Galaxy adminGalaxy, ConcurrentQueue<FlattiverseEvent> playerEvents)
    {
        Console.WriteLine("STATIC-MAP-LOCAL: triggering rebuild and verifying restart/progress on player ticks...");
        DrainEvents(playerEvents);
        await adminGalaxy.RebuildStaticMap().ConfigureAwait(false);

        List<FlattiverseEvent> firstProgressEvents = new List<FlattiverseEvent>();
        GalaxyTickEvent? firstProgressTick = await WaitForQueuedEvent(playerEvents, StaticMapRebuildTimeoutMs, firstProgressEvents,
            delegate(GalaxyTickEvent @event)
            {
                return @event.RemainingStaticSegments > 0;
            }).ConfigureAwait(false);

        if (firstProgressTick is null)
            throw new TimeoutException("STATIC-MAP-LOCAL: rebuild never exposed an initial positive remaining segment count.");

        ValidateTick(firstProgressTick, "STATIC-MAP-LOCAL:player-rebuild-start");
        int expectedRemainingSegments = firstProgressTick.RemainingStaticSegments;

        List<FlattiverseEvent> decreasedEvents = new List<FlattiverseEvent>();
        GalaxyTickEvent? decreasedTick = await WaitForQueuedEvent(playerEvents, StaticMapRebuildTimeoutMs, decreasedEvents,
            delegate(GalaxyTickEvent @event)
            {
                return @event.RemainingStaticSegments > 0 && @event.RemainingStaticSegments < expectedRemainingSegments;
            }).ConfigureAwait(false);

        if (decreasedTick is null)
            throw new TimeoutException("STATIC-MAP-LOCAL: rebuild progress never decreased before the restart test.");

        ValidateTick(decreasedTick, "STATIC-MAP-LOCAL:player-rebuild-progress");
        DrainEvents(playerEvents);
        await adminGalaxy.RebuildStaticMap().ConfigureAwait(false);

        List<FlattiverseEvent> restartedEvents = new List<FlattiverseEvent>();
        GalaxyTickEvent? restartedTick = await WaitForQueuedEvent(playerEvents, StaticMapRebuildTimeoutMs, restartedEvents,
            delegate(GalaxyTickEvent @event)
            {
                return @event.RemainingStaticSegments > decreasedTick.RemainingStaticSegments;
            }).ConfigureAwait(false);

        if (restartedTick is null)
            throw new TimeoutException("STATIC-MAP-LOCAL: rebuild restart did not increase the remaining segment count again.");

        ValidateTick(restartedTick, "STATIC-MAP-LOCAL:player-rebuild-restarted");
        await WaitForStaticRebuildFinished(playerEvents, "STATIC-MAP-LOCAL:player-rebuild-finished").ConfigureAwait(false);
    }

    private static async Task VerifyTournamentCommandsRejectedDuringRebuild(Galaxy adminGalaxy, ConcurrentQueue<FlattiverseEvent> playerEvents,
        TournamentConfiguration tournamentConfiguration)
    {
        Console.WriteLine("STATIC-MAP-LOCAL: triggering rebuild and verifying tournament command lock...");
        DrainEvents(playerEvents);
        await adminGalaxy.RebuildStaticMap().ConfigureAwait(false);

        List<FlattiverseEvent> progressEvents = new List<FlattiverseEvent>();
        GalaxyTickEvent? progressTick = await WaitForQueuedEvent(playerEvents, StaticMapRebuildTimeoutMs, progressEvents,
            delegate(GalaxyTickEvent @event)
            {
                return @event.RemainingStaticSegments > 0;
            }).ConfigureAwait(false);

        if (progressTick is null)
            throw new TimeoutException("STATIC-MAP-LOCAL: rebuild did not start before tournament lock verification.");

        ValidateTick(progressTick, "STATIC-MAP-LOCAL:admin-tournament-lock-progress");
        await ExpectRebuildInProgressException(delegate
            {
                return adminGalaxy.ConfigureTournament(tournamentConfiguration);
            }, "ConfigureTournament").ConfigureAwait(false);
        await ExpectRebuildInProgressException(adminGalaxy.CommenceTournament, "CommenceTournament").ConfigureAwait(false);
        await ExpectRebuildInProgressException(adminGalaxy.StartTournament, "StartTournament").ConfigureAwait(false);
        await WaitForStaticRebuildFinished(playerEvents, "STATIC-MAP-LOCAL:admin-tournament-lock-finished").ConfigureAwait(false);
    }

    private static async Task WaitForStaticRebuildFinished(ConcurrentQueue<FlattiverseEvent> events, string label)
    {
        List<FlattiverseEvent> finishEvents = new List<FlattiverseEvent>();
        GalaxyTickEvent? finishedTick = await WaitForQueuedEvent(events, StaticMapRebuildTimeoutMs, finishEvents,
            delegate(GalaxyTickEvent @event)
            {
                return @event.RemainingStaticSegments == 0;
            }).ConfigureAwait(false);

        if (finishedTick is null)
            throw new TimeoutException($"{label}: rebuild did not finish in time.");

        ValidateTick(finishedTick, label);
    }

    private static async Task ExpectRebuildInProgressException(Func<Task> action, string label)
    {
        bool rejected = false;

        try
        {
            await action().ConfigureAwait(false);
        }
        catch (StaticMapRebuildInProgressGameException)
        {
            rejected = true;
        }

        if (!rejected)
            throw new InvalidOperationException($"STATIC-MAP-LOCAL: {label} unexpectedly succeeded during static-map rebuild.");
    }

    private static void ValidateTick(GalaxyTickEvent tickEvent, string label)
    {
        if (tickEvent.ScanMs < 0f || tickEvent.SteadyMs < 0f || tickEvent.GravityMs < 0f || tickEvent.EnginesMs < 0f ||
            tickEvent.LimitMs < 0f || tickEvent.MovementMs < 0f || tickEvent.CollisionsMs < 0f || tickEvent.ActionsMs < 0f ||
            tickEvent.VisibilityMs < 0f || tickEvent.TotalMs < 0f)
            throw new InvalidOperationException($"{label}: received a negative timing value in the tick profile.");
    }
}
