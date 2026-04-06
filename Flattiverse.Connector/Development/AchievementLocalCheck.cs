using System.Diagnostics;
using Flattiverse.Connector;
using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Units;
using Npgsql;

namespace Development;

partial class Program
{
    private readonly struct DatabaseAchievementRow
    {
        public readonly long Counter;
        public readonly long FirstOccurence;
        public readonly long LastOccurence;

        public DatabaseAchievementRow(long counter, long firstOccurence, long lastOccurence)
        {
            Counter = counter;
            FirstOccurence = firstOccurence;
            LastOccurence = lastOccurence;
        }
    }

    private static async Task RunAchievementCheckLocal()
    {
        Process? galaxyProcess = null;
        Galaxy? adminGalaxy = null;
        Galaxy? playerGalaxy = null;
        Galaxy? allowedGalaxy = null;
        string? restoreConfigurationXml = null;
        Dictionary<byte, string>? restoreRegionsByCluster = null;

        DatabaseAccountRow adminAccount = QueryAccountRow(LocalSwitchGateAdminAuth);
        DatabaseAccountRow awardedAccount = QueryAccountRow(LocalSwitchGatePlayerAuth);
        string deniedPlayerAuth = QueryAlternativePlayerAuth(adminAccount.AccountId, awardedAccount.AccountId);
        DatabaseAccountRow deniedAccount = QueryAccountRow(deniedPlayerAuth);
        string achievementName = $"ACH{Environment.ProcessId:X8}";
        string testClusterName = $"Achievement{Environment.ProcessId}";
        string missionTargetName = $"AchievementTarget{Environment.ProcessId}";
        string shipName = $"AchievementShip{Environment.ProcessId}";

        await EnsureSessionCleared(LocalSwitchGateAdminAuth, "ACH-LOCAL:admin").ConfigureAwait(false);
        await EnsureSessionCleared(LocalSwitchGatePlayerAuth, "ACH-LOCAL:player").ConfigureAwait(false);
        await EnsureSessionCleared(deniedPlayerAuth, "ACH-LOCAL:denied-player").ConfigureAwait(false);

        DeleteAchievementRows(new int[] { awardedAccount.AccountId, deniedAccount.AccountId }, achievementName);

        try
        {
            Console.WriteLine("ACH-LOCAL: starting local galaxy 666...");
            galaxyProcess = StartLocalGalaxyProcess();
            (adminGalaxy, _) = await ConnectLocalAdminAfterInitialRebuild(galaxyProcess).ConfigureAwait(false);

            restoreConfigurationXml = BuildConfigurationXml(adminGalaxy, null, null, adminGalaxy.RequiresSelfDisclosure,
                adminGalaxy.RequiredAchievement, true);
            restoreRegionsByCluster = await CaptureRegionsByCluster(adminGalaxy).ConfigureAwait(false);

            List<ClusterSpec> clusterConfigurations = new List<ClusterSpec>(BuildClusterSpecs(adminGalaxy, true));

            if (!TryGetUnusedClusterId(adminGalaxy, 255, out byte testClusterId))
                throw new InvalidOperationException("ACH-LOCAL: no free cluster id available for the achievement test.");

            clusterConfigurations.Add(new ClusterSpec(testClusterId, testClusterName, true, true));

            Console.WriteLine("ACH-LOCAL: configuring dedicated test cluster...");
            await adminGalaxy.Configure(BuildConfigurationXml(adminGalaxy, null, clusterConfigurations.ToArray(), false, null, true))
                .ConfigureAwait(false);

            if (!await WaitForCondition(delegate
                {
                    return adminGalaxy.Clusters.TryGet(testClusterId, out Cluster? _);
                }, 5000).ConfigureAwait(false))
                throw new InvalidOperationException("ACH-LOCAL: dedicated cluster did not appear after configure.");

            if (adminGalaxy.RequiredAchievement is not null)
                throw new InvalidOperationException("ACH-LOCAL: test configuration did not clear RequiredAchievement.");

            if (adminGalaxy.RequiresSelfDisclosure)
                throw new InvalidOperationException("ACH-LOCAL: test configuration did not clear RequiresSelfDisclosure.");

            await RemoveAllRegions(adminGalaxy).ConfigureAwait(false);

            Cluster testCluster = adminGalaxy.Clusters[testClusterId];

            if (!TryGetTeamByName(adminGalaxy, TeamName, out Team? pinkTeam) || pinkTeam is null)
                throw new InvalidOperationException($"ACH-LOCAL: team {TeamName} not found.");

            byte testRegionId = await FindUnusedRegionId(testCluster).ConfigureAwait(false);
            Vector targetPosition = new Vector(160f, 0f);
            Vector shootingPosition = new Vector(80f, 0f);
            string regionXml =
                $"<Region Id=\"{testRegionId}\" Name=\"AchievementLocalRegion\" Left=\"-220\" Top=\"-20\" Right=\"-140\" Bottom=\"20\"><Team Id=\"{pinkTeam.Id}\" /></Region>";
            string missionTargetXml =
                $"<MissionTarget Name=\"{missionTargetName}\" X=\"{targetPosition.X:0.###}\" Y=\"{targetPosition.Y:0.###}\" Radius=\"40\" Gravity=\"0\" Team=\"{pinkTeam.Id}\" SequenceNumber=\"0\" Achievement=\"{achievementName}\" />";

            Console.WriteLine("ACH-LOCAL: creating start region and mission target...");
            await testCluster.SetRegion(regionXml).ConfigureAwait(false);
            await testCluster.SetUnit(missionTargetXml).ConfigureAwait(false);

            Console.WriteLine("ACH-LOCAL: connecting award player...");
            playerGalaxy = await ConnectLocalPlayer(LocalSwitchGatePlayerAuth, TeamName, "ACH-LOCAL:player").ConfigureAwait(false);

            ClassicShipControllable ship = await playerGalaxy.CreateClassicShip(shipName).ConfigureAwait(false);

            await ship.Continue().ConfigureAwait(false);

            if (!await WaitForAliveState(ship, true, 10000).ConfigureAwait(false))
                throw new InvalidOperationException("ACH-LOCAL: controllable did not become alive after the first Continue().");

            if (!await WaitForCondition(delegate { return ship.Cluster.Id == testClusterId; }, 10000).ConfigureAwait(false))
                throw new InvalidOperationException("ACH-LOCAL: controllable did not spawn in the dedicated cluster.");

            Vector shotMovement = new Vector(2f, 0f);
            ushort shotTicks = 40;
            float shotLoad = MathF.Min(ship.ShotLauncher.MaximumLoad, 10f);
            float shotDamage = ship.ShotLauncher.MinimumDamage;

            Console.WriteLine("ACH-LOCAL: teleporting controllable into firing position...");
            await testCluster.DebugSetPlayerUnitPosition(ship.Name, shootingPosition).ConfigureAwait(false);

            if (!await WaitForCondition(delegate { return Vector.Distance(ship.Position, shootingPosition) < 1f; }, 5000).ConfigureAwait(false))
                throw new InvalidOperationException("ACH-LOCAL: controllable did not reach the firing position before the first shot.");

            Console.WriteLine("ACH-LOCAL: firing first shot at the mission target...");
            await ship.ShotLauncher.Shoot(shotMovement, shotTicks, shotLoad, shotDamage).ConfigureAwait(false);

            if (!await WaitForCondition(delegate
                {
                    return TryQueryAchievementRow(awardedAccount.AccountId, achievementName, out DatabaseAchievementRow currentRow) &&
                           currentRow.Counter == 1;
                }, 7000).ConfigureAwait(false))
                throw new InvalidOperationException("ACH-LOCAL: first achievement award was not persisted.");

            Console.WriteLine("ACH-LOCAL: firing second shot at the mission target...");
            await testCluster.DebugSetPlayerUnitPosition(ship.Name, shootingPosition).ConfigureAwait(false);

            if (!await WaitForCondition(delegate { return Vector.Distance(ship.Position, shootingPosition) < 1f; }, 5000).ConfigureAwait(false))
                throw new InvalidOperationException("ACH-LOCAL: controllable did not reach the firing position before the second shot.");

            await ship.ShotLauncher.Shoot(shotMovement, shotTicks, shotLoad, shotDamage).ConfigureAwait(false);

            if (!await WaitForCondition(delegate
                {
                    return TryQueryAchievementRow(awardedAccount.AccountId, achievementName, out DatabaseAchievementRow currentRow) &&
                           currentRow.Counter == 2;
                }, 7000).ConfigureAwait(false))
                throw new InvalidOperationException("ACH-LOCAL: second achievement award was not persisted.");

            playerGalaxy.Dispose();
            playerGalaxy = null;

            if (!await WaitForSessionGalaxy(LocalSwitchGatePlayerAuth, null, 7000).ConfigureAwait(false))
                throw new InvalidOperationException("ACH-LOCAL: award player session did not close after disconnect.");

            if (!TryQueryAchievementRow(awardedAccount.AccountId, achievementName, out DatabaseAchievementRow achievementRow))
                throw new InvalidOperationException("ACH-LOCAL: achievement row disappeared before verification.");

            if (achievementRow.Counter != 2)
                throw new InvalidOperationException($"ACH-LOCAL: expected achievement counter 2, received {achievementRow.Counter}.");

            if (achievementRow.FirstOccurence <= 0 || achievementRow.LastOccurence < achievementRow.FirstOccurence)
                throw new InvalidOperationException("ACH-LOCAL: persisted achievement timestamps are invalid.");

            Console.WriteLine(
                $"ACH-LOCAL: persisted achievement row OK (counter={achievementRow.Counter}, first={achievementRow.FirstOccurence}, last={achievementRow.LastOccurence}).");

            Console.WriteLine("ACH-LOCAL: enabling RequiredAchievement...");
            await adminGalaxy.Configure(BuildConfigurationXml(adminGalaxy, null, null, false, achievementName, true))
                .ConfigureAwait(false);

            if (adminGalaxy.RequiredAchievement != achievementName)
                throw new InvalidOperationException("ACH-LOCAL: admin connector did not observe the configured RequiredAchievement.");

            Console.WriteLine("ACH-LOCAL: verifying login for awarded player...");
            allowedGalaxy = await ConnectLocalPlayer(LocalSwitchGatePlayerAuth, TeamName, "ACH-LOCAL:allowed-player").ConfigureAwait(false);

            if (allowedGalaxy.RequiredAchievement != achievementName)
                throw new InvalidOperationException("ACH-LOCAL: awarded player snapshot did not contain the required achievement key.");

            allowedGalaxy.Dispose();
            allowedGalaxy = null;

            if (!await WaitForSessionGalaxy(LocalSwitchGatePlayerAuth, null, 7000).ConfigureAwait(false))
                throw new InvalidOperationException("ACH-LOCAL: allowed player session did not close after disconnect.");

            Console.WriteLine("ACH-LOCAL: verifying login denial for player without the achievement...");

            try
            {
                Galaxy deniedGalaxy = await Galaxy.Connect(LocalSwitchGateUri, deniedPlayerAuth, TeamName).ConfigureAwait(false);
                deniedGalaxy.Dispose();
                throw new InvalidOperationException("ACH-LOCAL: expected MissingAchievementGameException for the denied player.");
            }
            catch (MissingAchievementGameException exception)
            {
                if (exception.AchievementName != achievementName)
                    throw new InvalidOperationException(
                        $"ACH-LOCAL: denied player received wrong achievement key \"{exception.AchievementName}\".");

                Console.WriteLine($"ACH-LOCAL: denial OK ({exception.Message})");
            }

            Console.WriteLine("ACH-LOCAL: success");
        }
        finally
        {
            if (allowedGalaxy is not null)
                allowedGalaxy.Dispose();

            if (playerGalaxy is not null)
                playerGalaxy.Dispose();

            if (adminGalaxy is not null && restoreConfigurationXml is not null)
                try
                {
                    await adminGalaxy.Configure(restoreConfigurationXml).ConfigureAwait(false);
                    Console.WriteLine("ACH-LOCAL: configuration restored.");
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"ACH-LOCAL: configuration restore failed ({exception.GetType().Name}: {exception.Message}).");
                }

            if (adminGalaxy is not null && restoreRegionsByCluster is not null)
                try
                {
                    await RestoreRegionsByCluster(adminGalaxy, restoreRegionsByCluster).ConfigureAwait(false);
                    Console.WriteLine("ACH-LOCAL: regions restored.");
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"ACH-LOCAL: region restore failed ({exception.GetType().Name}: {exception.Message}).");
                }

            if (adminGalaxy is not null)
                adminGalaxy.Dispose();

            await WaitForSessionGalaxy(LocalSwitchGatePlayerAuth, null, 7000).ConfigureAwait(false);
            await WaitForSessionGalaxy(deniedPlayerAuth, null, 2000).ConfigureAwait(false);
            await WaitForSessionGalaxy(LocalSwitchGateAdminAuth, null, 7000).ConfigureAwait(false);

            DeleteAchievementRows(new int[] { awardedAccount.AccountId, deniedAccount.AccountId }, achievementName);

            if (galaxyProcess is not null)
                StopProcess(galaxyProcess);
        }
    }

    private static void DeleteAchievementRows(int[] accountIds, string achievementName)
    {
        if (accountIds.Length == 0)
            return;

        using NpgsqlConnection connection = new NpgsqlConnection(DatabaseConnectionString);
        connection.Open();

        using NpgsqlCommand command = new NpgsqlCommand(
            """
            DELETE FROM public."accountAchievements"
            WHERE "accountId" = ANY(@accountIds)
              AND upper("name") = @achievementName
            """,
            connection);

        command.Parameters.AddWithValue("@accountIds", accountIds);
        command.Parameters.AddWithValue("@achievementName", achievementName);
        command.ExecuteNonQuery();
    }

    private static bool TryQueryAchievementRow(int accountId, string achievementName, out DatabaseAchievementRow row)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(DatabaseConnectionString);
        connection.Open();

        using NpgsqlCommand command = new NpgsqlCommand(
            """
            SELECT "counter", "firstOccurence", "lastOccurence"
            FROM public."accountAchievements"
            WHERE "accountId" = @accountId
              AND upper("name") = @achievementName
            LIMIT 1
            """,
            connection);

        command.Parameters.AddWithValue("@accountId", accountId);
        command.Parameters.AddWithValue("@achievementName", achievementName);

        using NpgsqlDataReader reader = command.ExecuteReader();

        if (!reader.Read())
        {
            row = default;
            return false;
        }

        row = new DatabaseAchievementRow(
            (long)reader["counter"],
            (long)reader["firstOccurence"],
            (long)reader["lastOccurence"]);
        return true;
    }
}
