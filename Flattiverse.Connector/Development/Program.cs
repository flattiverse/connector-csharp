using System.Collections.Concurrent;
using System.Xml.Linq;
using Flattiverse.Connector;
using Flattiverse.Connector.Events;
using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;
using Npgsql;
using System.Net.WebSockets;

namespace Development;

class Program
{
    private const string Uri = "ws://127.0.0.1:5000";
    private const string SpectatorWatchUri = "ws://127.0.0.1:5666";
    private const string TeamName = "Pink";
    private const byte SpectatorsTeamId = 12;
    private const ushort DatabaseGalaxyId = 0;
    private const string DatabaseConnectionString = "Host=10.252.7.136;Port=5432;Database=flattiverse;Username=postgres";
    private const string AdminAuth = "<INSERT ADMIN API KEY HERE>";
    private const string PlayerAdminAuth = "<INSERT PLAYER-ADMIN API KEY HERE>";
    private const string PlayerAuth = "<INSERT PLAYER API KEY HERE>";
    private static readonly TimeSpan AccountSessionTimeout = new TimeSpan(0, 0, 15);

    private readonly struct ClusterSpec
    {
        public readonly byte Id;
        public readonly string Name;
        public readonly bool Start;
        public readonly bool Respawn;

        public ClusterSpec(byte id, string name, bool start, bool respawn)
        {
            Id = id;
            Name = name;
            Start = start;
            Respawn = respawn;
        }
    }

    private readonly struct TeamSpec
    {
        public readonly byte Id;
        public readonly string Name;
        public readonly byte Red;
        public readonly byte Green;
        public readonly byte Blue;

        public TeamSpec(byte id, string name, byte red, byte green, byte blue)
        {
            Id = id;
            Name = name;
            Red = red;
            Green = green;
            Blue = blue;
        }
    }

    private readonly struct UnitSpec
    {
        public readonly byte ClusterId;
        public readonly string Name;
        public readonly UnitKind Kind;

        public UnitSpec(byte clusterId, string name, UnitKind kind)
        {
            ClusterId = clusterId;
            Name = name;
            Kind = kind;
        }
    }

    private readonly struct DatabaseUnitRow
    {
        public readonly byte ClusterId;
        public readonly string Name;
        public readonly short Kind;
        public readonly string Xml;

        public DatabaseUnitRow(byte clusterId, string name, short kind, string xml)
        {
            ClusterId = clusterId;
            Name = name;
            Kind = kind;
            Xml = xml;
        }
    }

    private readonly struct DatabaseAccountRow
    {
        public readonly int AccountId;
        public readonly bool Admin;
        public readonly int Rank;
        public readonly long? DatePlayedStart;
        public readonly long? DatePlayedEnd;
        public readonly long StatsPlayerKills;
        public readonly long StatsPlayerDeaths;
        public readonly long StatsFriendlyKills;
        public readonly long StatsFriendlyDeaths;
        public readonly long StatsNpcKills;
        public readonly long StatsNpcDeaths;
        public readonly long StatsNeutralDeaths;
        public readonly short? SessionGalaxy;
        public readonly short? SessionTeam;
        public readonly long SessionPlayerKills;
        public readonly long SessionPlayerDeaths;
        public readonly long SessionFriendlyKills;
        public readonly long SessionFriendlyDeaths;
        public readonly long SessionNpcKills;
        public readonly long SessionNpcDeaths;
        public readonly long SessionNeutralDeaths;

        public DatabaseAccountRow(int accountId, bool admin, int rank, long? datePlayedStart, long? datePlayedEnd,
            long statsPlayerKills, long statsPlayerDeaths, long statsFriendlyKills, long statsFriendlyDeaths,
            long statsNpcKills, long statsNpcDeaths, long statsNeutralDeaths, short? sessionGalaxy, short? sessionTeam,
            long sessionPlayerKills, long sessionPlayerDeaths, long sessionFriendlyKills, long sessionFriendlyDeaths,
            long sessionNpcKills, long sessionNpcDeaths, long sessionNeutralDeaths)
        {
            AccountId = accountId;
            Admin = admin;
            Rank = rank;
            DatePlayedStart = datePlayedStart;
            DatePlayedEnd = datePlayedEnd;
            StatsPlayerKills = statsPlayerKills;
            StatsPlayerDeaths = statsPlayerDeaths;
            StatsFriendlyKills = statsFriendlyKills;
            StatsFriendlyDeaths = statsFriendlyDeaths;
            StatsNpcKills = statsNpcKills;
            StatsNpcDeaths = statsNpcDeaths;
            StatsNeutralDeaths = statsNeutralDeaths;
            SessionGalaxy = sessionGalaxy;
            SessionTeam = sessionTeam;
            SessionPlayerKills = sessionPlayerKills;
            SessionPlayerDeaths = sessionPlayerDeaths;
            SessionFriendlyKills = sessionFriendlyKills;
            SessionFriendlyDeaths = sessionFriendlyDeaths;
            SessionNpcKills = sessionNpcKills;
            SessionNpcDeaths = sessionNpcDeaths;
            SessionNeutralDeaths = sessionNeutralDeaths;
        }
    }

    private readonly struct DatabaseAvatarRow
    {
        public readonly int AccountId;
        public readonly byte[]? SmallAvatar;
        public readonly byte[]? BigAvatar;

        public DatabaseAvatarRow(int accountId, byte[]? smallAvatar, byte[]? bigAvatar)
        {
            AccountId = accountId;
            SmallAvatar = smallAvatar;
            BigAvatar = bigAvatar;
        }
    }

    private static async Task Main(string[] args)
    {
        if (args.Length > 0 && args[0] == "--spectator-watch")
        {
            await RunSpectatorWatch(args).ConfigureAwait(false);
            return;
        }

        if (args.Length > 0 && args[0] == "--units-db-roundtrip")
        {
            await RunUnitsDbRoundtrip().ConfigureAwait(false);
            return;
        }

        if (args.Length > 0 && args[0] == "--units-startup-check")
        {
            await RunUnitsStartupCheck().ConfigureAwait(false);
            return;
        }

        if (args.Length > 0 && args[0] == "--cleanup-roundtrip-units")
        {
            await CleanupRoundtripUnits().ConfigureAwait(false);
            return;
        }

        if (args.Length > 0 && args[0] == "--login-team-selection-check")
        {
            await RunLoginTeamSelectionCheck().ConfigureAwait(false);
            return;
        }

        if (args.Length > 0 && args[0] == "--account-session-check")
        {
            await RunAccountSessionCheck().ConfigureAwait(false);
            return;
        }

        if (args.Length > 0 && args[0] == "--hull-neutral-death-check")
        {
            await RunHullNeutralDeathCheck().ConfigureAwait(false);
            return;
        }

        if (args.Length > 0 && args[0] == "--friendly-stats-check")
        {
            await RunFriendlyStatsCheck().ConfigureAwait(false);
            return;
        }

        if (args.Length > 0 && args[0] == "--self-disclosure-check")
        {
            await RunSelfDisclosureCheck().ConfigureAwait(false);
            return;
        }

        if (args.Length > 0 && args[0] == "--avatar-download-check")
        {
            await RunAvatarDownloadCheck(args).ConfigureAwait(false);
            return;
        }

        Galaxy? adminGalaxy = null;
        Galaxy? playerGalaxy = null;
        Task? playerEventPump = null;
        string? restoreConfigurationXml = null;
        Dictionary<byte, string>? restoreRegionsByCluster = null;

        ConcurrentQueue<FlattiverseEvent> playerEvents = new ConcurrentQueue<FlattiverseEvent>();

        try
        {
            adminGalaxy = await Galaxy.Connect(Uri, AdminAuth, TeamName).ConfigureAwait(false);
            playerGalaxy = await Galaxy.Connect(Uri, PlayerAuth, null).ConfigureAwait(false);

            restoreConfigurationXml = BuildConfigurationXml(adminGalaxy, null);
            restoreRegionsByCluster = await CaptureRegionsByCluster(adminGalaxy).ConfigureAwait(false);

            playerEventPump = StartEventPump("MAIN", playerGalaxy, playerEvents);

            await Task.Delay(250).ConfigureAwait(false);
            DrainEvents(playerEvents);

            await TestNoStartCluster(adminGalaxy).ConfigureAwait(false);
            await TestNoClusterNodes(adminGalaxy).ConfigureAwait(false);
            await TestInvalidStartAttributeValue(adminGalaxy).ConfigureAwait(false);
            await TestTeamChildNodesRejected(adminGalaxy).ConfigureAwait(false);
            await TestClusterChildNodesRejected(adminGalaxy).ConfigureAwait(false);
            await TestEmptyGalaxyName(adminGalaxy).ConfigureAwait(false);
            await TestTooLongDescription(adminGalaxy).ConfigureAwait(false);
            await TestTooLongTeamName(adminGalaxy).ConfigureAwait(false);
            await TestDuplicateTeamNames(adminGalaxy).ConfigureAwait(false);
            await TestDeleteTeamBlockedByRegion(adminGalaxy).ConfigureAwait(false);
            await TestQueryRegions(adminGalaxy).ConfigureAwait(false);
            await TestSetQueryRemoveUnit(adminGalaxy).ConfigureAwait(false);
            await TestScoreUpdatesOnSuicide(playerGalaxy, playerEvents).ConfigureAwait(false);
            await TestDeleteClusterWithActiveShip(adminGalaxy, playerGalaxy, playerEvents).ConfigureAwait(false);
            await TestSetUnitWithUnknownTeamRejected(adminGalaxy).ConfigureAwait(false);
            await TestDeleteTeamAssignedToUnit(adminGalaxy).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            Console.WriteLine($"FATAL: {exception.GetType().Name}: {exception.Message}");
        }
        finally
        {
            if (adminGalaxy is not null && restoreConfigurationXml is not null)
                try
                {
                    await adminGalaxy.Configure(restoreConfigurationXml).ConfigureAwait(false);
                    Console.WriteLine("RESTORE: OK");
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"RESTORE: FAILED ({exception.GetType().Name}: {exception.Message})");
                }

            if (adminGalaxy is not null && restoreRegionsByCluster is not null)
                try
                {
                    await RestoreRegionsByCluster(adminGalaxy, restoreRegionsByCluster).ConfigureAwait(false);
                    Console.WriteLine("RESTORE REGIONS: OK");
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"RESTORE REGIONS: FAILED ({exception.GetType().Name}: {exception.Message})");
                }

            if (playerGalaxy is not null)
                playerGalaxy.Dispose();

            if (playerEventPump is not null)
                await Task.WhenAny(playerEventPump, Task.Delay(1000)).ConfigureAwait(false);

            if (adminGalaxy is not null)
                adminGalaxy.Dispose();
        }
    }

    private static async Task RunUnitsDbRoundtrip()
    {
        Galaxy? adminGalaxy = null;
        Task? eventPump = null;
        ConcurrentQueue<FlattiverseEvent> eventQueue = new ConcurrentQueue<FlattiverseEvent>();
        Dictionary<string, UnitSpec> unitsByKey = new Dictionary<string, UnitSpec>();

        try
        {
            Console.WriteLine("ROUNDTRIP: connecting admin...");
            adminGalaxy = await Galaxy.Connect(Uri, AdminAuth, TeamName).ConfigureAwait(false);
            eventPump = StartEventPump("UNITS-DB-ROUNDTRIP", adminGalaxy, eventQueue);

            await Task.Delay(1250).ConfigureAwait(false);
            List<FlattiverseEvent> initialEvents = DrainEvents(eventQueue);
            ApplyUnitEvents(initialEvents, unitsByKey);

            Console.WriteLine($"ROUNDTRIP: initial events={initialEvents.Count}, units via events={unitsByKey.Count}");
            PrintUnits(unitsByKey, "ROUNDTRIP: units before delete");

            List<UnitSpec> deleteTargets = new List<UnitSpec>(unitsByKey.Values);
            deleteTargets.Sort(delegate (UnitSpec left, UnitSpec right)
            {
                int clusterCompare = left.ClusterId.CompareTo(right.ClusterId);
                return clusterCompare != 0 ? clusterCompare : string.Compare(left.Name, right.Name, StringComparison.Ordinal);
            });

            int deleteOk = 0;
            int deleteFail = 0;

            foreach (UnitSpec unitSpec in deleteTargets)
            {
                if (!adminGalaxy.Clusters.TryGet(unitSpec.ClusterId, out Cluster? cluster))
                {
                    Console.WriteLine($"ROUNDTRIP: delete failed, cluster missing -> {unitSpec.ClusterId}:{unitSpec.Name}");
                    deleteFail++;
                    continue;
                }

                try
                {
                    await cluster.RemoveUnit(unitSpec.Name).ConfigureAwait(false);
                    Console.WriteLine($"ROUNDTRIP: delete ok -> {unitSpec.ClusterId}:{unitSpec.Name} ({unitSpec.Kind})");
                    deleteOk++;
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"ROUNDTRIP: delete failed -> {unitSpec.ClusterId}:{unitSpec.Name} ({exception.GetType().Name}: {exception.Message})");
                    deleteFail++;
                }
            }

            await Task.Delay(1000).ConfigureAwait(false);
            List<FlattiverseEvent> removeEvents = DrainEvents(eventQueue);
            ApplyUnitEvents(removeEvents, unitsByKey);

            Console.WriteLine($"ROUNDTRIP: delete summary ok={deleteOk}, fail={deleteFail}, units via events now={unitsByKey.Count}");
            PrintUnits(unitsByKey, "ROUNDTRIP: units after delete");

            List<DatabaseUnitRow> dbRowsAfterDelete = QueryUnitsFromDatabase(DatabaseGalaxyId);
            Console.WriteLine($"ROUNDTRIP: DB rows after delete for galaxy {DatabaseGalaxyId} = {dbRowsAfterDelete.Count}");
            foreach (DatabaseUnitRow row in dbRowsAfterDelete)
                Console.WriteLine($"  - DB cluster={row.ClusterId}, name={row.Name}, kind={row.Kind}");

            if (!TryGetActiveStartCluster(adminGalaxy, out Cluster? targetCluster) || targetCluster is null)
                throw new InvalidOperationException("No active cluster found for unit creation.");

            await TestBuoyMessageLengthLimit(targetCluster).ConfigureAwait(false);

            List<string> createXml = BuildRoundtripCreateXml();
            int createOk = 0;
            int createFail = 0;

            foreach (string xml in createXml)
            {
                string unitName = ReadUnitName(xml);

                try
                {
                    await targetCluster.SetUnit(xml).ConfigureAwait(false);
                    Console.WriteLine($"ROUNDTRIP: create ok -> {targetCluster.Id}:{unitName}");
                    createOk++;
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"ROUNDTRIP: create failed -> {targetCluster.Id}:{unitName} ({exception.GetType().Name}: {exception.Message})");
                    createFail++;
                }
            }

            await Task.Delay(1000).ConfigureAwait(false);
            List<FlattiverseEvent> createEvents = DrainEvents(eventQueue);
            ApplyUnitEvents(createEvents, unitsByKey);

            Console.WriteLine($"ROUNDTRIP: create summary ok={createOk}, fail={createFail}, units via events now={unitsByKey.Count}");
            PrintUnits(unitsByKey, "ROUNDTRIP: units after create");

            List<DatabaseUnitRow> dbRowsAfterCreate = QueryUnitsFromDatabase(DatabaseGalaxyId);
            Console.WriteLine($"ROUNDTRIP: DB XML for galaxy {DatabaseGalaxyId} after create:");
            foreach (DatabaseUnitRow row in dbRowsAfterCreate)
            {
                Console.WriteLine($"  - cluster={row.ClusterId}, name={row.Name}, kind={row.Kind}");
                Console.WriteLine($"    xml={row.Xml}");
            }
        }
        finally
        {
            if (adminGalaxy is not null)
                adminGalaxy.Dispose();

            if (eventPump is not null)
                await Task.WhenAny(eventPump, Task.Delay(1000)).ConfigureAwait(false);
        }
    }

    private static async Task RunUnitsStartupCheck()
    {
        Galaxy? adminGalaxy = null;
        Task? eventPump = null;
        ConcurrentQueue<FlattiverseEvent> eventQueue = new ConcurrentQueue<FlattiverseEvent>();
        Dictionary<string, UnitSpec> unitsByKey = new Dictionary<string, UnitSpec>();

        try
        {
            Console.WriteLine("STARTUP-CHECK: connecting admin...");
            adminGalaxy = await Galaxy.Connect(Uri, AdminAuth, TeamName).ConfigureAwait(false);
            eventPump = StartEventPump("UNITS-STARTUP-CHECK", adminGalaxy, eventQueue);

            await Task.Delay(1250).ConfigureAwait(false);
            List<FlattiverseEvent> events = DrainEvents(eventQueue);
            ApplyUnitEvents(events, unitsByKey);

            Console.WriteLine($"STARTUP-CHECK: events={events.Count}, units via events={unitsByKey.Count}");
            PrintUnits(unitsByKey, "STARTUP-CHECK: current units");

            string[] expectedNames = new string[]
            {
                "RoundtripSun",
                "RoundtripHole",
                "RoundtripPlanet",
                "RoundtripMoon",
                "RoundtripMeteoroid",
                "RoundtripBuoy",
                "RoundtripMissionTarget",
                "RoundtripFlag",
                "RoundtripDominationPoint"
            };

            bool allFound = true;

            foreach (string expectedName in expectedNames)
            {
                bool found = false;

                foreach (UnitSpec unitSpec in unitsByKey.Values)
                    if (unitSpec.Name == expectedName)
                    {
                        found = true;
                        break;
                    }

                Console.WriteLine($"STARTUP-CHECK: contains {expectedName} = {found}");

                if (!found)
                    allFound = false;
            }

            Console.WriteLine($"STARTUP-CHECK: roundtrip units present after startup = {allFound}");
        }
        finally
        {
            if (adminGalaxy is not null)
                adminGalaxy.Dispose();

            if (eventPump is not null)
                await Task.WhenAny(eventPump, Task.Delay(1000)).ConfigureAwait(false);
        }
    }

    private static async Task CleanupRoundtripUnits()
    {
        Galaxy? adminGalaxy = null;

        try
        {
            Console.WriteLine("CLEANUP: connecting admin...");
            adminGalaxy = await Galaxy.Connect(Uri, AdminAuth, TeamName).ConfigureAwait(false);

            if (!adminGalaxy.Clusters.TryGet(0, out Cluster? cluster))
                throw new InvalidOperationException("Cluster 0 not found during roundtrip cleanup.");

            foreach (string unitXml in BuildRoundtripCreateXml())
            {
                string unitName = ReadUnitName(unitXml);

                try
                {
                    await cluster.RemoveUnit(unitName).ConfigureAwait(false);
                    Console.WriteLine($"CLEANUP: removed {unitName}");
                }
                catch (InvalidArgumentGameException exception)
                {
                    if (exception.Reason == InvalidArgumentKind.EntityNotFound)
                        Console.WriteLine($"CLEANUP: already absent {unitName}");
                    else
                        throw;
                }
            }
        }
        finally
        {
            if (adminGalaxy is not null)
                adminGalaxy.Dispose();
        }
    }

    private static async Task RunLoginTeamSelectionCheck()
    {
        Galaxy? adminGalaxy = null;
        Galaxy? explicitTeamPlayer = null;
        Galaxy? autoTeamPlayer1 = null;
        Galaxy? autoTeamPlayer2 = null;
        string? restoreConfigurationXml = null;
        Dictionary<byte, string>? restoreRegionsByCluster = null;

        try
        {
            Console.WriteLine("LOGIN-TEAM-CHECK: connecting admin...");
            adminGalaxy = await Galaxy.Connect(Uri, AdminAuth, null).ConfigureAwait(false);

            restoreConfigurationXml = BuildConfigurationXml(adminGalaxy, null, null);
            restoreRegionsByCluster = await CaptureRegionsByCluster(adminGalaxy).ConfigureAwait(false);

            await RemoveAllRegions(adminGalaxy).ConfigureAwait(false);

            ClusterSpec[] clusters = BuildClusterSpecs(adminGalaxy, false);
            TeamSpec[] teams = new TeamSpec[]
            {
                new TeamSpec(0, "Pink", 255, 0, 200),
                new TeamSpec(1, "Green", 192, 255, 0)
            };

            await adminGalaxy.Configure(BuildConfigurationXml(adminGalaxy, teams, clusters)).ConfigureAwait(false);
            await Task.Delay(250).ConfigureAwait(false);

            explicitTeamPlayer = await Galaxy.Connect(Uri, PlayerAuth, "Green").ConfigureAwait(false);
            Console.WriteLine($"LOGIN-TEAM-CHECK: explicit Green -> {explicitTeamPlayer.Player.Team.Name}");

            try
            {
                Galaxy invalidTeamPlayer = await Galaxy.Connect(Uri, PlayerAuth, "DoesNotExist").ConfigureAwait(false);
                invalidTeamPlayer.Dispose();
                Console.WriteLine("LOGIN-TEAM-CHECK: invalid explicit team -> FAILED (unexpected success)");
            }
            catch (TeamSelectionFailedGameException exception)
            {
                Console.WriteLine($"LOGIN-TEAM-CHECK: invalid explicit team -> OK ({exception.Message})");
            }

            autoTeamPlayer1 = await Galaxy.Connect(Uri, PlayerAuth, null).ConfigureAwait(false);
            Console.WriteLine($"LOGIN-TEAM-CHECK: auto #1 -> {autoTeamPlayer1.Player.Team.Name}");

            autoTeamPlayer2 = await Galaxy.Connect(Uri, PlayerAuth, null).ConfigureAwait(false);
            Console.WriteLine($"LOGIN-TEAM-CHECK: auto #2 -> {autoTeamPlayer2.Player.Team.Name}");

            bool explicitTeamOk = explicitTeamPlayer.Player.Team.Name == "Green";
            bool autoTeam1Ok = autoTeamPlayer1.Player.Team.Name == "Pink";
            bool autoTeam2Ok = autoTeamPlayer2.Player.Team.Name == "Pink";

            Console.WriteLine($"LOGIN-TEAM-CHECK: explicit team selection correct = {explicitTeamOk}");
            Console.WriteLine($"LOGIN-TEAM-CHECK: auto team selection #1 correct = {autoTeam1Ok}");
            Console.WriteLine($"LOGIN-TEAM-CHECK: auto team selection #2 tie-break correct = {autoTeam2Ok}");

            explicitTeamPlayer.Dispose();
            explicitTeamPlayer = null;

            autoTeamPlayer1.Dispose();
            autoTeamPlayer1 = null;

            autoTeamPlayer2.Dispose();
            autoTeamPlayer2 = null;

            await Task.Delay(500).ConfigureAwait(false);

            await RemoveAllRegions(adminGalaxy).ConfigureAwait(false);
            await adminGalaxy.Configure(BuildConfigurationXml(adminGalaxy, Array.Empty<TeamSpec>(), clusters)).ConfigureAwait(false);
            await Task.Delay(250).ConfigureAwait(false);

            try
            {
                Galaxy noTeamPlayer = await Galaxy.Connect(Uri, PlayerAuth, null).ConfigureAwait(false);
                noTeamPlayer.Dispose();
                Console.WriteLine("LOGIN-TEAM-CHECK: auto team with no teams available -> FAILED (unexpected success)");
            }
            catch (TeamSelectionFailedGameException exception)
            {
                Console.WriteLine($"LOGIN-TEAM-CHECK: auto team with no teams available -> OK ({exception.Message})");
            }
        }
        finally
        {
            if (explicitTeamPlayer is not null)
                explicitTeamPlayer.Dispose();

            if (autoTeamPlayer1 is not null)
                autoTeamPlayer1.Dispose();

            if (autoTeamPlayer2 is not null)
                autoTeamPlayer2.Dispose();

            if (adminGalaxy is not null && restoreConfigurationXml is not null)
                try
                {
                    await adminGalaxy.Configure(restoreConfigurationXml).ConfigureAwait(false);
                    Console.WriteLine("LOGIN-TEAM-CHECK: restore configuration OK");
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"LOGIN-TEAM-CHECK: restore configuration FAILED ({exception.GetType().Name}: {exception.Message})");
                }

            if (adminGalaxy is not null && restoreRegionsByCluster is not null)
                try
                {
                    await RestoreRegionsByCluster(adminGalaxy, restoreRegionsByCluster).ConfigureAwait(false);
                    Console.WriteLine("LOGIN-TEAM-CHECK: restore regions OK");
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"LOGIN-TEAM-CHECK: restore regions FAILED ({exception.GetType().Name}: {exception.Message})");
                }

            if (adminGalaxy is not null)
                adminGalaxy.Dispose();
        }
    }

    private static async Task RunAccountSessionCheck()
    {
        DatabaseAccountRow originalAccount = QueryAccountRow(PlayerAuth);
        DatabaseAccountRow originalAdminAccount = QueryAccountRow(AdminAuth);
        Galaxy? adminGalaxy = null;
        Galaxy? playerAdminGalaxy = null;
        Galaxy? playerGalaxy = null;
        Task? eventPump = null;
        ConcurrentQueue<FlattiverseEvent> playerEvents = new ConcurrentQueue<FlattiverseEvent>();
        ClassicShipControllable? ship = null;
        Cluster? adminCluster = null;
        string shipName = $"Acc{DateTimeOffset.UtcNow.ToUnixTimeSeconds() % 1000000:000000}";
        string neutralUnitName = $"AccountSyncPlanet{DateTime.UtcNow.Ticks}";

        if (HasFreshSession(originalAccount))
        {
            Console.WriteLine($"ACCOUNT-SESSION-CHECK: SKIPPED (account already has fresh session, {FormatSessionState(originalAccount)})");
            return;
        }

        if (HasFreshSession(originalAdminAccount))
        {
            Console.WriteLine($"ACCOUNT-SESSION-CHECK: SKIPPED (admin helper account already has fresh session, {FormatSessionState(originalAdminAccount)})");
            return;
        }

        try
        {
            Console.WriteLine("ACCOUNT-SESSION-CHECK: connecting same-account admin...");
            playerAdminGalaxy = await Galaxy.Connect(Uri, PlayerAdminAuth, null).ConfigureAwait(false);

            DatabaseAccountRow adminConnectedAccount = QueryAccountRow(PlayerAuth);
            bool adminSessionClaimed = adminConnectedAccount.SessionGalaxy == DatabaseGalaxyId &&
                                       adminConnectedAccount.SessionTeam == SpectatorsTeamId &&
                                       adminConnectedAccount.SessionPlayerKills == 0 &&
                                       adminConnectedAccount.SessionPlayerDeaths == 0 &&
                                       adminConnectedAccount.SessionNpcKills == 0 &&
                                       adminConnectedAccount.SessionNpcDeaths == 0 &&
                                       adminConnectedAccount.SessionNeutralDeaths == 0;

            Console.WriteLine($"ACCOUNT-SESSION-CHECK: admin session claimed in DB = {adminSessionClaimed}");

            try
            {
                Galaxy duplicatePlayerAfterAdmin = await Galaxy.Connect(Uri, PlayerAuth, TeamName).ConfigureAwait(false);
                duplicatePlayerAfterAdmin.Dispose();
                Console.WriteLine("ACCOUNT-SESSION-CHECK: player denied while same account uses admin key = False");
            }
            catch (AccountAlreadyLoggedInGameException exception)
            {
                Console.WriteLine($"ACCOUNT-SESSION-CHECK: player denied while same account uses admin key = True ({exception.Message})");
            }

            playerAdminGalaxy.Dispose();
            playerAdminGalaxy = null;

            bool adminSessionCleared = await WaitForSessionGalaxy(PlayerAdminAuth, null, 7000).ConfigureAwait(false);
            Console.WriteLine($"ACCOUNT-SESSION-CHECK: admin session cleared after disconnect = {adminSessionCleared}");

            Console.WriteLine("ACCOUNT-SESSION-CHECK: connecting helper admin...");
            adminGalaxy = await Galaxy.Connect(Uri, AdminAuth, null).ConfigureAwait(false);

            Console.WriteLine("ACCOUNT-SESSION-CHECK: connecting player...");
            playerGalaxy = await Galaxy.Connect(Uri, PlayerAuth, TeamName).ConfigureAwait(false);
            eventPump = StartEventPump("ACCOUNT-SESSION-CHECK", playerGalaxy, playerEvents);

            await Task.Delay(500).ConfigureAwait(false);
            DrainEvents(playerEvents);

            bool initialFieldsOk = playerGalaxy.Player.Admin == originalAccount.Admin &&
                                   playerGalaxy.Player.Rank == originalAccount.Rank &&
                                   playerGalaxy.Player.PlayerKills == originalAccount.StatsPlayerKills &&
                                   playerGalaxy.Player.PlayerDeaths == originalAccount.StatsPlayerDeaths &&
                                   playerGalaxy.Player.NpcKills == originalAccount.StatsNpcKills &&
                                   playerGalaxy.Player.NpcDeaths == originalAccount.StatsNpcDeaths &&
                                   playerGalaxy.Player.NeutralDeaths == originalAccount.StatsNeutralDeaths;

            Console.WriteLine($"ACCOUNT-SESSION-CHECK: initial player fields correct = {initialFieldsOk}");

            DatabaseAccountRow connectedAccount = QueryAccountRow(PlayerAuth);
            bool sessionClaimed = connectedAccount.SessionGalaxy == DatabaseGalaxyId &&
                                  connectedAccount.SessionTeam == playerGalaxy.Player.Team.Id &&
                                  connectedAccount.SessionPlayerKills == 0 &&
                                  connectedAccount.SessionPlayerDeaths == 0 &&
                                  connectedAccount.SessionNpcKills == 0 &&
                                  connectedAccount.SessionNpcDeaths == 0 &&
                                  connectedAccount.SessionNeutralDeaths == 0;

            Console.WriteLine($"ACCOUNT-SESSION-CHECK: session claimed in DB = {sessionClaimed}");

            try
            {
                Galaxy duplicatePlayer = await Galaxy.Connect(Uri, PlayerAuth, null).ConfigureAwait(false);
                duplicatePlayer.Dispose();
                Console.WriteLine("ACCOUNT-SESSION-CHECK: duplicate login denied = False");
            }
            catch (AccountAlreadyLoggedInGameException exception)
            {
                Console.WriteLine($"ACCOUNT-SESSION-CHECK: duplicate login denied = True ({exception.Message})");
            }

            try
            {
                Galaxy duplicateAdmin = await Galaxy.Connect(Uri, PlayerAdminAuth, null).ConfigureAwait(false);
                duplicateAdmin.Dispose();
                Console.WriteLine("ACCOUNT-SESSION-CHECK: admin denied while same account already plays = False");
            }
            catch (AccountAlreadyLoggedInGameException exception)
            {
                Console.WriteLine($"ACCOUNT-SESSION-CHECK: admin denied while same account already plays = True ({exception.Message})");
            }

            bool targetAdmin = !originalAccount.Admin;
            int targetRank = originalAccount.Rank + 7;

            ExecuteDatabaseNonQuery($"""
                UPDATE public.accounts
                SET "admin" = {FormatBool(targetAdmin)},
                    "rank" = {targetRank}
                WHERE "id" = {originalAccount.AccountId}
                """);

            bool accountRefreshObserved = await WaitForPlayerAccountState(playerGalaxy.Player, targetAdmin, targetRank, 7000).ConfigureAwait(false);
            Console.WriteLine($"ACCOUNT-SESSION-CHECK: admin/rank refresh observed = {accountRefreshObserved}");

            ship = await playerGalaxy.CreateClassicShip(shipName).ConfigureAwait(false);
            await ship.Continue().ConfigureAwait(false);

            bool shipAlive = await WaitForAliveState(ship, true, 3000).ConfigureAwait(false);
            Console.WriteLine($"ACCOUNT-SESSION-CHECK: ship alive after continue = {shipAlive}");

            bool initialHullOk = shipAlive &&
                                 ship.Hull.Exists &&
                                 ship.Hull.Maximum == 50f &&
                                 ship.Hull.Current == 50f;

            Console.WriteLine($"ACCOUNT-SESSION-CHECK: initial hull correct = {initialHullOk}");

            if (shipAlive)
            {
                if (!adminGalaxy.Clusters.TryGet(ship.Cluster.Id, out adminCluster))
                    throw new InvalidOperationException($"Admin cluster {ship.Cluster.Id} not found.");

                string unitXml =
                    $"<Planet Name=\"{neutralUnitName}\" X=\"{ship.Position.X:0.###}\" Y=\"{ship.Position.Y:0.###}\" Radius=\"50\" Gravity=\"0\" Type=\"OceanWorld\" Metal=\"0\" Carbon=\"0\" Hydrogen=\"0\" Silicon=\"0\" />";

                await adminCluster.SetUnit(unitXml).ConfigureAwait(false);

                bool shipDead = await WaitForAliveState(ship, false, 5000).ConfigureAwait(false);
                Console.WriteLine($"ACCOUNT-SESSION-CHECK: neutral death observed = {shipDead}");

                await Task.Delay(3500).ConfigureAwait(false);
                DrainEvents(playerEvents);

                DatabaseAccountRow afterNeutralDeath = QueryAccountRow(PlayerAuth);
                bool neutralDeathUpdated = playerGalaxy.Player.NeutralDeaths == originalAccount.StatsNeutralDeaths + 1 &&
                                           afterNeutralDeath.StatsNeutralDeaths == originalAccount.StatsNeutralDeaths + 1 &&
                                           afterNeutralDeath.SessionNeutralDeaths == 1;

                Console.WriteLine($"ACCOUNT-SESSION-CHECK: neutral death stats updated = {neutralDeathUpdated}");

                await adminCluster.RemoveUnit(neutralUnitName).ConfigureAwait(false);
                adminCluster = null;
            }

            playerGalaxy.Dispose();
            playerGalaxy = null;

            if (eventPump is not null)
                await Task.WhenAny(eventPump, Task.Delay(1000)).ConfigureAwait(false);

            bool sessionCleared = await WaitForSessionGalaxy(PlayerAuth, null, 7000).ConfigureAwait(false);
            Console.WriteLine($"ACCOUNT-SESSION-CHECK: session cleared after disconnect = {sessionCleared}");

            Galaxy reconnectGalaxy = await Galaxy.Connect(Uri, PlayerAuth, null).ConfigureAwait(false);
            Console.WriteLine($"ACCOUNT-SESSION-CHECK: reconnect after cleanup succeeded = {reconnectGalaxy.Player.Active}");
            reconnectGalaxy.Dispose();

            bool secondSessionCleared = await WaitForSessionGalaxy(PlayerAuth, null, 7000).ConfigureAwait(false);
            Console.WriteLine($"ACCOUNT-SESSION-CHECK: session cleared after reconnect dispose = {secondSessionCleared}");
        }
        finally
        {
            if (playerAdminGalaxy is not null)
                playerAdminGalaxy.Dispose();

            if (playerGalaxy is not null)
                playerGalaxy.Dispose();

            if (eventPump is not null)
                await Task.WhenAny(eventPump, Task.Delay(1000)).ConfigureAwait(false);

            if (adminGalaxy is not null && adminCluster is not null)
                try
                {
                    await adminCluster.RemoveUnit(neutralUnitName).ConfigureAwait(false);
                }
                catch (Exception)
                {
                }

            if (adminGalaxy is not null)
                adminGalaxy.Dispose();

            await WaitForSessionGalaxy(PlayerAdminAuth, null, 7000).ConfigureAwait(false);
            await WaitForSessionGalaxy(PlayerAuth, null, 7000).ConfigureAwait(false);
            RestoreAccountRow(originalAccount);
            await WaitForSessionGalaxy(AdminAuth, null, 7000).ConfigureAwait(false);
            RestoreAccountRow(originalAdminAccount);
            Console.WriteLine("ACCOUNT-SESSION-CHECK: account row restored");
        }
    }

    private static async Task RunHullNeutralDeathCheck()
    {
        DatabaseAccountRow originalAccount = QueryAccountRow(PlayerAuth);
        Galaxy? adminGalaxy = null;
        Galaxy? playerGalaxy = null;
        Cluster? adminCluster = null;
        ClassicShipControllable? ship = null;
        ConcurrentQueue<FlattiverseEvent> playerEvents = new ConcurrentQueue<FlattiverseEvent>();
        Task? eventPump = null;
        string shipName = $"HullCheck{Environment.ProcessId}";
        string neutralUnitName = $"HullCheckPlanet{Environment.ProcessId}";

        if (HasFreshSession(originalAccount))
        {
            Console.WriteLine($"HULL-NEUTRAL-DEATH-CHECK: SKIPPED (account already has fresh session, {FormatSessionState(originalAccount)})");
            return;
        }

        try
        {
            Console.WriteLine("HULL-NEUTRAL-DEATH-CHECK: connecting admin...");
            adminGalaxy = await Galaxy.Connect(Uri, AdminAuth, null).ConfigureAwait(false);

            Console.WriteLine("HULL-NEUTRAL-DEATH-CHECK: connecting player...");
            playerGalaxy = await Galaxy.Connect(Uri, PlayerAuth, null).ConfigureAwait(false);
            eventPump = StartEventPump("HULL-NEUTRAL-DEATH-CHECK", playerGalaxy, playerEvents);

            await Task.Delay(500).ConfigureAwait(false);
            DrainEvents(playerEvents);

            ship = await playerGalaxy.CreateClassicShip(shipName).ConfigureAwait(false);
            await ship.Continue().ConfigureAwait(false);

            bool shipAlive = await WaitForAliveState(ship, true, 3000).ConfigureAwait(false);
            Console.WriteLine($"HULL-NEUTRAL-DEATH-CHECK: ship alive after continue = {shipAlive}");

            bool initialHullOk = shipAlive &&
                                 ship.Hull.Exists &&
                                 ship.Hull.Maximum == 50f &&
                                 ship.Hull.Current == 50f;

            Console.WriteLine($"HULL-NEUTRAL-DEATH-CHECK: initial hull correct = {initialHullOk}");

            if (!shipAlive)
                return;

            if (!adminGalaxy.Clusters.TryGet(ship.Cluster.Id, out adminCluster))
                throw new InvalidOperationException($"Admin cluster {ship.Cluster.Id} not found.");

            string unitXml =
                $"<Planet Name=\"{neutralUnitName}\" X=\"{ship.Position.X:0.###}\" Y=\"{ship.Position.Y:0.###}\" Radius=\"50\" Gravity=\"0\" Type=\"OceanWorld\" Metal=\"0\" Carbon=\"0\" Hydrogen=\"0\" Silicon=\"0\" />";

            await adminCluster.SetUnit(unitXml).ConfigureAwait(false);

            bool shipDead = await WaitForAliveState(ship, false, 5000).ConfigureAwait(false);
            Console.WriteLine($"HULL-NEUTRAL-DEATH-CHECK: neutral death observed = {shipDead}");

            DateTime hullDeadline = DateTime.UtcNow.AddMilliseconds(1000);

            while (DateTime.UtcNow < hullDeadline && ship.Hull.Current != 0f)
                await Task.Delay(50).ConfigureAwait(false);

            bool hullDepleted = ship.Hull.Current == 0f;
            Console.WriteLine($"HULL-NEUTRAL-DEATH-CHECK: hull depleted after death = {hullDepleted} (current={ship.Hull.Current:0.###})");

            await Task.Delay(3500).ConfigureAwait(false);
            DrainEvents(playerEvents);

            DatabaseAccountRow afterNeutralDeath = QueryAccountRow(PlayerAuth);
            bool neutralDeathUpdated = playerGalaxy.Player.NeutralDeaths == originalAccount.StatsNeutralDeaths + 1 &&
                                       afterNeutralDeath.StatsNeutralDeaths == originalAccount.StatsNeutralDeaths + 1 &&
                                       afterNeutralDeath.SessionNeutralDeaths == 1;

            Console.WriteLine($"HULL-NEUTRAL-DEATH-CHECK: neutral death stats updated = {neutralDeathUpdated}");
        }
        finally
        {
            if (playerGalaxy is not null)
                playerGalaxy.Dispose();

            if (eventPump is not null)
                await Task.WhenAny(eventPump, Task.Delay(1000)).ConfigureAwait(false);

            if (adminGalaxy is not null && adminCluster is not null)
                try
                {
                    await adminCluster.RemoveUnit(neutralUnitName).ConfigureAwait(false);
                }
                catch (Exception)
                {
                }

            if (adminGalaxy is not null)
                adminGalaxy.Dispose();

            await WaitForSessionGalaxy(PlayerAuth, null, 7000).ConfigureAwait(false);
            RestoreAccountRow(originalAccount);
            Console.WriteLine("HULL-NEUTRAL-DEATH-CHECK: account row restored");
        }
    }

    private static async Task RunFriendlyStatsCheck()
    {
        DatabaseAccountRow originalAccount = QueryAccountRow(PlayerAuth);
        Galaxy? playerGalaxy = null;
        ConcurrentQueue<FlattiverseEvent> playerEvents = new ConcurrentQueue<FlattiverseEvent>();
        Task? eventPump = null;
        string shipName = $"FriendlyCheck{Environment.ProcessId}";

        if (HasFreshSession(originalAccount))
        {
            Console.WriteLine($"FRIENDLY-STATS-CHECK: SKIPPED (account already has fresh session, {FormatSessionState(originalAccount)})");
            return;
        }

        try
        {
            Console.WriteLine("FRIENDLY-STATS-CHECK: connecting player...");
            playerGalaxy = await Galaxy.Connect(Uri, PlayerAuth, null).ConfigureAwait(false);
            eventPump = StartEventPump("FRIENDLY-STATS-CHECK", playerGalaxy, playerEvents);

            await Task.Delay(500).ConfigureAwait(false);
            DrainEvents(playerEvents);

            uint initialPlayerDeaths = playerGalaxy.Player.Score.PlayerDeaths;
            uint initialPlayerFriendlyDeaths = playerGalaxy.Player.Score.FriendlyDeaths;
            uint initialTeamDeaths = playerGalaxy.Player.Team.Score.PlayerDeaths;
            uint initialTeamFriendlyDeaths = playerGalaxy.Player.Team.Score.FriendlyDeaths;

            ClassicShipControllable ship = await playerGalaxy.CreateClassicShip(shipName).ConfigureAwait(false);
            await ship.Continue().ConfigureAwait(false);

            bool alive = await WaitForAliveState(ship, true, 3000).ConfigureAwait(false);
            Console.WriteLine($"FRIENDLY-STATS-CHECK: ship alive after continue = {alive}");

            if (!alive)
                return;

            if (!playerGalaxy.Player.ControllableInfos.TryGet(ship.Id, out ControllableInfo? controllableInfoBeforeSuicide) || controllableInfoBeforeSuicide is null)
            {
                Console.WriteLine("FRIENDLY-STATS-CHECK: controllable info missing before suicide");
                return;
            }

            uint initialControllableFriendlyDeaths = controllableInfoBeforeSuicide.Score.FriendlyDeaths;

            DrainEvents(playerEvents);
            await ship.Suicide().ConfigureAwait(false);

            bool dead = await WaitForAliveState(ship, false, 3000).ConfigureAwait(false);
            Console.WriteLine($"FRIENDLY-STATS-CHECK: suicide death observed = {dead}");

            await Task.Delay(250).ConfigureAwait(false);
            List<FlattiverseEvent> eventsAfterSuicide = DrainEvents(playerEvents);
            bool playerScoreUpdated = false;
            bool teamScoreUpdated = false;
            bool controllableScoreUpdated = false;

            foreach (FlattiverseEvent @event in eventsAfterSuicide)
                if (@event is PlayerScoreUpdatedEvent playerScoreUpdatedEvent &&
                    playerScoreUpdatedEvent.Player.Id == playerGalaxy.Player.Id &&
                    playerScoreUpdatedEvent.NewPlayerDeaths == initialPlayerDeaths &&
                    playerScoreUpdatedEvent.NewFriendlyDeaths == initialPlayerFriendlyDeaths + 1U)
                    playerScoreUpdated = true;
                else if (@event is TeamScoreUpdatedEvent teamScoreUpdatedEvent &&
                         teamScoreUpdatedEvent.Team.Id == playerGalaxy.Player.Team.Id &&
                         teamScoreUpdatedEvent.NewPlayerDeaths == initialTeamDeaths &&
                         teamScoreUpdatedEvent.NewFriendlyDeaths == initialTeamFriendlyDeaths + 1U)
                    teamScoreUpdated = true;
                else if (@event is ControllableInfoScoreUpdatedEvent controllableInfoScoreUpdatedEvent &&
                         controllableInfoScoreUpdatedEvent.Player.Id == playerGalaxy.Player.Id &&
                         controllableInfoScoreUpdatedEvent.ControllableInfo.Id == ship.Id &&
                         controllableInfoScoreUpdatedEvent.NewFriendlyDeaths == initialControllableFriendlyDeaths + 1U)
                    controllableScoreUpdated = true;

            bool playerScoreApplied = playerGalaxy.Player.Score.PlayerDeaths == initialPlayerDeaths &&
                                      playerGalaxy.Player.Score.FriendlyDeaths == initialPlayerFriendlyDeaths + 1U;
            bool teamScoreApplied = playerGalaxy.Player.Team.Score.PlayerDeaths == initialTeamDeaths &&
                                    playerGalaxy.Player.Team.Score.FriendlyDeaths == initialTeamFriendlyDeaths + 1U;
            bool controllableScoreApplied = playerGalaxy.Player.ControllableInfos.TryGet(ship.Id, out ControllableInfo? controllableInfoAfterSuicide) &&
                                            controllableInfoAfterSuicide is not null &&
                                            controllableInfoAfterSuicide.Score.FriendlyDeaths == initialControllableFriendlyDeaths + 1U;

            await Task.Delay(3500).ConfigureAwait(false);
            DatabaseAccountRow afterFriendlyDeath = QueryAccountRow(PlayerAuth);
            bool friendlyStatsUpdated = afterFriendlyDeath.StatsFriendlyDeaths == originalAccount.StatsFriendlyDeaths + 1 &&
                                        afterFriendlyDeath.SessionFriendlyDeaths == 1 &&
                                        afterFriendlyDeath.StatsPlayerDeaths == originalAccount.StatsPlayerDeaths &&
                                        afterFriendlyDeath.SessionPlayerDeaths == 0;

            Console.WriteLine($"FRIENDLY-STATS-CHECK: player score event = {playerScoreUpdated}");
            Console.WriteLine($"FRIENDLY-STATS-CHECK: team score event = {teamScoreUpdated}");
            Console.WriteLine($"FRIENDLY-STATS-CHECK: controllable score event = {controllableScoreUpdated}");
            Console.WriteLine($"FRIENDLY-STATS-CHECK: player score applied = {playerScoreApplied}");
            Console.WriteLine($"FRIENDLY-STATS-CHECK: team score applied = {teamScoreApplied}");
            Console.WriteLine($"FRIENDLY-STATS-CHECK: controllable score applied = {controllableScoreApplied}");
            Console.WriteLine($"FRIENDLY-STATS-CHECK: db row statsFriendlyDeaths={afterFriendlyDeath.StatsFriendlyDeaths}, sessionFriendlyDeaths={afterFriendlyDeath.SessionFriendlyDeaths}, statsPlayerDeaths={afterFriendlyDeath.StatsPlayerDeaths}, sessionPlayerDeaths={afterFriendlyDeath.SessionPlayerDeaths}, sessionGalaxy={(afterFriendlyDeath.SessionGalaxy is null ? "null" : afterFriendlyDeath.SessionGalaxy.Value.ToString())}");
            Console.WriteLine($"FRIENDLY-STATS-CHECK: db stats updated = {friendlyStatsUpdated}");
        }
        finally
        {
            if (playerGalaxy is not null)
                playerGalaxy.Dispose();

            if (eventPump is not null)
                await Task.WhenAny(eventPump, Task.Delay(1000)).ConfigureAwait(false);

            await WaitForSessionGalaxy(PlayerAuth, null, 7000).ConfigureAwait(false);
            RestoreAccountRow(originalAccount);
            Console.WriteLine("FRIENDLY-STATS-CHECK: account row restored");
        }
    }

    private static async Task RunAvatarDownloadCheck(string[] args)
    {
        if (args.Length < 3 || args.Length > 4)
        {
            Console.WriteLine("AVATAR-DOWNLOAD-CHECK: usage Development --avatar-download-check <uri> <auth> [team|-]");
            return;
        }

        string uri = args[1];
        string auth = args[2];
        string? team = args.Length >= 4 && args[3] != "-" ? args[3] : null;
        byte[] originalSmallAvatar = new byte[] { 0x01, 0x23, 0x45, 0x67 };
        byte[] originalBigAvatar = new byte[] { 0x89, 0xAB, 0xCD, 0xEF, 0x10 };
        byte[] changedSmallAvatar = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF };
        byte[] changedBigAvatar = new byte[] { 0xCA, 0xFE, 0xBA, 0xBE, 0x11, 0x22 };
        DatabaseAvatarRow avatarRow = QueryAvatarRow(auth);
        DatabaseAccountRow accountRow = QueryAccountRow(auth);
        Galaxy? playerGalaxy = null;
        Galaxy? spectatorGalaxy = null;

        if (HasFreshSession(accountRow))
        {
            Console.WriteLine($"AVATAR-DOWNLOAD-CHECK: SKIPPED (account already has fresh session, {FormatSessionState(accountRow)})");
            return;
        }

        try
        {
            UpdateAvatarRow(avatarRow.AccountId, originalSmallAvatar, originalBigAvatar);

            playerGalaxy = await Galaxy.Connect(uri, auth, team).ConfigureAwait(false);
            spectatorGalaxy = await Galaxy.Connect(uri, null, null).ConfigureAwait(false);

            await Task.Delay(250).ConfigureAwait(false);

            Player spectatorView = FindPlayerById(spectatorGalaxy, playerGalaxy.Player.Id);

            byte[] selfSmallAvatar = await playerGalaxy.Player.DownloadSmallAvatar().ConfigureAwait(false);
            byte[] selfBigAvatar = await playerGalaxy.Player.DownloadBigAvatar().ConfigureAwait(false);
            byte[] spectatorSmallAvatar = await spectatorView.DownloadSmallAvatar().ConfigureAwait(false);
            byte[] spectatorBigAvatar = await spectatorView.DownloadBigAvatar().ConfigureAwait(false);

            bool initialSelfMatches = selfSmallAvatar.SequenceEqual(originalSmallAvatar) &&
                                      selfBigAvatar.SequenceEqual(originalBigAvatar);
            bool initialSpectatorMatches = spectatorSmallAvatar.SequenceEqual(originalSmallAvatar) &&
                                           spectatorBigAvatar.SequenceEqual(originalBigAvatar);
            bool hasAvatarInitial = playerGalaxy.Player.HasAvatar && spectatorView.HasAvatar;

            Console.WriteLine($"AVATAR-DOWNLOAD-CHECK: has avatar initially = {hasAvatarInitial}");
            Console.WriteLine($"AVATAR-DOWNLOAD-CHECK: self initial avatar download = {initialSelfMatches}");
            Console.WriteLine($"AVATAR-DOWNLOAD-CHECK: spectator initial avatar download = {initialSpectatorMatches}");

            UpdateAvatarRow(avatarRow.AccountId, changedSmallAvatar, changedBigAvatar);

            byte[] cachedSelfSmallAvatar = await playerGalaxy.Player.DownloadSmallAvatar().ConfigureAwait(false);
            byte[] cachedSelfBigAvatar = await playerGalaxy.Player.DownloadBigAvatar().ConfigureAwait(false);
            byte[] cachedSpectatorSmallAvatar = await spectatorView.DownloadSmallAvatar().ConfigureAwait(false);
            byte[] cachedSpectatorBigAvatar = await spectatorView.DownloadBigAvatar().ConfigureAwait(false);

            bool selfCached = cachedSelfSmallAvatar.SequenceEqual(originalSmallAvatar) &&
                              cachedSelfBigAvatar.SequenceEqual(originalBigAvatar);
            bool spectatorCached = cachedSpectatorSmallAvatar.SequenceEqual(originalSmallAvatar) &&
                                   cachedSpectatorBigAvatar.SequenceEqual(originalBigAvatar);

            Console.WriteLine($"AVATAR-DOWNLOAD-CHECK: self uses login-time cache = {selfCached}");
            Console.WriteLine($"AVATAR-DOWNLOAD-CHECK: spectator uses login-time cache = {spectatorCached}");

            spectatorGalaxy.Dispose();
            spectatorGalaxy = null;
            playerGalaxy.Dispose();
            playerGalaxy = null;

            await Task.Delay(250).ConfigureAwait(false);

            UpdateAvatarRow(avatarRow.AccountId, null, null);

            playerGalaxy = await Galaxy.Connect(uri, auth, team).ConfigureAwait(false);
            spectatorGalaxy = await Galaxy.Connect(uri, null, null).ConfigureAwait(false);

            await Task.Delay(250).ConfigureAwait(false);

            spectatorView = FindPlayerById(spectatorGalaxy, playerGalaxy.Player.Id);

            bool hasAvatarMissing = !playerGalaxy.Player.HasAvatar && !spectatorView.HasAvatar;
            bool selfThrows = false;
            bool spectatorThrows = false;

            try
            {
                await playerGalaxy.Player.DownloadSmallAvatar().ConfigureAwait(false);
            }
            catch (AvatarNotAvailableGameException)
            {
                selfThrows = true;
            }

            try
            {
                await spectatorView.DownloadBigAvatar().ConfigureAwait(false);
            }
            catch (AvatarNotAvailableGameException)
            {
                spectatorThrows = true;
            }

            Console.WriteLine($"AVATAR-DOWNLOAD-CHECK: has avatar missing = {hasAvatarMissing}");
            Console.WriteLine($"AVATAR-DOWNLOAD-CHECK: self throws without avatar = {selfThrows}");
            Console.WriteLine($"AVATAR-DOWNLOAD-CHECK: spectator throws without avatar = {spectatorThrows}");
        }
        finally
        {
            if (spectatorGalaxy is not null)
                spectatorGalaxy.Dispose();

            if (playerGalaxy is not null)
                playerGalaxy.Dispose();

            await Task.Delay(250).ConfigureAwait(false);
            RestoreAvatarRow(avatarRow);
            Console.WriteLine("AVATAR-DOWNLOAD-CHECK: avatar row restored");
        }
    }

    private static async Task RunSelfDisclosureCheck()
    {
        DatabaseAccountRow playerAccount = QueryAccountRow(PlayerAuth);
        DatabaseAccountRow adminAccount = QueryAccountRow(AdminAuth);
        DatabaseAccountRow helperAdminAccount = QueryAccountRow(PlayerAdminAuth);
        Galaxy? adminGalaxy = null;
        Galaxy? helperAdminGalaxy = null;
        Galaxy? optionalPlayerGalaxy = null;
        Galaxy? disclosedPlayerGalaxy = null;
        Galaxy? spectatorGalaxy = null;
        Task? adminEventPump = null;
        Task? spectatorEventPump = null;
        ConcurrentQueue<FlattiverseEvent> adminEvents = new ConcurrentQueue<FlattiverseEvent>();
        ConcurrentQueue<FlattiverseEvent> spectatorEvents = new ConcurrentQueue<FlattiverseEvent>();
        string? restoreConfigurationXml = null;

        if (HasFreshSession(playerAccount))
        {
            Console.WriteLine($"SELF-DISCLOSURE-CHECK: SKIPPED (player account already has fresh session, {FormatSessionState(playerAccount)})");
            return;
        }

        if (HasFreshSession(adminAccount))
        {
            Console.WriteLine($"SELF-DISCLOSURE-CHECK: SKIPPED (admin account already has fresh session, {FormatSessionState(adminAccount)})");
            return;
        }

        if (HasFreshSession(helperAdminAccount))
        {
            Console.WriteLine($"SELF-DISCLOSURE-CHECK: SKIPPED (helper admin account already has fresh session, {FormatSessionState(helperAdminAccount)})");
            return;
        }

        try
        {
            adminGalaxy = await Galaxy.Connect(Uri, AdminAuth, null).ConfigureAwait(false);
            adminEventPump = StartEventPump("SELF-DISCLOSURE-ADMIN", adminGalaxy, adminEvents);
            restoreConfigurationXml = BuildConfigurationXml(adminGalaxy, null, null, adminGalaxy.RequiresSelfDisclosure);

            await Task.Delay(250).ConfigureAwait(false);
            DrainEvents(adminEvents);

            await adminGalaxy.Configure(BuildConfigurationXml(adminGalaxy, null, null, false)).ConfigureAwait(false);

            bool requiresFalseObserved = await WaitForRequiresSelfDisclosure(adminGalaxy, false, 3000).ConfigureAwait(false);
            Console.WriteLine($"SELF-DISCLOSURE-CHECK: requires false observed = {requiresFalseObserved}");

            optionalPlayerGalaxy = await Galaxy.Connect(Uri, PlayerAuth, null).ConfigureAwait(false);

            await Task.Delay(250).ConfigureAwait(false);

            bool optionalSelfNull = optionalPlayerGalaxy.Player.RuntimeDisclosure is null &&
                                    optionalPlayerGalaxy.Player.BuildDisclosure is null;
            bool optionalAdminNull = adminGalaxy.Players.TryGet(optionalPlayerGalaxy.Player.Id, out Player? optionalAdminView) &&
                                     optionalAdminView.RuntimeDisclosure is null &&
                                     optionalAdminView.BuildDisclosure is null;
            bool optionalAdminJoinEventNull = false;
            List<FlattiverseEvent> optionalAdminEvents = DrainEvents(adminEvents);

            foreach (FlattiverseEvent @event in optionalAdminEvents)
                if (@event is JoinedPlayerEvent joinedPlayerEvent &&
                    joinedPlayerEvent.Player.Id == optionalPlayerGalaxy.Player.Id &&
                    joinedPlayerEvent.Player.RuntimeDisclosure is null &&
                    joinedPlayerEvent.Player.BuildDisclosure is null)
                    optionalAdminJoinEventNull = true;

            Console.WriteLine($"SELF-DISCLOSURE-CHECK: optional connect without disclosure = {optionalSelfNull}");
            Console.WriteLine($"SELF-DISCLOSURE-CHECK: admin sees null disclosure on optional player = {optionalAdminNull}");
            Console.WriteLine($"SELF-DISCLOSURE-CHECK: admin receives PlayerJoined with null disclosure = {optionalAdminJoinEventNull}");

            optionalPlayerGalaxy.Dispose();
            optionalPlayerGalaxy = null;

            bool optionalSessionCleared = await WaitForSessionGalaxy(PlayerAuth, null, 7000).ConfigureAwait(false);
            Console.WriteLine($"SELF-DISCLOSURE-CHECK: optional session cleared = {optionalSessionCleared}");

            await adminGalaxy.Configure(BuildConfigurationXml(adminGalaxy, null, null, true)).ConfigureAwait(false);

            bool requiresTrueObserved = await WaitForRequiresSelfDisclosure(adminGalaxy, true, 3000).ConfigureAwait(false);
            Console.WriteLine($"SELF-DISCLOSURE-CHECK: requires true observed = {requiresTrueObserved}");

            spectatorGalaxy = await Galaxy.Connect(Uri, null, null).ConfigureAwait(false);
            spectatorEventPump = StartEventPump("SELF-DISCLOSURE-SPECTATOR", spectatorGalaxy, spectatorEvents);

            await Task.Delay(250).ConfigureAwait(false);
            DrainEvents(spectatorEvents);

            bool spectatorOptional = spectatorGalaxy.Player.Kind == PlayerKind.Spectator &&
                                     spectatorGalaxy.Player.RuntimeDisclosure is null &&
                                     spectatorGalaxy.Player.BuildDisclosure is null;
            Console.WriteLine($"SELF-DISCLOSURE-CHECK: spectator still connects without disclosure = {spectatorOptional}");

            try
            {
                Galaxy missingDisclosureGalaxy = await Galaxy.Connect(Uri, PlayerAuth, null).ConfigureAwait(false);
                missingDisclosureGalaxy.Dispose();
                Console.WriteLine("SELF-DISCLOSURE-CHECK: missing disclosure denied = False");
            }
            catch (SelfDisclosureRequiredGameException exception)
            {
                Console.WriteLine($"SELF-DISCLOSURE-CHECK: missing disclosure denied = True ({exception.Message})");
            }

            byte malformedCode = await ConnectAndReadInitialExceptionCode(
                $"{Uri}?version=13&auth={PlayerAuth}&runtimeDisclosure=123&buildDisclosure=000000000000").ConfigureAwait(false);

            Console.WriteLine($"SELF-DISCLOSURE-CHECK: malformed disclosure rejected with 0x0D = {malformedCode == 0x0D}");

            RuntimeDisclosure runtimeDisclosure = new RuntimeDisclosure(
                RuntimeDisclosureLevel.Manual,
                RuntimeDisclosureLevel.Assisted,
                RuntimeDisclosureLevel.Automated,
                RuntimeDisclosureLevel.Autonomous,
                RuntimeDisclosureLevel.AiControlled,
                RuntimeDisclosureLevel.Manual,
                RuntimeDisclosureLevel.Automated,
                RuntimeDisclosureLevel.Assisted,
                RuntimeDisclosureLevel.Autonomous,
                RuntimeDisclosureLevel.Manual);

            BuildDisclosure buildDisclosure = new BuildDisclosure(
                BuildDisclosureLevel.AgenticTool,
                BuildDisclosureLevel.PaidLlm,
                BuildDisclosureLevel.SearchOnly,
                BuildDisclosureLevel.None,
                BuildDisclosureLevel.PaidLlm,
                BuildDisclosureLevel.AgenticTool,
                BuildDisclosureLevel.IntegratedLlm,
                BuildDisclosureLevel.PaidLlm,
                BuildDisclosureLevel.FreeLlm,
                BuildDisclosureLevel.AgenticTool,
                BuildDisclosureLevel.SearchOnly,
                BuildDisclosureLevel.None);

            disclosedPlayerGalaxy = await Galaxy.Connect(Uri, PlayerAuth, null, runtimeDisclosure, buildDisclosure).ConfigureAwait(false);

            await Task.Delay(250).ConfigureAwait(false);

            bool disclosedSelfMatches = disclosedPlayerGalaxy.Player.RuntimeDisclosure is not null &&
                                        disclosedPlayerGalaxy.Player.BuildDisclosure is not null &&
                                        disclosedPlayerGalaxy.Player.RuntimeDisclosure[RuntimeDisclosureAspect.EngineControl] == RuntimeDisclosureLevel.Manual &&
                                        disclosedPlayerGalaxy.Player.RuntimeDisclosure[RuntimeDisclosureAspect.WeaponTargetSelection] == RuntimeDisclosureLevel.AiControlled &&
                                        disclosedPlayerGalaxy.Player.BuildDisclosure[BuildDisclosureAspect.SoftwareDesign] == BuildDisclosureLevel.AgenticTool &&
                                        disclosedPlayerGalaxy.Player.BuildDisclosure[BuildDisclosureAspect.Chat] == BuildDisclosureLevel.None;

            bool disclosedAdminMatches = adminGalaxy.Players.TryGet(disclosedPlayerGalaxy.Player.Id, out Player? disclosedAdminView) &&
                                         disclosedAdminView.RuntimeDisclosure is not null &&
                                         disclosedAdminView.BuildDisclosure is not null &&
                                         disclosedAdminView.RuntimeDisclosure[RuntimeDisclosureAspect.Navigation] == RuntimeDisclosureLevel.Assisted &&
                                         disclosedAdminView.RuntimeDisclosure[RuntimeDisclosureAspect.LoadoutControl] == RuntimeDisclosureLevel.Autonomous &&
                                         disclosedAdminView.BuildDisclosure[BuildDisclosureAspect.ScannerControl] == BuildDisclosureLevel.IntegratedLlm &&
                                         disclosedAdminView.BuildDisclosure[BuildDisclosureAspect.MissionControl] == BuildDisclosureLevel.SearchOnly;
            bool disclosedSpectatorMatches = spectatorGalaxy.Players.TryGet(disclosedPlayerGalaxy.Player.Id, out Player? disclosedSpectatorView) &&
                                             disclosedSpectatorView.RuntimeDisclosure is not null &&
                                             disclosedSpectatorView.BuildDisclosure is not null &&
                                             disclosedSpectatorView.RuntimeDisclosure[RuntimeDisclosureAspect.EngineControl] == RuntimeDisclosureLevel.Manual &&
                                             disclosedSpectatorView.RuntimeDisclosure[RuntimeDisclosureAspect.WeaponTargetSelection] == RuntimeDisclosureLevel.AiControlled &&
                                             disclosedSpectatorView.BuildDisclosure[BuildDisclosureAspect.SoftwareDesign] == BuildDisclosureLevel.AgenticTool &&
                                             disclosedSpectatorView.BuildDisclosure[BuildDisclosureAspect.Chat] == BuildDisclosureLevel.None;
            bool disclosedAdminJoinEventMatches = false;
            bool disclosedSpectatorJoinEventMatches = false;
            List<FlattiverseEvent> disclosedAdminEvents = DrainEvents(adminEvents);
            List<FlattiverseEvent> disclosedSpectatorEvents = DrainEvents(spectatorEvents);

            foreach (FlattiverseEvent @event in disclosedAdminEvents)
                if (@event is JoinedPlayerEvent joinedPlayerEvent &&
                    joinedPlayerEvent.Player.Id == disclosedPlayerGalaxy.Player.Id &&
                    joinedPlayerEvent.Player.RuntimeDisclosure is not null &&
                    joinedPlayerEvent.Player.BuildDisclosure is not null &&
                    joinedPlayerEvent.Player.RuntimeDisclosure[RuntimeDisclosureAspect.Navigation] == RuntimeDisclosureLevel.Assisted &&
                    joinedPlayerEvent.Player.BuildDisclosure[BuildDisclosureAspect.ScannerControl] == BuildDisclosureLevel.IntegratedLlm)
                    disclosedAdminJoinEventMatches = true;

            foreach (FlattiverseEvent @event in disclosedSpectatorEvents)
                if (@event is JoinedPlayerEvent joinedPlayerEvent &&
                    joinedPlayerEvent.Player.Id == disclosedPlayerGalaxy.Player.Id &&
                    joinedPlayerEvent.Player.RuntimeDisclosure is not null &&
                    joinedPlayerEvent.Player.BuildDisclosure is not null &&
                    joinedPlayerEvent.Player.RuntimeDisclosure[RuntimeDisclosureAspect.EngineControl] == RuntimeDisclosureLevel.Manual &&
                    joinedPlayerEvent.Player.BuildDisclosure[BuildDisclosureAspect.Chat] == BuildDisclosureLevel.None)
                    disclosedSpectatorJoinEventMatches = true;

            Console.WriteLine($"SELF-DISCLOSURE-CHECK: disclosed self parsed = {disclosedSelfMatches}");
            Console.WriteLine($"SELF-DISCLOSURE-CHECK: disclosed admin view parsed = {disclosedAdminMatches}");
            Console.WriteLine($"SELF-DISCLOSURE-CHECK: disclosed spectator view parsed = {disclosedSpectatorMatches}");
            Console.WriteLine($"SELF-DISCLOSURE-CHECK: admin receives PlayerJoined with disclosure = {disclosedAdminJoinEventMatches}");
            Console.WriteLine($"SELF-DISCLOSURE-CHECK: spectator receives PlayerJoined with disclosure = {disclosedSpectatorJoinEventMatches}");

            disclosedPlayerGalaxy.Dispose();
            disclosedPlayerGalaxy = null;

            bool disclosedSessionCleared = await WaitForSessionGalaxy(PlayerAuth, null, 7000).ConfigureAwait(false);
            Console.WriteLine($"SELF-DISCLOSURE-CHECK: disclosed session cleared = {disclosedSessionCleared}");

            helperAdminGalaxy = await Galaxy.Connect(Uri, PlayerAdminAuth, null).ConfigureAwait(false);
            bool helperAdminOptional = helperAdminGalaxy.Player.Kind == PlayerKind.Admin &&
                                       helperAdminGalaxy.Player.RuntimeDisclosure is null &&
                                       helperAdminGalaxy.Player.BuildDisclosure is null;
            Console.WriteLine($"SELF-DISCLOSURE-CHECK: admin still connects without disclosure = {helperAdminOptional}");
        }
        finally
        {
            if (optionalPlayerGalaxy is not null)
                optionalPlayerGalaxy.Dispose();

            if (disclosedPlayerGalaxy is not null)
                disclosedPlayerGalaxy.Dispose();

            if (spectatorGalaxy is not null)
                spectatorGalaxy.Dispose();

            if (helperAdminGalaxy is not null)
                helperAdminGalaxy.Dispose();

            await WaitForSessionGalaxy(PlayerAuth, null, 7000).ConfigureAwait(false);
            await WaitForSessionGalaxy(PlayerAdminAuth, null, 7000).ConfigureAwait(false);

            if (adminGalaxy is not null && restoreConfigurationXml is not null)
                try
                {
                    await adminGalaxy.Configure(restoreConfigurationXml).ConfigureAwait(false);
                    Console.WriteLine("SELF-DISCLOSURE-CHECK: restore configuration OK");
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"SELF-DISCLOSURE-CHECK: restore configuration FAILED ({exception.GetType().Name}: {exception.Message})");
                }

            if (adminGalaxy is not null)
                adminGalaxy.Dispose();

            await WaitForSessionGalaxy(AdminAuth, null, 7000).ConfigureAwait(false);

            if (adminEventPump is not null)
                await Task.WhenAny(adminEventPump, Task.Delay(1000)).ConfigureAwait(false);

            if (spectatorEventPump is not null)
                await Task.WhenAny(spectatorEventPump, Task.Delay(1000)).ConfigureAwait(false);
        }
    }

    private static async Task RunSpectatorWatch(string[] args)
    {
        string uri = SpectatorWatchUri;
        int durationSeconds = 0;
        Galaxy? spectatorGalaxy = null;
        Queue<string> recentEvents = new Queue<string>(64);
        DateTime started = DateTime.UtcNow;

        if (args.Length >= 2)
            uri = args[1];

        if (args.Length >= 3)
            durationSeconds = int.Parse(args[2]);

        try
        {
            Console.WriteLine($"SPECTATOR-WATCH: connecting to {uri}...");
            spectatorGalaxy = await Galaxy.Connect(uri, null, null).ConfigureAwait(false);

            Console.WriteLine(
                $"SPECTATOR-WATCH: connected as {spectatorGalaxy.Player.Name} ({spectatorGalaxy.Player.Kind}), teams={spectatorGalaxy.Teams.Count}, players={spectatorGalaxy.Players.Count}.");

            DumpSpectatorState(spectatorGalaxy);

            while (spectatorGalaxy.Active)
            {
                if (durationSeconds > 0 && (DateTime.UtcNow - started).TotalSeconds >= durationSeconds)
                {
                    Console.WriteLine($"SPECTATOR-WATCH: duration limit {durationSeconds}s reached.");
                    break;
                }

                FlattiverseEvent @event = await spectatorGalaxy.NextEvent().ConfigureAwait(false);
                string line = DescribeSpectatorEvent(@event);

                Console.WriteLine(line);
                recentEvents.Enqueue(line);

                while (recentEvents.Count > 64)
                    recentEvents.Dequeue();
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine($"SPECTATOR-WATCH: FATAL {exception.GetType().Name}: {exception.Message}");

            if (recentEvents.Count > 0)
            {
                Console.WriteLine("SPECTATOR-WATCH: recent events:");

                foreach (string line in recentEvents)
                    Console.WriteLine($"  {line}");
            }

            if (spectatorGalaxy is not null)
                DumpSpectatorState(spectatorGalaxy);

            throw;
        }
        finally
        {
            if (spectatorGalaxy is not null)
                spectatorGalaxy.Dispose();
        }
    }

    private static void ApplyUnitEvents(List<FlattiverseEvent> events, Dictionary<string, UnitSpec> unitsByKey)
    {
        foreach (FlattiverseEvent @event in events)
            if (@event is NewUnitFlattiverseEvent newUnitEvent)
            {
                Unit unit = newUnitEvent.Unit;
                string key = BuildUnitKey(unit.Cluster.Id, unit.Name);
                unitsByKey[key] = new UnitSpec(unit.Cluster.Id, unit.Name, unit.Kind);
            }
            else if (@event is RemovedUnitFlattiverseEvent removedUnitEvent)
            {
                Unit unit = removedUnitEvent.Unit;
                string key = BuildUnitKey(unit.Cluster.Id, unit.Name);
                unitsByKey.Remove(key);
            }
    }

    private static void PrintUnits(Dictionary<string, UnitSpec> unitsByKey, string title)
    {
        Console.WriteLine($"{title}: count={unitsByKey.Count}");

        List<UnitSpec> units = new List<UnitSpec>(unitsByKey.Values);
        units.Sort(delegate (UnitSpec left, UnitSpec right)
        {
            int clusterCompare = left.ClusterId.CompareTo(right.ClusterId);
            return clusterCompare != 0 ? clusterCompare : string.Compare(left.Name, right.Name, StringComparison.Ordinal);
        });

        foreach (UnitSpec unitSpec in units)
            Console.WriteLine($"  - cluster={unitSpec.ClusterId}, name={unitSpec.Name}, kind={unitSpec.Kind}");
    }

    private static List<string> BuildRoundtripCreateXml()
    {
        List<string> xml = new List<string>();

        xml.Add("<Sun Name=\"RoundtripSun\" X=\"100\" Y=\"20\" Radius=\"45\" Gravity=\"0.04\" Energy=\"1.30\" Ions=\"0.20\" Neutrinos=\"0.10\" Heat=\"0.60\" Drain=\"0.05\" />");
        xml.Add("<BlackHole Name=\"RoundtripHole\" X=\"-120\" Y=\"70\" Radius=\"30\" Gravity=\"0.07\" GravityWellRadius=\"220\" GravityWellForce=\"0.18\" />");
        xml.Add("<Planet Name=\"RoundtripPlanet\" X=\"-40\" Y=\"-160\" Radius=\"60\" Gravity=\"0.03\" Type=\"OceanWorld\" Metal=\"0.12\" Carbon=\"0.28\" Hydrogen=\"0.74\" Silicon=\"0.19\" />");
        xml.Add("<Moon Name=\"RoundtripMoon\" X=\"170\" Y=\"-140\" Radius=\"24\" Gravity=\"0.01\" Type=\"IceMoon\" Metal=\"0.15\" Carbon=\"0.22\" Hydrogen=\"0.66\" Silicon=\"0.18\" />");
        xml.Add("<Meteoroid Name=\"RoundtripMeteoroid\" X=\"260\" Y=\"110\" Radius=\"15\" Gravity=\"0.002\" Type=\"MetallicSlug\" Metal=\"0.89\" Carbon=\"0.05\" Hydrogen=\"0.03\" Silicon=\"0.21\" />");
        xml.Add("<Buoy Name=\"RoundtripBuoy\" X=\"-260\" Y=\"-120\" Radius=\"10\" Gravity=\"0\" Message=\"Roundtrip marker buoy\" />");
        xml.Add("<MissionTarget Name=\"RoundtripMissionTarget\" X=\"20\" Y=\"30\" Radius=\"8\" Gravity=\"0\" Team=\"0\" SequenceNumber=\"7\"><Vector X=\"40\" Y=\"50\" /><Vector X=\"-60\" Y=\"70\" /></MissionTarget>");
        xml.Add("<Flag Name=\"RoundtripFlag\" X=\"-70\" Y=\"95\" Radius=\"9\" Gravity=\"0\" Team=\"0\" GraceTicks=\"120\" />");
        xml.Add("<DominationPoint Name=\"RoundtripDominationPoint\" X=\"180\" Y=\"-75\" Radius=\"14\" Gravity=\"0\" Team=\"0\" DominationRadius=\"90\" />");

        return xml;
    }

    private static string ReadUnitName(string unitXml)
    {
        XDocument document = XDocument.Parse(unitXml, LoadOptions.None);
        XElement? root = document.Root;

        if (root is null)
            throw new InvalidDataException("Unit XML has no root element.");

        XAttribute? nameAttribute = root.Attribute("Name");

        if (nameAttribute is null || string.IsNullOrEmpty(nameAttribute.Value))
            throw new InvalidDataException("Unit XML has no Name attribute.");

        return nameAttribute.Value;
    }

    private static string BuildUnitKey(byte clusterId, string name)
    {
        return $"{clusterId}:{name}";
    }

    private static async Task TestBuoyMessageLengthLimit(Cluster cluster)
    {
        string tooLongMessage = new string('A', 385);
        string xml = $"<Buoy Name=\"RoundtripBuoyTooLong\" X=\"0\" Y=\"0\" Radius=\"10\" Gravity=\"0\" Message=\"{tooLongMessage}\" />";

        Console.WriteLine("ROUNDTRIP: validating buoy message length...");

        try
        {
            await cluster.SetUnit(xml).ConfigureAwait(false);
            Console.WriteLine("ROUNDTRIP: buoy message length check FAILED (unexpected success)");
        }
        catch (InvalidXmlNodeValueGameException exception)
        {
            Console.WriteLine($"ROUNDTRIP: buoy message length check OK ({exception.Reason}, {exception.NodePath})");
        }
    }

    private static List<DatabaseUnitRow> QueryUnitsFromDatabase(ushort galaxyId)
    {
        List<DatabaseUnitRow> rows = new List<DatabaseUnitRow>();

        using NpgsqlConnection connection = new NpgsqlConnection(DatabaseConnectionString);
        connection.Open();

        using NpgsqlCommand command = new NpgsqlCommand(
            "SELECT cluster, name, kind, configuration FROM public.\"units\" WHERE galaxy=@galaxy ORDER BY cluster, name",
            connection);

        command.Parameters.AddWithValue("@galaxy", (short)galaxyId);

        using NpgsqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
            rows.Add(new DatabaseUnitRow((byte)(short)reader["cluster"], (string)reader["name"], (short)reader["kind"], (string)reader["configuration"]));

        return rows;
    }

    private static DatabaseAccountRow QueryAccountRow(string auth)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(DatabaseConnectionString);
        connection.Open();

        using NpgsqlCommand command = new NpgsqlCommand(
            """
            SELECT "id",
                   "admin",
                   "rank",
                   "datePlayedStart",
                   "datePlayedEnd",
                   "statsPlayerKills",
                   "statsPlayerDeaths",
                   "statsFriendlyKills",
                   "statsFriendlyDeaths",
                   "statsNpcKills",
                   "statsNpcDeaths",
                   "statsNeutralDeaths",
                   "sessionGalaxy",
                   "sessionTeam",
                   "sessionPlayerKills",
                   "sessionPlayerDeaths",
                   "sessionFriendlyKills",
                   "sessionFriendlyDeaths",
                   "sessionNpcKills",
                   "sessionNpcDeaths",
                   "sessionNeutralDeaths"
            FROM public.accounts
            WHERE encode("apiPlayer", 'hex') = @auth
               OR encode("apiAdmin", 'hex') = @auth
            LIMIT 1
            """,
            connection);

        command.Parameters.AddWithValue("@auth", auth);

        using NpgsqlDataReader reader = command.ExecuteReader();

        if (!reader.Read())
            throw new InvalidOperationException("Could not find account row for the specified API key.");

        return new DatabaseAccountRow(
            (int)reader["id"],
            (bool)reader["admin"],
            (int)reader["rank"],
            reader["datePlayedStart"] is DBNull ? null : (long)reader["datePlayedStart"],
            reader["datePlayedEnd"] is DBNull ? null : (long)reader["datePlayedEnd"],
            (long)reader["statsPlayerKills"],
            (long)reader["statsPlayerDeaths"],
            (long)reader["statsFriendlyKills"],
            (long)reader["statsFriendlyDeaths"],
            (long)reader["statsNpcKills"],
            (long)reader["statsNpcDeaths"],
            (long)reader["statsNeutralDeaths"],
            reader["sessionGalaxy"] is DBNull ? null : (short)reader["sessionGalaxy"],
            reader["sessionTeam"] is DBNull ? null : (short)reader["sessionTeam"],
            (long)reader["sessionPlayerKills"],
            (long)reader["sessionPlayerDeaths"],
            (long)reader["sessionFriendlyKills"],
            (long)reader["sessionFriendlyDeaths"],
            (long)reader["sessionNpcKills"],
            (long)reader["sessionNpcDeaths"],
            (long)reader["sessionNeutralDeaths"]);
    }

    private static DatabaseAvatarRow QueryAvatarRow(string auth)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(DatabaseConnectionString);
        connection.Open();

        using NpgsqlCommand command = new NpgsqlCommand(
            """
            SELECT "id",
                   "avatarSmall",
                   "avatarBig"
            FROM public.accounts
            WHERE encode("apiPlayer", 'hex') = @auth
               OR encode("apiAdmin", 'hex') = @auth
            LIMIT 1
            """,
            connection);

        command.Parameters.AddWithValue("@auth", auth);

        using NpgsqlDataReader reader = command.ExecuteReader();

        if (!reader.Read())
            throw new InvalidOperationException("Could not find avatar row for the specified API key.");

        return new DatabaseAvatarRow(
            (int)reader["id"],
            reader["avatarSmall"] is byte[] smallAvatar ? smallAvatar : null,
            reader["avatarBig"] is byte[] bigAvatar ? bigAvatar : null);
    }

    private static void RestoreAccountRow(DatabaseAccountRow row)
    {
        ExecuteDatabaseNonQuery($"""
            UPDATE public.accounts
            SET "admin" = {FormatBool(row.Admin)},
                "rank" = {row.Rank},
                "datePlayedStart" = {FormatNullableLong(row.DatePlayedStart)},
                "datePlayedEnd" = {FormatNullableLong(row.DatePlayedEnd)},
                "statsPlayerKills" = {row.StatsPlayerKills},
                "statsPlayerDeaths" = {row.StatsPlayerDeaths},
                "statsFriendlyKills" = {row.StatsFriendlyKills},
                "statsFriendlyDeaths" = {row.StatsFriendlyDeaths},
                "statsNpcKills" = {row.StatsNpcKills},
                "statsNpcDeaths" = {row.StatsNpcDeaths},
                "statsNeutralDeaths" = {row.StatsNeutralDeaths},
                "sessionGalaxy" = {FormatNullableShort(row.SessionGalaxy)},
                "sessionTeam" = {FormatNullableShort(row.SessionTeam)},
                "sessionPlayerKills" = {row.SessionPlayerKills},
                "sessionPlayerDeaths" = {row.SessionPlayerDeaths},
                "sessionFriendlyKills" = {row.SessionFriendlyKills},
                "sessionFriendlyDeaths" = {row.SessionFriendlyDeaths},
                "sessionNpcKills" = {row.SessionNpcKills},
                "sessionNpcDeaths" = {row.SessionNpcDeaths},
                "sessionNeutralDeaths" = {row.SessionNeutralDeaths}
            WHERE "id" = {row.AccountId}
            """);
    }

    private static void UpdateAvatarRow(int accountId, byte[]? smallAvatar, byte[]? bigAvatar)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(DatabaseConnectionString);
        connection.Open();

        using NpgsqlCommand command = new NpgsqlCommand(
            """
            UPDATE public.accounts
            SET "avatarSmall" = @smallAvatar,
                "avatarBig" = @bigAvatar
            WHERE "id" = @accountId
            """,
            connection);

        command.Parameters.AddWithValue("@accountId", accountId);
        command.Parameters.AddWithValue("@smallAvatar", smallAvatar is not null ? (object)smallAvatar : DBNull.Value);
        command.Parameters.AddWithValue("@bigAvatar", bigAvatar is not null ? (object)bigAvatar : DBNull.Value);
        command.ExecuteNonQuery();
    }

    private static void RestoreAvatarRow(DatabaseAvatarRow row)
    {
        UpdateAvatarRow(row.AccountId, row.SmallAvatar, row.BigAvatar);
    }

    private static async Task<bool> WaitForPlayerAccountState(Player player, bool admin, int rank, int timeoutMs)
    {
        DateTime endTime = DateTime.UtcNow.AddMilliseconds(timeoutMs);

        while (DateTime.UtcNow < endTime)
        {
            if (player.Admin == admin && player.Rank == rank)
                return true;

            await Task.Delay(100).ConfigureAwait(false);
        }

        return false;
    }

    private static async Task<bool> WaitForSessionGalaxy(string auth, short? expectedSessionGalaxy, int timeoutMs)
    {
        DateTime endTime = DateTime.UtcNow.AddMilliseconds(timeoutMs);

        while (DateTime.UtcNow < endTime)
        {
            DatabaseAccountRow row = QueryAccountRow(auth);

            if (row.SessionGalaxy == expectedSessionGalaxy)
                return true;

            await Task.Delay(100).ConfigureAwait(false);
        }

        return false;
    }

    private static bool HasFreshSession(DatabaseAccountRow row)
    {
        if (row.SessionGalaxy is null)
            return false;

        if (row.DatePlayedEnd is not long datePlayedEndTicks || datePlayedEndTicks <= 0)
            return true;

        return new DateTime(datePlayedEndTicks, DateTimeKind.Utc) + AccountSessionTimeout >= DateTime.UtcNow;
    }

    private static Player FindPlayerById(Galaxy galaxy, byte playerId)
    {
        if (galaxy.Players.TryGet(playerId, out Player? player))
            return player;

        throw new InvalidOperationException($"Could not resolve player {playerId} in galaxy view.");
    }

    private static string FormatSessionState(DatabaseAccountRow row)
    {
        if (row.SessionGalaxy is null)
            return "sessionGalaxy=null";

        if (row.DatePlayedEnd is not long datePlayedEndTicks || datePlayedEndTicks <= 0)
            return $"sessionGalaxy={row.SessionGalaxy.Value}, datePlayedEnd=null";

        TimeSpan age = DateTime.UtcNow - new DateTime(datePlayedEndTicks, DateTimeKind.Utc);
        return $"sessionGalaxy={row.SessionGalaxy.Value}, datePlayedEndAge={age.TotalSeconds:0.0}s";
    }

    private static string FormatBool(bool value)
    {
        return value ? "TRUE" : "FALSE";
    }

    private static string FormatNullableShort(short? value)
    {
        return value is short present ? present.ToString() : "NULL";
    }

    private static string FormatNullableLong(long? value)
    {
        return value is long present ? present.ToString() : "NULL";
    }

    private static void ExecuteDatabaseNonQuery(string sql)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(DatabaseConnectionString);
        connection.Open();

        using NpgsqlCommand command = new NpgsqlCommand(sql, connection);
        command.ExecuteNonQuery();
    }

    private static async Task TestNoStartCluster(Galaxy adminGalaxy)
    {
        ClusterSpec[] clusterSpecs = BuildClusterSpecs(adminGalaxy, forceStartFalse: true);
        string xml = BuildConfigurationXml(adminGalaxy, clusterSpecs);

        Console.WriteLine("TEST no-start-cluster:");

        try
        {
            await adminGalaxy.Configure(xml).ConfigureAwait(false);
            Console.WriteLine("  RESULT: FAILED (configure unexpectedly succeeded)");
        }
        catch (InvalidXmlNodeValueGameException exception)
        {
            Console.WriteLine($"  RESULT: OK ({exception.Reason}, {exception.NodePath})");
            Console.WriteLine($"  HINT: {exception.Hint}");
        }
    }

    private static async Task TestNoClusterNodes(Galaxy adminGalaxy)
    {
        ClusterSpec[] emptyClusters = Array.Empty<ClusterSpec>();
        string xml = BuildConfigurationXml(adminGalaxy, emptyClusters);

        Console.WriteLine("TEST no-cluster-nodes:");

        try
        {
            await adminGalaxy.Configure(xml).ConfigureAwait(false);
            Console.WriteLine("  RESULT: FAILED (configure unexpectedly succeeded)");
        }
        catch (InvalidXmlNodeValueGameException exception)
        {
            Console.WriteLine($"  RESULT: OK ({exception.Reason}, {exception.NodePath})");
            Console.WriteLine($"  HINT: {exception.Hint}");
        }
    }

    private static async Task TestInvalidStartAttributeValue(Galaxy adminGalaxy)
    {
        if (!TryGetAnyClusterId(adminGalaxy, out byte clusterId))
        {
            Console.WriteLine("TEST invalid-start-value:");
            Console.WriteLine("  RESULT: SKIPPED (no cluster found)");
            return;
        }

        string xml = $"<Galaxy><Cluster Id=\"{clusterId}\" Start=\"not-a-bool\" /></Galaxy>";

        Console.WriteLine("TEST invalid-start-value:");

        try
        {
            await adminGalaxy.Configure(xml).ConfigureAwait(false);
            Console.WriteLine("  RESULT: FAILED (configure unexpectedly succeeded)");
        }
        catch (InvalidXmlNodeValueGameException exception)
        {
            Console.WriteLine($"  RESULT: OK ({exception.Reason}, {exception.NodePath})");
            Console.WriteLine($"  HINT: {exception.Hint}");
        }
    }

    private static async Task TestTeamChildNodesRejected(Galaxy adminGalaxy)
    {
        if (!TryGetFirstNonSpectatorTeam(adminGalaxy, out Team? team))
        {
            Console.WriteLine("TEST team-child-nodes-rejected:");
            Console.WriteLine("  RESULT: SKIPPED (no non-spectator team found)");
            return;
        }

        Team teamValue = team!;
        string xml = $"<Galaxy><Team Id=\"{teamValue.Id}\"><Invalid /></Team></Galaxy>";

        Console.WriteLine("TEST team-child-nodes-rejected:");

        try
        {
            await adminGalaxy.Configure(xml).ConfigureAwait(false);
            Console.WriteLine("  RESULT: FAILED (configure unexpectedly succeeded)");
        }
        catch (InvalidXmlNodeValueGameException exception)
        {
            Console.WriteLine($"  RESULT: OK ({exception.Reason}, {exception.NodePath})");
            Console.WriteLine($"  HINT: {exception.Hint}");
        }
    }

    private static async Task TestClusterChildNodesRejected(Galaxy adminGalaxy)
    {
        if (!TryGetAnyClusterId(adminGalaxy, out byte clusterId))
        {
            Console.WriteLine("TEST cluster-child-nodes-rejected:");
            Console.WriteLine("  RESULT: SKIPPED (no cluster found)");
            return;
        }

        string xml = $"<Galaxy><Cluster Id=\"{clusterId}\"><Invalid /></Cluster></Galaxy>";

        Console.WriteLine("TEST cluster-child-nodes-rejected:");

        try
        {
            await adminGalaxy.Configure(xml).ConfigureAwait(false);
            Console.WriteLine("  RESULT: FAILED (configure unexpectedly succeeded)");
        }
        catch (InvalidXmlNodeValueGameException exception)
        {
            Console.WriteLine($"  RESULT: OK ({exception.Reason}, {exception.NodePath})");
            Console.WriteLine($"  HINT: {exception.Hint}");
        }
    }

    private static async Task TestDuplicateTeamNames(Galaxy adminGalaxy)
    {
        if (!TryGetTwoNonSpectatorTeams(adminGalaxy, out Team? firstTeam, out Team? secondTeam))
        {
            Console.WriteLine("TEST duplicate-team-name:");
            Console.WriteLine("  RESULT: SKIPPED (need at least two non-spectator teams)");
            return;
        }

        Team firstTeamValue = firstTeam!;
        Team secondTeamValue = secondTeam!;

        XElement root = new XElement("Galaxy");

        foreach (Team team in adminGalaxy.Teams)
        {
            if (team.Id == SpectatorsTeamId)
                continue;

            string teamName = team.Id == secondTeamValue.Id ? firstTeamValue.Name : team.Name;

            root.Add(new XElement("Team",
                new XAttribute("Id", team.Id),
                new XAttribute("Name", teamName),
                new XAttribute("ColorR", team.Red),
                new XAttribute("ColorG", team.Green),
                new XAttribute("ColorB", team.Blue)));
        }

        foreach (Cluster cluster in adminGalaxy.Clusters)
            root.Add(new XElement("Cluster",
                new XAttribute("Id", cluster.Id),
                new XAttribute("Name", cluster.Name),
                new XAttribute("Start", cluster.Start),
                new XAttribute("Respawn", cluster.Respawn)));

        XDocument document = new XDocument(root);
        string xml = document.ToString(SaveOptions.DisableFormatting);

        Console.WriteLine("TEST duplicate-team-name:");

        try
        {
            await adminGalaxy.Configure(xml).ConfigureAwait(false);
            Console.WriteLine("  RESULT: FAILED (configure unexpectedly succeeded)");
        }
        catch (InvalidXmlNodeValueGameException exception)
        {
            Console.WriteLine($"  RESULT: OK ({exception.Reason}, {exception.NodePath})");
            Console.WriteLine($"  HINT: {exception.Hint}");
        }
    }

    private static async Task TestDeleteTeamBlockedByRegion(Galaxy adminGalaxy)
    {
        if (!TryGetTwoNonSpectatorTeams(adminGalaxy, out Team? keepTeam, out Team? deleteTeam))
        {
            Console.WriteLine("TEST delete-team-blocked-by-region:");
            Console.WriteLine("  RESULT: SKIPPED (need at least two non-spectator teams)");
            return;
        }

        if (!TryGetAnyClusterId(adminGalaxy, out byte clusterId))
        {
            Console.WriteLine("TEST delete-team-blocked-by-region:");
            Console.WriteLine("  RESULT: SKIPPED (no cluster found)");
            return;
        }

        if (!adminGalaxy.Clusters.TryGet(clusterId, out Cluster? cluster))
        {
            Console.WriteLine("TEST delete-team-blocked-by-region:");
            Console.WriteLine("  RESULT: SKIPPED (cluster lookup failed)");
            return;
        }

        Team keepTeamValue = keepTeam!;
        Team deleteTeamValue = deleteTeam!;
        Cluster clusterValue = cluster!;
        const int regionId = 66;

        string regionXml = $"<Region Id=\"{regionId}\" Name=\"TmpTeamGuard\" Left=\"-30\" Top=\"-30\" Right=\"30\" Bottom=\"30\"><Team Id=\"{deleteTeamValue.Id}\" /></Region>";
        bool regionCreated = false;

        try
        {
            await clusterValue.SetRegion(regionXml).ConfigureAwait(false);
            regionCreated = true;

            XElement root = new XElement("Galaxy");

            foreach (Team team in adminGalaxy.Teams)
            {
                if (team.Id == SpectatorsTeamId || team.Id == deleteTeamValue.Id)
                    continue;

                root.Add(new XElement("Team",
                    new XAttribute("Id", team.Id),
                    new XAttribute("Name", team.Name),
                    new XAttribute("ColorR", team.Red),
                    new XAttribute("ColorG", team.Green),
                    new XAttribute("ColorB", team.Blue)));
            }

            foreach (Cluster existingCluster in adminGalaxy.Clusters)
                root.Add(new XElement("Cluster",
                    new XAttribute("Id", existingCluster.Id),
                    new XAttribute("Name", existingCluster.Name),
                    new XAttribute("Start", existingCluster.Start),
                    new XAttribute("Respawn", existingCluster.Respawn)));

            string xml = new XDocument(root).ToString(SaveOptions.DisableFormatting);

            Console.WriteLine("TEST delete-team-blocked-by-region:");
            Console.WriteLine($"  TEAM TO DELETE: {deleteTeamValue.Id} ({deleteTeamValue.Name})");
            Console.WriteLine($"  TEAM TO KEEP: {keepTeamValue.Id} ({keepTeamValue.Name})");

            try
            {
                await adminGalaxy.Configure(xml).ConfigureAwait(false);
                Console.WriteLine("  RESULT: FAILED (configure unexpectedly succeeded)");
            }
            catch (InvalidXmlNodeValueGameException exception)
            {
                Console.WriteLine($"  RESULT: OK ({exception.Reason}, {exception.NodePath})");
                Console.WriteLine($"  HINT: {exception.Hint}");
            }
        }
        finally
        {
            if (regionCreated)
                try
                {
                    await clusterValue.RemoveRegion(regionId).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"  CLEANUP remove-region FAILED ({exception.GetType().Name}: {exception.Message})");
                }
        }
    }

    private static async Task TestQueryRegions(Galaxy adminGalaxy)
    {
        if (!TryGetAnyClusterId(adminGalaxy, out byte clusterId))
        {
            Console.WriteLine("TEST query-regions:");
            Console.WriteLine("  RESULT: SKIPPED (no cluster found)");
            return;
        }

        if (!TryGetFirstNonSpectatorTeam(adminGalaxy, out Team? team))
        {
            Console.WriteLine("TEST query-regions:");
            Console.WriteLine("  RESULT: SKIPPED (no non-spectator team found)");
            return;
        }

        if (!adminGalaxy.Clusters.TryGet(clusterId, out Cluster? cluster))
        {
            Console.WriteLine("TEST query-regions:");
            Console.WriteLine("  RESULT: SKIPPED (cluster lookup failed)");
            return;
        }

        Team teamValue = team!;
        Cluster clusterValue = cluster!;
        int[] regionIds = new[] { 250, 251, 252 };
        string regionName = $"TmpQuery{DateTimeOffset.UtcNow.ToUnixTimeSeconds() % 1000000:000000}";
        List<int> createdRegionIds = new List<int>();

        Console.WriteLine("TEST query-regions:");

        try
        {
            foreach (int regionId in regionIds)
            {
                string regionXml = $"<Region Id=\"{regionId}\" Name=\"{regionName}-{regionId}\" Left=\"-90\" Top=\"-70\" Right=\"90\" Bottom=\"70\"><Team Id=\"{teamValue.Id}\" /></Region>";
                await clusterValue.SetRegion(regionXml).ConfigureAwait(false);
                createdRegionIds.Add(regionId);
            }

            string queriedXml = await clusterValue.QueryRegions().ConfigureAwait(false);
            XDocument queryDocument = XDocument.Parse(queriedXml, LoadOptions.None);
            XElement? queryRoot = queryDocument.Root;
            XElement? queryRegion = queryRoot?.Elements("Region").FirstOrDefault(element => element.Attribute("Id")?.Value == regionIds[0].ToString());

            bool hasRegion = queryRegion is not null;
            bool hasName = queryRegion?.Attribute("Name")?.Value == $"{regionName}-{regionIds[0]}";
            bool hasTeam = queryRegion?.Elements("Team").Any(element => element.Attribute("Id")?.Value == teamValue.Id.ToString()) == true;
            bool hasAllRegions = queryRoot?.Elements("Region").Count(element => regionIds.Contains(int.Parse(element.Attribute("Id")!.Value))) == regionIds.Length;
            bool longReply = queriedXml.Length > 255;

            Console.WriteLine($"  RESULT: {(hasRegion && hasName && hasTeam && hasAllRegions && longReply ? "OK" : "FAILED")} (region={hasRegion}, name={hasName}, team={hasTeam}, all={hasAllRegions}, len={queriedXml.Length})");
        }
        catch (Exception exception)
        {
            Console.WriteLine($"  RESULT: FAILED ({exception.GetType().Name}: {exception.Message})");
        }
        finally
        {
            foreach (int regionId in createdRegionIds)
                try
                {
                    await clusterValue.RemoveRegion(regionId).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"  CLEANUP remove-region {regionId} FAILED ({exception.GetType().Name}: {exception.Message})");
                }
        }
    }

    private static async Task TestSetQueryRemoveUnit(Galaxy adminGalaxy)
    {
        if (!TryGetAnyClusterId(adminGalaxy, out byte clusterId))
        {
            Console.WriteLine("TEST set-query-remove-unit:");
            Console.WriteLine("  RESULT: SKIPPED (no cluster found)");
            return;
        }

        if (!adminGalaxy.Clusters.TryGet(clusterId, out Cluster? cluster))
        {
            Console.WriteLine("TEST set-query-remove-unit:");
            Console.WriteLine("  RESULT: SKIPPED (cluster lookup failed)");
            return;
        }

        if (!TryGetFirstNonSpectatorTeam(adminGalaxy, out Team? missionTargetTeam))
        {
            Console.WriteLine("TEST set-query-remove-unit:");
            Console.WriteLine("  RESULT: SKIPPED (no non-spectator team found)");
            return;
        }

        Cluster clusterValue = cluster!;
        Team missionTargetTeamValue = missionTargetTeam!;
        string unitName = $"CfgUnit{DateTimeOffset.UtcNow.ToUnixTimeSeconds() % 1000000:000000}";
        bool unitCreated = false;

        Console.WriteLine("TEST set-query-remove-unit:");

        try
        {
            string sunXml = $"<Sun Name=\"{unitName}\" X=\"123.5\" Y=\"-45.25\" Radius=\"42\" Gravity=\"0.03\" Energy=\"1.8\" Ions=\"1.25\" Neutrinos=\"0.6\" Heat=\"0.35\" Drain=\"0.18\" />";
            await clusterValue.SetUnit(sunXml).ConfigureAwait(false);
            unitCreated = true;

            string queriedSunXml = await clusterValue.QueryUnitXml(unitName).ConfigureAwait(false);
            XDocument queriedSunDocument = XDocument.Parse(queriedSunXml, LoadOptions.None);
            bool isSun = queriedSunDocument.Root?.Name.LocalName == "Sun";

            string planetXml = $"<Planet Name=\"{unitName}\" X=\"12\" Y=\"34\" Radius=\"25\" Gravity=\"0.02\" Type=\"RockyFrontier\" Metal=\"0.3\" Carbon=\"0.2\" Hydrogen=\"0.1\" Silicon=\"0.4\" />";
            await clusterValue.SetUnit(planetXml).ConfigureAwait(false);

            string queriedPlanetXml = await clusterValue.QueryUnitXml(unitName).ConfigureAwait(false);
            XDocument queriedPlanetDocument = XDocument.Parse(queriedPlanetXml, LoadOptions.None);
            bool kindSwitchWorked = queriedPlanetDocument.Root?.Name.LocalName == "Planet";

            string missionTargetXml = $"<MissionTarget Name=\"{unitName}\" X=\"-20\" Y=\"15\" Radius=\"11\" Gravity=\"0\" Team=\"{missionTargetTeamValue.Id}\" SequenceNumber=\"3\"><Vector X=\"1.5\" Y=\"2.5\" /><Vector X=\"-4\" Y=\"8\" /></MissionTarget>";
            await clusterValue.SetUnit(missionTargetXml).ConfigureAwait(false);

            string queriedMissionTargetXml = await clusterValue.QueryUnitXml(unitName).ConfigureAwait(false);
            XDocument queriedMissionTargetDocument = XDocument.Parse(queriedMissionTargetXml, LoadOptions.None);

            bool missionTargetKindWorked = queriedMissionTargetDocument.Root?.Name.LocalName == "MissionTarget";
            bool missionTargetVectorCountWorked = false;
            bool missionTargetTeamWorked = false;
            bool missionTargetSequenceWorked = false;

            XElement? missionTargetRoot = queriedMissionTargetDocument.Root;

            if (missionTargetRoot is not null)
            {
                missionTargetTeamWorked = missionTargetRoot.Attribute("Team")?.Value == missionTargetTeamValue.Id.ToString();
                missionTargetSequenceWorked = missionTargetRoot.Attribute("SequenceNumber")?.Value == "3";

                int vectorCount = 0;

                foreach (XElement child in missionTargetRoot.Elements())
                    if (child.Name.LocalName == "Vector")
                        vectorCount++;

                missionTargetVectorCountWorked = vectorCount == 2;
            }

            string flagXml = $"<Flag Name=\"{unitName}\" X=\"-12\" Y=\"18\" Radius=\"7\" Gravity=\"0\" Team=\"{missionTargetTeamValue.Id}\" GraceTicks=\"123\" />";
            await clusterValue.SetUnit(flagXml).ConfigureAwait(false);

            string queriedFlagXml = await clusterValue.QueryUnitXml(unitName).ConfigureAwait(false);
            XDocument queriedFlagDocument = XDocument.Parse(queriedFlagXml, LoadOptions.None);
            bool flagKindWorked = queriedFlagDocument.Root?.Name.LocalName == "Flag";
            bool flagTeamWorked = queriedFlagDocument.Root?.Attribute("Team")?.Value == missionTargetTeamValue.Id.ToString();
            bool flagGraceTicksWorked = queriedFlagDocument.Root?.Attribute("GraceTicks")?.Value == "123";

            string dominationPointXml = $"<DominationPoint Name=\"{unitName}\" X=\"25\" Y=\"-35\" Radius=\"13\" Gravity=\"0\" Team=\"{missionTargetTeamValue.Id}\" DominationRadius=\"88\" />";
            await clusterValue.SetUnit(dominationPointXml).ConfigureAwait(false);

            string queriedDominationPointXml = await clusterValue.QueryUnitXml(unitName).ConfigureAwait(false);
            XDocument queriedDominationPointDocument = XDocument.Parse(queriedDominationPointXml, LoadOptions.None);
            bool dominationPointKindWorked = queriedDominationPointDocument.Root?.Name.LocalName == "DominationPoint";
            bool dominationPointRadiusWorked = queriedDominationPointDocument.Root?.Attribute("DominationRadius")?.Value == "88";

            await clusterValue.RemoveUnit(unitName).ConfigureAwait(false);
            unitCreated = false;

            bool queryAfterRemoveFailed;

            try
            {
                await clusterValue.QueryUnitXml(unitName).ConfigureAwait(false);
                queryAfterRemoveFailed = false;
            }
            catch (InvalidArgumentGameException exception)
            {
                bool expectedParameter = exception.Parameter == "name" || exception.Parameter == "unit";
                queryAfterRemoveFailed = exception.Reason == InvalidArgumentKind.EntityNotFound && expectedParameter;
            }

            Console.WriteLine($"  RESULT: {(isSun && kindSwitchWorked && missionTargetKindWorked && missionTargetVectorCountWorked && missionTargetTeamWorked && missionTargetSequenceWorked && flagKindWorked && flagTeamWorked && flagGraceTicksWorked && dominationPointKindWorked && dominationPointRadiusWorked && queryAfterRemoveFailed ? "OK" : "FAILED")} " +
                              $"(sun={isSun}, kindSwitch={kindSwitchWorked}, missionTargetKind={missionTargetKindWorked}, missionTargetVectors={missionTargetVectorCountWorked}, missionTargetTeam={missionTargetTeamWorked}, missionTargetSequence={missionTargetSequenceWorked}, flagKind={flagKindWorked}, flagTeam={flagTeamWorked}, flagGraceTicks={flagGraceTicksWorked}, dominationPointKind={dominationPointKindWorked}, dominationPointRadius={dominationPointRadiusWorked}, removed={queryAfterRemoveFailed})");
        }
        catch (Exception exception)
        {
            Console.WriteLine($"  RESULT: FAILED ({exception.GetType().Name}: {exception.Message})");
        }
        finally
        {
            if (unitCreated)
                try
                {
                    await clusterValue.RemoveUnit(unitName).ConfigureAwait(false);
                }
                catch
                {
                }
        }
    }

    private static async Task TestSetUnitWithUnknownTeamRejected(Galaxy adminGalaxy)
    {
        if (!TryGetAnyClusterId(adminGalaxy, out byte clusterId))
        {
            Console.WriteLine("TEST set-unit-unknown-team:");
            Console.WriteLine("  RESULT: SKIPPED (no cluster found)");
            return;
        }

        if (!adminGalaxy.Clusters.TryGet(clusterId, out Cluster? cluster))
        {
            Console.WriteLine("TEST set-unit-unknown-team:");
            Console.WriteLine("  RESULT: SKIPPED (cluster lookup failed)");
            return;
        }

        Cluster clusterValue = cluster!;
        string missingTeamName = $"TeamProbeMissing{DateTimeOffset.UtcNow.ToUnixTimeSeconds() % 1000000:000000}";
        string missingTeamXml = $"<MissionTarget Name=\"{missingTeamName}\" X=\"10\" Y=\"-12\" Radius=\"8\" Gravity=\"0\"><Vector X=\"1\" Y=\"2\" /></MissionTarget>";
        string unknownTeamName = $"TeamProbeUnknown{DateTimeOffset.UtcNow.ToUnixTimeSeconds() % 1000000:000000}";
        string unknownTeamXml = $"<MissionTarget Name=\"{unknownTeamName}\" X=\"10\" Y=\"-12\" Radius=\"8\" Gravity=\"0\" Team=\"99\" SequenceNumber=\"1\"><Vector X=\"1\" Y=\"2\" /></MissionTarget>";

        Console.WriteLine("TEST set-unit-unknown-team:");

        bool missingTeamRejected = false;
        bool unknownTeamRejected = false;

        try
        {
            await clusterValue.SetUnit(missingTeamXml).ConfigureAwait(false);
            Console.WriteLine("  RESULT: FAILED (MissionTarget without Team was accepted unexpectedly)");
            return;
        }
        catch (InvalidXmlNodeValueGameException exception)
        {
            missingTeamRejected = exception.Reason == InvalidArgumentKind.EntityNotFound && exception.NodePath == "MissionTarget.Team";
            Console.WriteLine($"  MISSING TEAM: {(missingTeamRejected ? "OK" : "FAILED")} ({exception.Reason}, {exception.NodePath})");
            Console.WriteLine($"  HINT: {exception.Hint}");
        }

        try
        {
            await clusterValue.SetUnit(unknownTeamXml).ConfigureAwait(false);

            Console.WriteLine("  RESULT: FAILED (unit with Team attribute was accepted unexpectedly)");
            return;
        }
        catch (InvalidXmlNodeValueGameException exception)
        {
            bool expectedReason = exception.Reason == InvalidArgumentKind.EntityNotFound;
            bool expectedNode = exception.NodePath == "MissionTarget.Team";
            unknownTeamRejected = expectedReason && expectedNode;

            Console.WriteLine($"  UNKNOWN TEAM: {(unknownTeamRejected ? "OK" : "FAILED")} ({exception.Reason}, {exception.NodePath})");
            Console.WriteLine($"  HINT: {exception.Hint}");

            Console.WriteLine($"  RESULT: {(missingTeamRejected && unknownTeamRejected ? "OK" : "FAILED")}");
        }
        catch (Exception exception)
        {
            Console.WriteLine($"  RESULT: FAILED ({exception.GetType().Name}: {exception.Message})");
        }
    }

    private static async Task TestDeleteTeamAssignedToUnit(Galaxy adminGalaxy)
    {
        if (!TryGetFirstNonSpectatorTeam(adminGalaxy, out Team? team))
        {
            Console.WriteLine("TEST delete-team-assigned-to-unit:");
            Console.WriteLine("  RESULT: SKIPPED (no non-spectator team found)");
            return;
        }

        if (!TryGetAnyClusterId(adminGalaxy, out byte clusterId))
        {
            Console.WriteLine("TEST delete-team-assigned-to-unit:");
            Console.WriteLine("  RESULT: SKIPPED (no cluster found)");
            return;
        }

        if (!adminGalaxy.Clusters.TryGet(clusterId, out Cluster? cluster))
        {
            Console.WriteLine("TEST delete-team-assigned-to-unit:");
            Console.WriteLine("  RESULT: SKIPPED (cluster lookup failed)");
            return;
        }

        Team teamValue = team!;
        Cluster clusterValue = cluster!;
        string unitName = $"TeamBound{DateTimeOffset.UtcNow.ToUnixTimeSeconds() % 1000000:000000}";
        string setXml = $"<MissionTarget Name=\"{unitName}\" X=\"1\" Y=\"2\" Radius=\"9\" Gravity=\"0\" Team=\"{teamValue.Id}\" SequenceNumber=\"5\"><Vector X=\"3\" Y=\"4\" /></MissionTarget>";

        Console.WriteLine("TEST delete-team-assigned-to-unit:");

        try
        {
            await clusterValue.SetUnit(setXml).ConfigureAwait(false);
        }
        catch (InvalidXmlNodeValueGameException exception)
        {
            Console.WriteLine($"  RESULT: FAILED (unexpected XML validation failure: {exception.Reason}, {exception.NodePath})");
            Console.WriteLine($"  HINT: {exception.Hint}");
            return;
        }

        try
        {
            string baselineXml = BuildConfigurationXml(adminGalaxy, null);
            XDocument document = XDocument.Parse(baselineXml, LoadOptions.None);
            XElement? root = document.Root;

            if (root is null)
            {
                Console.WriteLine("  RESULT: FAILED (missing Galaxy root while preparing delete test)");
                return;
            }

            XElement? teamElement = root.Elements("Team").FirstOrDefault(element => element.Attribute("Id")?.Value == teamValue.Id.ToString());

            if (teamElement is null)
            {
                Console.WriteLine("  RESULT: SKIPPED (selected team not present in configuration XML)");
                return;
            }

            teamElement.Remove();

            string removeTeamXml = document.ToString(SaveOptions.DisableFormatting);

            try
            {
                await adminGalaxy.Configure(removeTeamXml).ConfigureAwait(false);
                Console.WriteLine("  RESULT: FAILED (team deletion succeeded although unit references team)");
            }
            catch (InvalidXmlNodeValueGameException exception)
            {
                bool expectedReason = exception.Reason == InvalidArgumentKind.NameInUse;
                bool expectedNode = exception.NodePath == "Galaxy>Team.Id";
                Console.WriteLine($"  RESULT: {(expectedReason && expectedNode ? "OK" : "FAILED")} ({exception.Reason}, {exception.NodePath})");
                Console.WriteLine($"  HINT: {exception.Hint}");
            }
        }
        finally
        {
            try
            {
                await clusterValue.RemoveUnit(unitName).ConfigureAwait(false);
            }
            catch
            {
            }
        }
    }

    private static async Task TestEmptyGalaxyName(Galaxy adminGalaxy)
    {
        XElement root = new XElement("Galaxy", new XAttribute("Name", ""));

        foreach (Team team in adminGalaxy.Teams)
        {
            if (team.Id == SpectatorsTeamId)
                continue;

            root.Add(new XElement("Team",
                new XAttribute("Id", team.Id),
                new XAttribute("Name", team.Name),
                new XAttribute("ColorR", team.Red),
                new XAttribute("ColorG", team.Green),
                new XAttribute("ColorB", team.Blue)));
        }

        foreach (Cluster cluster in adminGalaxy.Clusters)
            root.Add(new XElement("Cluster",
                new XAttribute("Id", cluster.Id),
                new XAttribute("Name", cluster.Name),
                new XAttribute("Start", cluster.Start),
                new XAttribute("Respawn", cluster.Respawn)));

        string xml = new XDocument(root).ToString(SaveOptions.DisableFormatting);

        Console.WriteLine("TEST empty-galaxy-name:");

        try
        {
            await adminGalaxy.Configure(xml).ConfigureAwait(false);
            Console.WriteLine("  RESULT: FAILED (configure unexpectedly succeeded)");
        }
        catch (InvalidXmlNodeValueGameException exception)
        {
            Console.WriteLine($"  RESULT: OK ({exception.Reason}, {exception.NodePath})");
            Console.WriteLine($"  HINT: {exception.Hint}");
        }
    }

    private static async Task TestTooLongDescription(Galaxy adminGalaxy)
    {
        string xml = BuildConfigurationXml(adminGalaxy, null);
        XDocument document = XDocument.Parse(xml, LoadOptions.None);
        XElement root = document.Root!;

        root.SetAttributeValue("Description", new string('D', 4097));
        xml = document.ToString(SaveOptions.DisableFormatting);

        Console.WriteLine("TEST too-long-description:");

        try
        {
            await adminGalaxy.Configure(xml).ConfigureAwait(false);
            Console.WriteLine("  RESULT: FAILED (configure unexpectedly succeeded)");
        }
        catch (InvalidXmlNodeValueGameException exception)
        {
            Console.WriteLine($"  RESULT: OK ({exception.Reason}, {exception.NodePath})");
            Console.WriteLine($"  HINT: {exception.Hint}");
        }
    }

    private static async Task TestTooLongTeamName(Galaxy adminGalaxy)
    {
        if (!TryGetFirstNonSpectatorTeam(adminGalaxy, out Team? selectedTeam))
        {
            Console.WriteLine("TEST too-long-team-name:");
            Console.WriteLine("  RESULT: SKIPPED (no non-spectator team found)");
            return;
        }

        Team selectedTeamValue = selectedTeam!;
        XElement root = new XElement("Galaxy");

        foreach (Team team in adminGalaxy.Teams)
        {
            if (team.Id == SpectatorsTeamId)
                continue;

            string teamName = team.Id == selectedTeamValue.Id ? new string('T', 33) : team.Name;

            root.Add(new XElement("Team",
                new XAttribute("Id", team.Id),
                new XAttribute("Name", teamName),
                new XAttribute("ColorR", team.Red),
                new XAttribute("ColorG", team.Green),
                new XAttribute("ColorB", team.Blue)));
        }

        foreach (Cluster cluster in adminGalaxy.Clusters)
            root.Add(new XElement("Cluster",
                new XAttribute("Id", cluster.Id),
                new XAttribute("Name", cluster.Name),
                new XAttribute("Start", cluster.Start),
                new XAttribute("Respawn", cluster.Respawn)));

        string xml = new XDocument(root).ToString(SaveOptions.DisableFormatting);

        Console.WriteLine("TEST too-long-team-name:");

        try
        {
            await adminGalaxy.Configure(xml).ConfigureAwait(false);
            Console.WriteLine("  RESULT: FAILED (configure unexpectedly succeeded)");
        }
        catch (InvalidXmlNodeValueGameException exception)
        {
            Console.WriteLine($"  RESULT: OK ({exception.Reason}, {exception.NodePath})");
            Console.WriteLine($"  HINT: {exception.Hint}");
        }
    }

    private static async Task TestDeleteClusterWithActiveShip(Galaxy adminGalaxy, Galaxy playerGalaxy, ConcurrentQueue<FlattiverseEvent> playerEvents)
    {
        if (!TryGetActiveStartCluster(adminGalaxy, out Cluster? activeCluster))
        {
            Console.WriteLine("TEST delete-active-cluster:");
            Console.WriteLine("  RESULT: SKIPPED (no active cluster found)");
            return;
        }

        Cluster activeClusterValue = activeCluster!;

        if (!TryGetUnusedClusterId(adminGalaxy, activeClusterValue.Id, out byte helperClusterId))
        {
            Console.WriteLine("TEST delete-active-cluster:");
            Console.WriteLine("  RESULT: SKIPPED (no free helper cluster id found)");
            return;
        }

        string helperClusterName = $"TmpCluster{helperClusterId}";

        ClusterSpec[] stageOneClusters = new[]
        {
            new ClusterSpec(activeClusterValue.Id, activeClusterValue.Name, true, false),
            new ClusterSpec(helperClusterId, helperClusterName, false, false)
        };

        await adminGalaxy.Configure(BuildConfigurationXml(adminGalaxy, stageOneClusters)).ConfigureAwait(false);
        await Task.Delay(200).ConfigureAwait(false);
        DrainEvents(playerEvents);

        string shipName = $"DevShip{DateTimeOffset.UtcNow.ToUnixTimeSeconds() % 1000000:000000}";
        ClassicShipControllable ship = await playerGalaxy.CreateClassicShip(shipName).ConfigureAwait(false);

        await ship.Continue().ConfigureAwait(false);

        bool aliveBeforeDelete = await WaitForAliveState(ship, true, 3000).ConfigureAwait(false);

        Console.WriteLine("TEST delete-active-cluster:");
        Console.WriteLine($"  SHIP ALIVE BEFORE DELETE: {aliveBeforeDelete}");

        DrainEvents(playerEvents);

        ClusterSpec[] stageTwoClusters = new[]
        {
            new ClusterSpec(helperClusterId, helperClusterName, true, false)
        };

        await adminGalaxy.Configure(BuildConfigurationXml(adminGalaxy, stageTwoClusters)).ConfigureAwait(false);

        bool reachedDeadStateAfterDelete = await WaitForAliveState(ship, false, 3000).ConfigureAwait(false);

        await Task.Delay(200).ConfigureAwait(false);

        List<FlattiverseEvent> eventsAfterDelete = DrainEvents(playerEvents);

        bool clusterRemoved = false;
        bool ownShipDestroyed = false;
        PlayerUnitDestroyedReason destroyReason = PlayerUnitDestroyedReason.ByRules;
        bool ownUnitRemoved = false;

        foreach (FlattiverseEvent @event in eventsAfterDelete)
            if (@event is ClusterRemovedEvent clusterRemovedEvent && clusterRemovedEvent.Cluster.Id == activeClusterValue.Id)
                clusterRemoved = true;
            else if (@event is DestroyedControllableInfoPlayerEvent destroyedEvent &&
                     destroyedEvent.Player.Id == playerGalaxy.Player.Id &&
                     destroyedEvent.ControllableInfo.Name == ship.Name)
            {
                ownShipDestroyed = true;
                destroyReason = destroyedEvent.Reason;
            }
            else if (@event is RemovedUnitFlattiverseEvent removedUnitEvent && removedUnitEvent.Unit.Name == ship.Name)
                ownUnitRemoved = true;

        Console.WriteLine($"  CLUSTER REMOVED EVENT: {clusterRemoved}");
        Console.WriteLine($"  SHIP REACHED DEAD STATE AFTER DELETE: {reachedDeadStateAfterDelete}");
        Console.WriteLine($"  SHIP.ALIVE VALUE AFTER DELETE: {ship.Alive}");
        Console.WriteLine($"  DESTROYED EVENT FOR OWN SHIP: {ownShipDestroyed}");
        Console.WriteLine($"  DESTROY REASON: {destroyReason}");
        Console.WriteLine($"  DESTROY REASON IS ByClusterRemoval: {destroyReason == PlayerUnitDestroyedReason.ByClusterRemoval}");
        Console.WriteLine($"  REMOVED UNIT EVENT FOR OWN SHIP: {ownUnitRemoved}");

        try
        {
            await ship.Continue().ConfigureAwait(false);
            bool aliveAfterContinue = await WaitForAliveState(ship, true, 3000).ConfigureAwait(false);
            Console.WriteLine($"  CONTINUE AFTER DELETE: {aliveAfterContinue}");
        }
        catch (Exception exception)
        {
            Console.WriteLine($"  CONTINUE AFTER DELETE: FAILED ({exception.GetType().Name}: {exception.Message})");
        }

        ship.RequestClose();
    }

    private static async Task TestScoreUpdatesOnSuicide(Galaxy playerGalaxy, ConcurrentQueue<FlattiverseEvent> playerEvents)
    {
        string shipName = $"ScoreShip{DateTimeOffset.UtcNow.ToUnixTimeSeconds() % 1000000:000000}";
        uint initialPlayerKills = playerGalaxy.Player.Score.PlayerKills;
        uint initialPlayerDeaths = playerGalaxy.Player.Score.PlayerDeaths;
        uint initialPlayerFriendlyDeaths = playerGalaxy.Player.Score.FriendlyDeaths;
        uint initialTeamKills = playerGalaxy.Player.Team.Score.PlayerKills;
        uint initialTeamDeaths = playerGalaxy.Player.Team.Score.PlayerDeaths;
        uint initialTeamFriendlyDeaths = playerGalaxy.Player.Team.Score.FriendlyDeaths;

        DrainEvents(playerEvents);

        ClassicShipControllable ship = await playerGalaxy.CreateClassicShip(shipName).ConfigureAwait(false);
        await ship.Continue().ConfigureAwait(false);

        bool alive = await WaitForAliveState(ship, true, 3000).ConfigureAwait(false);

        Console.WriteLine("TEST score-updates-on-suicide:");
        Console.WriteLine($"  SHIP ALIVE BEFORE SUICIDE: {alive}");

        if (!alive)
        {
            ship.RequestClose();
            Console.WriteLine("  RESULT: FAILED (ship did not become alive before suicide)");
            return;
        }

        DrainEvents(playerEvents);

        await ship.Suicide().ConfigureAwait(false);

        bool dead = await WaitForAliveState(ship, false, 3000).ConfigureAwait(false);

        await Task.Delay(250).ConfigureAwait(false);

        List<FlattiverseEvent> eventsAfterSuicide = DrainEvents(playerEvents);
        bool playerScoreUpdated = false;
        bool teamScoreUpdated = false;

        foreach (FlattiverseEvent @event in eventsAfterSuicide)
            if (@event is PlayerScoreUpdatedEvent playerScoreUpdatedEvent &&
                playerScoreUpdatedEvent.Player.Id == playerGalaxy.Player.Id &&
                playerScoreUpdatedEvent.NewPlayerDeaths == initialPlayerDeaths &&
                playerScoreUpdatedEvent.NewPlayerKills == initialPlayerKills &&
                playerScoreUpdatedEvent.NewFriendlyDeaths == initialPlayerFriendlyDeaths + 1U)
                playerScoreUpdated = true;
            else if (@event is TeamScoreUpdatedEvent teamScoreUpdatedEvent &&
                     teamScoreUpdatedEvent.Team.Id == playerGalaxy.Player.Team.Id &&
                     teamScoreUpdatedEvent.NewPlayerDeaths == initialTeamDeaths &&
                     teamScoreUpdatedEvent.NewPlayerKills == initialTeamKills &&
                     teamScoreUpdatedEvent.NewFriendlyDeaths == initialTeamFriendlyDeaths + 1U)
                teamScoreUpdated = true;

        bool playerScoreApplied = playerGalaxy.Player.Score.PlayerDeaths == initialPlayerDeaths &&
                                  playerGalaxy.Player.Score.PlayerKills == initialPlayerKills &&
                                  playerGalaxy.Player.Score.FriendlyDeaths == initialPlayerFriendlyDeaths + 1U;
        bool teamScoreApplied = playerGalaxy.Player.Team.Score.PlayerDeaths == initialTeamDeaths &&
                                playerGalaxy.Player.Team.Score.PlayerKills == initialTeamKills &&
                                playerGalaxy.Player.Team.Score.FriendlyDeaths == initialTeamFriendlyDeaths + 1U;

        Console.WriteLine($"  SHIP REACHED DEAD STATE: {dead}");
        Console.WriteLine($"  PLAYER SCORE EVENT: {playerScoreUpdated}");
        Console.WriteLine($"  TEAM SCORE EVENT: {teamScoreUpdated}");
        Console.WriteLine($"  PLAYER SCORE APPLIED: {playerScoreApplied}");
        Console.WriteLine($"  TEAM SCORE APPLIED: {teamScoreApplied}");
        Console.WriteLine($"  RESULT: {(dead && playerScoreUpdated && teamScoreUpdated && playerScoreApplied && teamScoreApplied ? "OK" : "FAILED")}");

        ship.RequestClose();
    }

    private static async Task<Dictionary<byte, string>> CaptureRegionsByCluster(Galaxy adminGalaxy)
    {
        Dictionary<byte, string> regionsByCluster = new Dictionary<byte, string>();

        foreach (Cluster cluster in adminGalaxy.Clusters)
            regionsByCluster[cluster.Id] = await cluster.QueryRegions().ConfigureAwait(false);

        return regionsByCluster;
    }

    private static async Task RestoreRegionsByCluster(Galaxy adminGalaxy, Dictionary<byte, string> regionsByCluster)
    {
        foreach (Cluster cluster in adminGalaxy.Clusters)
        {
            string currentRegionsXml = await cluster.QueryRegions().ConfigureAwait(false);
            XDocument currentRegionsDocument = XDocument.Parse(currentRegionsXml, LoadOptions.None);
            XElement? currentRegionsRoot = currentRegionsDocument.Root;

            if (currentRegionsRoot is null || currentRegionsRoot.Name.LocalName != "Regions")
                throw new InvalidDataException("Current region query XML has no Regions root.");

            foreach (XElement regionElement in currentRegionsRoot.Elements("Region"))
            {
                XAttribute? idAttribute = regionElement.Attribute("Id");

                if (idAttribute is null || !int.TryParse(idAttribute.Value, out int id))
                    throw new InvalidDataException("Current region query XML contains a Region without a valid Id.");

                await cluster.RemoveRegion(id).ConfigureAwait(false);
            }
        }

        foreach (KeyValuePair<byte, string> entry in regionsByCluster)
        {
            if (!adminGalaxy.Clusters.TryGet(entry.Key, out Cluster? cluster))
                continue;

            XDocument restoreDocument = XDocument.Parse(entry.Value, LoadOptions.None);
            XElement? restoreRoot = restoreDocument.Root;

            if (restoreRoot is null || restoreRoot.Name.LocalName != "Regions")
                throw new InvalidDataException("Stored region query XML has no Regions root.");

            foreach (XElement regionElement in restoreRoot.Elements("Region"))
            {
                XElement regionCopy = new XElement(regionElement);
                string regionXml = new XDocument(regionCopy).ToString(SaveOptions.DisableFormatting);
                await cluster.SetRegion(regionXml).ConfigureAwait(false);
            }
        }
    }

    private static Task StartEventPump(string name, Galaxy playerGalaxy, ConcurrentQueue<FlattiverseEvent> playerEvents)
    {
        return Task.Run(async delegate
        {
            try
            {
                while (true)
                {
                    FlattiverseEvent @event = await playerGalaxy.NextEvent().ConfigureAwait(false);
                    playerEvents.Enqueue(@event);
                }
            }
            catch (ConnectionTerminatedGameException exception)
            {
                Console.WriteLine($"{name}: event pump terminated ({exception.Message})");
            }
        });
    }

    private static string DescribeSpectatorEvent(FlattiverseEvent @event)
    {
        if (@event is JoinedPlayerEvent joinedPlayerEvent)
            return $"{joinedPlayerEvent.Stamp:HH:mm:ss.fff} PLAYER+ id={joinedPlayerEvent.Player.Id} name={joinedPlayerEvent.Player.Name} team={joinedPlayerEvent.Player.Team.Name} kind={joinedPlayerEvent.Player.Kind}";

        if (@event is PartedPlayerEvent partedPlayerEvent)
            return $"{partedPlayerEvent.Stamp:HH:mm:ss.fff} PLAYER- id={partedPlayerEvent.Player.Id} name={partedPlayerEvent.Player.Name}";

        if (@event is DisconnectedPlayerEvent disconnectedPlayerEvent)
            return $"{disconnectedPlayerEvent.Stamp:HH:mm:ss.fff} PLAYER-DISCONNECTED id={disconnectedPlayerEvent.Player.Id} name={disconnectedPlayerEvent.Player.Name}";

        if (@event is RegisteredControllableInfoPlayerEvent registeredControllableInfoPlayerEvent)
            return $"{registeredControllableInfoPlayerEvent.Stamp:HH:mm:ss.fff} CTRL+ player={registeredControllableInfoPlayerEvent.Player.Id}:{registeredControllableInfoPlayerEvent.Player.Name} controllable={registeredControllableInfoPlayerEvent.ControllableInfo.Id}:{registeredControllableInfoPlayerEvent.ControllableInfo.Name} kind={registeredControllableInfoPlayerEvent.ControllableInfo.Kind} alive={registeredControllableInfoPlayerEvent.ControllableInfo.Alive}";

        if (@event is ClosedControllableInfoPlayerEvent closedControllableInfoPlayerEvent)
            return $"{closedControllableInfoPlayerEvent.Stamp:HH:mm:ss.fff} CTRL- player={closedControllableInfoPlayerEvent.Player.Id}:{closedControllableInfoPlayerEvent.Player.Name} controllable={closedControllableInfoPlayerEvent.ControllableInfo.Id}:{closedControllableInfoPlayerEvent.ControllableInfo.Name}";

        if (@event is ContinuedControllableInfoPlayerEvent continuedControllableInfoPlayerEvent)
            return $"{continuedControllableInfoPlayerEvent.Stamp:HH:mm:ss.fff} CTRL-ALIVE player={continuedControllableInfoPlayerEvent.Player.Id}:{continuedControllableInfoPlayerEvent.Player.Name} controllable={continuedControllableInfoPlayerEvent.ControllableInfo.Id}:{continuedControllableInfoPlayerEvent.ControllableInfo.Name}";

        if (@event is DestroyedControllableInfoPlayerEvent destroyedControllableInfoPlayerEvent)
            return $"{destroyedControllableInfoPlayerEvent.Stamp:HH:mm:ss.fff} CTRL-DEAD player={destroyedControllableInfoPlayerEvent.Player.Id}:{destroyedControllableInfoPlayerEvent.Player.Name} controllable={destroyedControllableInfoPlayerEvent.ControllableInfo.Id}:{destroyedControllableInfoPlayerEvent.ControllableInfo.Name} event={destroyedControllableInfoPlayerEvent.Kind}";

        if (@event is NewUnitFlattiverseEvent newUnitFlattiverseEvent)
            return DescribeUnitEvent("UNIT+", newUnitFlattiverseEvent.Stamp, newUnitFlattiverseEvent.Unit);

        if (@event is RemovedUnitFlattiverseEvent removedUnitFlattiverseEvent)
            return DescribeUnitEvent("UNIT-", removedUnitFlattiverseEvent.Stamp, removedUnitFlattiverseEvent.Unit);

        if (@event is UpdatedUnitFlattiverseEvent updatedUnitFlattiverseEvent)
            if (updatedUnitFlattiverseEvent.Unit.Kind == UnitKind.Shot || updatedUnitFlattiverseEvent.Unit.Kind == UnitKind.Explosion)
                return DescribeUnitEvent("UNIT*", updatedUnitFlattiverseEvent.Stamp, updatedUnitFlattiverseEvent.Unit);

        return @event.ToString();
    }

    private static string DescribeUnitEvent(string prefix, DateTime stamp, Unit unit)
    {
        if (unit is Shot shot)
        {
            string playerName = shot.Player is null ? "-" : $"{shot.Player.Id}:{shot.Player.Name}";
            string controllableName = shot.ControllableInfo is null ? "-" : $"{shot.ControllableInfo.Id}:{shot.ControllableInfo.Name}";

            return $"{stamp:HH:mm:ss.fff} {prefix} kind=Shot cluster={unit.Cluster.Id}:{unit.Cluster.Name} name={unit.Name} player={playerName} controllable={controllableName} ticks={shot.Ticks}";
        }

        if (unit is Explosion explosion)
        {
            string playerName = explosion.Player is null ? "-" : $"{explosion.Player.Id}:{explosion.Player.Name}";
            string controllableName = explosion.ControllableInfo is null ? "-" : $"{explosion.ControllableInfo.Id}:{explosion.ControllableInfo.Name}";

            return $"{stamp:HH:mm:ss.fff} {prefix} kind=Explosion cluster={unit.Cluster.Id}:{unit.Cluster.Name} name={unit.Name} player={playerName} controllable={controllableName} radius={unit.Radius:0.###}";
        }

        if (unit is ClassicShipPlayerUnit classicShipPlayerUnit)
        {
            string playerName = classicShipPlayerUnit.Player is null ? "-" : $"{classicShipPlayerUnit.Player.Id}:{classicShipPlayerUnit.Player.Name}";
            string controllableName = classicShipPlayerUnit.ControllableInfo is null ? "-" : $"{classicShipPlayerUnit.ControllableInfo.Id}:{classicShipPlayerUnit.ControllableInfo.Name}";

            return $"{stamp:HH:mm:ss.fff} {prefix} kind={unit.Kind} cluster={unit.Cluster.Id}:{unit.Cluster.Name} name={unit.Name} player={playerName} controllable={controllableName} pos={unit.Position}";
        }

        return $"{stamp:HH:mm:ss.fff} {prefix} kind={unit.Kind} cluster={unit.Cluster.Id}:{unit.Cluster.Name} name={unit.Name}";
    }

    private static void DumpSpectatorState(Galaxy galaxy)
    {
        Console.WriteLine("SPECTATOR-WATCH: players snapshot:");

        foreach (Player player in galaxy.Players)
        {
            Console.WriteLine($"  player {player.Id}:{player.Name} active={player.Active} disconnected={player.Disconnected} kind={player.Kind} team={player.Team.Name}");

            foreach (ControllableInfo controllableInfo in player.ControllableInfos)
                Console.WriteLine($"    controllable {controllableInfo.Id}:{controllableInfo.Name} active={controllableInfo.Active} alive={controllableInfo.Alive} kind={controllableInfo.Kind}");
        }

        Console.WriteLine("SPECTATOR-WATCH: units snapshot:");

        foreach (Cluster cluster in galaxy.Clusters)
        {
            int unitCount = 0;
            int shotCount = 0;
            int explosionCount = 0;

            foreach (Unit unit in cluster.Units)
            {
                unitCount++;

                if (unit.Kind == UnitKind.Shot)
                    shotCount++;
                else if (unit.Kind == UnitKind.Explosion)
                    explosionCount++;
            }

            Console.WriteLine($"  cluster {cluster.Id}:{cluster.Name} units={unitCount} shots={shotCount} explosions={explosionCount}");
        }
    }

    private static List<FlattiverseEvent> DrainEvents(ConcurrentQueue<FlattiverseEvent> events)
    {
        List<FlattiverseEvent> result = new List<FlattiverseEvent>();

        while (events.TryDequeue(out FlattiverseEvent? @event))
            result.Add(@event);

        return result;
    }

    private static string BuildConfigurationXml(Galaxy galaxy, ClusterSpec[]? clusters)
    {
        return BuildConfigurationXml(galaxy, null, clusters, null);
    }

    private static string BuildConfigurationXml(Galaxy galaxy, TeamSpec[]? teams, ClusterSpec[]? clusters, bool? requiresSelfDisclosure = null)
    {
        XElement root = new XElement("Galaxy");

        if (requiresSelfDisclosure is not null)
            root.Add(new XAttribute("RequiresSelfDisclosure", requiresSelfDisclosure.Value));

        if (teams is null)
            foreach (Team team in galaxy.Teams)
            {
                if (team.Id == SpectatorsTeamId)
                    continue;

                root.Add(new XElement("Team",
                    new XAttribute("Id", team.Id),
                    new XAttribute("Name", team.Name),
                    new XAttribute("ColorR", team.Red),
                    new XAttribute("ColorG", team.Green),
                    new XAttribute("ColorB", team.Blue)));
            }
        else
            foreach (TeamSpec team in teams)
                root.Add(new XElement("Team",
                    new XAttribute("Id", team.Id),
                    new XAttribute("Name", team.Name),
                    new XAttribute("ColorR", team.Red),
                    new XAttribute("ColorG", team.Green),
                    new XAttribute("ColorB", team.Blue)));

        if (clusters is null)
            foreach (Cluster cluster in galaxy.Clusters)
                root.Add(new XElement("Cluster",
                    new XAttribute("Id", cluster.Id),
                    new XAttribute("Name", cluster.Name),
                    new XAttribute("Start", cluster.Start),
                    new XAttribute("Respawn", cluster.Respawn)));
        else
            foreach (ClusterSpec cluster in clusters)
                root.Add(new XElement("Cluster",
                    new XAttribute("Id", cluster.Id),
                    new XAttribute("Name", cluster.Name),
                    new XAttribute("Start", cluster.Start),
                    new XAttribute("Respawn", cluster.Respawn)));

        XDocument document = new XDocument(root);

        return document.ToString(SaveOptions.DisableFormatting);
    }

    private static async Task RemoveAllRegions(Galaxy adminGalaxy)
    {
        foreach (Cluster cluster in adminGalaxy.Clusters)
        {
            string currentRegionsXml = await cluster.QueryRegions().ConfigureAwait(false);
            XDocument currentRegionsDocument = XDocument.Parse(currentRegionsXml, LoadOptions.None);
            XElement? currentRegionsRoot = currentRegionsDocument.Root;

            if (currentRegionsRoot is null || currentRegionsRoot.Name.LocalName != "Regions")
                throw new InvalidDataException("Current region query XML has no Regions root.");

            foreach (XElement regionElement in currentRegionsRoot.Elements("Region"))
            {
                XAttribute? idAttribute = regionElement.Attribute("Id");

                if (idAttribute is null || !int.TryParse(idAttribute.Value, out int id))
                    throw new InvalidDataException("Current region query XML contains a Region without a valid Id.");

                await cluster.RemoveRegion(id).ConfigureAwait(false);
            }
        }
    }

    private static ClusterSpec[] BuildClusterSpecs(Galaxy galaxy, bool forceStartFalse)
    {
        List<ClusterSpec> clusters = new List<ClusterSpec>();

        foreach (Cluster cluster in galaxy.Clusters)
            clusters.Add(new ClusterSpec(cluster.Id, cluster.Name, forceStartFalse ? false : cluster.Start, cluster.Respawn));

        return clusters.ToArray();
    }

    private static bool TryGetAnyClusterId(Galaxy galaxy, out byte id)
    {
        foreach (Cluster cluster in galaxy.Clusters)
        {
            id = cluster.Id;
            return true;
        }

        id = 0;
        return false;
    }

    private static bool TryGetFirstNonSpectatorTeam(Galaxy galaxy, out Team? team)
    {
        foreach (Team existingTeam in galaxy.Teams)
            if (existingTeam.Id != SpectatorsTeamId)
            {
                team = existingTeam;
                return true;
            }

        team = null;
        return false;
    }

    private static bool TryGetTwoNonSpectatorTeams(Galaxy galaxy, out Team? firstTeam, out Team? secondTeam)
    {
        firstTeam = null;
        secondTeam = null;

        foreach (Team team in galaxy.Teams)
            if (team.Id != SpectatorsTeamId)
                if (firstTeam is null)
                    firstTeam = team;
                else
                {
                    secondTeam = team;
                    return true;
                }

        return false;
    }

    private static bool TryGetActiveStartCluster(Galaxy galaxy, out Cluster? cluster)
    {
        foreach (Cluster existingCluster in galaxy.Clusters)
            if (existingCluster.Start)
            {
                cluster = existingCluster;
                return true;
            }

        foreach (Cluster existingCluster in galaxy.Clusters)
        {
            cluster = existingCluster;
            return true;
        }

        cluster = null;
        return false;
    }

    private static bool TryGetUnusedClusterId(Galaxy galaxy, byte occupiedId, out byte freeId)
    {
        for (int id = 0; id < 24; id++)
        {
            if (id == occupiedId)
                continue;

            if (!galaxy.Clusters.TryGet(id, out Cluster? _))
            {
                freeId = (byte)id;
                return true;
            }
        }

        freeId = 0;
        return false;
    }

    private static async Task<bool> WaitForAliveState(Controllable controllable, bool expectedAlive, int timeoutMs)
    {
        DateTime deadline = DateTime.UtcNow.AddMilliseconds(timeoutMs);

        while (DateTime.UtcNow < deadline)
        {
            if (controllable.Alive == expectedAlive)
                return true;

            await Task.Delay(50).ConfigureAwait(false);
        }

        return controllable.Alive == expectedAlive;
    }

    private static async Task<bool> WaitForRequiresSelfDisclosure(Galaxy galaxy, bool expectedValue, int timeoutMs)
    {
        DateTime deadline = DateTime.UtcNow.AddMilliseconds(timeoutMs);

        while (DateTime.UtcNow < deadline)
        {
            if (galaxy.RequiresSelfDisclosure == expectedValue)
                return true;

            await Task.Delay(50).ConfigureAwait(false);
        }

        return galaxy.RequiresSelfDisclosure == expectedValue;
    }

    private static async Task<byte> ConnectAndReadInitialExceptionCode(string uri)
    {
        using ClientWebSocket socket = new ClientWebSocket();
        byte[] data = new byte[512];

        await socket.ConnectAsync(new Uri(uri), CancellationToken.None).ConfigureAwait(false);

        WebSocketReceiveResult result = await socket.ReceiveAsync(new ArraySegment<byte>(data), CancellationToken.None).ConfigureAwait(false);

        if (result.Count < 5)
            throw new InvalidDataException("Expected an initial exception packet.");

        if (data[0] != 0xFF || data[1] != 0x01)
            throw new InvalidDataException($"Expected an initial exception packet, got command 0x{data[0]:X02} session 0x{data[1]:X02}.");

        return data[4];
    }
}
