using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    [UnitIdentifier("planet")]
    public class Planet : SteadyUnit
    {
        public Planet(string name, Vector position, Vector movement) : base(name, position, movement)
        {
        }

        public Planet(string name, Vector position) : base(name, position)
        {
        }

        public Planet(string name) : base(name)
        {
        }

        public Planet() : base()
        {
        }

        internal Planet(UniverseGroup group, JsonElement element) : base(group, element)
        {
        }

        public override UnitKind Kind => UnitKind.Planet;

        public override string ToString()
        {
            return $"[{Kind}] {Name} at {Position}.";
        }
    }
}