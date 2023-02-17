using System.Text;
using System.Text.Json;
using static System.Collections.Specialized.BitVector32;

namespace Flattiverse.Connector.Units
{
    [UnitIdentifier("missionTarget")]
    public class MissionTarget : SteadyUnit
    {
        public int? Sequence;
        public double? DominationRadius;
        public double? DominationProgress;
        public Team? DominationTeam;
        public List<Vector>? Hints;

        public MissionTarget(string name, Vector position, Vector movement) : base(name, position, movement)
        {
        }

        public MissionTarget(string name, Vector position) : base(name, position)
        {
        }

        public MissionTarget(string name) : base(name)
        {
        }

        public MissionTarget() : base()
        {
        }

        internal MissionTarget(UniverseGroup group, JsonElement element) : base(group, element)
        {
            IsSolid = false;

            if (Utils.Traverse(element, out int sequence, "sequence"))
                Sequence = sequence;

            if (Utils.Traverse(element, out JsonElement hints, "hints"))
            {
                Hints = new List<Vector>();
                foreach (JsonElement hint in hints.EnumerateArray())
                    Hints.Add(new Vector(hint));
            }

            if (Utils.Traverse(element, out double dominationRadius, "dominationRadius"))
                DominationRadius = dominationRadius;

            if (Utils.Traverse(element, out double dominationProgress, "dominationProgress"))
                DominationProgress = dominationProgress;

            if (Utils.Traverse(element, out int team, "dominationTeam"))
                DominationTeam = group.teamsId[team];
        }

        public override UnitKind Kind => UnitKind.MissionTarget;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder($"[{Kind}] {Name} at {Position}");            

            if (Sequence is not null)
                sb.Append($" with sequence #{Sequence}");

            if (DominationRadius is not null)
                sb.Append($" with dominationradius #{DominationRadius}");

            if (Hints is not null)
                sb.Append($" with {Hints.Count} hint vectors");

            sb.Append(".");

            return sb.ToString();            
        }
    }
}