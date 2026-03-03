using System.Collections.Concurrent;
using System.Xml.Linq;
using Flattiverse.Connector;
using Flattiverse.Connector.Events;
using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;

namespace Development;

class Program
{
    private const string Uri = "ws://127.0.0.1:5000";
    private const string TeamName = "Pink";
    private const byte SpectatorsTeamId = 12;
    private const ushort DatabaseGalaxyId = 0;
    private const string DatabasePsqlConnection = "host=10.252.7.136 port=5432 dbname=flattiverse user=postgres";

    private const string AdminAuth = "7666fc8bdadc000acde68691ebe7d30f6d0f6ac001431a18886ee2d9f176ab9e";
    private const string PlayerAuth = "28a00943f2c0181a0c5db3f4de3e23e987a4c060cc39f45dcb6ed2a86f00eac5";

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

    private static async Task Main(string[] args)
    {
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

        Galaxy? adminGalaxy = null;
        Galaxy? playerGalaxy = null;
        Task? playerEventPump = null;
        string? restoreConfigurationXml = null;
        Dictionary<byte, string>? restoreRegionsByCluster = null;

        ConcurrentQueue<FlattiverseEvent> playerEvents = new ConcurrentQueue<FlattiverseEvent>();

        try
        {
            adminGalaxy = await Galaxy.Connect(Uri, AdminAuth, TeamName).ConfigureAwait(false);
            playerGalaxy = await Galaxy.Connect(Uri, PlayerAuth, TeamName).ConfigureAwait(false);

            restoreConfigurationXml = BuildConfigurationXml(adminGalaxy, null);
            restoreRegionsByCluster = await CaptureRegionsByCluster(adminGalaxy).ConfigureAwait(false);

            playerEventPump = StartEventPump(playerGalaxy, playerEvents);

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
            eventPump = StartEventPump(adminGalaxy, eventQueue);

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
            eventPump = StartEventPump(adminGalaxy, eventQueue);

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
                "RoundtripMissionTarget"
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
        xml.Add("<MissionTarget Name=\"RoundtripMissionTarget\" X=\"20\" Y=\"30\" Radius=\"8\" Gravity=\"0\" Team=\"0\"><Vector X=\"40\" Y=\"50\" /><Vector X=\"-60\" Y=\"70\" /></MissionTarget>");

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

        string sql = $"SELECT cluster, name, kind, configuration FROM public.\"units\" WHERE galaxy={galaxyId} ORDER BY cluster, name";

        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo("psql");
        startInfo.UseShellExecute = false;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;
        startInfo.ArgumentList.Add(DatabasePsqlConnection);
        startInfo.ArgumentList.Add("-At");
        startInfo.ArgumentList.Add("-F");
        startInfo.ArgumentList.Add("\t");
        startInfo.ArgumentList.Add("-c");
        startInfo.ArgumentList.Add(sql);

        System.Diagnostics.Process? process = System.Diagnostics.Process.Start(startInfo);

        if (process is null)
            throw new InvalidOperationException("Could not start psql process for database query.");

        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode != 0)
            throw new InvalidOperationException($"psql query failed with exit code {process.ExitCode}: {error}");

        string[] lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines)
        {
            string[] columns = line.Split('\t', 4);

            if (columns.Length < 4)
                continue;

            if (!byte.TryParse(columns[0], out byte clusterId))
                continue;

            if (!short.TryParse(columns[2], out short kind))
                continue;

            rows.Add(new DatabaseUnitRow(clusterId, columns[1], kind, columns[3]));
        }

        return rows;
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

            string missionTargetXml = $"<MissionTarget Name=\"{unitName}\" X=\"-20\" Y=\"15\" Radius=\"11\" Gravity=\"0\" Team=\"{missionTargetTeamValue.Id}\"><Vector X=\"1.5\" Y=\"2.5\" /><Vector X=\"-4\" Y=\"8\" /></MissionTarget>";
            await clusterValue.SetUnit(missionTargetXml).ConfigureAwait(false);

            string queriedMissionTargetXml = await clusterValue.QueryUnitXml(unitName).ConfigureAwait(false);
            XDocument queriedMissionTargetDocument = XDocument.Parse(queriedMissionTargetXml, LoadOptions.None);

            bool missionTargetKindWorked = queriedMissionTargetDocument.Root?.Name.LocalName == "MissionTarget";
            bool missionTargetVectorCountWorked = false;
            bool missionTargetTeamWorked = false;

            XElement? missionTargetRoot = queriedMissionTargetDocument.Root;

            if (missionTargetRoot is not null)
            {
                missionTargetTeamWorked = missionTargetRoot.Attribute("Team")?.Value == missionTargetTeamValue.Id.ToString();

                int vectorCount = 0;

                foreach (XElement child in missionTargetRoot.Elements())
                    if (child.Name.LocalName == "Vector")
                        vectorCount++;

                missionTargetVectorCountWorked = vectorCount == 2;
            }

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

            Console.WriteLine($"  RESULT: {(isSun && kindSwitchWorked && missionTargetKindWorked && missionTargetVectorCountWorked && missionTargetTeamWorked && queryAfterRemoveFailed ? "OK" : "FAILED")} " +
                              $"(sun={isSun}, kindSwitch={kindSwitchWorked}, missionTargetKind={missionTargetKindWorked}, missionTargetVectors={missionTargetVectorCountWorked}, missionTargetTeam={missionTargetTeamWorked}, removed={queryAfterRemoveFailed})");
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
        string unknownTeamXml = $"<MissionTarget Name=\"{unknownTeamName}\" X=\"10\" Y=\"-12\" Radius=\"8\" Gravity=\"0\" Team=\"99\"><Vector X=\"1\" Y=\"2\" /></MissionTarget>";

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
        string setXml = $"<MissionTarget Name=\"{unitName}\" X=\"1\" Y=\"2\" Radius=\"9\" Gravity=\"0\" Team=\"{teamValue.Id}\"><Vector X=\"3\" Y=\"4\" /></MissionTarget>";

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

        ship.Dispose();
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

    private static Task StartEventPump(Galaxy playerGalaxy, ConcurrentQueue<FlattiverseEvent> playerEvents)
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
            catch (ConnectionTerminatedGameException)
            {
            }
        });
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
        XElement root = new XElement("Galaxy");

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
}
