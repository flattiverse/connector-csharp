using System.Text.Json;

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
    }
}