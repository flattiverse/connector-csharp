using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Xml.Linq;
using Flattiverse.Connector.Account;
using Flattiverse.Connector;
using Flattiverse.Connector.Events;
using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;
using Npgsql;
using TournamentAccount = Flattiverse.Connector.Account.Account;

namespace Development;

partial class Program
{
    private const ushort TournamentGalaxyId = 666;
    private const string LocalTournamentHomepageListUri = "http://127.0.0.1:8000/galaxies/list";
    private const string LocalHomepageProjectPath = "D:\\Projects\\fv\\fv-homepage\\homepage\\homepage.csproj";
    private const string LocalHomepageWorkingDirectory = "D:\\Projects\\fv";

    private static async Task RunTournamentCheckLocal()
    {
        const uint TournamentDurationTicks = 1400;
        const int TournamentWaitTimeoutMs = 120000;
        const int HomepageTimeoutMs = 45000;

        Galaxy? adminGalaxy = null;
        Galaxy? participantGalaxy = null;
        Task? adminEventPump = null;
        Task? participantEventPump = null;
        Process? homepageProcess = null;
        ConcurrentQueue<FlattiverseEvent> adminEvents = new ConcurrentQueue<FlattiverseEvent>();
        ConcurrentQueue<FlattiverseEvent> participantEvents = new ConcurrentQueue<FlattiverseEvent>();
        TournamentAccount[] queriedAccounts;
        string pinkParticipantAuth = string.Empty;
        string greenParticipantAuth = string.Empty;
        DatabaseAccountRow pinkAccountRow = default;
        DatabaseAccountRow greenAccountRow = default;
        float? pinkEloBefore = null;
        float? greenEloBefore = null;
        float? pinkEloAfterRated;
        float? greenEloAfterRated;
        byte? createdRegionId = null;
        string createdUnitName = $"TournamentLocalDomination{DateTime.UtcNow:yyyyMMddHHmmss}";
        GameMode restoreGameMode = GameMode.Mission;

        await WaitForSessionGalaxy(LocalSwitchGateAdminAuth, null, 7000).ConfigureAwait(false);

        try
        {
            Console.WriteLine("TOURNAMENT-LOCAL: connecting admin...");
            adminGalaxy = await Galaxy.Connect(LocalSwitchGateUri, LocalSwitchGateAdminAuth, null).ConfigureAwait(false);
            adminEventPump = StartEventPump("TOURNAMENT-LOCAL:ADMIN", adminGalaxy, adminEvents);
            DrainEvents(adminEvents);

            if (!TryGetTwoNonSpectatorTeams(adminGalaxy, out Team? pinkTeam, out Team? greenTeam) || pinkTeam is null || greenTeam is null)
                throw new InvalidOperationException("TOURNAMENT-LOCAL: galaxy 666 does not expose two non-spectator teams.");

            restoreGameMode = adminGalaxy.GameMode;

            if (adminGalaxy.Tournament is not null)
            {
                Console.WriteLine("TOURNAMENT-LOCAL: pre-existing tournament detected, cancelling it first.");
                await adminGalaxy.CancelTournament().ConfigureAwait(false);
                await Task.Delay(200).ConfigureAwait(false);
                DrainEvents(adminEvents);
            }

            string[] participantAuths = QueryCandidatePlayerAuths(Array.Empty<int>(), 8);

            if (participantAuths.Length < 2)
                throw new InvalidOperationException("TOURNAMENT-LOCAL: not enough regular user accounts with apiPlayer keys available for the tournament test.");

            pinkParticipantAuth = participantAuths[0];
            greenParticipantAuth = participantAuths[1];
            pinkAccountRow = QueryAccountRow(pinkParticipantAuth);
            greenAccountRow = QueryAccountRow(greenParticipantAuth);
            pinkEloBefore = QueryAccountElo(pinkAccountRow.AccountId);
            greenEloBefore = QueryAccountElo(greenAccountRow.AccountId);

            await WaitForSessionGalaxy(pinkParticipantAuth, null, 7000).ConfigureAwait(false);
            await WaitForSessionGalaxy(greenParticipantAuth, null, 7000).ConfigureAwait(false);

            if (QueryGalaxyTournamentCount(TournamentGalaxyId) != 0 ||
                QueryGalaxyTournamentParticipantCount(TournamentGalaxyId) != 0 ||
                QueryGalaxyTournamentResultCount(TournamentGalaxyId) != 0)
                throw new InvalidOperationException("TOURNAMENT-LOCAL: tournament persistence was not empty before the test.");

            bool startWithoutTournamentRejected = false;

            try
            {
                await adminGalaxy.StartTournament().ConfigureAwait(false);
            }
            catch (TournamentNotConfiguredGameException)
            {
                startWithoutTournamentRejected = true;
            }

            if (!startWithoutTournamentRejected)
                throw new InvalidOperationException("TOURNAMENT-LOCAL: StartTournament without configuration unexpectedly succeeded.");

            queriedAccounts = await adminGalaxy.QueryAccounts().ConfigureAwait(false);

            if (!ContainsAccount(queriedAccounts, pinkAccountRow.AccountId) ||
                !ContainsAccount(queriedAccounts, greenAccountRow.AccountId))
                throw new InvalidOperationException("TOURNAMENT-LOCAL: QueryAccounts did not return the configured tournament participants.");

            Console.WriteLine($"TOURNAMENT-LOCAL: QueryAccounts returned the participant accounts {pinkAccountRow.AccountId} and {greenAccountRow.AccountId}.");

            TournamentConfiguration tournamentConfiguration = BuildTournamentConfiguration(pinkTeam, greenTeam,
                pinkAccountRow.AccountId, greenAccountRow.AccountId, TournamentDurationTicks);
            string[] candidateAuths = QueryCandidatePlayerAuths(new int[] { pinkAccountRow.AccountId, greenAccountRow.AccountId }, 16);

            bool missionConfigureRejected = false;

            try
            {
                await adminGalaxy.ConfigureTournament(tournamentConfiguration).ConfigureAwait(false);
            }
            catch (TournamentModeNotAllowedGameException)
            {
                missionConfigureRejected = true;
            }

            if (!missionConfigureRejected)
                throw new InvalidOperationException("TOURNAMENT-LOCAL: configuring a tournament on Mission unexpectedly succeeded.");

            Console.WriteLine("TOURNAMENT-LOCAL: switching galaxy 666 to Domination...");
            await adminGalaxy.Configure(BuildGameModeConfigurationXml(adminGalaxy, GameMode.Domination)).ConfigureAwait(false);

            if (adminGalaxy.GameMode != GameMode.Domination)
                throw new InvalidOperationException($"TOURNAMENT-LOCAL: expected GameMode Domination, got {adminGalaxy.GameMode}.");

            if (!TryGetActiveStartCluster(adminGalaxy, out Cluster? startCluster) || startCluster is null)
                throw new InvalidOperationException("TOURNAMENT-LOCAL: no active start cluster available.");

            createdRegionId = await FindUnusedRegionId(startCluster).ConfigureAwait(false);

            string regionXml =
                $"<Region Id=\"{createdRegionId.Value}\" Name=\"Tournament Local Spawn\" Left=\"-120\" Top=\"-120\" Right=\"120\" Bottom=\"120\"><Team Id=\"{pinkTeam.Id}\" /><Team Id=\"{greenTeam.Id}\" /></Region>";

            Console.WriteLine($"TOURNAMENT-LOCAL: creating helper region #{createdRegionId.Value} in cluster {startCluster.Id}:{startCluster.Name}...");
            await startCluster.SetRegion(regionXml).ConfigureAwait(false);

            string dominationPointXml =
                $"<DominationPoint Name=\"{createdUnitName}\" X=\"0\" Y=\"0\" Radius=\"12\" Gravity=\"0\" Team=\"{pinkTeam.Id}\" DominationRadius=\"90\" />";

            Console.WriteLine($"TOURNAMENT-LOCAL: creating helper domination point {createdUnitName}...");
            await startCluster.SetUnit(dominationPointXml).ConfigureAwait(false);

            Console.WriteLine("TOURNAMENT-LOCAL: configuring tournament...");
            await adminGalaxy.ConfigureTournament(tournamentConfiguration).ConfigureAwait(false);

            List<FlattiverseEvent> configureEvents = new List<FlattiverseEvent>();
            CreatedTournamentEvent? createdEvent = await WaitForQueuedEvent(adminEvents, 4000, configureEvents,
                delegate (CreatedTournamentEvent @event)
                {
                    return @event.Tournament.Stage == TournamentStage.Preparation;
                }).ConfigureAwait(false);
            TournamentMessageEvent? configureMessage = await WaitForQueuedEvent(adminEvents, 2000, configureEvents,
                delegate (TournamentMessageEvent _)
                {
                    return true;
                }).ConfigureAwait(false);

            if (createdEvent is null || configureMessage is null || adminGalaxy.Tournament is null ||
                adminGalaxy.Tournament.Stage != TournamentStage.Preparation || adminGalaxy.Tournament.CurrentMatchNumber != 1)
                throw new InvalidOperationException("TOURNAMENT-LOCAL: configure did not produce the expected tournament state.");

            if (QueryGalaxyTournamentCount(TournamentGalaxyId) != 1 ||
                QueryGalaxyTournamentParticipantCount(TournamentGalaxyId) != 2 ||
                QueryGalaxyTournamentResultCount(TournamentGalaxyId) != 0)
                throw new InvalidOperationException("TOURNAMENT-LOCAL: tournament persistence after configure is inconsistent.");

            bool mapEditRejected = false;

            try
            {
                await startCluster.SetUnit(
                    $"<Buoy Name=\"TournamentLockProbe\" X=\"35\" Y=\"35\" Radius=\"8\" Gravity=\"0\" Message=\"Locked\" />").ConfigureAwait(false);
            }
            catch (TournamentMapEditingLockedGameException)
            {
                mapEditRejected = true;
            }

            if (!mapEditRejected)
                throw new InvalidOperationException("TOURNAMENT-LOCAL: map editing was not locked in Preparation.");

            Console.WriteLine("TOURNAMENT-LOCAL: starting local homepage for /galaxies/list verification...");
            homepageProcess = StartHomepageProcess();

            string homepagePayload = await WaitForHomepageTournamentStage(TournamentGalaxyId, TournamentStage.Preparation,
                pinkAccountRow.AccountId, greenAccountRow.AccountId, HomepageTimeoutMs).ConfigureAwait(false);

            if (!homepagePayload.Contains("\"mode\":\"Solo\"", StringComparison.Ordinal) ||
                !homepagePayload.Contains("\"currentMatchNumber\":1", StringComparison.Ordinal))
                throw new InvalidOperationException("TOURNAMENT-LOCAL: homepage payload does not contain the expected tournament fields.");

            StopProcess(homepageProcess);
            homepageProcess = null;

            bool playerConfigureRejected = false;
            Galaxy? permissionProbeGalaxy = null;
            string? permissionProbeAuth = null;

            try
            {
                for (int authIndex = 0; authIndex < candidateAuths.Length && !playerConfigureRejected; authIndex++)
                {
                    try
                    {
                        permissionProbeGalaxy = await Galaxy.Connect(LocalSwitchGateUri, candidateAuths[authIndex], pinkTeam.Name).ConfigureAwait(false);
                        permissionProbeAuth = candidateAuths[authIndex];

                        try
                        {
                            await permissionProbeGalaxy.ConfigureTournament(tournamentConfiguration).ConfigureAwait(false);
                        }
                        catch (PermissionFailedGameException)
                        {
                            playerConfigureRejected = true;
                        }

                        break;
                    }
                    catch (WrongAccountStateGameException)
                    {
                    }
                    catch (AccountAlreadyLoggedInGameException)
                    {
                    }
                    catch (TeamSelectionFailedGameException)
                    {
                    }
                }
            }
            finally
            {
                if (permissionProbeGalaxy is not null)
                    permissionProbeGalaxy.Dispose();

                if (permissionProbeAuth is not null)
                    await WaitForSessionGalaxy(permissionProbeAuth, null, 7000).ConfigureAwait(false);
            }

            if (!playerConfigureRejected)
                throw new InvalidOperationException("TOURNAMENT-LOCAL: non-admin ConfigureTournament unexpectedly succeeded.");

            bool startInPreparationRejected = false;

            try
            {
                await adminGalaxy.StartTournament().ConfigureAwait(false);
            }
            catch (TournamentWrongStageGameException)
            {
                startInPreparationRejected = true;
            }

            if (!startInPreparationRejected)
                throw new InvalidOperationException("TOURNAMENT-LOCAL: StartTournament in Preparation unexpectedly succeeded.");

            Console.WriteLine("TOURNAMENT-LOCAL: commencing tournament...");
            await adminGalaxy.CommenceTournament().ConfigureAwait(false);

            List<FlattiverseEvent> commenceEvents = new List<FlattiverseEvent>();
            UpdatedTournamentEvent? commenceEvent = await WaitForQueuedEvent(adminEvents, 4000, commenceEvents,
                delegate (UpdatedTournamentEvent @event)
                {
                    return @event.NewTournament.Stage == TournamentStage.Commencing;
                }).ConfigureAwait(false);

            if (commenceEvent is null || adminGalaxy.Tournament is null || adminGalaxy.Tournament.Stage != TournamentStage.Commencing)
                throw new InvalidOperationException("TOURNAMENT-LOCAL: CommenceTournament did not enter Commencing.");

            bool spectatorRejected = false;

            try
            {
                Galaxy spectatorGalaxy = await Galaxy.Connect(LocalSwitchGateUri, null, null).ConfigureAwait(false);
                spectatorGalaxy.Dispose();
            }
            catch (TournamentSpectatingForbiddenGameException)
            {
                spectatorRejected = true;
            }

            if (!spectatorRejected)
                throw new InvalidOperationException("TOURNAMENT-LOCAL: spectator login unexpectedly succeeded during Commencing.");

            bool wrongTeamRejected = false;

            try
            {
                Galaxy wrongTeamGalaxy = await Galaxy.Connect(LocalSwitchGateUri, pinkParticipantAuth, greenTeam.Name).ConfigureAwait(false);
                wrongTeamGalaxy.Dispose();
            }
            catch (TournamentTeamMismatchGameException)
            {
                wrongTeamRejected = true;
            }

            if (!wrongTeamRejected)
                throw new InvalidOperationException("TOURNAMENT-LOCAL: wrong-team login unexpectedly succeeded during Commencing.");

            bool nonParticipantRejected = false;

            for (int authIndex = 0; authIndex < candidateAuths.Length && !nonParticipantRejected; authIndex++)
                try
                {
                    Galaxy candidateGalaxy = await Galaxy.Connect(LocalSwitchGateUri, candidateAuths[authIndex], null).ConfigureAwait(false);
                    candidateGalaxy.Dispose();
                }
                catch (TournamentParticipantRequiredGameException)
                {
                    nonParticipantRejected = true;
                }
                catch (WrongAccountStateGameException)
                {
                }
                catch (AccountAlreadyLoggedInGameException)
                {
                }
                catch (TeamSelectionFailedGameException)
                {
                }

            if (!nonParticipantRejected)
                throw new InvalidOperationException("TOURNAMENT-LOCAL: no non-participant login produced TournamentParticipantRequiredGameException.");

            Console.WriteLine("TOURNAMENT-LOCAL: connecting participant without explicit team...");
            participantGalaxy = await Galaxy.Connect(LocalSwitchGateUri, pinkParticipantAuth, null).ConfigureAwait(false);
            participantEventPump = StartEventPump("TOURNAMENT-LOCAL:PLAYER", participantGalaxy, participantEvents);

            List<FlattiverseEvent> participantInitialEvents = new List<FlattiverseEvent>();
            CreatedTournamentEvent? participantCreatedEvent = await WaitForQueuedEvent(participantEvents, 4000, participantInitialEvents,
                delegate (CreatedTournamentEvent @event)
                {
                    return @event.Tournament.Stage == TournamentStage.Commencing;
                }).ConfigureAwait(false);
            TournamentMessageEvent? participantConnectMessage = await WaitForQueuedEvent(participantEvents, 2000, participantInitialEvents,
                delegate (TournamentMessageEvent _)
                {
                    return true;
                }).ConfigureAwait(false);

            if (participantCreatedEvent is null || participantConnectMessage is null)
                throw new InvalidOperationException("TOURNAMENT-LOCAL: participant did not receive initial tournament state and message.");

            if (participantGalaxy.Player.Team.Id != pinkTeam.Id)
                throw new InvalidOperationException("TOURNAMENT-LOCAL: participant without team was not auto-assigned to the configured tournament team.");

            bool createShipRejected = false;

            try
            {
                await participantGalaxy.CreateClassicShip("TournamentLocalCommenceProbe").ConfigureAwait(false);
            }
            catch (TournamentRegistrationClosedGameException)
            {
                createShipRejected = true;
            }

            if (!createShipRejected)
                throw new InvalidOperationException("TOURNAMENT-LOCAL: ship registration unexpectedly succeeded during Commencing.");

            Console.WriteLine("TOURNAMENT-LOCAL: starting tournament...");
            await adminGalaxy.StartTournament().ConfigureAwait(false);

            List<FlattiverseEvent> startEvents = new List<FlattiverseEvent>();
            UpdatedTournamentEvent? startEvent = await WaitForQueuedEvent(adminEvents, 4000, startEvents,
                delegate (UpdatedTournamentEvent @event)
                {
                    return @event.NewTournament.Stage == TournamentStage.Running;
                }).ConfigureAwait(false);

            if (startEvent is null || adminGalaxy.Tournament is null || adminGalaxy.Tournament.Stage != TournamentStage.Running)
                throw new InvalidOperationException("TOURNAMENT-LOCAL: StartTournament did not enter Running.");

            Console.WriteLine("TOURNAMENT-LOCAL: creating ship during Running...");
            await participantGalaxy.CreateClassicShip("TournamentLocalRunner").ConfigureAwait(false);

            List<FlattiverseEvent> ratedFinishEvents = new List<FlattiverseEvent>();
            RemovedTournamentEvent? ratedFinishEvent = await WaitForQueuedEvent(adminEvents, TournamentWaitTimeoutMs, ratedFinishEvents,
                delegate (RemovedTournamentEvent _)
                {
                    return true;
                }).ConfigureAwait(false);

            if (ratedFinishEvent is null || adminGalaxy.Tournament is not null)
                throw new InvalidOperationException("TOURNAMENT-LOCAL: rated tournament did not finish and clear.");

            pinkEloAfterRated = QueryAccountElo(pinkAccountRow.AccountId);
            greenEloAfterRated = QueryAccountElo(greenAccountRow.AccountId);

            if (pinkEloAfterRated is null || greenEloAfterRated is null)
                throw new InvalidOperationException("TOURNAMENT-LOCAL: rated tournament did not persist elo values.");

            if (pinkEloBefore == pinkEloAfterRated && greenEloBefore == greenEloAfterRated)
                throw new InvalidOperationException("TOURNAMENT-LOCAL: rated tournament left both elo values unchanged.");

            if (pinkEloAfterRated <= greenEloAfterRated)
                throw new InvalidOperationException("TOURNAMENT-LOCAL: expected the Pink participant to have the higher elo after the rated win.");

            if (QueryGalaxyTournamentCount(TournamentGalaxyId) != 0 ||
                QueryGalaxyTournamentParticipantCount(TournamentGalaxyId) != 0 ||
                QueryGalaxyTournamentResultCount(TournamentGalaxyId) != 0)
                throw new InvalidOperationException("TOURNAMENT-LOCAL: tournament persistence was not cleared after the rated finish.");

            TournamentAccount[] postRatedAccounts = await adminGalaxy.QueryAccounts().ConfigureAwait(false);
            TournamentAccount postRatedPinkAccount = FindAccount(postRatedAccounts, pinkAccountRow.AccountId);

            if (postRatedPinkAccount.TournamentElo is null)
                throw new InvalidOperationException("TOURNAMENT-LOCAL: QueryAccounts did not expose the newly persisted elo.");

            Console.WriteLine("TOURNAMENT-LOCAL: starting second tournament to verify Cancel() stays unrated...");
            await adminGalaxy.ConfigureTournament(tournamentConfiguration).ConfigureAwait(false);
            await adminGalaxy.CommenceTournament().ConfigureAwait(false);
            await adminGalaxy.StartTournament().ConfigureAwait(false);
            await adminGalaxy.CancelTournament().ConfigureAwait(false);

            List<FlattiverseEvent> cancelEvents = new List<FlattiverseEvent>();
            RemovedTournamentEvent? cancelEvent = await WaitForQueuedEvent(adminEvents, 4000, cancelEvents,
                delegate (RemovedTournamentEvent _)
                {
                    return true;
                }).ConfigureAwait(false);

            if (cancelEvent is null || adminGalaxy.Tournament is not null)
                throw new InvalidOperationException("TOURNAMENT-LOCAL: CancelTournament did not remove the tournament.");

            if (QueryAccountElo(pinkAccountRow.AccountId) != pinkEloAfterRated ||
                QueryAccountElo(greenAccountRow.AccountId) != greenEloAfterRated)
                throw new InvalidOperationException("TOURNAMENT-LOCAL: CancelTournament unexpectedly changed elo.");

            if (QueryGalaxyTournamentCount(TournamentGalaxyId) != 0 ||
                QueryGalaxyTournamentParticipantCount(TournamentGalaxyId) != 0 ||
                QueryGalaxyTournamentResultCount(TournamentGalaxyId) != 0)
                throw new InvalidOperationException("TOURNAMENT-LOCAL: tournament persistence was not cleared after CancelTournament.");

            Console.WriteLine("TOURNAMENT-LOCAL: SUCCESS");
            Console.WriteLine($"TOURNAMENT-LOCAL: elo before = pink {FormatNullableFloat(pinkEloBefore)}, green {FormatNullableFloat(greenEloBefore)}");
            Console.WriteLine($"TOURNAMENT-LOCAL: elo after rated = pink {FormatNullableFloat(pinkEloAfterRated)}, green {FormatNullableFloat(greenEloAfterRated)}");
        }
        finally
        {
            if (homepageProcess is not null)
                StopProcess(homepageProcess);

            if (participantGalaxy is not null)
                participantGalaxy.Dispose();

            if (pinkParticipantAuth.Length > 0)
                await WaitForSessionGalaxy(pinkParticipantAuth, null, 7000).ConfigureAwait(false);

            if (adminGalaxy is not null)
            {
                if (adminGalaxy.Tournament is not null)
                {
                    try
                    {
                        await adminGalaxy.CancelTournament().ConfigureAwait(false);
                    }
                    catch (GameException exception)
                    {
                        Console.WriteLine($"TOURNAMENT-LOCAL: cleanup CancelTournament failed: {exception.Message}");
                    }
                }

                if (createdRegionId is not null && createdUnitName.Length > 0 && TryGetActiveStartCluster(adminGalaxy, out Cluster? cleanupCluster) &&
                    cleanupCluster is not null && adminGalaxy.Tournament is null)
                {
                    try
                    {
                        await cleanupCluster.RemoveUnit(createdUnitName).ConfigureAwait(false);
                    }
                    catch (GameException exception)
                    {
                        Console.WriteLine($"TOURNAMENT-LOCAL: cleanup RemoveUnit failed: {exception.Message}");
                    }

                    try
                    {
                        await cleanupCluster.RemoveRegion(createdRegionId.Value).ConfigureAwait(false);
                    }
                    catch (GameException exception)
                    {
                        Console.WriteLine($"TOURNAMENT-LOCAL: cleanup RemoveRegion failed: {exception.Message}");
                    }
                }

                if (adminGalaxy.Tournament is null && adminGalaxy.GameMode != restoreGameMode)
                    try
                    {
                        await adminGalaxy.Configure(BuildGameModeConfigurationXml(adminGalaxy, restoreGameMode)).ConfigureAwait(false);
                    }
                    catch (GameException exception)
                    {
                        Console.WriteLine($"TOURNAMENT-LOCAL: cleanup game mode restore failed: {exception.Message}");
                    }

                adminGalaxy.Dispose();
            }

            await WaitForSessionGalaxy(LocalSwitchGateAdminAuth, null, 7000).ConfigureAwait(false);

            if (adminEventPump is not null)
                await Task.WhenAny(adminEventPump, Task.Delay(1000)).ConfigureAwait(false);

            if (participantEventPump is not null)
                await Task.WhenAny(participantEventPump, Task.Delay(1000)).ConfigureAwait(false);
        }
    }

    private static TournamentConfiguration BuildTournamentConfiguration(Team pinkTeam, Team greenTeam, int pinkAccountId, int greenAccountId,
        uint durationTicks)
    {
        TournamentTeamConfiguration[] teams = new TournamentTeamConfiguration[2];
        teams[0] = new TournamentTeamConfiguration(pinkTeam, new int[] { pinkAccountId });
        teams[1] = new TournamentTeamConfiguration(greenTeam, new int[] { greenAccountId });
        return new TournamentConfiguration(TournamentMode.Solo, durationTicks, teams, Array.Empty<byte>());
    }

    private static bool ContainsAccount(TournamentAccount[] accounts, int accountId)
    {
        for (int index = 0; index < accounts.Length; index++)
            if (accounts[index].Id == accountId)
                return true;

        return false;
    }

    private static TournamentAccount FindAccount(TournamentAccount[] accounts, int accountId)
    {
        for (int index = 0; index < accounts.Length; index++)
            if (accounts[index].Id == accountId)
                return accounts[index];

        throw new InvalidOperationException($"Account #{accountId} not found in query result.");
    }

    private static string BuildGameModeConfigurationXml(Galaxy galaxy, GameMode gameMode)
    {
        XDocument document = XDocument.Parse(BuildConfigurationXml(galaxy, (ClusterSpec[]?)null), LoadOptions.None);
        XElement? root = document.Root;

        if (root is null)
            throw new InvalidDataException("Galaxy configuration XML has no root element.");

        root.SetAttributeValue("GameMode", gameMode.ToString());
        return document.ToString(SaveOptions.DisableFormatting);
    }

    private static async Task<byte> FindUnusedRegionId(Cluster cluster)
    {
        string currentRegionsXml = await cluster.QueryRegions().ConfigureAwait(false);
        XDocument currentRegionsDocument = XDocument.Parse(currentRegionsXml, LoadOptions.None);
        XElement? currentRegionsRoot = currentRegionsDocument.Root;
        HashSet<byte> usedIds = new HashSet<byte>();

        if (currentRegionsRoot is not null)
            foreach (XElement regionElement in currentRegionsRoot.Elements("Region"))
            {
                XAttribute? idAttribute = regionElement.Attribute("Id");

                if (idAttribute is null || !byte.TryParse(idAttribute.Value, out byte id))
                    continue;

                usedIds.Add(id);
            }

        for (int candidate = 255; candidate >= 0; candidate--)
            if (!usedIds.Contains((byte)candidate))
                return (byte)candidate;

        throw new InvalidOperationException("No free region id available.");
    }

    private static Process StartHomepageProcess()
    {
        ProcessStartInfo startInfo = new ProcessStartInfo("dotnet", $"run --project \"{LocalHomepageProjectPath}\"");

        startInfo.WorkingDirectory = LocalHomepageWorkingDirectory;
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;

        Process? process = Process.Start(startInfo);

        if (process is null)
            throw new InvalidOperationException("Failed to start local homepage process.");

        return process;
    }

    private static void StopProcess(Process process)
    {
        try
        {
            if (!process.HasExited)
                process.Kill(true);
        }
        catch (Exception exception)
        {
            Console.WriteLine($"TOURNAMENT-LOCAL: cleanup process kill failed: {exception.Message}");
        }

        try
        {
            process.WaitForExit(5000);
        }
        catch (Exception exception)
        {
            Console.WriteLine($"TOURNAMENT-LOCAL: cleanup process wait failed: {exception.Message}");
        }
    }

    private static async Task<string> WaitForHomepageTournamentStage(ushort galaxyId, TournamentStage stage, int pinkAccountId, int greenAccountId,
        int timeoutMs)
    {
        using HttpClient client = new HttpClient();
        DateTime deadline = DateTime.UtcNow.AddMilliseconds(timeoutMs);
        string? lastMatchingGalaxyContent = null;

        while (DateTime.UtcNow < deadline)
        {
            try
            {
                string content = await client.GetStringAsync(LocalTournamentHomepageListUri).ConfigureAwait(false);
                using JsonDocument document = JsonDocument.Parse(content);

                JsonElement galaxiesElement = document.RootElement;

                if (document.RootElement.ValueKind == JsonValueKind.Object &&
                    document.RootElement.TryGetProperty("items", out JsonElement itemsElement))
                    galaxiesElement = itemsElement;

                if (galaxiesElement.ValueKind != JsonValueKind.Array)
                    throw new JsonException("Homepage /galaxies/list payload has no items array.");

                foreach (JsonElement galaxyElement in galaxiesElement.EnumerateArray())
                {
                    if (!galaxyElement.TryGetProperty("galaxyId", out JsonElement galaxyIdElement) ||
                        galaxyIdElement.GetInt32() != galaxyId)
                        continue;

                    lastMatchingGalaxyContent = galaxyElement.GetRawText();

                    if (!galaxyElement.TryGetProperty("tournament", out JsonElement tournamentElement) ||
                        tournamentElement.ValueKind != JsonValueKind.Object)
                        break;

                    if (!tournamentElement.TryGetProperty("stage", out JsonElement stageElement) ||
                        stageElement.GetString() != stage.ToString())
                        break;

                    if (!tournamentElement.TryGetProperty("teams", out JsonElement teamsElement) ||
                        teamsElement.ValueKind != JsonValueKind.Array)
                        break;

                    bool foundPinkParticipant = false;
                    bool foundGreenParticipant = false;

                    foreach (JsonElement teamElement in teamsElement.EnumerateArray())
                    {
                        if (!teamElement.TryGetProperty("participants", out JsonElement participantsElement) ||
                            participantsElement.ValueKind != JsonValueKind.Array)
                            continue;

                        foreach (JsonElement participantElement in participantsElement.EnumerateArray())
                        {
                            if (!participantElement.TryGetProperty("userId", out JsonElement userIdElement))
                                continue;

                            int userId = userIdElement.GetInt32();

                            if (userId == pinkAccountId)
                                foundPinkParticipant = true;
                            else if (userId == greenAccountId)
                                foundGreenParticipant = true;
                        }
                    }

                    if (foundPinkParticipant && foundGreenParticipant)
                        return content;

                    break;
                }
            }
            catch (HttpRequestException)
            {
            }
            catch (JsonException)
            {
            }

            await Task.Delay(250).ConfigureAwait(false);
        }

        throw new TimeoutException(
            $"TOURNAMENT-LOCAL: homepage did not expose the expected tournament state in time. Last galaxy payload: {lastMatchingGalaxyContent ?? "<none>"}");
    }

    private static string[] QueryCandidatePlayerAuths(int[] excludedAccountIds, int limit)
    {
        List<string> auths = new List<string>();

        using NpgsqlConnection connection = new NpgsqlConnection(DatabaseConnectionString);
        connection.Open();

        using NpgsqlCommand command = new NpgsqlCommand(
            """
            SELECT encode("apiPlayer", 'hex')
            FROM public."accounts"
            WHERE "apiPlayer" IS NOT NULL
              AND "status" = 'user'
              AND "admin" = FALSE
              AND "id" <> ALL(@excludedAccountIds)
            ORDER BY "id" ASC
            LIMIT @limit
            """,
            connection);

        command.Parameters.AddWithValue("@excludedAccountIds", excludedAccountIds);
        command.Parameters.AddWithValue("@limit", limit);

        using NpgsqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
            auths.Add((string)reader[0]);

        return auths.ToArray();
    }

    private static float? QueryAccountElo(int accountId)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(DatabaseConnectionString);
        connection.Open();

        using NpgsqlCommand command = new NpgsqlCommand(
            """
            SELECT "elo"
            FROM public."accounts"
            WHERE "id" = @accountId
            LIMIT 1
            """,
            connection);

        command.Parameters.AddWithValue("@accountId", accountId);

        object? elo = command.ExecuteScalar();

        if (elo is null || elo is DBNull)
            return null;

        if (elo is float value)
            return value;

        return (float)(double)elo;
    }

    private static int QueryGalaxyTournamentCount(ushort galaxyId)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(DatabaseConnectionString);
        connection.Open();

        using NpgsqlCommand command = new NpgsqlCommand(
            """
            SELECT COUNT(*)
            FROM public."galaxyTournaments"
            WHERE "galaxy" = @galaxy
            """,
            connection);

        command.Parameters.AddWithValue("@galaxy", (short)galaxyId);
        return Convert.ToInt32(command.ExecuteScalar());
    }

    private static int QueryGalaxyTournamentParticipantCount(ushort galaxyId)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(DatabaseConnectionString);
        connection.Open();

        using NpgsqlCommand command = new NpgsqlCommand(
            """
            SELECT COUNT(*)
            FROM public."galaxyTournamentParticipants"
            WHERE "galaxy" = @galaxy
            """,
            connection);

        command.Parameters.AddWithValue("@galaxy", (short)galaxyId);
        return Convert.ToInt32(command.ExecuteScalar());
    }

    private static int QueryGalaxyTournamentResultCount(ushort galaxyId)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(DatabaseConnectionString);
        connection.Open();

        using NpgsqlCommand command = new NpgsqlCommand(
            """
            SELECT COUNT(*)
            FROM public."galaxyTournamentResults"
            WHERE "galaxy" = @galaxy
            """,
            connection);

        command.Parameters.AddWithValue("@galaxy", (short)galaxyId);
        return Convert.ToInt32(command.ExecuteScalar());
    }

    private static string FormatNullableFloat(float? value)
    {
        return value is null ? "null" : value.Value.ToString("0.###", CultureInfo.InvariantCulture);
    }
}
