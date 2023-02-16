using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    [UnitIdentifier("moon")]
    public class Moon : SteadyUnit
    {
        public Moon(string name, Vector position, Vector movement) : base(name, position, movement)
        {
        }

        public Moon(string name, Vector position) : base(name, position)
        {
        }

        public Moon(string name) : base(name)
        {
        }

        public Moon() : base()
        {
        }

        internal Moon(UniverseGroup group, JsonElement element) : base(group, element)
        {
        }

        public override UnitKind Kind => UnitKind.Moon;

        public override string ToString()
        {
            return $"[{Kind}] {Name} at {Position}.";
        }
    }
}