using System.Collections.Concurrent;
using Flattiverse.Connector;
using Flattiverse.Connector.Events;
using Flattiverse.Connector.GalaxyHierarchy;
using Flattiverse.Connector.Units;

namespace Development;

partial class Program
{
    private static async Task RunDominationPointCheckLocal()
    {
        const int DominationPointStateTimeoutMs = 5000;
        const int IdleObservationMs = 750;

        Galaxy? adminGalaxy = null;
        Task? adminEventPump = null;
        ConcurrentQueue<FlattiverseEvent> adminEvents = new ConcurrentQueue<FlattiverseEvent>();
        string dominationPointName = $"DominationStartLocal{DateTime.UtcNow:yyyyMMddHHmmss}";
        bool dominationPointCreated = false;

        await WaitForSessionGalaxy(LocalSwitchGateAdminAuth, null, 7000).ConfigureAwait(false);

        try
        {
            Console.WriteLine("DOMINATION-POINT-LOCAL: connecting admin...");
            adminGalaxy = await Galaxy.Connect(LocalSwitchGateUri, LocalSwitchGateAdminAuth, null).ConfigureAwait(false);
            adminEventPump = StartEventPump("DOMINATION-POINT-LOCAL:ADMIN", adminGalaxy, adminEvents);
            DrainEvents(adminEvents);

            if (!TryGetFirstNonSpectatorTeam(adminGalaxy, out Team? team) || team is null)
                throw new InvalidOperationException("DOMINATION-POINT-LOCAL: galaxy 666 does not expose a non-spectator team.");

            if (!TryGetActiveStartCluster(adminGalaxy, out Cluster? cluster) || cluster is null)
                throw new InvalidOperationException("DOMINATION-POINT-LOCAL: no active start cluster available.");

            string dominationPointXml =
                $"<DominationPoint Name=\"{dominationPointName}\" X=\"150000\" Y=\"-150000\" Radius=\"12\" Gravity=\"0\" Team=\"{team.Id}\" DominationRadius=\"90\" />";

            Console.WriteLine($"DOMINATION-POINT-LOCAL: creating helper domination point {dominationPointName}...");
            await cluster.SetUnit(dominationPointXml).ConfigureAwait(false);
            dominationPointCreated = true;

            if (!await WaitForCondition(delegate
                {
                    return TryFindUnit<DominationPoint>(adminGalaxy, cluster.Id, dominationPointName, out DominationPoint? dominationPoint) &&
                           dominationPoint is not null &&
                           dominationPoint.Team is Team dominationTeam &&
                           dominationTeam.Id == team.Id &&
                           dominationPoint.Domination == 0 &&
                           dominationPoint.ScoreCountdown == 600;
                }, DominationPointStateTimeoutMs).ConfigureAwait(false))
            {
                bool pointFound = TryFindUnit<DominationPoint>(adminGalaxy, cluster.Id, dominationPointName, out DominationPoint? dominationPoint);
                string pointState = pointFound && dominationPoint is not null
                    ? $"team={(dominationPoint.Team is null ? "<null>" : dominationPoint.Team.Id.ToString())}, domination={dominationPoint.Domination}, countdown={dominationPoint.ScoreCountdown}"
                    : "<not found>";
                throw new InvalidOperationException($"DOMINATION-POINT-LOCAL: initial domination-point state was wrong: {pointState}.");
            }

            await Task.Delay(IdleObservationMs).ConfigureAwait(false);

            if (!TryFindUnit<DominationPoint>(adminGalaxy, cluster.Id, dominationPointName, out DominationPoint? idleDominationPoint) ||
                idleDominationPoint is null)
                throw new InvalidOperationException("DOMINATION-POINT-LOCAL: domination point disappeared during idle observation.");

            if (idleDominationPoint.Domination != 0 || idleDominationPoint.ScoreCountdown != 600)
                throw new InvalidOperationException(
                    $"DOMINATION-POINT-LOCAL: idle domination point changed unexpectedly. domination={idleDominationPoint.Domination}, countdown={idleDominationPoint.ScoreCountdown}.");

            List<FlattiverseEvent> idleEvents = DrainEvents(adminEvents);

            foreach (FlattiverseEvent @event in idleEvents)
                if (@event is DominationPointScoredChatEvent)
                    throw new InvalidOperationException("DOMINATION-POINT-LOCAL: domination point scored before it was captured.");

            Console.WriteLine("DOMINATION-POINT-LOCAL: fresh domination point stays uncaptured and does not start scoring.");
        }
        finally
        {
            if (dominationPointCreated && adminGalaxy is not null && TryGetActiveStartCluster(adminGalaxy, out Cluster? cluster) && cluster is not null)
            {
                try
                {
                    await cluster.RemoveUnit(dominationPointName).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"DOMINATION-POINT-LOCAL: cleanup failed for {dominationPointName}: {exception.Message}");
                }
            }

            if (adminGalaxy is not null)
                adminGalaxy.Dispose();

            if (adminEventPump is not null)
                await adminEventPump.ConfigureAwait(false);

            await WaitForSessionGalaxy(LocalSwitchGateAdminAuth, null, 7000).ConfigureAwait(false);
        }
    }
}
