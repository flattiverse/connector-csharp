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

        internal Meteoroid(JsonElement element) : base(element)
        {
        }

        public override UnitKind Kind => UnitKind.Meteoroid;
    }
}