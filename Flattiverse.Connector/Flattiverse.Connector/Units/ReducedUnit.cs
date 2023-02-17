using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    [UnitIdentifier("reduced")]
    public class ReducedUnit : Unit
    {
        public UnitKind ProbableKind;

        public ReducedUnit(string name, Vector position, Vector movement) : base(name, position, movement)
        {
        }

        public ReducedUnit(string name, Vector position) : base(name, position)
        {
        }

        public ReducedUnit(string name) : base(name)
        {
        }

        public ReducedUnit() : base()
        {
        }

        internal ReducedUnit(UniverseGroup group, JsonElement element) : base(group, element)
        {
            Utils.Traverse(element, out string probableKind, "probableKind");
            Enum.TryParse(probableKind, true, out ProbableKind);
            IsSolid = true;
            switch (ProbableKind)
            {
                case UnitKind.Buoy:
                case UnitKind.MissionTarget:
                    Mobility = Mobility.Still;
                    IsSolid = false;
                    break;
                case UnitKind.PlayerUnit:
                    Mobility = Mobility.Mobile;
                    IsSolid = true;
                    break;
                case UnitKind.Shot:
                    Mobility = Mobility.Mobile;
                    IsSolid = false;
                    break;
                default:
                    Mobility = Mobility.Still;
                    IsSolid = true;
                    break;
            }
        }

        public override UnitKind Kind => UnitKind.Reduced;

        public override string ToString()
        {
            return $"[{Kind}] {Name} at {Position} is probably a {ProbableKind}.";
        }
    }
}
