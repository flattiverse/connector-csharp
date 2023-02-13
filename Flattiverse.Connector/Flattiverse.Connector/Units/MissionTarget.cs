using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    [UnitIdentifier("missionTarget")]
    public class MissionTarget : SteadyUnit
    {
        public int Sequence;

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
            Utils.Traverse(element, out Sequence, "sequence");
        }

        public override UnitKind Kind => UnitKind.MissionTarget;
    }
}