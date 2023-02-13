using static System.Collections.Specialized.BitVector32;
using System.Text.Json;

namespace Flattiverse.Connector.Units
{
    [UnitIdentifier("buoy")]
    public class Buoy : SteadyUnit
    {
        public string Message;

        public Buoy(string name, Vector position, Vector movement) : base(name, position, movement)
        {
        }

        public Buoy(string name, Vector position) : base(name, position)
        {
        }

        public Buoy(string name) : base(name)
        {
        }

        public Buoy() : base()
        {
        }

        internal Buoy(UniverseGroup group, JsonElement element) : base(group, element)
        {
            Utils.Traverse(element, out Message, "message");
        }

        public override UnitKind Kind => UnitKind.Buoy;
    }
}