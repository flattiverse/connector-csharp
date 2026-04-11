using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Xml.Linq;
using Flattiverse.Connector;
using Flattiverse.Connector.Events;
using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Units;

namespace Development;

partial class Program
{
    private const int NpcUnitsLocalTimeoutMs = 15000;
    private const int NpcUnitsLocalScoreTimeoutMs = 20000;
    private const float NpcUnitsLocalShotSpeedLimit = 10f;
    private const float NpcUnitsLocalInterceptorSpeedLimit = 10f;
    private const float NpcUnitsLocalRailSpeedLimit = 15f;

    private static async Task RunNpcUnitsCheckLocal()
    {
        CultureInfo originalCulture = CultureInfo.CurrentCulture;
        CultureInfo originalUiCulture = CultureInfo.CurrentUICulture;
        string adminAuth = ResolveNpcUnitsLocalAdminAuth();
        DatabaseAccountRow originalAdminAccount = QueryAccountRow(adminAuth);
        string playerAuth = ResolveNpcUnitsLocalPlayerAuth(originalAdminAccount.AccountId);
        DatabaseAccountRow originalPlayerAccount = QueryAccountRow(playerAuth);
        Process? galaxyProcess = null;
        Galaxy? adminGalaxy = null;
        Galaxy? playerGalaxy = null;
        Galaxy? spectatorGalaxy = null;
        Task? adminEventPump = null;
        Task? playerEventPump = null;
        Task? spectatorEventPump = null;
        ConcurrentQueue<FlattiverseEvent> adminEvents = new ConcurrentQueue<FlattiverseEvent>();
        ConcurrentQueue<FlattiverseEvent> playerEvents = new ConcurrentQueue<FlattiverseEvent>();
        ConcurrentQueue<FlattiverseEvent> spectatorEvents = new ConcurrentQueue<FlattiverseEvent>();
        string? restoreConfigurationXml = null;
        Dictionary<byte, string>? restoreRegionsByCluster = null;
        GameMode restoreGameMode = GameMode.Mission;
        byte testClusterId = 0;
        string clusterName = $"NpcUnitsLocal{Environment.ProcessId}";
        string regionName = $"NpcUnitsLocalRegion{Environment.ProcessId}";
        string shipName = $"NpcUnitsLocalShip{Environment.ProcessId}";
        float anchorX = 0f;
        float anchorY = 0f;

        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;

        ClearLocalAccountSession(adminAuth, "NPC-LOCAL:ADMIN");
        ClearLocalAccountSession(playerAuth, "NPC-LOCAL:PLAYER");

        try
        {
            Console.WriteLine("NPC-LOCAL: starting local galaxy 666...");
            galaxyProcess = StartLocalGalaxyProcess();
            (adminGalaxy, _) = await ConnectLocalAdminAfterInitialRebuild(galaxyProcess, adminAuth).ConfigureAwait(false);
            adminEventPump = StartEventPump("NPC-LOCAL:ADMIN", adminGalaxy, adminEvents);
            DrainEvents(adminEvents);

            restoreConfigurationXml = BuildConfigurationXml(adminGalaxy, (ClusterSpec[]?)null);
            restoreRegionsByCluster = await CaptureRegionsByCluster(adminGalaxy).ConfigureAwait(false);
            restoreGameMode = adminGalaxy.GameMode;

            if (adminGalaxy.Tournament is not null)
            {
                Console.WriteLine("NPC-LOCAL: cancelling pre-existing tournament...");
                await adminGalaxy.CancelTournament().ConfigureAwait(false);
                await Task.Delay(250).ConfigureAwait(false);
                DrainEvents(adminEvents);
            }

            if (adminGalaxy.GameMode != GameMode.Mission)
            {
                Console.WriteLine("NPC-LOCAL: switching galaxy 666 to Mission...");
                await adminGalaxy.Configure(BuildGameModeConfigurationXml(adminGalaxy, GameMode.Mission)).ConfigureAwait(false);

                if (!await WaitForCondition(delegate { return adminGalaxy.GameMode == GameMode.Mission; }, 5000).ConfigureAwait(false))
                    throw new InvalidOperationException("NPC-LOCAL: galaxy did not switch to Mission.");
            }

            if (!TryGetUnusedClusterId(adminGalaxy, 255, out testClusterId))
                throw new InvalidOperationException("NPC-LOCAL: no free cluster id available.");

            Console.WriteLine($"NPC-LOCAL: configuring dedicated cluster #{testClusterId}...");
            await adminGalaxy.Configure(BuildNpcUnitsLocalConfigurationXml(adminGalaxy, testClusterId, clusterName)).ConfigureAwait(false);

            if (!await WaitForCondition(delegate
                {
                    return adminGalaxy.Clusters.TryGet(testClusterId, out Cluster? _);
                }, 5000).ConfigureAwait(false))
                throw new InvalidOperationException("NPC-LOCAL: dedicated cluster did not appear after configure.");

            await RemoveAllRegions(adminGalaxy).ConfigureAwait(false);

            Cluster adminCluster = adminGalaxy.Clusters[testClusterId];

            if (!TryGetTeamByName(adminGalaxy, TeamName, out Team? pinkTeam) || pinkTeam is null)
                throw new InvalidOperationException($"NPC-LOCAL: team {TeamName} not found.");

            if (!TryGetTeamByName(adminGalaxy, LocalSwitchGateGreenTeamName, out Team? greenTeam) || greenTeam is null)
                throw new InvalidOperationException($"NPC-LOCAL: team {LocalSwitchGateGreenTeamName} not found.");

            if (!adminGalaxy.Teams.TryGet(SpectatorsTeamId, out Team? spectatorsTeam) || spectatorsTeam is null)
                throw new InvalidOperationException($"NPC-LOCAL: team {SpectatorsTeamId} not found.");

            byte regionId = await FindUnusedRegionId(adminCluster).ConfigureAwait(false);
            string regionXml =
                $"<Region Id=\"{regionId}\" Name=\"{regionName}\" Left=\"-40\" Top=\"-40\" Right=\"40\" Bottom=\"40\"><Team Id=\"{pinkTeam.Id}\" /></Region>";

            Console.WriteLine("NPC-LOCAL: creating start region...");
            await adminCluster.SetRegion(regionXml).ConfigureAwait(false);

            Console.WriteLine("NPC-LOCAL: connecting local player...");
            playerGalaxy = await ConnectLocalPlayer(playerAuth, TeamName, "NPC-LOCAL:PLAYER").ConfigureAwait(false);
            playerEventPump = StartEventPump("NPC-LOCAL:PLAYER", playerGalaxy, playerEvents);
            spectatorGalaxy = await Galaxy.Connect(LocalSwitchGateUri, null, null).ConfigureAwait(false);
            spectatorEventPump = StartEventPump("NPC-LOCAL:SPECTATOR", spectatorGalaxy, spectatorEvents);
            DrainEvents(playerEvents);
            DrainEvents(spectatorEvents);

            ClassicShipControllable ship = await playerGalaxy.CreateClassicShip(shipName).ConfigureAwait(false);
            await ship.Continue().ConfigureAwait(false);

            if (!await WaitForAliveState(ship, true, 10000).ConfigureAwait(false))
                throw new InvalidOperationException("NPC-LOCAL: ship did not become alive after Continue().");

            if (!await WaitForCondition(delegate { return ship.Cluster.Id == testClusterId; }, 10000).ConfigureAwait(false))
                throw new InvalidOperationException("NPC-LOCAL: ship did not spawn in the dedicated cluster.");

            float scannerWidth = MathF.Min(ship.MainScanner.MaximumWidth, 90f);
            float scannerLength = MathF.Min(ship.MainScanner.MaximumLength, 220f);
            await ship.MainScanner.Set(scannerWidth, scannerLength, 0f).ConfigureAwait(false);
            await ship.MainScanner.On().ConfigureAwait(false);

            if (!await WaitForCondition(delegate
                {
                    return ship.MainScanner.Active &&
                           MathF.Abs(ship.MainScanner.CurrentWidth - scannerWidth) < 0.1f &&
                           MathF.Abs(ship.MainScanner.CurrentLength - scannerLength) < 0.1f;
                }, 5000).ConfigureAwait(false))
                throw new InvalidOperationException("NPC-LOCAL: main scanner did not reach its configured active state.");

            await Task.Delay(500).ConfigureAwait(false);
            DrainEvents(playerEvents);
            DrainEvents(spectatorEvents);

            string initialJellyName = $"NpcLocalJelly{Environment.ProcessId}";
            string initialBaseName = $"NpcLocalBase{Environment.ProcessId}";
            string initialTurretName = $"NpcLocalTurret{Environment.ProcessId}";
            string initialFreighterName = $"NpcLocalFreighter{Environment.ProcessId}";
            string initialShipName = $"NpcLocalAiShip{Environment.ProcessId}";
            string initialSpectatorProbeName = $"NpcLocalSpectatorProbe{Environment.ProcessId}";
            string initialEnemyProbeName = $"NpcLocalEnemyProbe{Environment.ProcessId}";
            float initialShipPatrolX = anchorX + 165f;
            float initialShipPatrolY = anchorY + 10f;
            float initialShipActionRadius = 60f;
            float initialJellyGravity = 0.0015f;
            float initialBaseGravity = 0.0006f;
            float initialTurretGravity = 0.0004f;
            float initialFreighterGravity = 0.0011f;
            float initialShipGravity = 0.0013f;
            float initialSpectatorProbeGravity = 0.0014f;
            float initialEnemyProbeGravity = 0.0009f;

            Console.WriteLine("NPC-LOCAL: creating spectator-owned editable NPC units...");
            await adminCluster.SetUnit(
                $"<SpaceJellyFish Name=\"{initialJellyName}\" Team=\"{spectatorsTeam.Id}\" X=\"{anchorX + 120f:0.###}\" Y=\"{anchorY:0.###}\" Radius=\"14\" Gravity=\"{initialJellyGravity:R}\" Hull=\"26\" RepairPerTick=\"0.01\" RespawnTicks=\"600\" RespawnPlayerDistance=\"180\" ActionRadius=\"60\" Speed=\"3.2\" Damage=\"0.1\" />")
                .ConfigureAwait(false);
            await adminCluster.SetUnit(
                $"<AiBase Name=\"{initialBaseName}\" Team=\"{spectatorsTeam.Id}\" X=\"{anchorX + 145f:0.###}\" Y=\"{anchorY + 35f:0.###}\" Radius=\"22\" Gravity=\"{initialBaseGravity:R}\" Hull=\"60\" RepairPerTick=\"0.02\" RespawnTicks=\"600\" RespawnPlayerDistance=\"180\" RailSpeed=\"7.5\" RailReloadTicks=\"8\" InterceptorSpeed=\"6.2\" InterceptorReloadTicks=\"8\" />")
                .ConfigureAwait(false);
            await adminCluster.SetUnit(
                $"<AiTurret Name=\"{initialTurretName}\" Team=\"{spectatorsTeam.Id}\" X=\"{anchorX + 145f:0.###}\" Y=\"{anchorY - 35f:0.###}\" Radius=\"16\" Gravity=\"{initialTurretGravity:R}\" Hull=\"28\" RepairPerTick=\"0.01\" RespawnTicks=\"600\" RespawnPlayerDistance=\"180\" ShotSpeed=\"5.0\" ShotDamage=\"0.1\" />")
                .ConfigureAwait(false);
            await adminCluster.SetUnit(BuildInitialFreighterXml(initialFreighterName, spectatorsTeam.Id, anchorX + 185f, anchorY,
                600, 180f, initialFreighterGravity, 7f, 16f, 6f, 3f, 40f)).ConfigureAwait(false);
            await adminCluster.SetUnit(
                $"<AiShip Name=\"{initialShipName}\" Team=\"{spectatorsTeam.Id}\" X=\"{initialShipPatrolX:0.###}\" Y=\"{initialShipPatrolY:0.###}\" Radius=\"12\" Gravity=\"{initialShipGravity:R}\" Hull=\"24\" RepairPerTick=\"0.01\" RespawnTicks=\"600\" RespawnPlayerDistance=\"180\" ActionRadius=\"{initialShipActionRadius:0.###}\" Speed=\"3.2\" ShotSpeed=\"5.2\" ShotDamage=\"0.1\" />")
                .ConfigureAwait(false);
            await adminCluster.SetUnit(
                $"<AiProbe Name=\"{initialSpectatorProbeName}\" Team=\"{spectatorsTeam.Id}\" X=\"{anchorX + 195f:0.###}\" Y=\"{anchorY - 15f:0.###}\" Radius=\"11\" Gravity=\"{initialSpectatorProbeGravity:R}\" Hull=\"18\" RepairPerTick=\"0.01\" RespawnTicks=\"600\" RespawnPlayerDistance=\"0\" ActionRadius=\"50\" Speed=\"3.0\" />")
                .ConfigureAwait(false);
            await adminCluster.SetUnit(
                $"<AiProbe Name=\"{initialEnemyProbeName}\" Team=\"{greenTeam.Id}\" X=\"{anchorX + 195f:0.###}\" Y=\"{anchorY + 15f:0.###}\" Radius=\"11\" Gravity=\"{initialEnemyProbeGravity:R}\" Hull=\"18\" RepairPerTick=\"0.01\" RespawnTicks=\"600\" RespawnPlayerDistance=\"0\" ActionRadius=\"50\" Speed=\"3.0\" />")
                .ConfigureAwait(false);

            UnitSpec[] expectedEditableUnits = new UnitSpec[]
            {
                new UnitSpec(testClusterId, initialJellyName, UnitKind.SpaceJellyFish),
                new UnitSpec(testClusterId, initialBaseName, UnitKind.AiBase),
                new UnitSpec(testClusterId, initialTurretName, UnitKind.AiTurret),
                new UnitSpec(testClusterId, initialFreighterName, UnitKind.AiFreighter),
                new UnitSpec(testClusterId, initialShipName, UnitKind.AiShip),
                new UnitSpec(testClusterId, initialSpectatorProbeName, UnitKind.AiProbe),
                new UnitSpec(testClusterId, initialEnemyProbeName, UnitKind.AiProbe)
            };

            EditableUnitSummary[] editableUnits = await adminCluster.QueryEditableUnits().ConfigureAwait(false);

            foreach (UnitSpec unitSpec in expectedEditableUnits)
            {
                bool found = false;

                for (int index = 0; index < editableUnits.Length; index++)
                    if (editableUnits[index].Name == unitSpec.Name && editableUnits[index].Kind == unitSpec.Kind)
                    {
                        found = true;
                        break;
                    }

                if (!found)
                    throw new InvalidOperationException($"NPC-LOCAL: editable unit {unitSpec.Name} ({unitSpec.Kind}) was not returned by QueryEditableUnits().");
            }

            VerifyNpcUnitXml(await adminCluster.QueryUnitXml(initialJellyName).ConfigureAwait(false), "SpaceJellyFish", SpectatorsTeamId,
                initialJellyGravity, 0);
            VerifyNpcUnitXml(await adminCluster.QueryUnitXml(initialBaseName).ConfigureAwait(false), "AiBase", SpectatorsTeamId,
                initialBaseGravity, 0);
            VerifyNpcUnitXml(await adminCluster.QueryUnitXml(initialTurretName).ConfigureAwait(false), "AiTurret", SpectatorsTeamId,
                initialTurretGravity, 0);
            VerifyNpcUnitXml(await adminCluster.QueryUnitXml(initialFreighterName).ConfigureAwait(false), "AiFreighter", SpectatorsTeamId,
                initialFreighterGravity, 2);
            string initialShipXml = await adminCluster.QueryUnitXml(initialShipName).ConfigureAwait(false);
            VerifyNpcUnitXml(initialShipXml, "AiShip", SpectatorsTeamId, initialShipGravity, 0);
            VerifyNpcUnitXml(await adminCluster.QueryUnitXml(initialSpectatorProbeName).ConfigureAwait(false), "AiProbe", SpectatorsTeamId,
                initialSpectatorProbeGravity, 0);
            VerifyNpcUnitXml(await adminCluster.QueryUnitXml(initialEnemyProbeName).ConfigureAwait(false), "AiProbe", greenTeam.Id,
                initialEnemyProbeGravity, 0);

            XDocument initialShipDocument = XDocument.Parse(initialShipXml, LoadOptions.None);
            XElement? initialShipRoot = initialShipDocument.Root;

            if (initialShipRoot is null ||
                !float.TryParse(initialShipRoot.Attribute("X")?.Value, NumberStyles.Float, CultureInfo.InvariantCulture,
                    out float queriedInitialShipPatrolX) ||
                !float.TryParse(initialShipRoot.Attribute("Y")?.Value, NumberStyles.Float, CultureInfo.InvariantCulture,
                    out float queriedInitialShipPatrolY) ||
                MathF.Abs(queriedInitialShipPatrolX - initialShipPatrolX) > 0.0001f ||
                MathF.Abs(queriedInitialShipPatrolY - initialShipPatrolY) > 0.0001f)
            {
                string queriedInitialShipPatrolXText = initialShipRoot?.Attribute("X")?.Value ?? "<missing>";
                string queriedInitialShipPatrolYText = initialShipRoot?.Attribute("Y")?.Value ?? "<missing>";
                throw new InvalidOperationException(
                    $"NPC-LOCAL: ai ship XML did not preserve the patrol center. x={queriedInitialShipPatrolXText}, y={queriedInitialShipPatrolYText}.");
            }

            if (!await WaitForCondition(delegate
                {
                    return spectatorGalaxy is not null &&
                           TryFindUnit<SpaceJellyFish>(spectatorGalaxy, testClusterId, initialJellyName, out SpaceJellyFish? _) &&
                           TryFindUnit<AiBase>(spectatorGalaxy, testClusterId, initialBaseName, out AiBase? _) &&
                           TryFindUnit<AiTurret>(spectatorGalaxy, testClusterId, initialTurretName, out AiTurret? _) &&
                           TryFindUnit<AiFreighter>(spectatorGalaxy, testClusterId, initialFreighterName, out AiFreighter? _) &&
                           TryFindUnit<AiShip>(spectatorGalaxy, testClusterId, initialShipName, out AiShip? _);
                }, NpcUnitsLocalTimeoutMs).ConfigureAwait(false))
            {
                List<string> visibleUnits = new List<string>();

                if (spectatorGalaxy is not null && spectatorGalaxy.Clusters.TryGet(testClusterId, out Cluster? visibleCluster) &&
                    visibleCluster is not null)
                    foreach (Unit visibleUnit in visibleCluster.Units)
                        visibleUnits.Add($"{visibleUnit.Kind}:{visibleUnit.Name}");

                throw new InvalidOperationException(
                    $"NPC-LOCAL: spectator did not see all new NPC unit classes. visible=[{string.Join(", ", visibleUnits)}]");
            }

            if (!TryFindUnit<AiTurret>(spectatorGalaxy, testClusterId, initialTurretName, out AiTurret? initialVisibleTurret) ||
                initialVisibleTurret is null ||
                initialVisibleTurret.Team is null ||
                initialVisibleTurret.Team.Id != SpectatorsTeamId)
            {
                string transmittedTeamId = initialVisibleTurret?.Team is null ? "<null>" : initialVisibleTurret.Team.Id.ToString(CultureInfo.InvariantCulture);
                throw new InvalidOperationException($"NPC-LOCAL: spectator ai turret team was not transmitted correctly. team={transmittedTeamId}.");
            }

            if (!TryFindUnit<AiShip>(spectatorGalaxy, testClusterId, initialShipName, out AiShip? initialVisibleAiShip) ||
                initialVisibleAiShip is null ||
                initialVisibleAiShip.Team is null ||
                initialVisibleAiShip.Team.Id != SpectatorsTeamId)
            {
                string transmittedTeamId = initialVisibleAiShip?.Team is null ? "<null>" : initialVisibleAiShip.Team.Id.ToString(CultureInfo.InvariantCulture);
                throw new InvalidOperationException($"NPC-LOCAL: spectator ai ship team was not transmitted correctly. team={transmittedTeamId}.");
            }

            if (!TryFindUnit<AiFreighter>(spectatorGalaxy, testClusterId, initialFreighterName, out AiFreighter? initialVisibleFreighter) ||
                initialVisibleFreighter is null ||
                initialVisibleFreighter.Metal != 7f ||
                initialVisibleFreighter.Carbon != 16f ||
                initialVisibleFreighter.Hydrogen != 6f ||
                initialVisibleFreighter.Silicon != 3f)
                throw new InvalidOperationException("NPC-LOCAL: freighter loot scan values were not transmitted correctly.");

            foreach (string unitName in new string[]
                     {
                         initialJellyName,
                         initialBaseName,
                         initialTurretName,
                         initialFreighterName,
                         initialShipName,
                         initialSpectatorProbeName,
                         initialEnemyProbeName
                     })
                await RemoveUnitIfPresent(adminCluster, unitName).ConfigureAwait(false);

            if (spectatorGalaxy is not null)
                await WaitForNpcUnitsLocalTransientUnitsClear(spectatorGalaxy, testClusterId).ConfigureAwait(false);

            await EnsureNpcUnitsLocalShipAlive(ship, testClusterId).ConfigureAwait(false);

            Console.WriteLine("NPC-LOCAL: observing space jellyfish slime behavior...");
            string jellyPhaseName = $"NpcLocalJellyPhase{Environment.ProcessId}";
            DrainEvents(spectatorEvents);
            await adminCluster.SetUnit(
                $"<SpaceJellyFish Name=\"{jellyPhaseName}\" Team=\"{spectatorsTeam.Id}\" X=\"{anchorX + 80f:0.###}\" Y=\"{anchorY:0.###}\" Radius=\"14\" Gravity=\"0.0012\" Hull=\"26\" RepairPerTick=\"0\" RespawnTicks=\"0\" RespawnPlayerDistance=\"180\" ActionRadius=\"30\" Speed=\"3.2\" Damage=\"0.5\" />")
                .ConfigureAwait(false);

            List<FlattiverseEvent> jellyPhaseEvents = new List<FlattiverseEvent>();
            AppearedUnitEvent? jellySlimeEvent = await WaitForQueuedEvent(spectatorEvents, NpcUnitsLocalTimeoutMs, jellyPhaseEvents,
                delegate(AppearedUnitEvent @event)
                {
                    return @event.Unit is SpaceJellyFishSlime;
                }).ConfigureAwait(false);

            if (jellySlimeEvent is null)
            {
                List<string> visibleUnits = new List<string>();

                if (spectatorGalaxy is not null && spectatorGalaxy.Clusters.TryGet(testClusterId, out Cluster? visibleCluster) && visibleCluster is not null)
                    foreach (Unit visibleUnit in visibleCluster.Units)
                        visibleUnits.Add($"{visibleUnit.Kind}:{visibleUnit.Name}");

                throw new InvalidOperationException($"NPC-LOCAL: jellyfish never produced a visible slime. visible=[{string.Join(", ", visibleUnits)}]");
            }

            if (!await WaitForCondition(delegate
                {
                    if (spectatorGalaxy is null || !spectatorGalaxy.Clusters.TryGet(testClusterId, out Cluster? cluster) || cluster is null)
                        return false;

                    foreach (Unit unit in cluster.Units)
                        if (unit is SpaceJellyFishSlime slime &&
                            slime.TargetClusterId == testClusterId &&
                            slime.TargetUnitName == ship.Name &&
                            slime.TargetUnitKind == ship.Kind)
                            return true;

                    return false;
                }, NpcUnitsLocalTimeoutMs).ConfigureAwait(false))
                throw new InvalidOperationException("NPC-LOCAL: no slime with the expected target reference became visible.");

            await RemoveUnitIfPresent(adminCluster, jellyPhaseName).ConfigureAwait(false);

            if (!await WaitForCondition(delegate
                {
                    if (spectatorGalaxy is null || !spectatorGalaxy.Clusters.TryGet(testClusterId, out Cluster? cluster) || cluster is null)
                        return true;

                    foreach (Unit unit in cluster.Units)
                        if (unit is SpaceJellyFishSlime)
                            return false;

                    return true;
                }, NpcUnitsLocalTimeoutMs).ConfigureAwait(false))
                throw new InvalidOperationException("NPC-LOCAL: jellyfish slime did not clear before the next phase.");

            await EnsureNpcUnitsLocalShipAlive(ship, testClusterId).ConfigureAwait(false);

            Console.WriteLine("NPC-LOCAL: observing turret shots...");
            string turretPhaseName = $"NpcLocalTurretPhase{Environment.ProcessId}";
            await adminCluster.SetUnit(
                $"<AiTurret Name=\"{turretPhaseName}\" Team=\"{spectatorsTeam.Id}\" X=\"{anchorX + 80f:0.###}\" Y=\"{anchorY:0.###}\" Radius=\"16\" Gravity=\"0\" Hull=\"30\" RepairPerTick=\"0\" RespawnTicks=\"0\" RespawnPlayerDistance=\"180\" ShotSpeed=\"5.2\" ShotDamage=\"0.2\" />")
                .ConfigureAwait(false);
            DrainEvents(spectatorEvents);

            List<FlattiverseEvent> turretPhaseEvents = new List<FlattiverseEvent>();
            AppearedUnitEvent? turretShotEvent = await WaitForQueuedEvent(spectatorEvents, NpcUnitsLocalTimeoutMs, turretPhaseEvents,
                delegate(AppearedUnitEvent @event)
                {
                    return @event.Unit is Shot;
                }).ConfigureAwait(false);

            if (turretShotEvent is null)
            {
                List<string> visibleUnits = new List<string>();

                if (spectatorGalaxy is not null && spectatorGalaxy.Clusters.TryGet(testClusterId, out Cluster? visibleCluster) && visibleCluster is not null)
                    foreach (Unit visibleUnit in visibleCluster.Units)
                        visibleUnits.Add($"{visibleUnit.Kind}:{visibleUnit.Name}");

                throw new InvalidOperationException(
                    $"NPC-LOCAL: turret never produced a visible shot. shipAlive={ship.Alive}, shipCluster={ship.Cluster.Id}, shipPosition={ship.Position}, visible=[{string.Join(", ", visibleUnits)}]");
            }

            Shot turretShot = (Shot)turretShotEvent.Unit;

            if (MathF.Abs(turretShot.SpeedLimit - NpcUnitsLocalShotSpeedLimit) > 0.001f)
                throw new InvalidOperationException(
                    $"NPC-LOCAL: turret shot speed limit mismatch. Expected={NpcUnitsLocalShotSpeedLimit:0.###}, Actual={turretShot.SpeedLimit:0.###}.");

            await RemoveUnitIfPresent(adminCluster, turretPhaseName).ConfigureAwait(false);

            if (spectatorGalaxy is not null)
                await WaitForNpcUnitsLocalTransientUnitsClear(spectatorGalaxy, testClusterId).ConfigureAwait(false);

            await EnsureNpcUnitsLocalShipAlive(ship, testClusterId).ConfigureAwait(false);

            Console.WriteLine("NPC-LOCAL: observing ai ship shots...");
            string aiShipPhaseName = $"NpcLocalAiShipPhase{Environment.ProcessId}";
            DrainEvents(spectatorEvents);
            await adminCluster.SetUnit(
                $"<AiShip Name=\"{aiShipPhaseName}\" Team=\"{spectatorsTeam.Id}\" X=\"{anchorX + 80f:0.###}\" Y=\"{anchorY:0.###}\" Radius=\"12\" Gravity=\"0.0017\" Hull=\"24\" RepairPerTick=\"0\" RespawnTicks=\"0\" RespawnPlayerDistance=\"0\" ActionRadius=\"30\" Speed=\"3.2\" ShotSpeed=\"5.2\" ShotDamage=\"0.2\" />")
                .ConfigureAwait(false);

            List<FlattiverseEvent> aiShipPhaseEvents = new List<FlattiverseEvent>();
            AppearedUnitEvent? aiShipShotEvent = await WaitForQueuedEvent(spectatorEvents, NpcUnitsLocalTimeoutMs, aiShipPhaseEvents,
                delegate(AppearedUnitEvent @event)
                {
                    return @event.Unit is Shot;
                }).ConfigureAwait(false);

            if (aiShipShotEvent is null)
                throw new InvalidOperationException("NPC-LOCAL: ai ship never produced a visible shot.");

            Shot aiShipShot = (Shot)aiShipShotEvent.Unit;

            if (MathF.Abs(aiShipShot.SpeedLimit - NpcUnitsLocalShotSpeedLimit) > 0.001f)
                throw new InvalidOperationException(
                    $"NPC-LOCAL: ai ship shot speed limit mismatch. Expected={NpcUnitsLocalShotSpeedLimit:0.###}, Actual={aiShipShot.SpeedLimit:0.###}.");

            await RemoveUnitIfPresent(adminCluster, aiShipPhaseName).ConfigureAwait(false);

            if (spectatorGalaxy is not null)
                await WaitForNpcUnitsLocalTransientUnitsClear(spectatorGalaxy, testClusterId).ConfigureAwait(false);

            await EnsureNpcUnitsLocalShipAlive(ship, testClusterId).ConfigureAwait(false);

            Console.WriteLine("NPC-LOCAL: observing ai ship removal and respawn...");
            string aiShipRespawnName = $"NpcLocalAiShipRespawn{Environment.ProcessId}";
            string aiShipRespawnAttackerName = $"NpcLocalAiShipRespawnAttacker{Environment.ProcessId}";
            float aiShipRespawnPatrolX = anchorX + 108f;
            float aiShipRespawnPatrolY = anchorY + 24f;
            float aiShipRespawnActionRadius = 0f;
            DrainEvents(spectatorEvents);
            await adminCluster.SetUnit(
                $"<AiShip Name=\"{aiShipRespawnName}\" Team=\"{spectatorsTeam.Id}\" X=\"{aiShipRespawnPatrolX:0.###}\" Y=\"{aiShipRespawnPatrolY:0.###}\" Radius=\"12\" Gravity=\"0.0016\" Hull=\"2\" RepairPerTick=\"0\" RespawnTicks=\"12\" RespawnPlayerDistance=\"35\" ActionRadius=\"{aiShipRespawnActionRadius:0.###}\" Speed=\"3.2\" ShotSpeed=\"5.2\" ShotDamage=\"0.2\" />")
                .ConfigureAwait(false);

            List<FlattiverseEvent> aiShipRespawnEvents = new List<FlattiverseEvent>();
            AppearedUnitEvent? aiShipInitialSpawnEvent = await WaitForQueuedEvent(spectatorEvents, NpcUnitsLocalTimeoutMs,
                aiShipRespawnEvents, delegate(AppearedUnitEvent @event)
                {
                    return @event.Unit.Kind == UnitKind.AiShip && @event.Unit.Name == aiShipRespawnName;
                }).ConfigureAwait(false);

            if (aiShipInitialSpawnEvent is null)
                throw new InvalidOperationException("NPC-LOCAL: ai ship respawn phase unit never became visible.");

            if (spectatorGalaxy is null)
                throw new InvalidOperationException("NPC-LOCAL: spectator galaxy vanished before the ai ship respawn phase query.");

            if (!await WaitForCondition(delegate
                {
                    return TryFindUnit<AiShip>(spectatorGalaxy, testClusterId, aiShipRespawnName, out AiShip? visibleRespawnShip) &&
                           visibleRespawnShip is not null;
                }, NpcUnitsLocalTimeoutMs).ConfigureAwait(false))
            {
                throw new InvalidOperationException("NPC-LOCAL: ai ship respawn phase unit did not reach the spectator galaxy mirror.");
            }

            if (!TryFindUnit<AiShip>(spectatorGalaxy, testClusterId, aiShipRespawnName, out AiShip? aiShipBeforeDeath) || aiShipBeforeDeath is null)
                throw new InvalidOperationException("NPC-LOCAL: ai ship respawn phase unit could not be queried after it became visible.");

            Vector aiShipBeforeDeathOffset = aiShipInitialSpawnEvent.Unit.Position - new Vector(aiShipRespawnPatrolX, aiShipRespawnPatrolY);

            if (aiShipBeforeDeathOffset.Length > aiShipRespawnActionRadius + 0.1f)
            {
                throw new InvalidOperationException(
                    $"NPC-LOCAL: ai ship respawn phase initial spawn outside patrol radius. offset={aiShipBeforeDeathOffset.Length:0.###}, radius={aiShipRespawnActionRadius:0.###}, position={aiShipInitialSpawnEvent.Unit.Position}.");
            }

            DrainEvents(spectatorEvents);
            await adminCluster.SetUnit(
                $"<AiTurret Name=\"{aiShipRespawnAttackerName}\" Team=\"{pinkTeam.Id}\" X=\"{aiShipRespawnPatrolX:0.###}\" Y=\"{aiShipRespawnPatrolY - 60f:0.###}\" Radius=\"16\" Gravity=\"0\" Hull=\"30\" RepairPerTick=\"0\" RespawnTicks=\"0\" RespawnPlayerDistance=\"180\" ShotSpeed=\"5.4\" ShotDamage=\"60\" />")
                .ConfigureAwait(false);

            RemovedUnitEvent? aiShipRemovedEvent = await WaitForQueuedEvent(spectatorEvents, NpcUnitsLocalTimeoutMs,
                aiShipRespawnEvents, delegate(RemovedUnitEvent @event)
                {
                    return @event.Unit.Kind == UnitKind.AiShip && @event.Unit.Name == aiShipRespawnName;
                }).ConfigureAwait(false);

            if (aiShipRemovedEvent is null)
                throw new InvalidOperationException("NPC-LOCAL: ai ship death did not produce a spectator remove event.");

            if (!await WaitForCondition(delegate
                {
                    return spectatorGalaxy is not null &&
                           (!TryFindUnit<AiShip>(spectatorGalaxy, testClusterId, aiShipRespawnName, out AiShip? remainingAiShip) ||
                            remainingAiShip is null);
                }, NpcUnitsLocalTimeoutMs).ConfigureAwait(false))
            {
                throw new InvalidOperationException("NPC-LOCAL: dead ai ship remained visible after the remove event.");
            }

            AppearedUnitEvent? aiShipRespawnEvent = await WaitForQueuedEvent(spectatorEvents, NpcUnitsLocalTimeoutMs, aiShipRespawnEvents,
                delegate(AppearedUnitEvent @event)
                {
                    return @event.Unit.Kind == UnitKind.AiShip && @event.Unit.Name == aiShipRespawnName;
                }).ConfigureAwait(false);

            if (aiShipRespawnEvent is null)
                throw new InvalidOperationException("NPC-LOCAL: ai ship never became visible again after respawn.");

            if (!TryFindUnit<AiShip>(spectatorGalaxy, testClusterId, aiShipRespawnName, out AiShip? aiShipAfterRespawn) || aiShipAfterRespawn is null)
                throw new InvalidOperationException("NPC-LOCAL: respawned ai ship was not present in the spectator galaxy mirror.");

            Vector aiShipAfterRespawnOffset = aiShipRespawnEvent.Unit.Position - new Vector(aiShipRespawnPatrolX, aiShipRespawnPatrolY);

            if (aiShipAfterRespawnOffset.Length > aiShipRespawnActionRadius + 0.1f)
            {
                throw new InvalidOperationException(
                    $"NPC-LOCAL: respawned ai ship appeared outside patrol radius. offset={aiShipAfterRespawnOffset.Length:0.###}, radius={aiShipRespawnActionRadius:0.###}, position={aiShipRespawnEvent.Unit.Position}.");
            }

            await RemoveUnitIfPresent(adminCluster, aiShipRespawnAttackerName).ConfigureAwait(false);
            await RemoveUnitIfPresent(adminCluster, aiShipRespawnName).ConfigureAwait(false);

            if (spectatorGalaxy is not null)
                await WaitForNpcUnitsLocalTransientUnitsClear(spectatorGalaxy, testClusterId).ConfigureAwait(false);

            await EnsureNpcUnitsLocalShipAlive(ship, testClusterId).ConfigureAwait(false);

            Console.WriteLine("NPC-LOCAL: observing ai base rail and interceptor...");
            string basePhaseName = $"NpcLocalBasePhase{Environment.ProcessId}";
            string baseVictimName = $"NpcLocalBaseVictim{Environment.ProcessId}";
            float basePhaseX = anchorX + 52f;
            float basePhaseY = anchorY + 18f;
            await ship.Suicide().ConfigureAwait(false);

            if (!await WaitForAliveState(ship, false, NpcUnitsLocalTimeoutMs).ConfigureAwait(false))
                throw new InvalidOperationException("NPC-LOCAL: ship did not die before the ai base phase.");

            await adminCluster.SetUnit(
                $"<AiTurret Name=\"{baseVictimName}\" Team=\"{pinkTeam.Id}\" X=\"{basePhaseX + 42f:0.###}\" Y=\"{basePhaseY:0.###}\" Radius=\"16\" Gravity=\"0\" Hull=\"30\" RepairPerTick=\"0\" RespawnTicks=\"0\" RespawnPlayerDistance=\"180\" ShotSpeed=\"5.0\" ShotDamage=\"0.1\" />")
                .ConfigureAwait(false);
            await adminCluster.SetUnit(
                $"<AiBase Name=\"{basePhaseName}\" Team=\"{spectatorsTeam.Id}\" X=\"{basePhaseX:0.###}\" Y=\"{basePhaseY:0.###}\" Radius=\"22\" Gravity=\"0.0008\" Hull=\"60\" RepairPerTick=\"0\" RespawnTicks=\"0\" RespawnPlayerDistance=\"180\" RailSpeed=\"7.8\" RailReloadTicks=\"8\" InterceptorSpeed=\"6.2\" InterceptorReloadTicks=\"8\" />")
                .ConfigureAwait(false);
            DrainEvents(spectatorEvents);

            List<FlattiverseEvent> baseRailEvents = new List<FlattiverseEvent>();
            AppearedUnitEvent? baseRailEvent = await WaitForQueuedEvent(spectatorEvents, NpcUnitsLocalTimeoutMs, baseRailEvents,
                delegate(AppearedUnitEvent @event)
                {
                    return @event.Unit is Rail;
                }).ConfigureAwait(false);

            if (baseRailEvent is null)
                throw new InvalidOperationException("NPC-LOCAL: ai base never produced a visible rail shot.");

            Rail baseRail = (Rail)baseRailEvent.Unit;

            if (MathF.Abs(baseRail.SpeedLimit - NpcUnitsLocalRailSpeedLimit) > 0.001f)
                throw new InvalidOperationException(
                    $"NPC-LOCAL: ai base rail speed limit mismatch. Expected={NpcUnitsLocalRailSpeedLimit:0.###}, Actual={baseRail.SpeedLimit:0.###}.");

            await RemoveUnitIfPresent(adminCluster, baseVictimName).ConfigureAwait(false);
            await EnsureNpcUnitsLocalShipAlive(ship, testClusterId).ConfigureAwait(false);
            DrainEvents(spectatorEvents);
            await EnsureNpcUnitsLocalShipCanShoot(ship).ConfigureAwait(false);
            ushort baseInterceptorShotTicks = 30;
            float baseInterceptorShotLoad = MathF.Min(8f, ship.ShotLauncher.MaximumLoad);
            float baseInterceptorShotDamage = MathF.Min(20f, ship.ShotLauncher.MaximumDamage);
            Vector baseInterceptorShotMovement =
                BuildNpcUnitsLocalPlayerShotMovement(ship, basePhaseX, basePhaseY, ref baseInterceptorShotTicks, baseInterceptorShotLoad,
                    baseInterceptorShotDamage);
            await ship.ShotLauncher.Shoot(baseInterceptorShotMovement, baseInterceptorShotTicks, baseInterceptorShotLoad,
                baseInterceptorShotDamage).ConfigureAwait(false);

            List<FlattiverseEvent> baseInterceptorEvents = new List<FlattiverseEvent>();
            AppearedUnitEvent? baseInterceptorEvent = await WaitForQueuedEvent(spectatorEvents, NpcUnitsLocalTimeoutMs, baseInterceptorEvents,
                delegate(AppearedUnitEvent @event)
                {
                    return @event.Unit is Interceptor;
                }).ConfigureAwait(false);

            if (baseInterceptorEvent is null)
            {
                while (spectatorEvents.TryDequeue(out FlattiverseEvent? @event))
                    baseInterceptorEvents.Add(@event);

                Console.WriteLine("NPC-LOCAL: ai base interceptor spectator trace:");

                for (int index = 0; index < baseInterceptorEvents.Count; index++)
                    Console.WriteLine($"  TRACE[{index}] {DescribeSpectatorEvent(baseInterceptorEvents[index])}");

                throw new InvalidOperationException("NPC-LOCAL: ai base never produced a visible interceptor.");
            }

            Interceptor baseInterceptor = (Interceptor)baseInterceptorEvent.Unit;

            if (MathF.Abs(baseInterceptor.SpeedLimit - NpcUnitsLocalInterceptorSpeedLimit) > 0.001f)
                throw new InvalidOperationException(
                    $"NPC-LOCAL: ai base interceptor speed limit mismatch. Expected={NpcUnitsLocalInterceptorSpeedLimit:0.###}, Actual={baseInterceptor.SpeedLimit:0.###}.");

            await RemoveUnitIfPresent(adminCluster, basePhaseName).ConfigureAwait(false);

            if (spectatorGalaxy is not null)
                await WaitForNpcUnitsLocalTransientUnitsClear(spectatorGalaxy, testClusterId).ConfigureAwait(false);

            await EnsureNpcUnitsLocalShipAlive(ship, testClusterId).ConfigureAwait(false);

            Console.WriteLine("NPC-LOCAL: observing freighter visibility and interceptor...");
            string freighterPhaseName = $"NpcLocalFreighterPhase{Environment.ProcessId}";
            float freighterPhaseX = anchorX + 52f;
            float freighterPhaseY = anchorY - 18f;
            await adminCluster.SetUnit(BuildInitialFreighterXml(freighterPhaseName, spectatorsTeam.Id, freighterPhaseX, freighterPhaseY, 0, 180f,
                0.0011f, 11f, 8f, 4f, 2f, 30f)).ConfigureAwait(false);

            if (!await WaitForCondition(delegate
                {
                    if (spectatorGalaxy is null ||
                        !TryFindUnit<AiFreighter>(spectatorGalaxy, testClusterId, freighterPhaseName, out AiFreighter? phaseFreighter) ||
                        phaseFreighter is null)
                        return false;

                    return phaseFreighter.Metal == 11f &&
                           phaseFreighter.Carbon == 8f &&
                           phaseFreighter.Hydrogen == 4f &&
                           phaseFreighter.Silicon == 2f;
                }, NpcUnitsLocalTimeoutMs).ConfigureAwait(false))
                throw new InvalidOperationException("NPC-LOCAL: freighter phase loot scan values were not visible.");

            string freighterAttackerName = $"NpcLocalFreighterAttacker{Environment.ProcessId}";
            DrainEvents(spectatorEvents);
            await adminCluster.SetUnit(
                $"<AiTurret Name=\"{freighterAttackerName}\" Team=\"{pinkTeam.Id}\" X=\"{freighterPhaseX:0.###}\" Y=\"{freighterPhaseY - 60f:0.###}\" Radius=\"16\" Gravity=\"0\" Hull=\"30\" RepairPerTick=\"0\" RespawnTicks=\"0\" RespawnPlayerDistance=\"180\" ShotSpeed=\"5.2\" ShotDamage=\"0.2\" />")
                .ConfigureAwait(false);

            List<FlattiverseEvent> freighterInterceptorEvents = new List<FlattiverseEvent>();
            AppearedUnitEvent? freighterInterceptorEvent = await WaitForQueuedEvent(spectatorEvents, NpcUnitsLocalTimeoutMs,
                freighterInterceptorEvents, delegate(AppearedUnitEvent @event)
                {
                    return @event.Unit is Interceptor;
                }).ConfigureAwait(false);

            if (freighterInterceptorEvent is null)
                throw new InvalidOperationException("NPC-LOCAL: ai freighter never produced a visible interceptor.");

            await RemoveUnitIfPresent(adminCluster, freighterAttackerName).ConfigureAwait(false);
            await RemoveUnitIfPresent(adminCluster, freighterPhaseName).ConfigureAwait(false);

            if (spectatorGalaxy is not null)
                await WaitForNpcUnitsLocalTransientUnitsClear(spectatorGalaxy, testClusterId).ConfigureAwait(false);

            await EnsureNpcUnitsLocalShipAlive(ship, testClusterId).ConfigureAwait(false);

            uint initialFriendlyKills = playerGalaxy.Player.Score.FriendlyKills;
            uint initialNpcKills = playerGalaxy.Player.Score.NpcKills;
            uint initialNpcDeaths = playerGalaxy.Player.Score.NpcDeaths;
            DatabaseAccountRow initialScoreRow = QueryAccountRow(playerAuth);

            Console.WriteLine("NPC-LOCAL: verifying FriendlyKill scoring against a friendly npc...");
            string friendlyTargetName = $"NpcLocalFriendlyTarget{Environment.ProcessId}";
            await EnsureNpcUnitsLocalShipAlive(ship, testClusterId).ConfigureAwait(false);
            Vector friendlyTargetOffset = Vector.FromAngleLength(ship.Angle, 30f);
            float friendlyTargetX = ship.Position.X + friendlyTargetOffset.X;
            float friendlyTargetY = ship.Position.Y + friendlyTargetOffset.Y;
            string friendlyTargetXString = friendlyTargetX.ToString("R", CultureInfo.InvariantCulture);
            string friendlyTargetYString = friendlyTargetY.ToString("R", CultureInfo.InvariantCulture);
            await adminCluster.SetUnit(
                $"<AiTurret Name=\"{friendlyTargetName}\" Team=\"{pinkTeam.Id}\" X=\"{friendlyTargetXString}\" Y=\"{friendlyTargetYString}\" Radius=\"16\" Gravity=\"0\" Hull=\"1\" RepairPerTick=\"0\" RespawnTicks=\"0\" RespawnPlayerDistance=\"180\" ShotSpeed=\"5.0\" ShotDamage=\"0.1\" />")
                .ConfigureAwait(false);

            if (!await WaitForCondition(delegate
                {
                    return spectatorGalaxy is not null &&
                           TryFindUnit<AiTurret>(spectatorGalaxy, testClusterId, friendlyTargetName, out AiTurret? _);
                }, NpcUnitsLocalTimeoutMs).ConfigureAwait(false))
                throw new InvalidOperationException("NPC-LOCAL: friendly npc target never became visible to the spectator.");

            DrainEvents(playerEvents);
            DrainEvents(spectatorEvents);
            await EnsureNpcUnitsLocalShipCanShoot(ship).ConfigureAwait(false);
            ushort friendlyShotTicks = 12;
            float friendlyShotLoad = MathF.Min(8f, ship.ShotLauncher.MaximumLoad);
            float friendlyShotDamage = MathF.Min(20f, ship.ShotLauncher.MaximumDamage);
            Vector friendlyShotMovement =
                BuildNpcUnitsLocalPlayerShotMovement(ship, friendlyTargetX, friendlyTargetY, ref friendlyShotTicks, friendlyShotLoad,
                    friendlyShotDamage);
            await ship.ShotLauncher.Shoot(friendlyShotMovement, friendlyShotTicks, friendlyShotLoad, friendlyShotDamage).ConfigureAwait(false);

            List<FlattiverseEvent> friendlyKillEvents = new List<FlattiverseEvent>();

            if (!await WaitForCondition(delegate
                 {
                     while (spectatorEvents.TryDequeue(out FlattiverseEvent? @event))
                         friendlyKillEvents.Add(@event);

                     DatabaseAccountRow currentRow = QueryAccountRow(playerAuth);
                     return playerGalaxy.Player.Score.FriendlyKills == initialFriendlyKills + 1U &&
                            currentRow.StatsFriendlyKills == initialScoreRow.StatsFriendlyKills + 1 &&
                            currentRow.SessionFriendlyKills == initialScoreRow.SessionFriendlyKills + 1;
                 }, NpcUnitsLocalScoreTimeoutMs).ConfigureAwait(false))
            {
                string visibleTarget = "missing";

                if (spectatorGalaxy is not null &&
                    TryFindUnit<AiTurret>(spectatorGalaxy, testClusterId, friendlyTargetName, out AiTurret? remainingFriendlyTarget) &&
                    remainingFriendlyTarget is not null)
                {
                    visibleTarget =
                        $"pos={remainingFriendlyTarget.Position}, move={remainingFriendlyTarget.Movement}, hull={remainingFriendlyTarget.Hull:0.###}/{remainingFriendlyTarget.HullMaximum:0.###}";
                }

                List<string> trace = new List<string>();

                for (int index = 0; index < friendlyKillEvents.Count; index++)
                    trace.Add(DescribeSpectatorEvent(friendlyKillEvents[index]));

                throw new InvalidOperationException(
                    $"NPC-LOCAL: friendly kill score was not applied. shipPos={ship.Position}, shipAngle={ship.Angle:0.###}, shotMove={friendlyShotMovement}, shotTicks={friendlyShotTicks}, shotLoad={friendlyShotLoad:0.###}, shotDamage={friendlyShotDamage:0.###}, target={visibleTarget}, spectatorTrace=[{string.Join(" || ", trace)}]");
            }

            Console.WriteLine("NPC-LOCAL: verifying NpcKill scoring against an enemy npc...");
            DatabaseAccountRow npcKillBaselineRow = QueryAccountRow(playerAuth);
            string enemyTargetName = $"NpcLocalEnemyTarget{Environment.ProcessId}";
            await EnsureNpcUnitsLocalShipAlive(ship, testClusterId).ConfigureAwait(false);
            Vector enemyTargetOffset = Vector.FromAngleLength(ship.Angle, 30f);
            float enemyTargetX = ship.Position.X + enemyTargetOffset.X;
            float enemyTargetY = ship.Position.Y + enemyTargetOffset.Y;
            string enemyTargetXString = enemyTargetX.ToString("R", CultureInfo.InvariantCulture);
            string enemyTargetYString = enemyTargetY.ToString("R", CultureInfo.InvariantCulture);
            await adminCluster.SetUnit(
                $"<AiTurret Name=\"{enemyTargetName}\" Team=\"{greenTeam.Id}\" X=\"{enemyTargetXString}\" Y=\"{enemyTargetYString}\" Radius=\"16\" Gravity=\"0\" Hull=\"1\" RepairPerTick=\"0\" RespawnTicks=\"0\" RespawnPlayerDistance=\"180\" ShotSpeed=\"5.0\" ShotDamage=\"0.1\" />")
                .ConfigureAwait(false);

            if (!await WaitForCondition(delegate
                {
                    return spectatorGalaxy is not null &&
                           TryFindUnit<AiTurret>(spectatorGalaxy, testClusterId, enemyTargetName, out AiTurret? _);
                }, NpcUnitsLocalTimeoutMs).ConfigureAwait(false))
                throw new InvalidOperationException("NPC-LOCAL: enemy npc target never became visible to the spectator.");

            DrainEvents(playerEvents);
            DrainEvents(spectatorEvents);
            await EnsureNpcUnitsLocalShipCanShoot(ship).ConfigureAwait(false);
            ushort enemyShotTicks = 12;
            float enemyShotLoad = MathF.Min(8f, ship.ShotLauncher.MaximumLoad);
            float enemyShotDamage = MathF.Min(20f, ship.ShotLauncher.MaximumDamage);
            Vector enemyShotMovement =
                BuildNpcUnitsLocalPlayerShotMovement(ship, enemyTargetX, enemyTargetY, ref enemyShotTicks, enemyShotLoad, enemyShotDamage);
            await ship.ShotLauncher.Shoot(enemyShotMovement, enemyShotTicks, enemyShotLoad, enemyShotDamage).ConfigureAwait(false);

            if (!await WaitForCondition(delegate
                {
                    DatabaseAccountRow currentRow = QueryAccountRow(playerAuth);
                    return playerGalaxy.Player.Score.NpcKills == initialNpcKills + 1U &&
                           currentRow.StatsNpcKills == npcKillBaselineRow.StatsNpcKills + 1 &&
                           currentRow.SessionNpcKills == npcKillBaselineRow.SessionNpcKills + 1;
                }, NpcUnitsLocalScoreTimeoutMs).ConfigureAwait(false))
            {
                DatabaseAccountRow currentRow = QueryAccountRow(playerAuth);
                List<string> trace = new List<string>();

                FlattiverseEvent[] npcKillEvents = playerEvents.ToArray();

                for (int index = 0; index < npcKillEvents.Length; index++)
                    trace.Add(DescribeSpectatorEvent(npcKillEvents[index]));

                throw new InvalidOperationException(
                    $"NPC-LOCAL: npc kill score was not applied. playerScoreNpcKills={playerGalaxy.Player.Score.NpcKills}, expectedPlayerScoreNpcKills={initialNpcKills + 1U}, dbNpcKills={currentRow.StatsNpcKills}, expectedDbNpcKills={npcKillBaselineRow.StatsNpcKills + 1}, sessionNpcKills={currentRow.SessionNpcKills}, expectedSessionNpcKills={npcKillBaselineRow.SessionNpcKills + 1}, trace=[{string.Join(" || ", trace)}]");
            }

            if (spectatorGalaxy is not null)
                await WaitForNpcUnitsLocalTransientUnitsClear(spectatorGalaxy, testClusterId).ConfigureAwait(false);

            await EnsureNpcUnitsLocalShipAlive(ship, testClusterId).ConfigureAwait(false);

            Console.WriteLine("NPC-LOCAL: verifying NpcDeath scoring against an enemy npc...");
            DatabaseAccountRow npcDeathBaselineRow = QueryAccountRow(playerAuth);
            string lethalTurretName = $"NpcLocalLethalTurret{Environment.ProcessId}";
            await adminCluster.SetUnit(
                $"<AiTurret Name=\"{lethalTurretName}\" Team=\"{greenTeam.Id}\" X=\"{anchorX + 120f:0.###}\" Y=\"{anchorY:0.###}\" Radius=\"16\" Gravity=\"0\" Hull=\"30\" RepairPerTick=\"0\" RespawnTicks=\"0\" RespawnPlayerDistance=\"180\" ShotSpeed=\"5.4\" ShotDamage=\"60\" />")
                .ConfigureAwait(false);
            DrainEvents(playerEvents);

            if (!await WaitForAliveState(ship, false, NpcUnitsLocalScoreTimeoutMs).ConfigureAwait(false))
                throw new InvalidOperationException("NPC-LOCAL: enemy npc never killed the player controllable.");

            if (!await WaitForCondition(delegate
                {
                    DatabaseAccountRow currentRow = QueryAccountRow(playerAuth);
                    return playerGalaxy.Player.Score.NpcDeaths == initialNpcDeaths + 1U &&
                           currentRow.StatsNpcDeaths == npcDeathBaselineRow.StatsNpcDeaths + 1 &&
                           currentRow.SessionNpcDeaths == npcDeathBaselineRow.SessionNpcDeaths + 1;
                }, NpcUnitsLocalScoreTimeoutMs).ConfigureAwait(false))
            {
                DatabaseAccountRow currentRow = QueryAccountRow(playerAuth);
                List<string> trace = new List<string>();

                FlattiverseEvent[] npcDeathEvents = playerEvents.ToArray();

                for (int index = 0; index < npcDeathEvents.Length; index++)
                    trace.Add(DescribeSpectatorEvent(npcDeathEvents[index]));

                throw new InvalidOperationException(
                    $"NPC-LOCAL: npc death score was not applied. playerScoreNpcDeaths={playerGalaxy.Player.Score.NpcDeaths}, expectedPlayerScoreNpcDeaths={initialNpcDeaths + 1U}, dbNpcDeaths={currentRow.StatsNpcDeaths}, expectedDbNpcDeaths={npcDeathBaselineRow.StatsNpcDeaths + 1}, sessionNpcDeaths={currentRow.SessionNpcDeaths}, expectedSessionNpcDeaths={npcDeathBaselineRow.SessionNpcDeaths + 1}, trace=[{string.Join(" || ", trace)}]");
            }

            Console.WriteLine("NPC-LOCAL: SUCCESS");
            Console.WriteLine("NPC-LOCAL: verified spectator-owned NPC XML roundtrip, editable listings, connector visibility, aggression against non-spectator players, projectile spawning and player score semantics for the new NPC units.");
        }
        finally
        {
            if (playerGalaxy is not null)
                playerGalaxy.Dispose();

            if (spectatorGalaxy is not null)
                spectatorGalaxy.Dispose();

            await WaitForSessionGalaxy(playerAuth, null, 7000).ConfigureAwait(false);
            RestoreAccountRow(originalPlayerAccount);

            if (adminGalaxy is not null)
            {
                if (adminGalaxy.Tournament is not null)
                    try
                    {
                        await adminGalaxy.CancelTournament().ConfigureAwait(false);
                    }
                    catch (GameException exception)
                    {
                        Console.WriteLine($"NPC-LOCAL: cleanup CancelTournament failed: {exception.Message}");
                    }

                if (restoreGameMode != adminGalaxy.GameMode && adminGalaxy.Tournament is null)
                    try
                    {
                        await adminGalaxy.Configure(BuildGameModeConfigurationXml(adminGalaxy, restoreGameMode)).ConfigureAwait(false);
                    }
                    catch (GameException exception)
                    {
                        Console.WriteLine($"NPC-LOCAL: cleanup game mode restore failed: {exception.Message}");
                    }

                if (restoreConfigurationXml is not null)
                    try
                    {
                        await adminGalaxy.Configure(restoreConfigurationXml).ConfigureAwait(false);
                    }
                    catch (GameException exception)
                    {
                        Console.WriteLine($"NPC-LOCAL: cleanup configuration restore failed: {exception.Message}");
                    }

                if (restoreRegionsByCluster is not null)
                    try
                    {
                        await RestoreRegionsByCluster(adminGalaxy, restoreRegionsByCluster).ConfigureAwait(false);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine($"NPC-LOCAL: cleanup region restore failed: {exception.Message}");
                    }

                adminGalaxy.Dispose();
            }

            await WaitForSessionGalaxy(adminAuth, null, 7000).ConfigureAwait(false);
            RestoreAccountRow(originalAdminAccount);

            if (playerEventPump is not null)
                await Task.WhenAny(playerEventPump, Task.Delay(1000)).ConfigureAwait(false);

            if (spectatorEventPump is not null)
                await Task.WhenAny(spectatorEventPump, Task.Delay(1000)).ConfigureAwait(false);

            if (adminEventPump is not null)
                await Task.WhenAny(adminEventPump, Task.Delay(1000)).ConfigureAwait(false);

            if (galaxyProcess is not null)
                StopProcess(galaxyProcess);

            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUiCulture;
        }
    }

    private static string ResolveNpcUnitsLocalAdminAuth()
    {
        if (!LocalSwitchGateAdminAuth.StartsWith("<INSERT", StringComparison.Ordinal))
            return LocalSwitchGateAdminAuth;

        return QueryAlternativeAdminAuth(0);
    }

    private static string ResolveNpcUnitsLocalPlayerAuth(int excludedAccountId)
    {
        if (!LocalSwitchGatePlayerAuth.StartsWith("<INSERT", StringComparison.Ordinal))
        {
            DatabaseAccountRow row = QueryAccountRow(LocalSwitchGatePlayerAuth);

            if (row.AccountId != excludedAccountId)
                return LocalSwitchGatePlayerAuth;
        }

        string[] candidateAuths = QueryCandidatePlayerAuths(new int[] { excludedAccountId }, 1);

        if (candidateAuths.Length == 0)
            throw new InvalidOperationException("NPC-LOCAL: no local player auth could be resolved from the database.");

        return candidateAuths[0];
    }

    private static string BuildNpcUnitsLocalConfigurationXml(Galaxy galaxy, byte testClusterId, string clusterName)
    {
        List<ClusterSpec> clusters = new List<ClusterSpec>();

        foreach (Cluster cluster in galaxy.Clusters)
            clusters.Add(new ClusterSpec(cluster.Id, cluster.Name, false, false));

        clusters.Add(new ClusterSpec(testClusterId, clusterName, true, true));
        return BuildConfigurationXml(galaxy, clusters.ToArray());
    }

    private static string BuildInitialFreighterXml(string name, byte teamId, float x, float y, int respawnTicks, float respawnPlayerDistance,
        float gravity, float metal, float carbon, float hydrogen, float silicon, float routeOffset)
    {
        return
            $"<AiFreighter Name=\"{name}\" Team=\"{teamId}\" X=\"{x:0.###}\" Y=\"{y:0.###}\" Radius=\"14\" Gravity=\"{gravity:R}\" Hull=\"48\" RepairPerTick=\"0.01\" RespawnTicks=\"{respawnTicks}\" RespawnPlayerDistance=\"{respawnPlayerDistance:0.###}\" InterceptorSpeed=\"6.2\" InterceptorReloadTicks=\"16\" LootMetal=\"{metal:0.###}\" LootCarbon=\"{carbon:0.###}\" LootHydrogen=\"{hydrogen:0.###}\" LootSilicon=\"{silicon:0.###}\"><Waypoint X=\"{x:0.###}\" Y=\"{y:0.###}\" Speed=\"2.2\" /><Waypoint X=\"{x + routeOffset:0.###}\" Y=\"{y:0.###}\" Speed=\"2.2\" /></AiFreighter>";
    }

    private static void VerifyNpcUnitXml(string xml, string expectedRootName, byte expectedTeamId, float expectedGravity,
        int expectedWaypointCount)
    {
        XDocument document = XDocument.Parse(xml, LoadOptions.None);
        XElement? root = document.Root;
        XAttribute? teamAttribute;

        if (root is null)
            throw new InvalidOperationException("NPC-LOCAL: queried unit XML has no root element.");

        if (root.Name.LocalName != expectedRootName)
            throw new InvalidOperationException($"NPC-LOCAL: expected XML root {expectedRootName}, got {root.Name.LocalName}.");

        teamAttribute = root.Attribute("Team");

        if (root.Attribute("Name") is null ||
            teamAttribute is null ||
            root.Attribute("X") is null ||
            root.Attribute("Y") is null ||
            root.Attribute("Radius") is null ||
            root.Attribute("Gravity") is null ||
            root.Attribute("Hull") is null ||
            root.Attribute("RepairPerTick") is null ||
            root.Attribute("RespawnTicks") is null ||
            root.Attribute("RespawnPlayerDistance") is null)
            throw new InvalidOperationException($"NPC-LOCAL: queried XML for {expectedRootName} is missing required base attributes.");

        if (!byte.TryParse(teamAttribute.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out byte teamId) || teamId != expectedTeamId)
            throw new InvalidOperationException($"NPC-LOCAL: queried XML for {expectedRootName} has unexpected Team={teamAttribute.Value}.");

        XAttribute? gravityAttribute = root.Attribute("Gravity");

        if (gravityAttribute is null ||
            !float.TryParse(gravityAttribute.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out float queriedGravity) ||
            MathF.Abs(queriedGravity - expectedGravity) > 0.0001f)
        {
            throw new InvalidOperationException(
                $"NPC-LOCAL: queried XML for {expectedRootName} has unexpected Gravity={gravityAttribute?.Value ?? "<missing>"}.");
        }

        if (expectedRootName == "AiFreighter")
        {
            if (root.Attribute("LootMetal") is null ||
                root.Attribute("LootCarbon") is null ||
                root.Attribute("LootHydrogen") is null ||
                root.Attribute("LootSilicon") is null ||
                root.Attribute("InterceptorSpeed") is null ||
                root.Attribute("InterceptorReloadTicks") is null)
                throw new InvalidOperationException("NPC-LOCAL: queried AiFreighter XML is missing freighter-specific attributes.");
        }

        if (expectedRootName == "AiBase" && root.Attribute("RailDamage") is not null)
            throw new InvalidOperationException("NPC-LOCAL: queried AiBase XML must not contain RailDamage anymore.");

        if (expectedWaypointCount != root.Elements("Waypoint").Count())
            throw new InvalidOperationException($"NPC-LOCAL: expected {expectedWaypointCount} waypoints in {expectedRootName} XML.");
    }

    private static async Task EnsureNpcUnitsLocalShipAlive(ClassicShipControllable ship, byte expectedClusterId)
    {
        for (int attempt = 0; attempt < 3; attempt++)
        {
            if (!ship.Alive)
            {
                await ship.Continue().ConfigureAwait(false);

                if (!await WaitForAliveState(ship, true, 10000).ConfigureAwait(false))
                    throw new InvalidOperationException("NPC-LOCAL: ship did not recover for the next phase.");
            }

            if (!await WaitForCondition(delegate { return ship.Cluster.Id == expectedClusterId; }, 10000).ConfigureAwait(false))
                throw new InvalidOperationException("NPC-LOCAL: ship respawned into an unexpected cluster.");

            try
            {
                await ship.Engine.Set(new Vector(0f, 0f)).ConfigureAwait(false);
                return;
            }
            catch (YouNeedToContinueFirstGameException) when (attempt < 2)
            {
                await Task.Delay(250).ConfigureAwait(false);
            }
        }

        throw new InvalidOperationException("NPC-LOCAL: ship could not be stabilized for the next phase.");
    }

    private static Vector BuildNpcUnitsLocalPlayerShotMovement(ClassicShipControllable ship, float targetX, float targetY, ref ushort ticks,
        float load, float damage)
    {
        Vector startOffset = new Vector(targetX - ship.Position.X, targetY - ship.Position.Y);

        if (startOffset.Length < 0.001f)
            throw new InvalidOperationException("NPC-LOCAL: cannot build a player shot towards the current ship position.");

        startOffset.Length = 16f;

        Vector start = ship.Position + startOffset;
        for (int currentTicks = ticks; currentTicks <= ship.ShotLauncher.MaximumTicks; currentTicks++)
        {
            Vector shotMovement = (new Vector(targetX - start.X, targetY - start.Y) / currentTicks) - ship.Movement;

            if (ship.ShotLauncher.CalculateCost(shotMovement, (ushort)currentTicks, load, damage, out float _, out float _, out float _))
            {
                ticks = (ushort)currentTicks;
                return shotMovement;
            }
        }

        Vector failingShotMovement = (new Vector(targetX - start.X, targetY - start.Y) / ticks) - ship.Movement;

        throw new InvalidOperationException(
            $"NPC-LOCAL: player shot request invalid for target ({targetX:0.###}, {targetY:0.###}), len={failingShotMovement.Length:0.###}, max={ship.ShotLauncher.MaximumRelativeMovement:0.###}.");
    }

    private static async Task EnsureNpcUnitsLocalShipCanShoot(ClassicShipControllable ship)
    {
        if (ship.ShotMagazine.CurrentShots >= 1f)
            return;

        await ship.ShotFabricator.Set(ship.ShotFabricator.MaximumRate).ConfigureAwait(false);
        await ship.ShotFabricator.On().ConfigureAwait(false);

        if (!await WaitForCondition(delegate
            {
                return ship.ShotFabricator.Status == SubsystemStatus.Worked && ship.ShotMagazine.CurrentShots >= 1f;
            }, 10000).ConfigureAwait(false))
            throw new InvalidOperationException(
                $"NPC-LOCAL: ship did not refill shots. magazine={ship.ShotMagazine.CurrentShots:0.###}, fabricatorStatus={ship.ShotFabricator.Status}, fabricatorRate={ship.ShotFabricator.Rate:0.###}.");

        await ship.ShotFabricator.Off().ConfigureAwait(false);
    }

    private static async Task RemoveUnitIfPresent(Cluster cluster, string name)
    {
        try
        {
            await cluster.RemoveUnit(name).ConfigureAwait(false);
        }
        catch (Exception)
        {
        }
    }

    private static async Task WaitForNpcUnitsLocalTransientUnitsClear(Galaxy spectatorGalaxy, byte clusterId)
    {
        if (!await WaitForCondition(delegate
            {
                if (!spectatorGalaxy.Clusters.TryGet(clusterId, out Cluster? cluster) || cluster is null)
                    return false;

                foreach (Unit unit in cluster.Units)
                    if (unit.Kind == UnitKind.SpaceJellyFishSlime ||
                        unit.Kind == UnitKind.Shot ||
                        unit.Kind == UnitKind.Rail ||
                        unit.Kind == UnitKind.Interceptor ||
                        unit.Kind == UnitKind.Explosion ||
                        unit.Kind == UnitKind.InterceptorExplosion)
                        return false;

                return true;
            }, NpcUnitsLocalTimeoutMs).ConfigureAwait(false))
            throw new InvalidOperationException("NPC-LOCAL: transient combat units did not clear before the next phase.");
    }
}
