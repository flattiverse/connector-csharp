using Flattiverse.Connector.GalaxyHierarchy;
using Npgsql;
using AccountSummary = Flattiverse.Connector.Account.Account;

namespace Development;

partial class Program
{
    private const ushort LocalSwitchGateGalaxyId = 666;

    private readonly struct DatabaseGalaxyAclRow
    {
        public readonly GalaxyAclKind Kind;
        public readonly int AccountId;

        public DatabaseGalaxyAclRow(GalaxyAclKind kind, int accountId)
        {
            Kind = kind;
            AccountId = accountId;
        }
    }

    private static async Task RunGalaxyAclCheckLocal(string[] args)
    {
        bool verifyPersisted = args.Length == 2 && string.Equals(args[1], "verify-persisted", StringComparison.Ordinal);

        if (args.Length > 2 || args.Length == 2 && !verifyPersisted)
            throw new InvalidOperationException("ACL-LOCAL: usage Development --galaxy-acl-check-local [verify-persisted]");

        if (verifyPersisted)
            await VerifyPersistedGalaxyAclLocal().ConfigureAwait(false);
        else
            await PrepareGalaxyAclLocal().ConfigureAwait(false);
    }

    private static async Task PrepareGalaxyAclLocal()
    {
        DatabaseAccountRow managerAdmin = QueryAccountRow(LocalSwitchGateAdminAuth);
        string secondaryAdminAuth = QueryAlternativeAdminAuth(managerAdmin.AccountId);
        DatabaseAccountRow secondaryAdmin = QueryAccountRow(secondaryAdminAuth);
        DatabaseAccountRow allowedPlayer = QueryAccountRow(LocalSwitchGatePlayerAuth);
        string deniedPlayerAuth = QueryAlternativePlayerAuth(managerAdmin.AccountId, allowedPlayer.AccountId);
        DatabaseAccountRow deniedPlayer = QueryAccountRow(deniedPlayerAuth);
        Galaxy? adminGalaxy = null;

        await EnsureSessionCleared(LocalSwitchGateAdminAuth, "ACL-LOCAL:allowed-admin").ConfigureAwait(false);
        await EnsureSessionCleared(secondaryAdminAuth, "ACL-LOCAL:secondary-admin").ConfigureAwait(false);
        await EnsureSessionCleared(LocalSwitchGatePlayerAuth, "ACL-LOCAL:allowed-player").ConfigureAwait(false);
        await EnsureSessionCleared(deniedPlayerAuth, "ACL-LOCAL:denied-player").ConfigureAwait(false);

        ClearGalaxyAclEntries(LocalSwitchGateGalaxyId);

        try
        {
            adminGalaxy = await ConnectLocalAdmin(LocalSwitchGateAdminAuth, "ACL-LOCAL:admin").ConfigureAwait(false);

            await VerifyAclList(adminGalaxy, GalaxyAclKind.Player, Array.Empty<int>(), "ACL-LOCAL: player ACL initially empty")
                .ConfigureAwait(false);
            await VerifyAclList(adminGalaxy, GalaxyAclKind.Admin, Array.Empty<int>(), "ACL-LOCAL: admin ACL initially empty")
                .ConfigureAwait(false);

            await ExpectPlayerLoginSuccess(LocalSwitchGatePlayerAuth, "ACL-LOCAL: empty player ACL allows player").ConfigureAwait(false);
            await ExpectPlayerLoginSuccess(deniedPlayerAuth, "ACL-LOCAL: empty player ACL allows second player").ConfigureAwait(false);
            await ExpectAdminLoginSuccess(secondaryAdminAuth, "ACL-LOCAL: empty admin ACL allows second admin").ConfigureAwait(false);

            await adminGalaxy.AddAclAccount(GalaxyAclKind.Player, allowedPlayer.AccountId).ConfigureAwait(false);
            await VerifyAclList(adminGalaxy, GalaxyAclKind.Player, new int[] { allowedPlayer.AccountId },
                    "ACL-LOCAL: player ACL after add")
                .ConfigureAwait(false);

            await ExpectPlayerLoginSuccess(LocalSwitchGatePlayerAuth, "ACL-LOCAL: listed player is allowed").ConfigureAwait(false);
            await ExpectPlayerAccessRestricted(deniedPlayerAuth, "ACL-LOCAL: unlisted player is denied")
                .ConfigureAwait(false);

            await adminGalaxy.RemoveAclAccount(GalaxyAclKind.Player, allowedPlayer.AccountId).ConfigureAwait(false);
            await VerifyAclList(adminGalaxy, GalaxyAclKind.Player, Array.Empty<int>(), "ACL-LOCAL: player ACL after remove")
                .ConfigureAwait(false);

            await ExpectPlayerLoginSuccess(deniedPlayerAuth, "ACL-LOCAL: empty player ACL reopens access")
                .ConfigureAwait(false);

            await adminGalaxy.AddAclAccount(GalaxyAclKind.Admin, secondaryAdmin.AccountId).ConfigureAwait(false);
            await VerifyAclList(adminGalaxy, GalaxyAclKind.Admin, new int[] { secondaryAdmin.AccountId },
                    "ACL-LOCAL: admin ACL after add")
                .ConfigureAwait(false);

            await ExpectAdminLoginSuccess(secondaryAdminAuth, "ACL-LOCAL: listed secondary admin is allowed").ConfigureAwait(false);

            adminGalaxy.Dispose();
            adminGalaxy = null;

            await ExpectAdminAccessRestricted(LocalSwitchGateAdminAuth, "ACL-LOCAL: unlisted manager admin is denied").ConfigureAwait(false);

            adminGalaxy = await ConnectLocalAdmin(secondaryAdminAuth, "ACL-LOCAL:secondary-admin-manager").ConfigureAwait(false);

            await adminGalaxy.RemoveAclAccount(GalaxyAclKind.Admin, secondaryAdmin.AccountId).ConfigureAwait(false);
            await VerifyAclList(adminGalaxy, GalaxyAclKind.Admin, Array.Empty<int>(), "ACL-LOCAL: admin ACL after remove")
                .ConfigureAwait(false);

            await ExpectAdminLoginSuccess(LocalSwitchGateAdminAuth, "ACL-LOCAL: empty admin ACL reopens access").ConfigureAwait(false);

            await adminGalaxy.AddAclAccount(GalaxyAclKind.Player, allowedPlayer.AccountId).ConfigureAwait(false);
            await adminGalaxy.AddAclAccount(GalaxyAclKind.Admin, managerAdmin.AccountId).ConfigureAwait(false);

            await VerifyAclList(adminGalaxy, GalaxyAclKind.Player, new int[] { allowedPlayer.AccountId },
                    "ACL-LOCAL: persisted player ACL prepared")
                .ConfigureAwait(false);
            await VerifyAclList(adminGalaxy, GalaxyAclKind.Admin, new int[] { managerAdmin.AccountId },
                    "ACL-LOCAL: persisted admin ACL prepared")
                .ConfigureAwait(false);

            DatabaseGalaxyAclRow[] persistedRows = QueryGalaxyAclRows(LocalSwitchGateGalaxyId);

            if (persistedRows.Length != 2 ||
                !persistedRows.Any(delegate (DatabaseGalaxyAclRow row) { return row.Kind == GalaxyAclKind.Player && row.AccountId == allowedPlayer.AccountId; }) ||
                !persistedRows.Any(delegate (DatabaseGalaxyAclRow row) { return row.Kind == GalaxyAclKind.Admin && row.AccountId == managerAdmin.AccountId; }))
                throw new InvalidOperationException("ACL-LOCAL: persisted DB rows do not match the prepared ACL state.");

            Console.WriteLine(
                $"ACL-LOCAL: prepared persisted ACL state for galaxy {LocalSwitchGateGalaxyId} (player={allowedPlayer.AccountId}, admin={managerAdmin.AccountId}, deniedAdmin={secondaryAdmin.AccountId}, deniedPlayer={deniedPlayer.AccountId}).");
            Console.WriteLine("ACL-LOCAL: restart the local galaxy 666 now and then run Development --galaxy-acl-check-local verify-persisted.");
        }
        finally
        {
            if (adminGalaxy is not null)
                adminGalaxy.Dispose();
        }
    }

    private static async Task VerifyPersistedGalaxyAclLocal()
    {
        DatabaseAccountRow allowedPlayer = QueryAccountRow(LocalSwitchGatePlayerAuth);
        DatabaseAccountRow allowedAdmin = QueryAccountRow(LocalSwitchGateAdminAuth);
        string deniedAdminAuth = QueryAlternativeAdminAuth(allowedAdmin.AccountId);
        string deniedPlayerAuth = QueryAlternativePlayerAuth(allowedAdmin.AccountId, allowedPlayer.AccountId);
        Galaxy? adminGalaxy = null;

        await EnsureSessionCleared(LocalSwitchGateAdminAuth, "ACL-LOCAL-VERIFY:allowed-admin").ConfigureAwait(false);
        await EnsureSessionCleared(deniedAdminAuth, "ACL-LOCAL-VERIFY:denied-admin").ConfigureAwait(false);
        await EnsureSessionCleared(LocalSwitchGatePlayerAuth, "ACL-LOCAL-VERIFY:allowed-player").ConfigureAwait(false);
        await EnsureSessionCleared(deniedPlayerAuth, "ACL-LOCAL-VERIFY:denied-player").ConfigureAwait(false);

        try
        {
            adminGalaxy = await ConnectLocalAdmin(LocalSwitchGateAdminAuth, "ACL-LOCAL-VERIFY:admin").ConfigureAwait(false);

            await VerifyAclList(adminGalaxy, GalaxyAclKind.Player, new int[] { allowedPlayer.AccountId },
                    "ACL-LOCAL-VERIFY: persisted player ACL loaded after restart")
                .ConfigureAwait(false);
            await VerifyAclList(adminGalaxy, GalaxyAclKind.Admin, new int[] { allowedAdmin.AccountId },
                    "ACL-LOCAL-VERIFY: persisted admin ACL loaded after restart")
                .ConfigureAwait(false);

            await ExpectPlayerLoginSuccess(LocalSwitchGatePlayerAuth, "ACL-LOCAL-VERIFY: listed player remains allowed")
                .ConfigureAwait(false);
            await ExpectPlayerAccessRestricted(deniedPlayerAuth,
                    "ACL-LOCAL-VERIFY: unlisted player remains denied after restart")
                .ConfigureAwait(false);

            Console.WriteLine("ACL-LOCAL-VERIFY: listed admin remains allowed: existing verify connection is active");
            await ExpectAdminAccessRestricted(deniedAdminAuth,
                    "ACL-LOCAL-VERIFY: unlisted admin remains denied after restart")
                .ConfigureAwait(false);

            await adminGalaxy.RemoveAclAccount(GalaxyAclKind.Player, allowedPlayer.AccountId).ConfigureAwait(false);
            await adminGalaxy.RemoveAclAccount(GalaxyAclKind.Admin, allowedAdmin.AccountId).ConfigureAwait(false);

            await VerifyAclList(adminGalaxy, GalaxyAclKind.Player, Array.Empty<int>(), "ACL-LOCAL-VERIFY: player ACL cleaned up")
                .ConfigureAwait(false);
            await VerifyAclList(adminGalaxy, GalaxyAclKind.Admin, Array.Empty<int>(), "ACL-LOCAL-VERIFY: admin ACL cleaned up")
                .ConfigureAwait(false);

            DatabaseGalaxyAclRow[] remainingRows = QueryGalaxyAclRows(LocalSwitchGateGalaxyId);

            if (remainingRows.Length != 0)
                throw new InvalidOperationException("ACL-LOCAL-VERIFY: ACL cleanup did not remove all DB rows.");

            await ExpectPlayerLoginSuccess(deniedPlayerAuth, "ACL-LOCAL-VERIFY: empty player ACL allows player again")
                .ConfigureAwait(false);
            await ExpectAdminLoginSuccess(deniedAdminAuth, "ACL-LOCAL-VERIFY: empty admin ACL allows admin again")
                .ConfigureAwait(false);
        }
        finally
        {
            if (adminGalaxy is not null)
                adminGalaxy.Dispose();
        }
    }

    private static async Task EnsureSessionCleared(string auth, string label)
    {
        if (await WaitForSessionGalaxy(auth, null, 1000).ConfigureAwait(false))
            return;

        ClearLocalAccountSession(auth, label);

        if (!await WaitForSessionGalaxy(auth, null, 7000).ConfigureAwait(false))
            throw new InvalidOperationException($"{label}: session could not be cleared.");
    }

    private static async Task<Galaxy> ConnectLocalAdmin(string auth, string label)
    {
        try
        {
            Console.WriteLine($"{label}: connecting...");
            return await Galaxy.Connect(LocalSwitchGateUri, auth, null).ConfigureAwait(false);
        }
        catch (AccountAlreadyLoggedInGameException)
        {
            ClearLocalAccountSession(auth, label);
            await Task.Delay(250).ConfigureAwait(false);
            Console.WriteLine($"{label}: retry after forced cleanup...");
            return await Galaxy.Connect(LocalSwitchGateUri, auth, null).ConfigureAwait(false);
        }
    }

    private static async Task ExpectPlayerLoginSuccess(string auth, string label)
    {
        Galaxy galaxy = await ConnectLocalPlayer(auth, TeamName, label).ConfigureAwait(false);
        galaxy.Dispose();
        Console.WriteLine($"{label}: success");
    }

    private static async Task ExpectAdminLoginSuccess(string auth, string label)
    {
        Galaxy galaxy = await ConnectLocalAdmin(auth, label).ConfigureAwait(false);
        galaxy.Dispose();
        Console.WriteLine($"{label}: success");
    }

    private static async Task ExpectPlayerAccessRestricted(string auth, string label)
    {
        try
        {
            Galaxy galaxy = await Galaxy.Connect(LocalSwitchGateUri, auth, TeamName).ConfigureAwait(false);
            galaxy.Dispose();
            throw new InvalidOperationException($"{label}: expected PlayerAccessRestrictedGameException.");
        }
        catch (PlayerAccessRestrictedGameException exception)
        {
            Console.WriteLine($"{label}: OK ({exception.Message})");
        }
    }

    private static async Task ExpectAdminAccessRestricted(string auth, string label)
    {
        try
        {
            Galaxy galaxy = await Galaxy.Connect(LocalSwitchGateUri, auth, null).ConfigureAwait(false);
            galaxy.Dispose();
            throw new InvalidOperationException($"{label}: expected AdminAccessRestrictedGameException.");
        }
        catch (AdminAccessRestrictedGameException exception)
        {
            Console.WriteLine($"{label}: OK ({exception.Message})");
        }
    }

    private static async Task VerifyAclList(Galaxy adminGalaxy, GalaxyAclKind kind, int[] expectedAccountIds, string label)
    {
        AccountSummary[] accounts = await adminGalaxy.QueryAclAccounts(kind).ConfigureAwait(false);
        HashSet<int> expected = new HashSet<int>(expectedAccountIds);
        HashSet<int> actual = new HashSet<int>(accounts.Select(delegate (AccountSummary account) { return account.Id; }));

        if (!expected.SetEquals(actual))
            throw new InvalidOperationException(
                $"{label}: expected [{string.Join(", ", expected.OrderBy(delegate (int accountId) { return accountId; }))}] but received [{string.Join(", ", actual.OrderBy(delegate (int accountId) { return accountId; }))}].");

        Console.WriteLine($"{label}: OK ({actual.Count} entries)");
    }

    private static string QueryAlternativeAdminAuth(int excludedAccountId)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(DatabaseConnectionString);
        connection.Open();

        using NpgsqlCommand command = new NpgsqlCommand(
            """
            SELECT encode("apiAdmin", 'hex')
            FROM public."accounts"
            WHERE "id" <> @excludedAccountId
              AND "status" = 'user'
              AND "apiAdmin" IS NOT NULL
            ORDER BY "id" ASC
            LIMIT 1
            """,
            connection);

        command.Parameters.AddWithValue("@excludedAccountId", excludedAccountId);

        object? scalar = command.ExecuteScalar();

        if (scalar is not string auth || auth.Length == 0)
            throw new InvalidOperationException("ACL-LOCAL: could not find a second admin API key in the database.");

        return auth;
    }

    private static string QueryAlternativePlayerAuth(int excludedAccountId0, int excludedAccountId1)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(DatabaseConnectionString);
        connection.Open();

        using NpgsqlCommand command = new NpgsqlCommand(
            """
            SELECT encode("apiPlayer", 'hex')
            FROM public."accounts"
            WHERE "id" <> @excludedAccountId0
              AND "id" <> @excludedAccountId1
              AND "status" = 'user'
              AND "apiPlayer" IS NOT NULL
            ORDER BY "id" ASC
            LIMIT 1
            """,
            connection);

        command.Parameters.AddWithValue("@excludedAccountId0", excludedAccountId0);
        command.Parameters.AddWithValue("@excludedAccountId1", excludedAccountId1);

        object? scalar = command.ExecuteScalar();

        if (scalar is not string auth || auth.Length == 0)
            throw new InvalidOperationException("ACL-LOCAL: could not find a second player API key in the database.");

        return auth;
    }

    private static DatabaseGalaxyAclRow[] QueryGalaxyAclRows(ushort galaxyId)
    {
        List<DatabaseGalaxyAclRow> rows = new List<DatabaseGalaxyAclRow>();

        using NpgsqlConnection connection = new NpgsqlConnection(DatabaseConnectionString);
        connection.Open();

        using NpgsqlCommand command = new NpgsqlCommand(
            """
            SELECT "kind", "account"
            FROM public."galaxyAclEntries"
            WHERE "galaxy" = @galaxy
            ORDER BY "kind" ASC, "account" ASC
            """,
            connection);

        command.Parameters.AddWithValue("@galaxy", (short)galaxyId);

        using NpgsqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
            rows.Add(new DatabaseGalaxyAclRow((GalaxyAclKind)(short)reader["kind"], (int)reader["account"]));

        return rows.ToArray();
    }

    private static void ClearGalaxyAclEntries(ushort galaxyId)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(DatabaseConnectionString);
        connection.Open();

        using NpgsqlCommand command = new NpgsqlCommand(
            """
            DELETE FROM public."galaxyAclEntries"
            WHERE "galaxy" = @galaxy
            """,
            connection);

        command.Parameters.AddWithValue("@galaxy", (short)galaxyId);
        command.ExecuteNonQuery();
    }
}
