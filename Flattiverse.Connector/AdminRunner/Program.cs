using System;
using System.Threading.Tasks;
using Flattiverse.Connector.GalaxyHierarchy;

namespace AdminRunner;

class Program
{
    private const string Uri = "ws://127.0.0.1:5666";
    private const string TeamName = "Pink";
    private const string AdminAuth = "replace-with-admin-auth";
    private const byte ClusterId = 0;
    private const float DefaultBaseX = -900f;
    private const float DefaultBaseY = -487f;

    private static readonly string[] FixtureNames = new string[]
    {
        "ProbeSun",
        "ProbeHole",
        "ProbePlanet",
        "ProbeMoon",
        "ProbeMeteoroid",
        "ProbeBuoy",
        "ProbeMissionTarget",
        "ProbeFlag",
        "ProbeDominationPoint",
        "ProbeEditBuoy",
        "ProbeEverSeenBuoy",
        "ProbeShotTarget"
    };

    private static async Task<int> Main(string[] args)
    {
        if (args.Length != 1 && args.Length != 3 && args.Length != 4)
        {
            Console.Error.WriteLine("Expected command plus optional <baseX> <baseY> or <kind> <x> <y>: prepare-fixture, cleanup-fixture, prepare-shot-target, edit-shot-target, remove-shot-target, edit-probe, remove-probe, edit-everseen, remove-everseen, set-single, remove-single.");
            return 1;
        }

        float baseX = DefaultBaseX;
        float baseY = DefaultBaseY;
        string? singleKind = null;

        if (args.Length == 3)
        {
            baseX = float.Parse(args[1], System.Globalization.CultureInfo.InvariantCulture);
            baseY = float.Parse(args[2], System.Globalization.CultureInfo.InvariantCulture);
        }
        else if (args.Length == 4)
        {
            singleKind = args[1];
            baseX = float.Parse(args[2], System.Globalization.CultureInfo.InvariantCulture);
            baseY = float.Parse(args[3], System.Globalization.CultureInfo.InvariantCulture);
        }

        Galaxy galaxy = await Galaxy.Connect(Uri, AdminAuth, TeamName).ConfigureAwait(false);

        try
        {
            if (!galaxy.Clusters.TryGet(ClusterId, out Cluster? cluster))
            {
                Console.Error.WriteLine($"Cluster {ClusterId} not found.");
                return 1;
            }

            Cluster clusterValue = cluster;
            string command = args[0];

            if (string.Equals(command, "prepare-fixture", StringComparison.Ordinal))
            {
                await CleanupFixture(clusterValue).ConfigureAwait(false);

                foreach (string xml in BuildFixtureXml(baseX, baseY))
                {
                    await clusterValue.SetUnit(xml).ConfigureAwait(false);
                    Console.WriteLine($"SET {ReadName(xml)}");
                }

                return 0;
            }

            if (string.Equals(command, "cleanup-fixture", StringComparison.Ordinal))
            {
                await CleanupFixture(clusterValue).ConfigureAwait(false);
                return 0;
            }

            if (string.Equals(command, "prepare-shot-target", StringComparison.Ordinal))
            {
                string xml = BuildShotTargetXml(baseX, baseY);
                await clusterValue.SetUnit(xml).ConfigureAwait(false);
                Console.WriteLine("SET ProbeShotTarget");
                return 0;
            }

            if (string.Equals(command, "edit-shot-target", StringComparison.Ordinal))
            {
                string xml = BuildEditShotTargetXml(baseX, baseY);
                await clusterValue.SetUnit(xml).ConfigureAwait(false);
                Console.WriteLine("SET ProbeShotTarget");
                return 0;
            }

            if (string.Equals(command, "remove-shot-target", StringComparison.Ordinal))
            {
                await clusterValue.RemoveUnit("ProbeShotTarget").ConfigureAwait(false);
                Console.WriteLine("REMOVE ProbeShotTarget");
                return 0;
            }

            if (string.Equals(command, "edit-probe", StringComparison.Ordinal))
            {
                string xml = BuildEditProbeXml(baseX, baseY);
                await clusterValue.SetUnit(xml).ConfigureAwait(false);
                Console.WriteLine("SET ProbeEditBuoy");
                return 0;
            }

            if (string.Equals(command, "remove-probe", StringComparison.Ordinal))
            {
                await clusterValue.RemoveUnit("ProbeEditBuoy").ConfigureAwait(false);
                Console.WriteLine("REMOVE ProbeEditBuoy");
                return 0;
            }

            if (string.Equals(command, "edit-everseen", StringComparison.Ordinal))
            {
                string xml = BuildEditEverSeenXml(baseX, baseY);
                await clusterValue.SetUnit(xml).ConfigureAwait(false);
                Console.WriteLine("SET ProbeEverSeenBuoy");
                return 0;
            }

            if (string.Equals(command, "remove-everseen", StringComparison.Ordinal))
            {
                await clusterValue.RemoveUnit("ProbeEverSeenBuoy").ConfigureAwait(false);
                Console.WriteLine("REMOVE ProbeEverSeenBuoy");
                return 0;
            }

            if (string.Equals(command, "set-single", StringComparison.Ordinal))
            {
                if (singleKind is null)
                {
                    Console.Error.WriteLine("set-single expects <kind> <x> <y>.");
                    return 1;
                }

                if (!TryBuildSingleUnitXml(singleKind, baseX, baseY, out string xml, out string unitName))
                {
                    Console.Error.WriteLine($"Unknown single-unit kind {singleKind}.");
                    return 1;
                }

                await clusterValue.SetUnit(xml).ConfigureAwait(false);
                Console.WriteLine($"SET {unitName}");
                return 0;
            }

            if (string.Equals(command, "remove-single", StringComparison.Ordinal))
            {
                if (singleKind is null)
                {
                    Console.Error.WriteLine("remove-single expects <kind> <x> <y>.");
                    return 1;
                }

                if (!TryGetSingleUnitName(singleKind, out string unitName))
                {
                    Console.Error.WriteLine($"Unknown single-unit kind {singleKind}.");
                    return 1;
                }

                await clusterValue.RemoveUnit(unitName).ConfigureAwait(false);
                Console.WriteLine($"REMOVE {unitName}");
                return 0;
            }

            Console.Error.WriteLine($"Unknown command {command}.");
            return 1;
        }
        finally
        {
            galaxy.Dispose();
        }
    }

    private static async Task CleanupFixture(Cluster cluster)
    {
        foreach (string name in FixtureNames)
            try
            {
                await cluster.RemoveUnit(name).ConfigureAwait(false);
                Console.WriteLine($"REMOVE {name}");
            }
            catch
            {
            }
    }

    private static string[] BuildFixtureXml(float baseX, float baseY)
    {
        string missionTarget = $"<MissionTarget Name=\"ProbeMissionTarget\" X=\"{Format(baseX + 140f)}\" Y=\"{Format(baseY + 24f)}\" Radius=\"7\" Gravity=\"0\" Team=\"0\" SequenceNumber=\"9\"><Vector X=\"{Format(baseX + 190f)}\" Y=\"{Format(baseY + 24f)}\" /><Vector X=\"{Format(baseX + 230f)}\" Y=\"{Format(baseY + 64f)}\" /></MissionTarget>";

        return new string[]
        {
            $"<Sun Name=\"ProbeSun\" X=\"{Format(baseX + 140f)}\" Y=\"{Format(baseY - 138f)}\" Radius=\"16\" Gravity=\"0.005\" Energy=\"1.80\" Ions=\"0.40\" Neutrinos=\"0.20\" Heat=\"0.70\" Drain=\"0.10\" />",
            $"<BlackHole Name=\"ProbeHole\" X=\"{Format(baseX + 140f)}\" Y=\"{Format(baseY - 111f)}\" Radius=\"12\" Gravity=\"0.010\" GravityWellRadius=\"70\" GravityWellForce=\"0.20\" />",
            $"<Planet Name=\"ProbePlanet\" X=\"{Format(baseX + 140f)}\" Y=\"{Format(baseY - 84f)}\" Radius=\"13\" Gravity=\"0.006\" Type=\"OceanWorld\" Metal=\"0.12\" Carbon=\"0.34\" Hydrogen=\"0.56\" Silicon=\"0.78\" />",
            $"<Moon Name=\"ProbeMoon\" X=\"{Format(baseX + 140f)}\" Y=\"{Format(baseY - 57f)}\" Radius=\"10\" Gravity=\"0.004\" Type=\"IceMoon\" Metal=\"0.22\" Carbon=\"0.18\" Hydrogen=\"0.44\" Silicon=\"0.30\" />",
            $"<Meteoroid Name=\"ProbeMeteoroid\" X=\"{Format(baseX + 140f)}\" Y=\"{Format(baseY - 30f)}\" Radius=\"8\" Gravity=\"0.002\" Type=\"MetallicSlug\" Metal=\"0.80\" Carbon=\"0.04\" Hydrogen=\"0.01\" Silicon=\"0.25\" />",
            $"<Buoy Name=\"ProbeBuoy\" X=\"{Format(baseX + 140f)}\" Y=\"{Format(baseY - 3f)}\" Radius=\"6\" Gravity=\"0\" Message=\"Probe buoy message A\" />",
            missionTarget,
            $"<Flag Name=\"ProbeFlag\" X=\"{Format(baseX + 140f)}\" Y=\"{Format(baseY + 51f)}\" Radius=\"7\" Gravity=\"0\" Team=\"0\" />",
            $"<DominationPoint Name=\"ProbeDominationPoint\" X=\"{Format(baseX + 140f)}\" Y=\"{Format(baseY + 78f)}\" Radius=\"8\" Gravity=\"0\" Team=\"0\" DominationRadius=\"55\" />",
            $"<Buoy Name=\"ProbeEditBuoy\" X=\"{Format(baseX + 140f)}\" Y=\"{Format(baseY + 105f)}\" Radius=\"6\" Gravity=\"0\" Message=\"Edit probe initial\" />",
            $"<Buoy Name=\"ProbeEverSeenBuoy\" X=\"{Format(baseX + 140f)}\" Y=\"{Format(baseY + 132f)}\" Radius=\"6\" Gravity=\"0\" Message=\"EverSeen initial\" />",
            BuildShotTargetXml(baseX, baseY)
        };
    }

    private static string BuildEditProbeXml(float baseX, float baseY)
    {
        return $"<Buoy Name=\"ProbeEditBuoy\" X=\"{Format(baseX + 142f)}\" Y=\"{Format(baseY + 105f)}\" Radius=\"6\" Gravity=\"0\" Message=\"Edit probe changed\" />";
    }

    private static string BuildEditEverSeenXml(float baseX, float baseY)
    {
        return $"<Buoy Name=\"ProbeEverSeenBuoy\" X=\"{Format(baseX + 142f)}\" Y=\"{Format(baseY + 132f)}\" Radius=\"6\" Gravity=\"0\" Message=\"EverSeen changed\" />";
    }

    private static string BuildShotTargetXml(float baseX, float baseY)
    {
        return $"<Planet Name=\"ProbeShotTarget\" X=\"{Format(baseX + 60f)}\" Y=\"{Format(baseY)}\" Radius=\"12\" Gravity=\"0.002\" Type=\"RockyFrontier\" Metal=\"0.40\" Carbon=\"0.20\" Hydrogen=\"0.10\" Silicon=\"0.50\" />";
    }

    private static string BuildEditShotTargetXml(float baseX, float baseY)
    {
        return $"<Planet Name=\"ProbeShotTarget\" X=\"{Format(baseX + 62f)}\" Y=\"{Format(baseY)}\" Radius=\"12\" Gravity=\"0.002\" Type=\"RockyFrontier\" Metal=\"0.45\" Carbon=\"0.25\" Hydrogen=\"0.15\" Silicon=\"0.55\" />";
    }

    private static bool TryBuildSingleUnitXml(string kind, float x, float y, out string xml, out string name)
    {
        switch (kind.ToLowerInvariant())
        {
            case "sun":
                name = "ProbeSun";
                xml = $"<Sun Name=\"{name}\" X=\"{Format(x)}\" Y=\"{Format(y)}\" Radius=\"16\" Gravity=\"0\" Energy=\"1.80\" Ions=\"0.40\" Neutrinos=\"0.20\" Heat=\"0.70\" Drain=\"0.10\" />";
                return true;
            case "blackhole":
            case "hole":
                name = "ProbeHole";
                xml = $"<BlackHole Name=\"{name}\" X=\"{Format(x)}\" Y=\"{Format(y)}\" Radius=\"12\" Gravity=\"0\" GravityWellRadius=\"70\" GravityWellForce=\"0.20\" />";
                return true;
            case "planet":
                name = "ProbePlanet";
                xml = $"<Planet Name=\"{name}\" X=\"{Format(x)}\" Y=\"{Format(y)}\" Radius=\"13\" Gravity=\"0\" Type=\"OceanWorld\" Metal=\"0.12\" Carbon=\"0.34\" Hydrogen=\"0.56\" Silicon=\"0.78\" />";
                return true;
            case "moon":
                name = "ProbeMoon";
                xml = $"<Moon Name=\"{name}\" X=\"{Format(x)}\" Y=\"{Format(y)}\" Radius=\"10\" Gravity=\"0\" Type=\"IceMoon\" Metal=\"0.22\" Carbon=\"0.18\" Hydrogen=\"0.44\" Silicon=\"0.30\" />";
                return true;
            case "meteoroid":
                name = "ProbeMeteoroid";
                xml = $"<Meteoroid Name=\"{name}\" X=\"{Format(x)}\" Y=\"{Format(y)}\" Radius=\"8\" Gravity=\"0\" Type=\"MetallicSlug\" Metal=\"0.80\" Carbon=\"0.04\" Hydrogen=\"0.01\" Silicon=\"0.25\" />";
                return true;
            case "buoy":
                name = "ProbeBuoy";
                xml = $"<Buoy Name=\"{name}\" X=\"{Format(x)}\" Y=\"{Format(y)}\" Radius=\"6\" Gravity=\"0\" Message=\"Probe buoy message A\" />";
                return true;
            case "missiontarget":
                name = "ProbeMissionTarget";
                xml = $"<MissionTarget Name=\"{name}\" X=\"{Format(x)}\" Y=\"{Format(y)}\" Radius=\"7\" Gravity=\"0\" Team=\"0\" SequenceNumber=\"9\"><Vector X=\"{Format(x + 50f)}\" Y=\"{Format(y)}\" /><Vector X=\"{Format(x + 90f)}\" Y=\"{Format(y + 40f)}\" /></MissionTarget>";
                return true;
            case "flag":
                name = "ProbeFlag";
                xml = $"<Flag Name=\"{name}\" X=\"{Format(x)}\" Y=\"{Format(y)}\" Radius=\"7\" Gravity=\"0\" Team=\"0\" />";
                return true;
            case "dominationpoint":
                name = "ProbeDominationPoint";
                xml = $"<DominationPoint Name=\"{name}\" X=\"{Format(x)}\" Y=\"{Format(y)}\" Radius=\"8\" Gravity=\"0\" Team=\"0\" DominationRadius=\"55\" />";
                return true;
            default:
                xml = string.Empty;
                name = string.Empty;
                return false;
        }
    }

    private static bool TryGetSingleUnitName(string kind, out string name)
    {
        switch (kind.ToLowerInvariant())
        {
            case "sun":
                name = "ProbeSun";
                return true;
            case "blackhole":
            case "hole":
                name = "ProbeHole";
                return true;
            case "planet":
                name = "ProbePlanet";
                return true;
            case "moon":
                name = "ProbeMoon";
                return true;
            case "meteoroid":
                name = "ProbeMeteoroid";
                return true;
            case "buoy":
                name = "ProbeBuoy";
                return true;
            case "missiontarget":
                name = "ProbeMissionTarget";
                return true;
            case "flag":
                name = "ProbeFlag";
                return true;
            case "dominationpoint":
                name = "ProbeDominationPoint";
                return true;
            default:
                name = string.Empty;
                return false;
        }
    }

    private static string ReadName(string xml)
    {
        int nameStart = xml.IndexOf("Name=\"", StringComparison.Ordinal);
        int valueStart = nameStart + 6;
        int valueEnd = xml.IndexOf('"', valueStart);
        return xml.Substring(valueStart, valueEnd - valueStart);
    }

    private static string Format(float value)
    {
        return value.ToString("R", System.Globalization.CultureInfo.InvariantCulture);
    }
}
