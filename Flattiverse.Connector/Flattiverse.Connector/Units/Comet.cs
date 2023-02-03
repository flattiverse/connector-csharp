using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    [UnitIdentifier("comet")]
    public class Comet : SteadyUnit
    {
        public Comet(string name, Vector position, Vector movement) : base(name, position, movement)
        {
        }

        public Comet(string name, Vector position) : base(name, position)
        {
        }

        public Comet(string name) : base(name)
        {
        }

        public Comet() : base()
        {
        }

        internal Comet(JsonElement element) : base(element)
        {
        }

        public override UnitKind Kind => UnitKind.Comet;
    }
}