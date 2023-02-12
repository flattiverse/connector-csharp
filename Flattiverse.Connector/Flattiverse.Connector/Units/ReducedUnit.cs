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
        }

        public override UnitKind Kind => UnitKind.Reduced;
    }
}
