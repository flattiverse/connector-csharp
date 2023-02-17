using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    [UnitIdentifier("meteoroid")]
    public class Meteoroid : SteadyUnit
    {
        public Meteoroid(string name, Vector position, Vector movement) : base(name, position, movement)
        {
        }

        public Meteoroid(string name, Vector position) : base(name, position)
        {
        }

        public Meteoroid(string name) : base(name)
        {
        }

        public Meteoroid() : base()
        {
        }

        internal Meteoroid(UniverseGroup group, JsonElement element) : base(group, element)
        {
            IsSolid = true;
        }

        public override UnitKind Kind => UnitKind.Meteoroid;

        public override string ToString()
        {
            return $"[{Kind}] {Name} at {Position}.";
        }
    }
}