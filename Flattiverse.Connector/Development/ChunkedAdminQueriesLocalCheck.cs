using System.Collections.Concurrent;
using System.Globalization;
using Flattiverse.Connector;
using Flattiverse.Connector.Account;
using Flattiverse.Connector.Events;
using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Network;
using Flattiverse.Connector.Units;
using Npgsql;
using TournamentAccount = Flattiverse.Connector.Account.Account;

namespace Development;

partial class Program
{
    private const int ExpectedAccountChunkMaximumCount = 8;
    private const int ExpectedEditableUnitChunkMaximumCount = 16;
    private const int ExpectedAvatarChunkMaximumLength = 16384;

    private static async Task RunChunkedAdminQueriesCheckLocal()
    {
        const int TemporaryEditableUnitCount = 40;
            const int HiddenPowerUpRespawnTicks = 600;
            const float HiddenPowerUpRespawnPlayerDistance = 800f;
            const float HiddenPowerUpOffsetX = 18f;

        DatabaseAvatarRow originalAvatarRow = QueryAvatarRow(LocalSwitchGatePlayerAuth);
        DatabaseAccountRow localPlayerAccount = QueryAccountRow(LocalSwitchGatePlayerAuth);
        byte[] oversizedSmallAvatar = CreatePatternBytes(ExpectedAvatarChunkMaximumLength + 7000, 0x31);
        byte[] oversizedBigAvatar = CreatePatternBytes(ExpectedAvatarChunkMaximumLength * 5 + 321, 0x67);
        List<string> cleanupUnitNames = new List<string>(TemporaryEditableUnitCount);
        ConcurrentQueue<FlattiverseEvent> playerEvents = new ConcurrentQueue<FlattiverseEvent>();
        Galaxy? adminGalaxy = null;
        Galaxy? playerGalaxy = null;
        Task? playerEventPump = null;

        await WaitForSessionGalaxy(LocalSwitchGateAdminAuth, null, 7000).ConfigureAwait(false);
        await WaitForSessionGalaxy(LocalSwitchGatePlayerAuth, null, 7000).ConfigureAwait(false);

        try
        {
            UpdateAvatarRow(originalAvatarRow.AccountId, oversizedSmallAvatar, oversizedBigAvatar);

            Console.WriteLine("CHUNKED-LOCAL: connecting admin...");
            adminGalaxy = await Galaxy.Connect(LocalSwitchGateUri, LocalSwitchGateAdminAuth, null).ConfigureAwait(false);

            playerGalaxy = await ConnectLocalPlayer(LocalSwitchGatePlayerAuth, TeamName, "CHUNKED-LOCAL:PLAYER").ConfigureAwait(false);
            playerEventPump = StartEventPump("CHUNKED-LOCAL:PLAYER", playerGalaxy, playerEvents);

            ProgressState queryAccountsProgress = new ProgressState();
            Task<TournamentAccount[]> queryAccountsTask = adminGalaxy.QueryAccounts(queryAccountsProgress);
            TournamentAccount[] queriedAccounts = await AwaitWithProgressPolling(queryAccountsTask, queryAccountsProgress, "CHUNKED-LOCAL:accounts")
                .ConfigureAwait(false);

            int expectedAccountCount;

            using (NpgsqlConnection connection = new NpgsqlConnection(DatabaseConnectionString))
            {
                connection.Open();

                using NpgsqlCommand command = new NpgsqlCommand(
                    """
                    SELECT COUNT(*)
                    FROM public."accounts"
                    WHERE "status" = 'user'
                       OR "status" = 'reoptin'
                    """,
                    connection);

                object? scalar = command.ExecuteScalar();
                expectedAccountCount = scalar is null || scalar is DBNull ? 0 : Convert.ToInt32(scalar);
            }

            if (queriedAccounts.Length != expectedAccountCount)
                throw new InvalidOperationException(
                    $"CHUNKED-LOCAL: QueryAccounts returned {queriedAccounts.Length} accounts, expected {expectedAccountCount}.");

            if (expectedAccountCount <= ExpectedAccountChunkMaximumCount)
                throw new InvalidOperationException(
                    $"CHUNKED-LOCAL: expected more than {ExpectedAccountChunkMaximumCount} user/reoptin accounts to force multiple chunks, got {expectedAccountCount}.");

            if (!queryAccountsProgress.Finished ||
                queryAccountsProgress.Current != queriedAccounts.Length ||
                queryAccountsProgress.Total != queriedAccounts.Length ||
                queryAccountsProgress.Updates < 2)
                throw new InvalidOperationException(
                    $"CHUNKED-LOCAL: QueryAccounts progress invalid (current={queryAccountsProgress.Current}, total={queryAccountsProgress.Total}, updates={queryAccountsProgress.Updates}, finished={queryAccountsProgress.Finished}).");

            bool playerAccountsRejected = false;

            try
            {
                await playerGalaxy.QueryAccounts().ConfigureAwait(false);
            }
            catch (PermissionFailedGameException)
            {
                playerAccountsRejected = true;
            }

            if (!playerAccountsRejected)
                throw new InvalidOperationException("CHUNKED-LOCAL: non-admin QueryAccounts unexpectedly succeeded.");

            TournamentAccount localPlayerAccountSnapshot = FindAccount(queriedAccounts, localPlayerAccount.AccountId);
            ProgressState playerSmallAvatarProgress = new ProgressState();
            ProgressState playerBigAvatarProgress = new ProgressState();
            ProgressState accountSmallAvatarProgress = new ProgressState();
            ProgressState accountBigAvatarProgress = new ProgressState();

            byte[] downloadedPlayerSmallAvatar = await AwaitWithProgressPolling(
                    playerGalaxy.Player.DownloadSmallAvatar(playerSmallAvatarProgress),
                    playerSmallAvatarProgress,
                    "CHUNKED-LOCAL:player-small-avatar")
                .ConfigureAwait(false);
            byte[] downloadedPlayerBigAvatar = await AwaitWithProgressPolling(
                    playerGalaxy.Player.DownloadBigAvatar(playerBigAvatarProgress),
                    playerBigAvatarProgress,
                    "CHUNKED-LOCAL:player-big-avatar")
                .ConfigureAwait(false);
            byte[] downloadedAccountSmallAvatar = await AwaitWithProgressPolling(
                    localPlayerAccountSnapshot.DownloadSmallAvatar(accountSmallAvatarProgress),
                    accountSmallAvatarProgress,
                    "CHUNKED-LOCAL:account-small-avatar")
                .ConfigureAwait(false);
            byte[] downloadedAccountBigAvatar = await AwaitWithProgressPolling(
                    localPlayerAccountSnapshot.DownloadBigAvatar(accountBigAvatarProgress),
                    accountBigAvatarProgress,
                    "CHUNKED-LOCAL:account-big-avatar")
                .ConfigureAwait(false);

            if (!downloadedPlayerSmallAvatar.SequenceEqual(oversizedSmallAvatar) ||
                !downloadedAccountSmallAvatar.SequenceEqual(oversizedSmallAvatar) ||
                !downloadedPlayerBigAvatar.SequenceEqual(oversizedBigAvatar) ||
                !downloadedAccountBigAvatar.SequenceEqual(oversizedBigAvatar))
                throw new InvalidOperationException("CHUNKED-LOCAL: avatar downloads did not roundtrip the oversized avatar bytes.");

            ValidateCompletedProgress(playerSmallAvatarProgress, oversizedSmallAvatar.Length, "player small avatar");
            ValidateCompletedProgress(playerBigAvatarProgress, oversizedBigAvatar.Length, "player big avatar");
            ValidateCompletedProgress(accountSmallAvatarProgress, oversizedSmallAvatar.Length, "account small avatar");
            ValidateCompletedProgress(accountBigAvatarProgress, oversizedBigAvatar.Length, "account big avatar");

            if (playerSmallAvatarProgress.Updates < 2 ||
                playerBigAvatarProgress.Updates < 2 ||
                accountSmallAvatarProgress.Updates < 2 ||
                accountBigAvatarProgress.Updates < 2)
                throw new InvalidOperationException("CHUNKED-LOCAL: at least one oversized avatar download did not use multiple chunks.");

            ClassicShipControllable ship = await playerGalaxy.CreateClassicShip($"ChunkedLocalShip{Environment.ProcessId}").ConfigureAwait(false);

            if (!adminGalaxy.Clusters.TryGet(ship.Cluster.Id, out Cluster? adminCluster) || adminCluster is null)
                throw new InvalidOperationException($"CHUNKED-LOCAL: could not resolve admin cluster #{ship.Cluster.Id}.");

            await ship.Continue().ConfigureAwait(false);

            bool shipAlive = await WaitForAliveState(ship, true, 3000).ConfigureAwait(false);

            if (!shipAlive)
                throw new InvalidOperationException("CHUNKED-LOCAL: ship did not become alive after Continue().");

            await ship.Engine.Off().ConfigureAwait(false);
            await ship.MainScanner.Set(90f, 60f, 0f).ConfigureAwait(false);
            await ship.MainScanner.On().ConfigureAwait(false);
            await Task.Delay(250).ConfigureAwait(false);
            DrainEvents(playerEvents);

            string hiddenPowerUpName = $"ChunkedLocalPowerUpHidden{Environment.ProcessId}";
            cleanupUnitNames.Add(hiddenPowerUpName);

            string hiddenPowerUpXml =
                $"<EnergyChargePowerUp Name=\"{hiddenPowerUpName}\" X=\"{(ship.Position.X + HiddenPowerUpOffsetX).ToString("R", CultureInfo.InvariantCulture)}\" Y=\"{ship.Position.Y.ToString("R", CultureInfo.InvariantCulture)}\" Radius=\"6\" Gravity=\"0\" Amount=\"4\" RespawnTicks=\"{HiddenPowerUpRespawnTicks}\" RespawnPlayerDistance=\"{HiddenPowerUpRespawnPlayerDistance.ToString("R", CultureInfo.InvariantCulture)}\" />";

            await adminCluster.SetUnit(hiddenPowerUpXml).ConfigureAwait(false);

            for (int index = 0; index < TemporaryEditableUnitCount - 1; index++)
            {
                string unitName = $"ChunkedLocalPowerUp{Environment.ProcessId:D6}{index:D2}";
                float unitX = ship.Position.X + 220f + index * 12f;
                float unitY = ship.Position.Y + 180f;
                string unitXml =
                    $"<EnergyChargePowerUp Name=\"{unitName}\" X=\"{unitX.ToString("R", CultureInfo.InvariantCulture)}\" Y=\"{unitY.ToString("R", CultureInfo.InvariantCulture)}\" Radius=\"6\" Gravity=\"0\" Amount=\"{(index + 2).ToString(CultureInfo.InvariantCulture)}\" RespawnTicks=\"{HiddenPowerUpRespawnTicks}\" RespawnPlayerDistance=\"{HiddenPowerUpRespawnPlayerDistance.ToString("R", CultureInfo.InvariantCulture)}\" />";

                cleanupUnitNames.Add(unitName);
                await adminCluster.SetUnit(unitXml).ConfigureAwait(false);
            }

            List<FlattiverseEvent> hiddenPowerUpEvents = new List<FlattiverseEvent>();
            PowerUpCollectedEvent? hiddenPowerUpCollectedEvent = await WaitForQueuedEvent(playerEvents, 4000, hiddenPowerUpEvents,
                delegate (PowerUpCollectedEvent @event)
                {
                    return @event.PowerUpName == hiddenPowerUpName;
                }).ConfigureAwait(false);

            if (hiddenPowerUpCollectedEvent is null)
                throw new InvalidOperationException("CHUNKED-LOCAL: the hidden power-up was not collected.");

            await Task.Delay(250).ConfigureAwait(false);

            if (TryFindUnit(adminGalaxy, ship.Cluster.Id, hiddenPowerUpName, out Flattiverse.Connector.Units.PowerUp? _))
                throw new InvalidOperationException("CHUNKED-LOCAL: the collected power-up is still visible in the admin cluster view.");

            ProgressState editableUnitsProgress = new ProgressState();
            EditableUnitSummary[] editableUnits = await AwaitWithProgressPolling(adminCluster.QueryEditableUnits(editableUnitsProgress),
                    editableUnitsProgress,
                    "CHUNKED-LOCAL:editable-units")
                .ConfigureAwait(false);

            if (editableUnitsProgress.Updates < 2 ||
                !editableUnitsProgress.Finished ||
                editableUnitsProgress.Current != editableUnits.Length ||
                editableUnitsProgress.Total != editableUnits.Length)
                throw new InvalidOperationException(
                    $"CHUNKED-LOCAL: editable-unit progress invalid (current={editableUnitsProgress.Current}, total={editableUnitsProgress.Total}, updates={editableUnitsProgress.Updates}, finished={editableUnitsProgress.Finished}).");

            if (editableUnits.Length <= ExpectedEditableUnitChunkMaximumCount)
                throw new InvalidOperationException(
                    $"CHUNKED-LOCAL: expected more than {ExpectedEditableUnitChunkMaximumCount} editable units to force multiple chunks, got {editableUnits.Length}.");

            EditableUnitSummary hiddenPowerUpSummary = FindEditableUnit(editableUnits, hiddenPowerUpName);

            if (hiddenPowerUpSummary.Kind != UnitKind.EnergyChargePowerUp)
                throw new InvalidOperationException(
                    $"CHUNKED-LOCAL: hidden power-up returned kind {hiddenPowerUpSummary.Kind} instead of {UnitKind.EnergyChargePowerUp}.");

            for (int index = 0; index < cleanupUnitNames.Count; index++)
                FindEditableUnit(editableUnits, cleanupUnitNames[index]);

            bool playerEditableUnitsRejected = false;

            try
            {
                await ship.Cluster.QueryEditableUnits().ConfigureAwait(false);
            }
            catch (PermissionFailedGameException)
            {
                playerEditableUnitsRejected = true;
            }

            if (!playerEditableUnitsRejected)
                throw new InvalidOperationException("CHUNKED-LOCAL: non-admin QueryEditableUnits unexpectedly succeeded.");

            Console.WriteLine("CHUNKED-LOCAL: SUCCESS");
            Console.WriteLine($"CHUNKED-LOCAL: QueryAccounts count = {queriedAccounts.Length}, progress updates = {queryAccountsProgress.Updates}");
            Console.WriteLine(
                $"CHUNKED-LOCAL: EditableUnits count = {editableUnits.Length}, progress updates = {editableUnitsProgress.Updates}, hidden unit = {hiddenPowerUpName}");
            Console.WriteLine(
                $"CHUNKED-LOCAL: Big avatar bytes = {oversizedBigAvatar.Length}, player progress updates = {playerBigAvatarProgress.Updates}, account progress updates = {accountBigAvatarProgress.Updates}");
        }
        finally
        {
            if (adminGalaxy is not null)
                for (int index = cleanupUnitNames.Count - 1; index >= 0; index--)
                    if (TryGetActiveStartCluster(adminGalaxy, out Cluster? cleanupCluster) && cleanupCluster is not null)
                        try
                        {
                            await cleanupCluster.RemoveUnit(cleanupUnitNames[index]).ConfigureAwait(false);
                        }
                        catch (GameException exception)
                        {
                            Console.WriteLine($"CHUNKED-LOCAL: cleanup RemoveUnit({cleanupUnitNames[index]}) failed: {exception.Message}");
                        }

            if (playerGalaxy is not null)
                playerGalaxy.Dispose();

            if (adminGalaxy is not null)
                adminGalaxy.Dispose();

            if (playerEventPump is not null)
                await Task.WhenAny(playerEventPump, Task.Delay(1000)).ConfigureAwait(false);

            RestoreAvatarRow(originalAvatarRow);
            await WaitForSessionGalaxy(LocalSwitchGatePlayerAuth, null, 7000).ConfigureAwait(false);
            await WaitForSessionGalaxy(LocalSwitchGateAdminAuth, null, 7000).ConfigureAwait(false);
        }
    }

    private static async Task<T> AwaitWithProgressPolling<T>(Task<T> task, ProgressState progressState, string label)
    {
        long lastUpdateCount = -1;

        while (!task.IsCompleted)
        {
            long currentUpdateCount = progressState.Updates;

            if (currentUpdateCount != lastUpdateCount)
            {
                Console.WriteLine(
                    $"{label}: progress current={progressState.Current} total={progressState.Total} updates={progressState.Updates} finished={progressState.Finished}");
                lastUpdateCount = currentUpdateCount;
            }

            await Task.Delay(5).ConfigureAwait(false);
        }

        T result = await task.ConfigureAwait(false);

        Console.WriteLine(
            $"{label}: final current={progressState.Current} total={progressState.Total} updates={progressState.Updates} finished={progressState.Finished}");

        return result;
    }

    private static void ValidateCompletedProgress(ProgressState progressState, int expectedTotal, string label)
    {
        if (!progressState.Finished ||
            progressState.Total != expectedTotal ||
            progressState.Current != expectedTotal)
            throw new InvalidOperationException(
                $"CHUNKED-LOCAL: {label} progress invalid (current={progressState.Current}, total={progressState.Total}, updates={progressState.Updates}, finished={progressState.Finished}).");
    }

    private static EditableUnitSummary FindEditableUnit(EditableUnitSummary[] editableUnits, string unitName)
    {
        for (int index = 0; index < editableUnits.Length; index++)
            if (editableUnits[index].Name == unitName)
                return editableUnits[index];

        throw new InvalidOperationException($"CHUNKED-LOCAL: editable unit {unitName} was not returned by QueryEditableUnits.");
    }

    private static byte[] CreatePatternBytes(int length, byte seed)
    {
        byte[] data = new byte[length];

        for (int index = 0; index < data.Length; index++)
            data[index] = (byte)(seed + index);

        return data;
    }
}
